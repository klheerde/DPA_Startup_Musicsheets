﻿using DPA_Musicsheets.SanfordAdapter.Tonal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DPA_Musicsheets.SanfordAdapter.Reading.Lilypond.Handling
{
    class RelativeHandler : OpeningBraceHandler
    {
        public static readonly int CENTER = 2;

        public override void Handle(LilypondArraySegment.Enumerator enumerator, LilypondArraySegment allWordsIncludingKeyword, Song.Builder songBuilder)
        {
            base.Handle(enumerator, allWordsIncludingKeyword, songBuilder);
            //NOTE: must exist because base.Handle gets or creates.
            TrackPart.Builder trackPartBuilder = songBuilder.CurrentTrackBuilder.CurrentTrackPartBuilder;

            int currentIndex = 0;
            string currentWord = allWordsIncludingKeyword.ElementAt(++currentIndex);
            Regex noteRegex = new Regex(NoteHandler.REGEXSTRING);
            Match noteMatch = noteRegex.Match(currentWord);
            if (noteMatch.Success)
            {
                //string toneString = noteMatch.Groups[1].Value;
                //string octaveUpString = noteMatch.Groups[4].Value;
                //string octaveDownString = noteMatch.Groups[5].Value;
                //int octave = octaveUpString.Length - octaveDownString.Length;

                LilypondReader lilypondReader = MusicReader.Singleton.Readers["ly"] as LilypondReader;
                NoteHandler noteHandler = lilypondReader.GetHandler(NoteHandler.REGEXSTRING) as NoteHandler;

                Note baseNote = noteHandler.BuildNote(noteMatch);
                new Note.Builder(baseNote).AddOctave(baseNote.Octave + CENTER);
                trackPartBuilder.AddBaseNote(baseNote);

                ////NOTE: throws if does not exists.
                //Tone tone = (Tone) Enum.Parse(typeof(Tone), toneString.ToUpper());
                //int baseKeycode = 12 * (octave + center) + (int)tone;
                //trackPartBuilder.AddBaseOctave(lilypondCenterOctaveNotes + octave);

                //NOTE: indicates words "//relative", some note e.g. "c'" and "{" have been handled.
                allWordsIncludingKeyword.Start += 2;
                //NOTE: makes foreach skip to and over '{' word.
                enumerator.CurrentIndex += 2;
            }

            //int offset = allWordsIncludingKeyword.Offset + currentIndex;
            //int count = allWordsIncludingKeyword.Count - offset - 1;
            //ArraySegment<string> segment = new ArraySegment<string>(allWordsIncludingKeyword.Array, offset, count);
            //LilypondReader reader = MusicReader.Singleton.Readers["ly"] as LilypondReader;
            //return reader.FindNextHandler(segment, songBuilder);
        }
    }
}
