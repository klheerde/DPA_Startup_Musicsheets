using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DPA_Musicsheets.SanfordAdapter.Reading.Lilypond
{
    class RelativeHandler : IHandler
    {
        public Song Handle(ArraySegment<string> allWordsIncludingKeyword, Song.Builder songBuilder)
        {
            int currentIndex = allWordsIncludingKeyword.Offset;
            string currentWord = allWordsIncludingKeyword.ElementAt(currentIndex++);
            Regex noteRegex = NoteHandler.REGEX;
            if (noteRegex.Match(currentWord).Success)
            {
                //TODO figure out what to do
                currentWord = allWordsIncludingKeyword.ElementAt(currentIndex++);
            }

            LilypondReader reader = MusicReader.Singleton.Readers["ly"] as LilypondReader;
            ArraySegment<string> segment = new ArraySegment<string>(allWordsIncludingKeyword.Array, currentIndex, allWordsIncludingKeyword.Count - currentIndex - 1);
            return reader.ReadWords(segment, songBuilder);
        }
    }
}
