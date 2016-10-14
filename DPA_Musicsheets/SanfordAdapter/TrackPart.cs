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
        public int BaseKeycode { get; private set; }
        public List<Note> Notes { get; private set; }
        public List<List<Note>> Alternatives { get; private set; }

        //TODO unnecessary?
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
            //private static int[] PREV_TIMESIG = null;

            private TrackPart buildee;
            public Note LastAddedNote { get; private set; }

            public Builder() : this(new TrackPart())
            {
                ////NOTE: only add timesig in this constructor, assume when given trackPart already has timesig.
                //if (PREV_TIMESIG != null)
                //    AddTimeSignature(PREV_TIMESIG[0], PREV_TIMESIG[1], PREV_TIMESIG[2]);
            }
            public Builder(TrackPart buildee)
            {
                this.buildee = buildee;
            }

            public Builder AddRepeat(int repeat)
            {
                buildee.Repeat = repeat;
                return this;
            } 

            public Builder AddBaseKeycode(int baseKeycode)
            {
                buildee.BaseKeycode = baseKeycode; //was 3 +
                return this;
            }

            public Builder AddNote(Note note)
            {
                if (buildee.Alternatives.Count > 0)
                {
                    List<Note> alternative = buildee.Alternatives.Last();
                    alternative.Add(note);
                    #region hide
                    //int timeSig0 = buildee.TimeSignature(0);
                    //int timeSig1 = buildee.TimeSignature(1);
                    ////NOTE: throws exception if devided by 0.
                    //double countPerBar = timeSig0 * (1.0 / timeSig1);

                    //List<Note> alternative;
                    //try
                    //{
                    //    alternative = buildee.Alternatives[currentAlternativeIndex];
                    //}
                    //catch (ArgumentOutOfRangeException e)
                    //{
                    //    alternative = new List<Note>();
                    //    buildee.Alternatives.Add(alternative);
                    //}

                    //alternative.Add(note);

                    //double countAlternatives = alternative.Sum(n => 1.0 / n.Count);
                    //if (countAlternatives >= countPerBar)
                    //{
                    //    currentAlternativeIndex++;
                    //}   
                    #endregion
                }
                else
                {
                    buildee.Notes.Add(note);
                }

                if (note.Tone != Tone.R)
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

            public Builder AddTimeSignature(int amountInBar, int countsPerBeat)
            {
                return AddTimeSignature(amountInBar, countsPerBeat, null);
            }
            //NOTE: not using song.TimeSignatureStartTimes...
            public Builder AddTimeSignature(int amountInBar, int countsPerBeat, Song song)
            {
                double quarterToSig1 = 4.0 / countsPerBeat;
                //NOTE: when added using lilypond fallback to default Sequence Division.
                double division = song == null || song.Sequence == null ? Song.DEFAULT_DIVISION : song.Sequence.Division;
                double ticksPerSig1 = division * quarterToSig1; //ticksPerBeat

                return AddTimeSignature(amountInBar, countsPerBeat, (int) ticksPerSig1);
            }
            public Builder AddTimeSignature(int amountInBar, int countsPerBeat, int ticksPerBeat)
            {
                buildee.timeSignature[0] = amountInBar;
                buildee.timeSignature[1] = countsPerBeat;
                buildee.timeSignature[2] = ticksPerBeat;
                //PREV_TIMESIG = new int[3] { amountInBar, countsPerBeat, ticksPerBeat };
                return this;
            }

            public TrackPart GetItem()
            {
                return buildee;
            }
        }
    }
}
