using DPA_Musicsheets.SanfordAdapter.Tonal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.SanfordAdapter
{
    public class TrackPart
    {
        public List<Note> Notes { get; private set; }
        public List<List<Note>> Alternatives { get; private set; }

        public int StartTime { get; private set; }
        private int[] timeSignature = new int[3];
        public int TimeSignature(int i) { return timeSignature[i]; }

        
        public int Repeat { get; private set; }

        public TrackPart()
        {
            Notes = new List<Note>();
            Alternatives = new List<List<Note>>();
        }


        public class Builder : IBuilder<TrackPart>
        {
            private TrackPart buildee;
            public int BaseOctave { get; private set; }
            public Note LastAddedNote { get; private set; }

            public Builder() : this(new TrackPart()) { }
            public Builder(TrackPart buildee)
            {
                this.buildee = buildee;
            }

            public Builder AddRepeat(int repeat)
            {
                buildee.Repeat = repeat;
                return this;
            }

            public Builder AddBaseOctave(int baseOctave)
            {
                BaseOctave = baseOctave;
                return this;
            }

            public Builder AddNote(Note note)
            {
                if (buildee.Alternatives.Count > 0)
                    buildee.Alternatives.Last().Add(note);
                else
                    buildee.Notes.Add(note);
                LastAddedNote = note;
                return this;
            }

            public Builder AddAlternative()
            {
                buildee.Alternatives.Add(new List<Note>());
                return this;
            }

            public Builder AddStartTime(int startTime)
            {
                buildee.StartTime = startTime;
                return this;
            }

            public Builder AddTimeSignature(int amountInBar, int countsPerBeat, int ticksPerBeat)
            {
                buildee.timeSignature[0] = amountInBar;
                buildee.timeSignature[1] = countsPerBeat;
                buildee.timeSignature[2] = ticksPerBeat;
                return this;
            }

            public TrackPart GetItem()
            {
                return buildee;
            }
        }
    }
}
