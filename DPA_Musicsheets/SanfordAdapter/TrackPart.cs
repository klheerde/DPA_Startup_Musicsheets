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

        public int StartTime { get; private set; }
        private int[] timeSignature = new int[3];
        public int TimeSignature(int i) { return timeSignature[i]; }

        
        public int Repeat { get; private set; }

        public TrackPart()
        {
            Notes = new List<Note>();
        }


        public class Builder : IBuilder<TrackPart>
        {
            private TrackPart buildee;
            public Builder() : this(new TrackPart()) { }
            public Builder(TrackPart buildee)
            {
                this.buildee = buildee;
            }

            public Builder AddNote(Note note)
            {
                buildee.Notes.Add(note);
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
