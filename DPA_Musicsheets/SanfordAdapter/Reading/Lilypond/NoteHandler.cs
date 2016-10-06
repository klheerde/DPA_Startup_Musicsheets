using DPA_Musicsheets.SanfordAdapter.Tonal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DPA_Musicsheets.SanfordAdapter.Reading.Lilypond
{
    class NoteHandler : IHandler
    {
        public static readonly string REGEXSTRING = @"([a-g])((?:is)*)((?:es)*)('*)(\,*)(\d{0,2})";

        public Song Handle(ArraySegment<string> allWordsIncludingKeyword, Song.Builder songBuilder)
        {
            string noteString = allWordsIncludingKeyword.ElementAt(0); //gets from offset (offset + n)
            Regex regex = new Regex(REGEXSTRING);
            Match match = regex.Match(noteString);

            string toneString = match.Groups[1].Value;
            string isString = match.Groups[2].Value;
            string esString = match.Groups[3].Value;
            string octaveUpString = match.Groups[4].Value;
            string octaveDownString = match.Groups[5].Value;
            string countString = match.Groups[6].Value;
        
            Note.Builder noteBuilder = new Note.Builder();

            //NOTE: must succeed, otherwise throw.
            Tone tone = (Tone) Enum.Parse(typeof(Tone), toneString);
            noteBuilder.AddTone(tone);

            //NOTE: is or es always longer than 2, can contain multiple isis or eseses
            if (isString.Length > 1)
                noteBuilder.AddRaise(isString.Length / 2); //two chars in the word "is"
            else if (esString.Length > 1)
                noteBuilder.AddRaise(-esString.Length / 2); //two chars in the word "es"

            int amountOctaveRaise;
            if (octaveUpString.Length > 0)
                amountOctaveRaise = octaveUpString.Length;
            else if (octaveDownString.Length > 0)
                amountOctaveRaise = -octaveDownString.Length;

            //TODO build current & prev note in trackpartbuilder
            //TODO noteBuilder raise from prev note + amountOcRaise

            //Tone tone;
            //if (Enum.TryParse(toneString, out tone))
            //    noteBuilder.AddTone(tone);

            TrackPart.Builder trackPartBuilder = songBuilder.CurrentTrackBuilder.CurrentTrackPartBuilder;

            //TODO dont forget to noteBuilder.AddKeycode()!

            return null;
        }
    }
}
