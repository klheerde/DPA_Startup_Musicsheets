using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.SanfordAdapter.Reading.Midi
{
    //NOTE: is singleton
    class MidiReader : IMusicReader
    {
        public Song Read(string filePath)
        {
            var sequence = new Sequence();
            sequence.Load(filePath);

            return MusicReader.Singleton.ReadSequence(sequence);
        }
    }
}
