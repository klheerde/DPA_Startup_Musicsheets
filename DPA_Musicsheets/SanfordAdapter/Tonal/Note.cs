using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.SanfordAdapter.Tonal
{
    public enum Tone { C = 0, D = 2, E = 4, F = 5, G = 7, A = 9, B = 11, R = -1 /* Rest -1 outside octave scope */ };

    public class Note
    {
        private static readonly int VELOCITY = 90;

        public Note()
        {
            Tone = Tone.R;
            Velocity = VELOCITY;
        }

        //public Note(int startTime, int duration) : this()
        //{
        //    StartTime = startTime;
        //    Duration = duration;
        //}

        public int Keycode { get { return 12 * Octave + ((int) Tone >= 0 ? (int) Tone : -999) + Raise; } }
        public int Octave { get; private set; }
        public Tone Tone { get; private set; }
        public int Raise { get; private set; }

        ////TODO unnecessary?
        //private int startTime;
        //public int StartTime {
        //    get { return startTime; }
        //    protected set { if (value >= 0) startTime = value; }
        //}
        ////TODO unnecessary?
        //private int duration;
        //public int Duration {
        //    get { return duration; }
        //    protected set { if (value > 0) duration = value; }
        //}
        ////TODO unnecessary?
        //public int EndTime {
        //    get { return StartTime + Duration; }
        //    set { if (value > StartTime) Duration = value - StartTime; }
        //}

        public int Count { get; private set; }
        //TODO make int Dots
        public bool Dotted { get; private set; }

        public int Velocity { get; private set; }


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

                buildee.Octave = octave;
                buildee.Tone = (Tone) key;
                buildee.Raise = raise;
                return this;
            }

            public Builder AddTone(Tone tone)
            {
                buildee.Tone = tone;
                return this;
            }
            public Builder AddOctave(int octave)
            {
                buildee.Octave = octave;
                return this;
            }
            public Builder AddRaise(int raise)
            {
                buildee.Raise = raise;
                return this;
            }

            //public Builder AddStart(int start)
            //{
            //    buildee.StartTime = start;
            //    return this;
            //}

            ////TODO change Song to TrackPart or parent reference in Note. Use TrackPart in Builder contructor as parent (force)
            //public Builder AddEnd(int end, Song song)
            //{
            //    buildee.EndTime = end;

            //    int[] startTimes = song.TimeSignatureStartTimes;
            //    int index = Array.FindIndex(startTimes, s => s > buildee.StartTime); //finds first hit which is bigger or equal
            //    int startTime = startTimes[index > -1 ? index - 1 : startTimes.Length - 1]; //use previous one, or last
            //    int[] timeSignature = song.TimeSignature(startTime);

                
            //    double percentageOfBeatNote = (double)buildee.Duration / (double)timeSignature[2]; //ticksPerBeat
            //    double percentageOfWholeNote = (1.0 / timeSignature[1]) * percentageOfBeatNote;
            //    for (int noteLength = 1; noteLength <= 32; noteLength *= 2)
            //    {
            //        double absoluteNoteLength = (1.0 / noteLength);
            //        if (percentageOfWholeNote >= absoluteNoteLength)
            //        {
            //            buildee.Dotted = absoluteNoteLength * 1.5 == percentageOfWholeNote;
            //            buildee.Count = noteLength;
            //            return this;
            //        }
            //    }

            //    return this;
            //}


            ////TODO to midireader
            //public Builder AddEnd(int end, TrackPart trackPart)
            //{
            //    buildee.EndTime = end;

            //    int timeSig1 = trackPart.TimeSignature(1);
            //    int ticksPerBeat = trackPart.TimeSignature(2);

            //    double percentageOfBeatNote = (double)buildee.Duration / (double)ticksPerBeat; //ticksPerBeat
            //    double percentageOfWholeNote = (1.0 / timeSig1) * percentageOfBeatNote;
            //    for (int noteLength = 1; noteLength <= 32; noteLength *= 2)
            //    {
            //        double absoluteNoteLength = (1.0 / noteLength);
            //        if (percentageOfWholeNote >= absoluteNoteLength)
            //        {
            //            buildee.Dotted = absoluteNoteLength * 1.5 == percentageOfWholeNote;
            //            buildee.Count = noteLength;
            //            return this;
            //        }
            //    }

            //    return this;
            //}

            //public Builder AddDuration(int duration)
            //{
            //    buildee.Duration = duration;
            //    return this;
            //}
            public Builder AddCount(int count)
            {
                buildee.Count = count;
                return this;
            }

            public Builder AddDots(int dots)
            {
                buildee.Dotted = dots > 0;
                return this;
            }

            public Builder AddVelocity(int velocity)
            {
                buildee.Velocity = velocity;
                return this;
            }

            public Note GetItem()
            {
                return buildee;
            }
        }
    }
}
