
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sanford.Multimedia.Midi;
using DPA_Musicsheets.SanfordAdapter.Tonal;
using SanfordTrack = Sanford.Multimedia.Midi.Track;

namespace DPA_Musicsheets.SanfordAdapter
{
    public class Track
    {
        public string Name { get; private set; }
        public List<TrackPart> Parts { get; private set; }
        public Track()
        {
            Parts = new List<TrackPart>();
        }
   
        public class Builder : IBuilder<Track>
        {
            private Track buildee;

            private int currentTrackPartBuilderIndex = -1;
            private List<TrackPart.Builder> trackPartBuilders = new List<TrackPart.Builder>();
            public TrackPart.Builder CurrentTrackPartBuilder {
                get {
                    return trackPartBuilders.ElementAt(currentTrackPartBuilderIndex);
                }
            }
            public TrackPart.Builder PreviousTrackPartBuilder {
                get {
                    return currentTrackPartBuilderIndex > 0 ? trackPartBuilders.ElementAt(currentTrackPartBuilderIndex - 1) : null;        
                }
            }

            public Builder() : this(new Track()) { }
            public Builder(Track buildee)
            {
                this.buildee = buildee;
            }

            //public Builder AddSanfordTrack(Song.Builder songBuilder, SanfordTrack convert)
            //{
            //    //TrackPart.Builder trackPartBuilder = null; //force the below if-statement to create trackPartBuilder.
            //    TrackPart.Builder trackPartBuilder = null;
            //    List<Note.Builder> pending = new List<Note.Builder>();

            //    int currentTrackPart = 0;
            //    int currentTime = 0;

            //    //NOTE:  called before foreach so when control track stays length 0.
            //    int[] startTimes = songBuilder.GetItem().TimeSignatureStartTimes;

            //    foreach (var midiEvent in convert.Iterator())
            //    {
            //        int startTime = 0;
            //        //NOTE: first track is control track, so doesn't enter this statement.
            //        if (currentTrackPart < startTimes.Length && currentTime >= (startTime = startTimes[currentTrackPart]))
            //        {
            //            int[] timeSignature = songBuilder.GetItem().TimeSignature(startTime);
            //            trackPartBuilder = new TrackPart.Builder()
            //                .AddStartTime(startTime)
            //                .AddTimeSignature(timeSignature[0], timeSignature[1], timeSignature[2]);
            //            AddTrackPart(trackPartBuilder.GetItem());
            //            currentTrackPart++;
            //        }

            //        switch (midiEvent.MidiMessage.MessageType)
            //        {
            //            case MessageType.Channel:
            //                var channelMessage = midiEvent.MidiMessage as ChannelMessage;

            //                //NOTE: velocity higher than 0 == start note
            //                //vel > 0 && delta > 0 = rest.len = delta
            //                if (channelMessage.Data2 > 0)
            //                {
            //                    Note.Builder noteBuilder = new Note.Builder()
            //                        .AddKeycode(channelMessage.Data1)
            //                        .AddVelocity(channelMessage.Data2)
            //                        .AddStart(midiEvent.AbsoluteTicks);

            //                    pending.Add(noteBuilder);

            //                    if (midiEvent.AbsoluteTicks > currentTime)
            //                    {
            //                        Note rest = new Note.Builder()
            //                            .AddStart(currentTime)
            //                            .AddEnd(midiEvent.AbsoluteTicks, trackPartBuilder.GetItem())
            //                            //.AddEnd(midiEvent.AbsoluteTicks, songBuilder.GetItem())
            //                            .GetItem();

            //                        trackPartBuilder.AddNote(rest);
            //                    }
            //                    //TODO use DeltaTicks:
            //                    //if (midiEvent.DeltaTicks > 0) //rest
            //                    //{
            //                    //    Note rest = new Note.Builder()
            //                    //        .AddStart(midiEvent.AbsoluteTicks)
            //                    //        .AddDuration(midiEvent.DeltaTicks)
            //                    //        .Build();
            //                    //    track.AddNote(rest);
            //                    //}

            //                }
            //                else //NOTE: end of note
            //                {
            //                    //NOTE: find returns first match
            //                    //TODO check if first with same keycode is actually one that endss
            //                    Note.Builder noteBuilder = pending.Find(n => n.GetItem().Keycode == channelMessage.Data1);
            //                    noteBuilder.AddEnd(midiEvent.AbsoluteTicks, trackPartBuilder.GetItem());
            //                    trackPartBuilder.AddNote(noteBuilder.GetItem());
            //                    pending.Remove(noteBuilder);
            //                }
                            
            //                //TODO could cause issues because current time = AbsoluteTicks + noteLength
            //                currentTime = midiEvent.AbsoluteTicks;

            //                break;

            //            case MessageType.SystemExclusive:
            //                break;
            //            case MessageType.SystemCommon:
            //                break;
            //            case MessageType.SystemRealtime:
            //                break;

            //            //NOTE: mostly called by control track.
            //            case MessageType.Meta:
            //                MetaMessage metaMessage = midiEvent.MidiMessage as MetaMessage;
                            
            //                byte[] bytes = metaMessage.GetBytes();
            //                switch (metaMessage.MetaType)
            //                {
            //                    case MetaType.Tempo:
            //                        ////TODO  0.25 = 1 / timeSignature[0] ?
            //                        //double quarterToSig1 = 4 / countsPerBeat;
            //                        //double tickPerSig1 = song.Sequence.Division * quarterToSig1;
            //                        //song.ticksPerBeat = (int)(tickPerSig1 * buildee.timeSignature[0]);

            //                        // Bitshifting is nodig om het tempo in BPM te be
            //                        int tempo = (bytes[0] & 0xff) << 16 | (bytes[1] & 0xff) << 8 | (bytes[2] & 0xff);
            //                        songBuilder.AddTempo(60000000 / tempo);
            //                        break;
            //                    case MetaType.TimeSignature:                               
            //                        //kwart = 1 / 0.25 = 4
            //                        int amountPerBar = bytes[0];
            //                        int countsPerBeat = (int) Math.Pow(2, bytes[1]);
            //                        //NOTE: setting both time sigs in song and trackpart.
            //                        songBuilder.AddTimeSignature(midiEvent.AbsoluteTicks, amountPerBar, countsPerBeat);
            //                        //trackPartBuilder.AddTimeSignature(amountPerBar, countsPerBeat, songBuilder.GetItem());
            //                        break;
            //                    case MetaType.TrackName:
            //                        buildee.Name = Encoding.Default.GetString(metaMessage.GetBytes());
            //                        break;
            //                }
            //                break;
            //        }
            //    }

            //    return this;
            //}

            public Builder AddName(string name)
            {
                buildee.Name = name;
                return this;
            }

            public Builder AddTrackPartBuilder(TrackPart.Builder trackPartBuilder)
            {
                AddTrackPart(trackPartBuilder.GetItem());
                trackPartBuilders.Add(trackPartBuilder);
                currentTrackPartBuilderIndex++;
                return this;
            }

            public Builder AddTrackPart(TrackPart trackPart)
            {
                buildee.Parts.Add(trackPart);
                return this;
            }

            public Track GetItem()
            {
                return buildee;
            }
        }
    }
}
