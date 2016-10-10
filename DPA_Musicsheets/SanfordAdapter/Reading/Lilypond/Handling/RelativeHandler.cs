using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DPA_Musicsheets.SanfordAdapter.Reading.Lilypond.Handling
{
    class RelativeHandler : IHandler
    {
        public void Handle(LilypondArraySegment.Enumerator enumerator, LilypondArraySegment allWordsIncludingKeyword, Song.Builder songBuilder)
        {
            //TODO find out when new track should be created. find lilypond track notation

            Track.Builder trackBuilder;
            try
            {
                trackBuilder = songBuilder.CurrentTrackBuilder;
            }
            catch (ArgumentOutOfRangeException e)
            {
                trackBuilder = new Track.Builder();
                songBuilder.AddTrackBuilder(trackBuilder);
            }

            TrackPart.Builder trackPartBuilder = new TrackPart.Builder();
            trackBuilder.AddTrackPartBuilder(trackPartBuilder);

            int currentIndex = 0;
            string currentWord = allWordsIncludingKeyword.ElementAt(++currentIndex);
            Regex noteRegex = new Regex(NoteHandler.REGEXSTRING);
            Match noteMatch = noteRegex.Match(currentWord);
            if (noteMatch.Success)
            { 
                string octaveUpString = noteMatch.Groups[4].Value;
                string octaveDownString = noteMatch.Groups[5].Value;
                int octave = octaveUpString.Length - octaveDownString.Length;
                trackPartBuilder.AddBaseOctave(octave);

                //NOTE: indicates words "//relative", some note e.g. "c'" and "{" have been handled.
                allWordsIncludingKeyword.Start += 2;
                //NOTE: makes foreach skip to and over '{' word.
                enumerator.CurrentIndex += 2;
            }

            //int offset = allWordsIncludingKeyword.Offset + currentIndex;
            //int count = allWordsIncludingKeyword.Count - offset - 1;
            //ArraySegment<string> segment = new ArraySegment<string>(allWordsIncludingKeyword.Array, offset, count);
            //LilypondReader reader = MusicReader.Singleton.Readers["ly"] as LilypondReader;
            //return reader.FindNextHandler(segment, songBuilder);
        }
    }
}
