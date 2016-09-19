using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.SanfordAdapter
{
    static class MidiReader
    {
        public static Song ReadMidi(string filePath)
        {
            var sequence = new Sequence();
            sequence.Load(filePath);
            return ReadSequence(sequence);
        }

        public static Song ReadSequence(Sequence sequence)
        {
            Song song = new Song(sequence);

            // De sequence heeft tracks. Deze zijn per index benaderbaar.
            for (int i = 0; i < sequence.Count; i++)
            {
                //NOTE: converted to custom track by implicit operator in Track class
                Track track = sequence[i];

                song.AddTrack(track);

                //MidiTrack trackLog = new MidiTrack() { TrackName = i.ToString() };

                //foreach (var midiEvent in track.Iterator())
                //{
                //    // Elke messagetype komt ook overeen met een class. Daarom moet elke keer gecast worden.
                //    switch (midiEvent.MidiMessage.MessageType)
                //    {
                //        // ChannelMessages zijn de inhoudelijke messages.
                //        case MessageType.Channel:
                //            var channelMessage = midiEvent.MidiMessage as ChannelMessage;
                //            // Data1: De keycode. 0 = laagste C, 1 = laagste C#, 2 = laagste D etc.
                //            // 160 is centrale C op piano.
                //            trackLog.Messages.Add(String.Format("Keycode: {0}, Command: {1}, absolute time: {2}, delta time: {3}"
                //                , channelMessage.Data1, channelMessage.Command, midiEvent.AbsoluteTicks, midiEvent.DeltaTicks));
                //            break;
                //        case MessageType.SystemExclusive:
                //            break;
                //        case MessageType.SystemCommon:
                //            break;
                //        case MessageType.SystemRealtime:
                //            break;
                //        // Meta zegt iets over de track zelf.
                //        case MessageType.Meta:
                //            var metaMessage = midiEvent.MidiMessage as MetaMessage;
                //            trackLog.Messages.Add(GetMetaString(metaMessage));
                //            if (metaMessage.MetaType == MetaType.TrackName)
                //            {
                //                trackLog.TrackName += " " + Encoding.Default.GetString(metaMessage.GetBytes());
                //            }
                //            break;
                //        default:
                //            trackLog.Messages.Add(String.Format("MidiEvent {0}, absolute ticks: {1}, deltaTicks: {2}", midiEvent.MidiMessage.MessageType, midiEvent.AbsoluteTicks, midiEvent.DeltaTicks));
                //            break;
                //    }
                //}
            }

            return song;



            //var song = new Song(sequence);
            //return song;



            //// De sequence heeft tracks. Deze zijn per index benaderbaar.
            //for (int i = 0; i < sequence.Count; i++)
            //{
            //    Track track = sequence[i];
            //    MidiTrack trackLog = new MidiTrack() { TrackName = i.ToString() };

            //    foreach (var midiEvent in track.Iterator())
            //    {
            //        // Elke messagetype komt ook overeen met een class. Daarom moet elke keer gecast worden.
            //        switch (midiEvent.MidiMessage.MessageType)
            //        {
            //            // ChannelMessages zijn de inhoudelijke messages.
            //            case MessageType.Channel:
            //                var channelMessage = midiEvent.MidiMessage as ChannelMessage;
            //                // Data1: De keycode. 0 = laagste C, 1 = laagste C#, 2 = laagste D etc.
            //                // 160 is centrale C op piano.
            //                trackLog.Messages.Add(String.Format("Keycode: {0}, Command: {1}, absolute time: {2}, delta time: {3}"
            //                    , channelMessage.Data1, channelMessage.Command, midiEvent.AbsoluteTicks, midiEvent.DeltaTicks));
            //                break;
            //            case MessageType.SystemExclusive:
            //                break;
            //            case MessageType.SystemCommon:
            //                break;
            //            case MessageType.SystemRealtime:
            //                break;
            //            // Meta zegt iets over de track zelf.
            //            case MessageType.Meta:
            //                var metaMessage = midiEvent.MidiMessage as MetaMessage;
            //                trackLog.Messages.Add(GetMetaString(metaMessage));
            //                if (metaMessage.MetaType == MetaType.TrackName)
            //                {
            //                    trackLog.TrackName += " " + Encoding.Default.GetString(metaMessage.GetBytes());
            //                }
            //                break;
            //            default:
            //                trackLog.Messages.Add(String.Format("MidiEvent {0}, absolute ticks: {1}, deltaTicks: {2}", midiEvent.MidiMessage.MessageType, midiEvent.AbsoluteTicks, midiEvent.DeltaTicks));
            //                break;
            //        }
            //    }

            //    yield return trackLog;
            //}
        }
    }
}
