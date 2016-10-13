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
        private Dictionary<Regex, IHandler> handlers = new Dictionary<Regex, IHandler>();

        public LilypondReader()
        {
            handlers.Add(new Regex(@"^{$"), new OpeningBraceHandler());
            handlers.Add(new Regex(@"^\\relative$"), new RelativeHandler());
            handlers.Add(new Regex(@"^\\clef"), new ClefHandler());
            handlers.Add(new Regex(@"^\\time$"), new TimeHandler());
            handlers.Add(new Regex(@"^\\tempo$"), new TempoHandler());
            handlers.Add(new Regex(NoteHandler.REGEXSTRING), new NoteHandler());
            handlers.Add(new Regex(@"^\\repeat$"), new RepeatHandler());
            handlers.Add(new Regex(@"^\\alternative$"), new AlternativeHandler());
        }

        private string[] delimiters = { " ", "\r\n" };
        public Song Read(string filePath)
        {
            string text = System.IO.File.ReadAllText(filePath);
            return ReadFromString(text);
        }

        public Song ReadFromString(string fileText)
        {
            string[] words = fileText.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            LilypondArraySegment segment = new LilypondArraySegment(words);
            Song.Builder songBuilder = new Song.Builder();

            LilypondArraySegment.Enumerator enumerator = segment.GetEnumerator() as LilypondArraySegment.Enumerator;
            while (enumerator.MoveNext())
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
            //TODO song.CreateSequence();
            return songBuilder.GetItem();
        }

        public IHandler GetHandler(string regexString)
        {
            try
            {
                return handlers.First(p => p.Key.ToString() == regexString).Value;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
