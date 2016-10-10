using DPA_Musicsheets.SanfordAdapter.Reading.Lilypond.Handling;
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

        private Dictionary<Regex, IHandler> handlers = new Dictionary<Regex, IHandler>();

        public LilypondReader()
        {
            //TODO new trackpart on '{'
            handlers.Add(new Regex(@"^\\relative$"), new RelativeHandler());
            handlers.Add(new Regex(@"^\\alternative$"), new AlternativeHandler());
            handlers.Add(new Regex(@"^\\tempo$"), new TempoHandler());
            handlers.Add(new Regex(@"^\\time$"), new TimeHandler());
            handlers.Add(new Regex(@"^\\repeat$"), new RelativeHandler());
            handlers.Add(new Regex(NoteHandler.REGEXSTRING), new NoteHandler());
        }

        private string[] delimiters = { " ", "\r\n" };
        public Song Read(string filePath)
        {
            string text = System.IO.File.ReadAllText(filePath);
            string[] words = text.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            LilypondArraySegment segment = new LilypondArraySegment(words);
            Song.Builder songBuilder = new Song.Builder();

            LilypondArraySegment.Enumerator enumerator = segment.GetEnumerator() as LilypondArraySegment.Enumerator;
            while(enumerator.MoveNext())
            {
                string word = enumerator.Current;
                foreach (Regex regex in handlers.Keys)
                {
                    if (regex.Match(word).Success)
                    {
                        //LilypondArraySegment segment = new LilypondArraySegment(words, index, words.Length - 1);
                        IHandler handler = handlers[regex];
                        handler.Handle(enumerator, segment, songBuilder);
                        break;
                    }
                }

                //NOTE: indicates word has been handled.
                segment.Start += 1;
                //NOTE: CurrentIndex is auto incemented.
            }

            Song song = songBuilder.GetItem();
            song.CreateSequence();
            return songBuilder.GetItem();
        }
    }
}
