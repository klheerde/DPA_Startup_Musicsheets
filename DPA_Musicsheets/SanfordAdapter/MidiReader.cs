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

        private static string[] keywords = { "\\relative", "\\clef", "treble", "\\alternative", "|" };
        public static Song ReadLily(string filePath)
        {
            //TODO register keywords and actions.
            string text = System.IO.File.ReadAllText(filePath);
            string[] words = text.Split(' ');
            foreach (string word in words)
            {
                if (keywords.Contains(word))
                {

                }
            }
            return new Song(null);
        }

        public static Song ReadSequence(Sequence sequence)
        {
            return new Song.Builder(sequence).Build();
        }
    }
}
