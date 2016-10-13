using DPA_Musicsheets.SanfordAdapter.Tonal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DPA_Musicsheets.SanfordAdapter.Reading.Lilypond.Handling
{
    class AlternativeHandler : IHandler
    {
        public void Handle(LilypondArraySegment.Enumerator enumerator, LilypondArraySegment allWordsIncludingKeyword, Song.Builder songBuilder)
        {
            TrackPart.Builder trackPartBuilder = songBuilder.CurrentTrackBuilder.CurrentTrackPartBuilder;

            //NOTE: indicates words "//alternative" and "{" have been handled.
            allWordsIncludingKeyword.Start += 2;
            //NOTE: makes foreach skip to and over word "{".
            enumerator.CurrentIndex += 2;

            LilypondReader lilypondReader = MusicReader.Singleton.Readers["ly"] as LilypondReader;
            NoteHandler noteHandler = lilypondReader.GetHandler(NoteHandler.REGEXSTRING) as NoteHandler;

            string previousWord = "";
            //NOTE: no more skip neccessary, done above. //skip to first '{' before first note.
            foreach (string word in allWordsIncludingKeyword)
            {
                switch (word)
                {
                    case "{":
                        trackPartBuilder.AddAlternative();
                        break;
                    //NOTE: do nothing.
                    case "|": break;
                    case "}":
                        //NOTE: two closing braces means done with alternative. do ++'s and return.
                        if (previousWord != "}")
                            break;
                        PlusPlus(enumerator, allWordsIncludingKeyword);
                        return;
                    //NOTE: must be note.
                    default:
                        noteHandler.Handle(enumerator, allWordsIncludingKeyword, songBuilder);
                        break;
                }

                previousWord = word;
                PlusPlus(enumerator, allWordsIncludingKeyword);
            }
        }

        private void PlusPlus(LilypondArraySegment.Enumerator enumerator, LilypondArraySegment allWordsIncludingKeyword)
        {
            allWordsIncludingKeyword.Start += 1;
            enumerator.CurrentIndex += 1;
        }
    }
}
