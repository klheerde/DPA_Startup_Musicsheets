using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.SanfordAdapter.Tonal
{
    enum Tone { C = 0, D = 2, E = 4, F = 5, G = 7, A = 9, B = 11 };

    class Note : Rest
    {
        private static readonly int VELOCITY = 90;

        private int keycode;
        private Tone tone;
        private int raise;

        private int velocity = VELOCITY;
        Note() { }

        public int Keycode { get { return keycode; } }
        public Tone Tone { get { return tone; } }

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
                buildee.tone = (Tone) key;
                buildee.raise = raise;
                return this;
            }
            public Builder AddStart(int start)
            {
                buildee.StartTime = start;
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
