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
        private int count;
        private bool dotted;

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

        public int Count { get { return count; } }
        public bool Dotted { get { return dotted; } }


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
            public Builder AddEnd(int end, TrackPart trackPart)
            {
                double percentageOfBeatNote = song.NoteDurationInCounts(buildee);
                double percentageOfWholeNote = (1.0 / timeSignature[1]) * percentageOfBeatNote;
                for (int noteLength = 32; noteLength >= 1; noteLength /= 2)
                {
                    double absoluteNoteLength = (1.0 / noteLength);
                    if (percentageOfWholeNote <= absoluteNoteLength)
                    {
                        buildee.dotted = absoluteNoteLength * 1.5 == percentageOfBeatNote;
                        buildee.count = noteLength;
                        return this;
                    }
                }

                return this;
            }
            public Builder AddEnd(int end, Song song)
            {
                buildee.EndTime = end;

                int[] startTimes = song.TimeSignatureStartTimes;
                int index = Array.FindIndex(startTimes, s => s >= buildee.StartTime);
                int startTime = startTimes[index > -1 ? index : startTimes.Length - 1];
                int[] timeSignature = song.TimeSignature(startTime);

                
                double percentageOfBeatNote = (double)buildee.Duration / (double)timeSignature[2]; //ticksPerBeat
                double percentageOfWholeNote = (1.0 / timeSignature[1]) * percentageOfBeatNote;
                for (int noteLength = 32; noteLength >= 1; noteLength /= 2)
                {
                    double absoluteNoteLength = (1.0 / noteLength);
                    if (percentageOfWholeNote <= absoluteNoteLength)
                    {
                        buildee.dotted = absoluteNoteLength * 1.5 == percentageOfBeatNote;
                        buildee.count = noteLength;
                        return this;
                    }
                }

                return this;
            }
            public Builder AddDuration(int duration)
            {
                buildee.Duration = duration;
                return this;
            }
            public Builder AddCount(int count)
            {
                buildee.count = count;
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
