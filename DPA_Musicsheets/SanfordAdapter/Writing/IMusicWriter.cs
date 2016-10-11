using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.SanfordAdapter.Writing
{
    interface IMusicWriter
    {
        string Write(Song song);
    }
}
