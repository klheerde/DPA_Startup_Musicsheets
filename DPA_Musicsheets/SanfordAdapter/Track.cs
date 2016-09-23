 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sanford.Multimedia.Midi;
using DPA_Musicsheets.SanfordAdapter.Tonal;

namespace DPA_Musicsheets.SanfordAdapter
{
    public class Track
    {
        private string name;

        //private int[] timeSignature = new int[2];

        private List<Note> events = new List<Note>();

        private void AddNote(Note e) { events.Add(e); }
        public Note GetNote(int index) { return events[index]; }
        public int NoteCount { get { return events.Count; } }

        public string Name { get { return name; } set { name = value; } }
   
        public class Builder : IBuilder<Track>
        {
            private Track buildee;
            public Builder() : this(new Track()) { }
            public Builder(Track track)
            {
                buildee = track;
            }

            public Builder SetMetaData(MetaMessage metaMessage)
            {
                byte[] bytes = metaMessage.GetBytes();

                switch (metaMessage.MetaType)
                {
                    //case MetaType.Tempo:
                    //    // Bitshifting is nodig om het tempo in BPM te be
                    //    int tempo = (bytes[0] & 0xff) << 16 | (bytes[1] & 0xff) << 8 | (bytes[2] & 0xff);
                    //    buildee.bpm = 60000000 / tempo;
                    //    break;
                    //case MetaType.SmpteOffset:
                    //    break;
                    //case MetaType.TimeSignature:
                    //    //NOTE: kwart = 1 / 0.25 = 4
                    //    buildee.timeSignature[0] = bytes[0];
                    //    buildee.timeSignature[1] = (int) Math.Pow(2, bytes[1]);
                    //break;
                    //case MetaType.KeySignature:
                    //    break;
                    //case MetaType.ProprietaryEvent:
                    //    break;
                    case MetaType.TrackName:
                        buildee.name = Encoding.Default.GetString(bytes);
                        break;
                    default:
                        //return metaMessage.MetaType + ": " + Encoding.Default.GetString(metaMessage.GetBytes());
                        break;
                }

                return this;
            }

            public Track Build()
            {
                return buildee;
            }
        }

        public static Track Convert(Sanford.Multimedia.Midi.Track convert, Song song)
        {
            Track track = new Track();

            List<Note> pending = new List<Note>();

            foreach (var midiEvent in convert.Iterator())
            {
                // Elke messagetype komt ook overeen met een class. Daarom moet elke keer gecast worden.
                switch (midiEvent.MidiMessage.MessageType)
                {
                    // ChannelMessages zijn de inhoudelijke messages.
                    case MessageType.Channel:
                        var channelMessage = midiEvent.MidiMessage as ChannelMessage;
                        // Data1: De keycode. 0 = laagste C, 1 = laagste C#, 2 = laagste D etc.
                        // 160 is centrale C op piano.
                        //trackLog.Messages.Add(String.Format("Keycode: {0}, Command: {1}, absolute time: {2}, delta time: {3}"
                        //    , channelMessage.Data1, channelMessage.Command, midiEvent.AbsoluteTicks, midiEvent.DeltaTicks));

                        //NOTE: velocity higher than 0 == start note
                        if (channelMessage.Data2 > 0)
                        {
                            Note note = new Note.Builder()
                                .AddKeycode(channelMessage.Data1)
                                .AddVelocity(channelMessage.Data2)
                                .AddStart(midiEvent.AbsoluteTicks)
                                .Build();

                            pending.Add(note);
                        }
                        else //NOTE: end of note
                        {
                            //NOTE: find returns first match
                            Note note = pending.Find(n => n.Keycode == channelMessage.Data1);
                            note.EndTime = midiEvent.AbsoluteTicks;
                            track.AddNote(note);
                            pending.Remove(note);

                            if (midiEvent.DeltaTicks > 0) //rest
                            {
                                Note rest = new Note.Builder()
                                    .AddStart(midiEvent.AbsoluteTicks)
                                    .AddDuration(midiEvent.DeltaTicks)
                                    .Build();

                                track.AddNote(rest);
                            }
                        }

                        break;
                    case MessageType.SystemExclusive:
                        break;
                    case MessageType.SystemCommon:
                        break;
                    case MessageType.SystemRealtime:
                        break;
                    case MessageType.Meta:
                        //NOTE: Meta zegt iets over de track zelf.
                        MetaMessage metaMessage = midiEvent.MidiMessage as MetaMessage;
                        new Song.Builder(song).SetMetaData(metaMessage);
                        new Track.Builder(track).SetMetaData(metaMessage);
                            //track.name = Encoding.Default.GetString(metaMessage.GetBytes());
                        break;
                    default:
                        //trackLog.Messages.Add(String.Format("MidiEvent {0}, absolute ticks: {1}, deltaTicks: {2}", midiEvent.MidiMessage.MessageType, midiEvent.AbsoluteTicks, midiEvent.DeltaTicks));
                        break;
                }
            }

            return track;
        }
    }
}
