using SanfordTrack = Sanford.Multimedia.Midi.Track;
using DPA_Musicsheets.SanfordAdapter.Tonal;
using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.SanfordAdapter.Reading.Midi
{
    //NOTE: is singleton
    class MidiReader : IMusicReader
    {
        public Song Read(string filePath)
        {
            var sequence = new Sequence();
            sequence.Load(filePath);

            return ReadSequence(sequence);
        }

        public Song ReadSequence(Sequence sequence)
        {
            //return new Song.Builder().AddSequence(sequence).GetItem();

            Song.Builder songBuilder = new Song.Builder();

            Dictionary<int, int[]> timeSignaturesByStartTime = new Dictionary<int, int[]>();

            for (int i = 0; i < sequence.Count; i++)
            {
                //buildee.Tracks.Add(new Track.Builder().AddSanfordTrack(this, sequence[i]).GetItem());
                SanfordTrack sanfordTrack = sequence[i];

                Track.Builder trackBuilder = new Track.Builder();
                songBuilder.AddTrackBuilder(trackBuilder);

                //NOTE: force the below if-statement to create trackPartBuilder.
                TrackPart.Builder trackPartBuilder = null; 
                List<Note.Builder> pending = new List<Note.Builder>();

                //int currentTrackPart = 0;
                int currentTime = 0;

                ////NOTE:  called before foreach so when control track stays length 0.
                //int[] startTimes = songBuilder.GetItem().TimeSignatureStartTimes;

                foreach (var midiEvent in sanfordTrack.Iterator())
                {
                    int startTime = 0;
                    int[] startTimes = timeSignaturesByStartTime.Keys.ToArray();
                    //NOTE: when sanfordTrack.Iterator gets to tracks containing notes 
                    //      start creating trackparts on timesigs given during first iteration (control track)
                    //NOTE: first track (control track) doesn't enter this statement.
                    if (i < startTimes.Length && currentTime >= (startTime = startTimes[i]))
                    {
                        //int[] timeSignature = songBuilder.GetItem().TimeSignature(startTime);
                        int[] timeSignature = timeSignaturesByStartTime[startTime];

                        trackPartBuilder = new TrackPart.Builder()
                            .AddStartTime(startTime)
                            .AddTimeSignature(timeSignature[0], timeSignature[1], timeSignature[2]);

                        //songBuilder.CurrentTrackBuilder.AddTrackPart(trackPartBuilder.GetItem());
                        //TODO better to use addTrackPartBuilder?
                        trackBuilder.AddTrackPart(trackPartBuilder.GetItem());

                        //currentTrackPart++;
                    }

                    switch (midiEvent.MidiMessage.MessageType)
                    {
                        case MessageType.Channel:
                            var channelMessage = midiEvent.MidiMessage as ChannelMessage;

                            //NOTE: velocity higher than 0 == start note
                            //vel > 0 && delta > 0 = rest.len = delta
                            if (channelMessage.Data2 > 0)
                            {
                                Note.Builder noteBuilder = new Note.Builder()
                                    .AddKeycode(channelMessage.Data1)
                                    .AddVelocity(channelMessage.Data2)
                                    .AddStart(midiEvent.AbsoluteTicks);

                                pending.Add(noteBuilder);

                                if (midiEvent.AbsoluteTicks > currentTime)
                                {
                                    Note rest = new Note.Builder()
                                        .AddStart(currentTime)
                                        .AddEnd(midiEvent.AbsoluteTicks, trackPartBuilder.GetItem())
                                        //.AddEnd(midiEvent.AbsoluteTicks, songBuilder.GetItem())
                                        .GetItem();

                                    trackPartBuilder.AddNote(rest);
                                }

                                //TODO use DeltaTicks:
                                //if (midiEvent.DeltaTicks > 0) //rest
                                //{
                                //    Note rest = new Note.Builder()
                                //        .AddStart(midiEvent.AbsoluteTicks)
                                //        .AddDuration(midiEvent.DeltaTicks)
                                //        .Build();
                                //    track.AddNote(rest);
                                //}

                            }
                            else //NOTE: end of note.
                            {
                                //NOTE: find returns first match.
                                //TODO check if first with same keycode is actually one that endss
                                Note.Builder noteBuilder = pending.Find(n => n.GetItem().Keycode == channelMessage.Data1);
                                //TODO dont calculate Count using ticks in domain classes
                                noteBuilder.AddEnd(midiEvent.AbsoluteTicks, trackPartBuilder.GetItem());
                                trackPartBuilder.AddNote(noteBuilder.GetItem());
                                pending.Remove(noteBuilder);
                            }

                            //TODO could cause issues because current time = AbsoluteTicks + noteLength
                            currentTime = midiEvent.AbsoluteTicks;
                            break;

                        case MessageType.SystemExclusive:
                            break;
                        case MessageType.SystemCommon:
                            break;
                        case MessageType.SystemRealtime:
                            break;

                        //NOTE: mostly called by control track, except for trackName.
                        case MessageType.Meta:
                            MetaMessage metaMessage = midiEvent.MidiMessage as MetaMessage;

                            byte[] bytes = metaMessage.GetBytes();
                            switch (metaMessage.MetaType)
                            {
                                case MetaType.Tempo:
                                    ////TODO  0.25 = 1 / timeSignature[0] ?
                                    //double quarterToSig1 = 4 / countsPerBeat;
                                    //double tickPerSig1 = song.Sequence.Division * quarterToSig1;
                                    //song.ticksPerBeat = (int)(tickPerSig1 * buildee.timeSignature[0]);

                                    // Bitshifting is nodig om het tempo in BPM te be
                                    int tempo = (bytes[0] & 0xff) << 16 | (bytes[1] & 0xff) << 8 | (bytes[2] & 0xff);
                                    songBuilder.AddTempo(60000000 / tempo);
                                    break;
                                case MetaType.TimeSignature:
                                    //kwart = 1 / 0.25 = 4
                                    int amountPerBar = bytes[0];
                                    int countsPerBeat = (int)Math.Pow(2, bytes[1]);
                                    ////NOTE: setting both time sigs in song and trackpart.
                                    //songBuilder.AddTimeSignature(midiEvent.AbsoluteTicks, amountPerBar, countsPerBeat);

                                    double quarterToSig1 = 4.0 / countsPerBeat;
                                    double tickPerSig1 = sequence.Division * quarterToSig1; //ticksPerBeat

                                    timeSignaturesByStartTime.Add(midiEvent.AbsoluteTicks, new int[] { amountPerBar, countsPerBeat, (int)tickPerSig1 });
                                    ////trackPartBuilder.AddTimeSignature(amountPerBar, countsPerBeat, songBuilder.GetItem());
                                    break;
                                case MetaType.TrackName:
                                    trackBuilder.AddName(Encoding.Default.GetString(metaMessage.GetBytes()));
                                    break;
                            }
                            break;
                    } //end: switch (midiEvent.MidiMessage.MessageType)
                } //end: foreach (var midiEvent in sanfordTrack.Iterator())
            } //end: for (int i = 0; i < sequence.Count; i++)

            return songBuilder.GetItem();
        }
    }
}
