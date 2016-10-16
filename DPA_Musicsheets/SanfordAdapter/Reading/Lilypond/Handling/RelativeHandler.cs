using DPA_Musicsheets.SanfordAdapter.Tonal;
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

        public static string DEFAULT = "c";

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
                //NOTE: indicates words "//relative", some note e.g. "c'" and "{" have been handled.
                allWordsIncludingKeyword.Start += 2;
                //NOTE: makes foreach skip to and over '{' word.
                enumerator.CurrentIndex += 2;
            }
            else
            {
                noteMatch = noteRegex.Match(DEFAULT);
                //NOTE: indicates words "//relative" and "{" have been handled.
                allWordsIncludingKeyword.Start += 1;
                //NOTE: makes foreach skip to and over '{' word.
                enumerator.CurrentIndex += 1;
            }

            LilypondReader lilypondReader = MusicReader.Singleton.Readers["ly"] as LilypondReader;
            NoteHandler noteHandler = lilypondReader.GetHandler(NoteHandler.REGEXSTRING) as NoteHandler;

            Note baseNote = noteHandler.BuildNote(noteMatch);
            new Note.Builder(baseNote).AddOctave(baseNote.Octave + CENTER);
            trackPartBuilder.AddBaseNote(baseNote);
        }
    }
}
