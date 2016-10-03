using DPA_Musicsheets.SanfordAdapter.Tonal;
using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.SanfordAdapter
{
    public class Song
    {
        private Sequence sequence;

        //private int ticksPerBeat;

        private Dictionary<int, int[]> timeSignaturesByStartTimes = new Dictionary<int, int[]>();
        public int[] TimeSignatureStartTimes { get { return timeSignaturesByStartTimes.Keys.ToArray(); } }
        public int[] TimeSignature(int startTime) { return timeSignaturesByStartTimes[startTime]; }

    
        public int Tempo { get; private set; }

        private List<Track> tracks = new List<Track>();

        private Song() { }
        public Song(Sequence sequence)
        {
            this.sequence = sequence;
            //ticksPerBeat = sequence.Division;
            //ticksPerBeat = (int)(sequence.Division * (noteLength / 0.25));
        }

        //TODO cleaner
        public void AddTrack(Track track) { tracks.Add(track); }
        public Track GetTrack(int index) { return tracks[index]; }
        public int TrackCount { get { return tracks.Count; } }
        //public int TimeSignature(int i) { return timeSignature[i]; }
        public Sequence Sequence { get { return sequence; } }

        public class Builder : IBuilder<Song>
        {
            private Song buildee;
            public Builder() : this(new Song()) { }
            public Builder(Song song) : this(song, null) { }
            public Builder (Sequence sequence) : this(new Song(sequence), sequence) { }
            public Builder(Song song, Sequence sequence)
            {
                buildee = song;
                if (sequence == null)
                    return;
                AddSequence(sequence);
            }

            public Builder AddSequence(Sequence sequence)
            {
                for (int i = 0; i < sequence.Count; i++)
                    buildee.AddTrack(new Track.Builder(this, sequence[i]).Build());
                return this;
            }

            public Builder AddTimeSignature(int startTime, int amountPerBar, int countsPerbeat)
            { 
                double quarterToSig1 = 4 / countsPerbeat;
                double tickPerSig1 = buildee.Sequence.Division * quarterToSig1;
                //int ticksPerBeat = (int)(tickPerSig1 * amountPerBar);
                buildee.timeSignaturesByStartTimes.Add(startTime, new int[]{amountPerBar, countsPerbeat, (int)tickPerSig1/*ticksPerBeat */});
                return this;
            }

            public Builder AddTempo(int tempo)
            {
                buildee.Tempo = tempo;
                return this;
            }

            //public Builder SetMetaData(MetaMessage metaMessage)
            //{
            //    byte[] bytes = metaMessage.GetBytes();

            //    switch (metaMessage.MetaType)
            //    {
            //        case MetaType.Tempo:
            //            // Bitshifting is nodig om het tempo in BPM te be
            //            int tempo = (bytes[0] & 0xff) << 16 | (bytes[1] & 0xff) << 8 | (bytes[2] & 0xff);
            //            buildee.bpm = 60000000 / tempo;
            //            break;
            //        //case MetaType.SmpteOffset:
            //        //    break;
            //        case MetaType.TimeSignature:
            //            if (buildee.timeSignature[0] != 0)
            //                return this;
            //            //NOTE: kwart = 1 / 0.25 = 4
            //            buildee.timeSignature[0] = bytes[0];
            //            buildee.timeSignature[1] = (int)Math.Pow(2, bytes[1]);
            //            //TODO  0.25 = 1 / timeSignature[0] ?
            //            double quarterToSig1 = 4 / buildee.TimeSignature(1);
            //            double tickPerSig1 = buildee.Sequence.Division * quarterToSig1;
            //            buildee.ticksPerBeat = (int)(tickPerSig1 * buildee.timeSignature[0]);
            //        break;
            //        //case MetaType.KeySignature:
            //        //    break;
            //        //case MetaType.ProprietaryEvent:
            //        //    break;
            //        //case MetaType.TrackName:
            //        //    name = Encoding.Default.GetString(bytes);
            //        //    break;
            //        default:
            //            //return metaMessage.MetaType + ": " + Encoding.Default.GetString(metaMessage.GetBytes());
            //            break;
            //    }

            //    return this;
            //}

            public Song Build()
            {
                return buildee;
            }
        }
    }
}
