using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.SanfordAdapter.Reading.Lilypond
{
    interface IHandler
    {
        Song Handle(ArraySegment<string> allWordsIncludingKeyword, Song.Builder songBuilder);
    }
}
