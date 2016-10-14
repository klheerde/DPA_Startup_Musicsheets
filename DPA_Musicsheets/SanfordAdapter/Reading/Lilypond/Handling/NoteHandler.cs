using DPA_Musicsheets.SanfordAdapter.Tonal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DPA_Musicsheets.SanfordAdapter.Reading.Lilypond.Handling
{
    class NoteHandler : IHandler
    {
        public static readonly string REGEXSTRING = @"^([a-gr])((?:is)*)((?:es)*)('*)(\,*)(\d{0,2})(\.*)$";

        public void Handle(LilypondArraySegment.Enumerator enumerator, LilypondArraySegment allWordsIncludingKeyword, Song.Builder songBuilder)
        {
            string noteString = allWordsIncludingKeyword.ElementAt(0); //gets from offset (offset + n)
            TrackPart.Builder trackPartBuilder = songBuilder.CurrentTrackBuilder.CurrentTrackPartBuilder;
            Note note = BuildNote(noteString, songBuilder);
            trackPartBuilder.AddNote(note);

            ////TODO fix ~ as note symbol
            //if (noteString == "~")
            //    return;

            //Regex regex = new Regex(REGEXSTRING);
            //Match match = regex.Match(noteString);

            //string toneString = match.Groups[1].Value;
            //string isString = match.Groups[2].Value;
            //string esString = match.Groups[3].Value;
            //string octaveUpString = match.Groups[4].Value;
            //string octaveDownString = match.Groups[5].Value; 
            //string countString = match.Groups[6].Value;
            //string dotsString = match.Groups[7].Value;

            //Note.Builder noteBuilder = new Note.Builder();

            ////NOTE: must succeed, otherwise throw.
            // Tone tone = (Tone) Enum.Parse(typeof(Tone), toneString.ToUpper());
            //noteBuilder.AddTone(tone);

            ////NOTE: can contain multiple isis or eseses
            //int raise = isString.Length / 2 - esString.Length / 2; //two chars in the words "is" and  "es"
            //noteBuilder.AddRaise(raise);

            //int amountOctaveRaise = octaveUpString.Length - octaveDownString.Length;

            //TrackPart.Builder trackPartBuilder = songBuilder.CurrentTrackBuilder.CurrentTrackPartBuilder;
            //TrackPart.Builder previousTrackPartBuilder = songBuilder.CurrentTrackBuilder.PreviousTrackPartBuilder;

            ////TODO if /relative octave by previous note
            ////NOTE: for now assume always /relative
            ////int baseOctave = trackPartBuilder.LastAddedNote == null ? previousPartBuilder == null ? trackPartBuilder.BaseOctave : trackPartBuilder.LastAddedNote.Octave;
            //int octave = BaseOctave(tone, trackPartBuilder, previousTrackPartBuilder);
            //noteBuilder.AddOctave(octave + amountOctaveRaise);

            //if (countString.Length > 0)
            //{
            //    //NOTE: throws if wrong.
            //    int count = Int32.Parse(countString);
            //    noteBuilder.AddCount(count);
            //    noteBuilder.AddDots(dotsString.Length);
            //}
            //else
            //{
            //    //NOTE: if no lastAdded throw NullPointerException
            //    int count = trackPartBuilder.LastAddedNote.Count;
            //    noteBuilder.AddCount(count);
            //    noteBuilder.AddDots(trackPartBuilder.LastAddedNote.Dotted ? 1 : 0);
            //}

            ////Tone tone;
            ////if (Enum.TryParse(toneString, out tone))
            ////    noteBuilder.AddTone(tone);

            //trackPartBuilder.AddNote(noteBuilder.GetItem());
        }

        public Note BuildNote(string noteString, Song.Builder songBuilder = null)
        {
            Regex regex = new Regex(REGEXSTRING);
            Match match = regex.Match(noteString);
            return BuildNote(match, songBuilder);
        }

        public Note BuildNote(Match noteMatch, Song.Builder songBuilder = null)
        {
            string toneString = noteMatch.Groups[1].Value;
            string isString = noteMatch.Groups[2].Value;
            string esString = noteMatch.Groups[3].Value;
            string octaveUpString = noteMatch.Groups[4].Value;
            string octaveDownString = noteMatch.Groups[5].Value;
            string countString = noteMatch.Groups[6].Value;
            string dotsString = noteMatch.Groups[7].Value;

            Note.Builder noteBuilder = new Note.Builder();

            //NOTE: must succeed, otherwise throw.
            Tone tone = (Tone)Enum.Parse(typeof(Tone), toneString.ToUpper());
            noteBuilder.AddTone(tone);

            //NOTE: can contain multiple isis or eseses.
            int raise = isString.Length / 2 - esString.Length / 2; //two chars in the words "is" and  "es"
            noteBuilder.AddRaise(raise);

            int amountOctaveRaise = octaveUpString.Length - octaveDownString.Length;

            int octave = 0;
            TrackPart.Builder trackPartBuilder = null;
            TrackPart.Builder previousTrackPartBuilder = null;
            if (songBuilder != null)
            {
                trackPartBuilder = songBuilder.CurrentTrackBuilder.CurrentTrackPartBuilder;
                previousTrackPartBuilder = songBuilder.CurrentTrackBuilder.PreviousTrackPartBuilder;

                //TODO if /relative octave by previous note
                //NOTE: for now assume always /relative
                //int baseOctave = trackPartBuilder.LastAddedNote == null ? previousPartBuilder == null ? trackPartBuilder.BaseOctave : trackPartBuilder.LastAddedNote.Octave;
                octave = BaseOctave(tone, trackPartBuilder, previousTrackPartBuilder);
            }

            noteBuilder.AddOctave(octave + amountOctaveRaise);

            if (countString.Length > 0)
            {
                //NOTE: throws if wrong.
                int count = Int32.Parse(countString);
                noteBuilder.AddCount(count);
                noteBuilder.AddDots(dotsString.Length);
            }
            else if (songBuilder != null)
            {
                //NOTE: if no lastAdded throw NullPointerException
                int count = trackPartBuilder.LastAddedNote.Count;
                noteBuilder.AddCount(count);
                noteBuilder.AddDots(trackPartBuilder.LastAddedNote.Dotted ? 1 : 0);
            }

            return noteBuilder.GetItem();
        }

        private int BaseOctave(Tone tone, TrackPart.Builder currentTrackPartBuilder, TrackPart.Builder previousTrackPartBuilder)
        {
            if (tone == Tone.R)
                return 0;

            int toneKeycode = (int)tone;

            if (currentTrackPartBuilder.LastAddedNote != null)
            {
                //return currentTrackPartBuilder.LastAddedNote.Octave;
                return BaseOctave(currentTrackPartBuilder.LastAddedNote.Keycode, toneKeycode);
            }

            if (previousTrackPartBuilder != null)
            {
                if (previousTrackPartBuilder.LastAddedNote != null)
                {
                    //return previousTrackPartBuilder.LastAddedNote.Octave;
                    return BaseOctave(previousTrackPartBuilder.LastAddedNote.Keycode, toneKeycode);
                }
            }
            
            return BaseOctave(currentTrackPartBuilder.GetItem().BaseNote.Keycode, toneKeycode);
        }

        //private int BaseOctave(Note previousNote, Tone currentTone)
        //{
        //    return Octave(previousNote.Keycode, (int)currentTone);
        //    //Tone previousTone = previousNote.Tone;
        //    //int diff = previousTone - currentTone;

        //    ////NOTE: 6 because 12 notes in octave.
        //    //int add = diff < -6 ? -1 : diff > 6 ? 1 : 0;

        //    //return previousNote.Octave + add;
        //}

        //NOTE: currentKeycode is never above 12.
        private int BaseOctave(int previousKeycode, int currentKeycode)
        {
            int diff = (previousKeycode % 12) - currentKeycode;

            //NOTE: 6 because 12 notes in octave.
            int add = diff < -6 ? -1 : diff > 6 ? 1 : 0;

            return previousKeycode / 12 + add;
        }
    }
}
