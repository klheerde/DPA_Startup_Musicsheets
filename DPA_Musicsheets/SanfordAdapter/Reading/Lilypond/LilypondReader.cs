using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DPA_Musicsheets.SanfordAdapter.Reading.Lilypond
{
    //NOTE: is singleton
    class LilypondReader : IMusicReader
    {
        //private static string[] keywords = { "\\relative", "\\clef", "\\tempo", "\\time", "\\repeat", "\\alternative", "treble", "|" };

        private Dictionary<Regex, IHandler> handlers;
        private string[] delimiters = { " ", "\r\n" };
        
        private void Init()
        {
            handlers = new Dictionary<Regex, IHandler>();

            handlers.Add(new Regex(@"\\relative"), new TempoHandler());
            handlers.Add(new Regex(@"\\tempo"), new RelativeHandler());
        }

        public Song Read(string filePath)
        {
            if (handlers == null)
                Init();

            string text = System.IO.File.ReadAllText(filePath);
            string[] words = text.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            ArraySegment<string> segment = new ArraySegment<string>(words, 0, words.Count() - 1);
            Song.Builder songBuilder = new Song.Builder();
            return ReadWords(segment, songBuilder);
        }

        public Song ReadWords(ArraySegment<string> words, Song.Builder songBuilder)
        {
            int index = words.Offset;

            foreach (string word in words)
            {
                foreach (Regex regex in handlers.Keys)
                {
                    if (regex.Match(word).Success)
                    {
                        ArraySegment<string> segment = new ArraySegment<string>(words.Array, index, words.Count() - index - 1);
                        handlers[regex].Handle(segment, songBuilder);
                        break; //can not match multiple, go to next word
                    }
                }

                index++;
            }

            Song song = songBuilder.Build();
            song.CreateSequence();
            return song;
        }
    }
}
