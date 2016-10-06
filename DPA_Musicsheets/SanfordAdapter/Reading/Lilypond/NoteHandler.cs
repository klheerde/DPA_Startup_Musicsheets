using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DPA_Musicsheets.SanfordAdapter.Reading.Lilypond
{
    class NoteHandler : IHandler
    {
        static readonly public Regex REGEX = new Regex(@"([a-g])(is|es)?('*)(\d{0,2})");

        public Song Handle(ArraySegment<string> allWordsIncludingKeyword, Song.Builder songBuilder)
        {
            return null;
        }
    }
}
