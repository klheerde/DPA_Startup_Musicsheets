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
        public static readonly int DEFAULT_DIVISION = 384;

        public Sequence Sequence { get; private set; }

        private Dictionary<int, int[]> timeSignaturesByStartTimes = new Dictionary<int, int[]>();
        public int[] TimeSignatureStartTimes { get { return timeSignaturesByStartTimes.Keys.ToArray(); } }
        public int[] TimeSignature(int startTime) { return timeSignaturesByStartTimes[startTime]; }

        public int Tempo { get; private set; }

        public List<Track> Tracks { get; private set; }
    
        public Song()
        {
            Tracks = new List<Track>();
        }

        public Song(Sequence sequence) : this()
        {
            Sequence = sequence;
        }

        public Sequence CreateSequence()
        {
            return new Sequence();
        }


        public class Builder : IBuilder<Song>
        {
            private Song buildee;

            private int currentTrackBuilderIndex = -1;
            private List<Track.Builder> trackBuilders = new List<Track.Builder>();
            public Track.Builder CurrentTrackBuilder { get { return trackBuilders.ElementAt(currentTrackBuilderIndex); } }

            public Builder() : this(new Song()) { }
            public Builder(Song song)
            {
                buildee = song;
            }

            public Builder AddSequence(Sequence sequence)
            {
                buildee.Sequence = sequence;
                for (int i = 0; i < sequence.Count; i++)
                    buildee.Tracks.Add(new Track.Builder().AddSanfordTrack(this, sequence[i]).GetItem());
                return this;
            }

            public Builder AddTimeSignature(int startTime, int amountPerBar, int countsPerbeat)
            {
                double quarterToSig1 = 4.0 / countsPerbeat;
                double division = buildee.Sequence == null ? DEFAULT_DIVISION : buildee.Sequence.Division;
                double tickPerSig1 = division * quarterToSig1; //ticksPerBeat
                buildee.timeSignaturesByStartTimes.Add(startTime, new int[] { amountPerBar, countsPerbeat, (int)tickPerSig1 });
                return this;
            }

            public Builder AddTempo(int tempo)
            {
                buildee.Tempo = tempo;
                return this;
            }

            public Builder AddTrackBuilder(Track.Builder trackBuilder)
            {
                buildee.Tracks.Add(trackBuilder.GetItem());
                trackBuilders.Add(trackBuilder);
                currentTrackBuilderIndex++;
                return this;
            }

            public Song GetItem()
            {
                return buildee;
            }
        }
    }
}
