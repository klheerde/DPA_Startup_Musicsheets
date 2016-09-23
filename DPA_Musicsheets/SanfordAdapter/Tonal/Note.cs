using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.SanfordAdapter.Tonal
{
    public enum Tone { C = 0, D = 2, E = 4, F = 5, G = 7, A = 9, B = 11, Rest = 13 /*Rest 13 outside octave scope*/ };

    public class Note
    {
        private static readonly int VELOCITY = 90;

        private int startTime;
        private int duration;

        //default rest unless added keycode
        private int keycode = 13;
        private int octave = 0;
        private Tone tone = Tone.Rest;
        private int raise = 0;

        private int velocity = VELOCITY;

        private Note() { }
        public Note(int startTime, int duration)
        {
            StartTime = startTime;
            Duration = duration;
        }

        public int Keycode { get { return keycode; } }
        public int Octave { get { return octave; } }
        public Tone Tone { get { return tone; } }
        public int Raise { get { return raise; } }

        public int StartTime {
            get { return startTime; }
            protected set { if (value >= 0) startTime = value; }
        }
        public int Duration {
            get { return duration; }
            protected set { if (value > 0) duration = value; }
        }
        public int EndTime {
            get { return StartTime + Duration; }
            set { if (value > StartTime) Duration = value - StartTime; }
        }


        public class Builder : IBuilder<Note>
        {
            private Note buildee;
            public Builder() : this(new Note()) { }
            public Builder(Note buildee)
            {
                this.buildee = buildee;
            }

            public Builder AddKeycode(int keycode)
            {
                int octave = keycode / 12;
                int key = keycode % 12;
                int raise = 0;

                //NOTE: check if key is black
                if (!Enum.IsDefined(typeof(Tone), key))
                {
                    key--;
                    raise++;
                }

                buildee.keycode = keycode;
                buildee.octave = octave;
                buildee.tone = (Tone) key;
                buildee.raise = raise;
                return this;
            }
            public Builder AddStart(int start)
            {
                buildee.StartTime = start;
                return this;
            }
            public Builder AddDuration(int duration)
            {
                buildee.Duration = duration;
                return this;
            }
            public Builder AddVelocity(int velocity)
            {
                buildee.velocity = velocity;
                return this;
            }

            public Note Build()
            {
                return buildee;
            }
        }
    }
}
