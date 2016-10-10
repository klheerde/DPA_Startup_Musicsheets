using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.SanfordAdapter.Reading.Lilypond
{
    class LilypondArraySegment : IEnumerable<string>
    {
        public string[] Array { get; set; }
        public int Start { get; set; }
        public int End { get; set; }

        public LilypondArraySegment(string[] words) : this(words, 0, words.Length - 1) { }
        public LilypondArraySegment(string[] words, int start, int end)
        {
            Array = words;
            Start = start;
            End = end;
        }

        #region IEnumerable<T>
        public IEnumerator<string> GetEnumerator()
        {
            return new Enumerator(this);
        }
        #endregion
        #region IEnumerable
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }
        #endregion

        public class Enumerator : IEnumerator<string>
        {
            private LilypondArraySegment adapter;
            public int CurrentIndex { get; set; }

            internal Enumerator(LilypondArraySegment adapter)
            {
                this.adapter = adapter;
                CurrentIndex = adapter.Start - 1;
            }

            public bool MoveNext()
            {
                if (CurrentIndex < adapter.End)
                    return (++CurrentIndex < adapter.End);

                return false;
            }

            public string Current {
                get {
                    if (CurrentIndex < adapter.Start || CurrentIndex >= adapter.End)
                        return null;

                    return adapter.Array[CurrentIndex];
                }
            }

            object IEnumerator.Current { get { return Current; } }

            void IEnumerator.Reset()
            {
                CurrentIndex = adapter.Start - 1;
            }

            public void Dispose() { }
        }
    }
}
