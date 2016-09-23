using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.SanfordAdapter
{
    public class _Rest
    {
        private int startTime;
        private int duration;
        //private bool dotted;

        //NOTE: for builder use
        public _Rest() { }

        public _Rest(int startTime, int duration)
        {
            StartTime = startTime;
            Duration = duration;
        }

        public int StartTime
        {
            get { return startTime; }
            protected set { if (value >= 0) startTime = value; }
        }
        public int Duration
        {
            get { return duration; }
            protected set { if (value > 0) duration = value; }
        }
        public int EndTime
        {
            get { return StartTime + Duration; }
            set { if (value > StartTime) Duration = value - StartTime; }
        }
        //TODO build in Song for calc using bpm and shit
        //public bool Dotted { get { return dotted; } protected set { dotted = value; } }
    }
}
