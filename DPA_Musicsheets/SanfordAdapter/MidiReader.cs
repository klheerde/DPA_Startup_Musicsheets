using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.SanfordAdapter
{
    static class MidiReader
    {
        public static Song ReadMidi(string filePath)
        {
            var sequence = new Sequence();
            sequence.Load(filePath);
            return ReadSequence(sequence);
        }

        public static Song ReadSequence(Sequence sequence)
        {
            return new Song.Builder(sequence).Build();
        }
    }
}
