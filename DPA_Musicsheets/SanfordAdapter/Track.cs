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

        //NOTE: rest is Note without tonal data
        private List<Rest> events = new List<Rest>();

        //NOTE: Rest is Note without tonal data
        public int NoteDurationInCounts(Rest note)
        {
            return 4;
        }

        private void AddEvent(Rest e)
        {
            events.Add(e);
        }
        public Rest GetEvent(int index)
        {
            return events[index];
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
                            track.AddEvent(note);
                            pending.Remove(note);

                            if (midiEvent.DeltaTicks > 0) //rest
                            {
                                Rest rest = new Rest(midiEvent.AbsoluteTicks, midiEvent.DeltaTicks);
                                track.AddEvent(rest);
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
                        if (metaMessage.MetaType == MetaType.TrackName)
                            track.name = Encoding.Default.GetString(metaMessage.GetBytes());
                        else
                            //NOTE: Build() not necessary
                            new Song.Builder(song).SetMetaData(metaMessage);
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
