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
            return new SequenceToSongConverter(sequence).Convert();
        }

        private sealed class SequenceToSongConverter
        {
            private Sequence Sequence { get; set; }
            public SequenceToSongConverter(Sequence sequence)
            {
                Sequence = sequence;
                songBuilder.AddSequence(sequence);
            }

            private int currentTime = 0;
            private Dictionary<int, int[]> timeSignaturesByStartTime = new Dictionary<int, int[]>();
            private Song.Builder songBuilder = new Song.Builder();
            private List<Note.Builder> pending = new List<Note.Builder>();

            public Song Convert()
            {
                ReadControlTrack();
                ReadTracks();
                return songBuilder.GetItem();
            }

            private void ReadControlTrack()
            {
                SanfordTrack controlTrack = Sequence[0];
                foreach (var midiEvent in controlTrack.Iterator())
                {
                    if (midiEvent.MidiMessage.MessageType != MessageType.Meta)
                        continue;
                    MetaMessage metaMessage = midiEvent.MidiMessage as MetaMessage;
                    byte[] bytes = metaMessage.GetBytes();
                    switch (metaMessage.MetaType)
                    {
                        case MetaType.Tempo:
                            int tempo = (bytes[0] & 0xff) << 16 | (bytes[1] & 0xff) << 8 | (bytes[2] & 0xff);
                            songBuilder.AddTempo(60000000 / tempo);
                            break;
                        case MetaType.TimeSignature:
                            int amountPerBar = bytes[0];
                            int countsPerBeat = (int)Math.Pow(2, bytes[1]);
                            double quarterToSig1 = 4.0 / countsPerBeat;
                            double tickPerSig1 = Sequence.Division * quarterToSig1; //ticksPerBeat
                            timeSignaturesByStartTime.Add(midiEvent.AbsoluteTicks, new int[] { amountPerBar, countsPerBeat, (int)tickPerSig1 });
                            break;
                    }
                }
            }

            private void ReadTracks()
            {
                int currentStartTimeIndex = 0;
                int[] startTimes = timeSignaturesByStartTime.Keys.ToArray();

                //NOTE: starting from 1 because 0 is control track.
                for (int i = 1; i < Sequence.Count; i++)
                {
                    SanfordTrack sanfordTrack = Sequence[i];
                    Track.Builder trackBuilder = new Track.Builder();
                    songBuilder.AddTrackBuilder(trackBuilder);
                    //NOTE: force the below if-statement to create trackPartBuilder. If something goes wrong will throw nullpointer exception.
                    TrackPart.Builder trackPartBuilder = null;

                    foreach (var midiEvent in sanfordTrack.Iterator())
                    {
                        //NOTE: set startTime to avoid compile errors, is reset in below if statement.
                        int startTime = -1;
                        //NOTE: when sanfordTrack.Iterator gets to tracks containing notes 
                        //      start creating trackparts on timesigs given during first iteration (control track)
                        //NOTE: currentTime set by ReadChannelMessage().
                        if (currentStartTimeIndex < startTimes.Length && currentTime >= (startTime = startTimes[currentStartTimeIndex]))
                        {
                            int[] timeSignature = timeSignaturesByStartTime[startTime];
                            trackPartBuilder = new TrackPart.Builder()
                                .AddStartTime(startTime)
                                .AddTimeSignature(timeSignature[0], timeSignature[1], timeSignature[2]);
                            trackBuilder.AddTrackPartBuilder(trackPartBuilder);
                            currentStartTimeIndex++;
                        }

                        switch (midiEvent.MidiMessage.MessageType)
                        {
                            case MessageType.Channel:
                                ReadChannelMessage(midiEvent);
                                break;
                            case MessageType.Meta:
                                ReadMetaMessage(midiEvent);
                                break;
                        }
                    } //end: foreach (var midiEvent in sanfordTrack.Iterator())
                } //end: for (int i = 0; i < sequence.Count; i++)

            }

            private void ReadChannelMessage(MidiEvent midiEvent)
            {
                ChannelMessage channelMessage = midiEvent.MidiMessage as ChannelMessage;
                //NOTE: if order execution wrong will throw nullpointer exception.
                TrackPart.Builder trackPartBuilder = songBuilder.CurrentTrackBuilder.CurrentTrackPartBuilder;
                //NOTE: NoteOn and velocity higher than 0 == start note
                //vel > 0 && delta > 0 = rest.len = delta
                if (channelMessage.Command == ChannelCommand.NoteOn && channelMessage.Data2 > 0)
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
                else if (channelMessage.Command == ChannelCommand.NoteOff || channelMessage.Data2 == 0) //NOTE: end of note.
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
            }

            //NOTE: currently only reading trackname for non-control tracks.
            private void ReadMetaMessage(MidiEvent midiEvent)
            {
                MetaMessage metaMessage = midiEvent.MidiMessage as MetaMessage;
                if (metaMessage.MetaType != MetaType.TrackName)
                    return;
                Track.Builder trackBuilder = songBuilder.CurrentTrackBuilder;
                byte[] bytes = metaMessage.GetBytes();
                string trackName = Encoding.Default.GetString(bytes);
                trackBuilder.AddName(trackName);
            }
        }
    }
}
