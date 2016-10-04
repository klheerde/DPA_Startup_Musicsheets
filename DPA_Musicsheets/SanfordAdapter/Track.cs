
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
            public Builder() : this(new Track()) { }
            public Builder(Track track) : this(track, null, null) { }
            public Builder(Song.Builder songBuilder) : this(new Track(), songBuilder, null) { }
            public Builder(SanfordTrack convert) : this(new Track(), null, convert) { }
            public Builder(Song.Builder songBuilder, SanfordTrack convert) : this(new Track(), songBuilder, convert) { }
            public Builder(Track track, Song.Builder songBuilder, SanfordTrack convert)
            {
                buildee = track;
                if (convert == null)
                    return;
                AddSanfordTrack(songBuilder, convert);
            }

            public Builder AddSanfordTrack(Song.Builder songBuilder, SanfordTrack convert)
            {
                TrackPart.Builder trackPartBuilder = null; //force the below if statement to create trackPartBuilder.
                List<Note.Builder> pending = new List<Note.Builder>();

                int currentTrackPart = 0;
                int currentTime = 0;
                //NOTE:  called before foreach so when control track stays length 0.
                int[] startTimes = songBuilder.Build().TimeSignatureStartTimes;

                foreach (var midiEvent in convert.Iterator())
                {
                    int startTime = 0;
                    //NOTE: first track is control track, so doesn't enter this statement.
                    if (currentTrackPart < startTimes.Length && currentTime >= (startTime = startTimes[currentTrackPart]))
                    {
                        int[] timeSignature = songBuilder.Build().TimeSignature(startTime);
                        trackPartBuilder = new TrackPart.Builder()
                            .AddStartTime(startTime)
                            .AddTimeSignature(timeSignature[0], timeSignature[1], timeSignature[2]);
                        AddTrackPart(trackPartBuilder.Build());
                        currentTrackPart++;
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
                                        .AddEnd(midiEvent.AbsoluteTicks, songBuilder.Build())
                                        .Build();

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
                            else //NOTE: end of note
                            {
                                //NOTE: find returns first match
                                //TODO check if first with same keycode is actually one that endss
                                Note.Builder noteBuilder = pending.Find(n => n.Build().Keycode == channelMessage.Data1);
                                noteBuilder.AddEnd(midiEvent.AbsoluteTicks, songBuilder.Build());
                                trackPartBuilder.AddNote(noteBuilder.Build());
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

                        //NOTE: mostly called by control track.
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
                                    int countsPerBeat = (int) Math.Pow(2, bytes[1]);
                                    songBuilder.AddTimeSignature(midiEvent.AbsoluteTicks, amountPerBar, countsPerBeat);
                                    break;
                                case MetaType.TrackName:
                                    buildee.Name = Encoding.Default.GetString(metaMessage.GetBytes());
                                    break;
                            }
                            break;
                    }
                }

                return this;
            }

            public Builder AddTrackPart(TrackPart trackPart)
            {
                buildee.Parts.Add(trackPart);
                return this;
            }

            //public Builder SetMetaData(MetaMessage metaMessage)
            //{
            //    byte[] bytes = metaMessage.GetBytes();

            //    switch (metaMessage.MetaType)
            //    {
            //        //case MetaType.Tempo:
            //        //    // Bitshifting is nodig om het tempo in BPM te be
            //        //    int tempo = (bytes[0] & 0xff) << 16 | (bytes[1] & 0xff) << 8 | (bytes[2] & 0xff);
            //        //    buildee.bpm = 60000000 / tempo;
            //        //    break;
            //        //case MetaType.SmpteOffset:
            //        //    break;
            //        //case MetaType.TimeSignature:
            //        //    //NOTE: kwart = 1 / 0.25 = 4
            //        //    buildee.timeSignature[0] = bytes[0];
            //        //    buildee.timeSignature[1] = (int) Math.Pow(2, bytes[1]);
            //        //break;
            //        //case MetaType.KeySignature:
            //        //    break;
            //        //case MetaType.ProprietaryEvent:
            //        //    break;
            //        case MetaType.TrackName:
            //            buildee.Name = Encoding.Default.GetString(bytes);
            //            break;
            //        default:
            //            //return metaMessage.MetaType + ": " + Encoding.Default.GetString(metaMessage.GetBytes());
            //            break;
            //    }

            //    return this;
            //}

            public Track Build()
            {
                return buildee;
            }
        }

        //public static Track Convert(Sanford.Multimedia.Midi.Track convert, Song song)
        //{
        //    Track track = new Track();
        //    TrackPart part = new TrackPart();
        //    List<Note> pending = new List<Note>();

        //    bool firstTrack = true;
        //    foreach (var midiEvent in convert.Iterator())
        //    {
        //        // Elke messagetype komt ook overeen met een class. Daarom moet elke keer gecast worden.
        //        switch (midiEvent.MidiMessage.MessageType)
        //        {
        //            // ChannelMessages zijn de inhoudelijke messages.
        //            case MessageType.Channel:
        //                var channelMessage = midiEvent.MidiMessage as ChannelMessage;

        //                //NOTE: velocity higher than 0 == start note
        //                //vel > 0 && delta > 0 = rest.len = delta
        //                if (channelMessage.Data2 > 0)
        //                {
        //                    Note note = new Note.Builder()
        //                        .AddKeycode(channelMessage.Data1)
        //                        .AddVelocity(channelMessage.Data2)
        //                        .AddStart(midiEvent.AbsoluteTicks)
        //                        .Build();

        //                    pending.Add(note);
        //                }
        //                else //NOTE: end of note
        //                {
        //                    //NOTE: find returns first match
        //                    Note note = pending.Find(n => n.Keycode == channelMessage.Data1);
        //                    new Note.Builder(note).AddEnd(midiEvent.AbsoluteTicks, song);
        //                    track.AddNote(note);
        //                    pending.Remove(note);

        //                    if (midiEvent.DeltaTicks > 0) //rest
        //                    {
        //                        Note rest = new Note.Builder()
        //                            .AddStart(midiEvent.AbsoluteTicks)
        //                            .AddDuration(midiEvent.DeltaTicks)
        //                            .Build();

        //                        track.AddNote(rest);
        //                    }
        //                }

        //                break;
        //            case MessageType.SystemExclusive:
        //                break;
        //            case MessageType.SystemCommon:
        //                break;
        //            case MessageType.SystemRealtime:
        //                break;
        //            case MessageType.Meta:
        //                MetaMessage metaMessage = midiEvent.MidiMessage as MetaMessage;

        //                if (metaMessage.MetaType == MetaType.TrackName)
        //                    track.Name = Encoding.Default.GetString(metaMessage.GetBytes());

        //                byte[] bytes = metaMessage.GetBytes();
        //                switch (metaMessage.MetaType)
        //                {
        //                    case MetaType.Tempo:
        //                        // Bitshifting is nodig om het tempo in BPM te be
        //                        int tempo = (bytes[0] & 0xff) << 16 | (bytes[1] & 0xff) << 8 | (bytes[2] & 0xff);
        //                        int bpm = 60000000 / tempo;
        //                        return metaMessage.MetaType + ": " + bpm;
        //                    //case MetaType.SmpteOffset:
        //                    //    break;
        //                    case MetaType.TimeSignature:                               //kwart = 1 / 0.25 = 4
        //                        return metaMessage.MetaType + ": (" + bytes[0] + " / " + 1 / Math.Pow(bytes[1], -2) + ") ";
        //                    //case MetaType.KeySignature:
        //                    //    break;
        //                    //case MetaType.ProprietaryEvent:
        //                    //    break;
        //                    case MetaType.TrackName:
        //                        return metaMessage.MetaType + ": " + Encoding.Default.GetString(metaMessage.GetBytes());
        //                    default:
        //                        return metaMessage.MetaType + ": " + Encoding.Default.GetString(metaMessage.GetBytes());
        //                }

        //                //new Song.Builder(song).SetMetaData(metaMessage);
        //                //new Track.Builder(track).SetMetaData(metaMessage);
        //                    //track.name = Encoding.Default.GetString(metaMessage.GetBytes());
        //                break;
        //            default:
        //                //trackLog.Messages.Add(String.Format("MidiEvent {0}, absolute ticks: {1}, deltaTicks: {2}", midiEvent.MidiMessage.MessageType, midiEvent.AbsoluteTicks, midiEvent.DeltaTicks));
        //                break;
        //        }

        //        if (firstTrack)
        //            firstTrack = false;
        //    }

        //    return track;
        //}
    }
}
