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
            Track.Builder trackBuilder = new Track.Builder();
            songBuilder.AddTrackBuilder(trackBuilder);

            int currentIndex = 0;
            string currentWord = allWordsIncludingKeyword.ElementAt(currentIndex++);
            Regex noteRegex = new Regex(NoteHandler.REGEXSTRING);
            if (noteRegex.Match(currentWord).Success)
            {
                //TODO figure out what to do, maybe CurrentTrackBuilder.key-ish?
                currentWord = allWordsIncludingKeyword.ElementAt(currentIndex++);
            }

            int offset = allWordsIncludingKeyword.Offset + currentIndex;
            int count = allWordsIncludingKeyword.Count - offset - 1;
            ArraySegment<string> segment = new ArraySegment<string>(allWordsIncludingKeyword.Array, offset, count);
            LilypondReader reader = MusicReader.Singleton.Readers["ly"] as LilypondReader;
            return reader.ReadWords(segment, songBuilder);
        }
    }
}
