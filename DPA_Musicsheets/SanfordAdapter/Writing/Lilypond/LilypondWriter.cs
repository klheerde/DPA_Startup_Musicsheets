using DPA_Musicsheets.SanfordAdapter.Tonal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.SanfordAdapter.Writing.Lilypond
{
    class LilypondWriter : IMusicWriter
    {
        //TODO write using registered handlers
        public string Write(Song song)
        {
            string output = "";
            string end = " " + System.Environment.NewLine;
            Stack <string> append = new Stack<string>();

            //TODO base octave
            output += "//relative c' {" + end;
            append.Push("}");

            //TODO tempo by beat note
            output += "\\tempo 4=" + song.Tempo + end;

            foreach (Track track in song.Tracks)
            {
                foreach (TrackPart trackPart in track.Parts)
                {
                    if (trackPart.Repeat > 1)
                    {
                        output += "\\repeat volta " + trackPart.Repeat + "{" + end;
                        append.Push("}");
                    }

                    if (trackPart.TimeSignature(0) > 0)
                    {
                        output += "\\time " + trackPart.TimeSignature(0) + "/" + trackPart.TimeSignature(1) + end;
                    }

                    foreach (Note note in trackPart.Notes)
                    {
                        //public static readonly string REGEXSTRING = @"^([a-gr])((?:is)*)((?:es)*)('*)(\,*)(\d{0,2})(\.*)$";
                        output += note.Tone.ToString().ToLower();
                        string raise = note.Raise > 0 ? "is" : "es";
                        output += string.Concat(Enumerable.Repeat(raise, Math.Abs(note.Raise)));
                        //TODO octave raise or low
                        output += note.Count;
                        //output += new string('.', note.Dots);
                        if (note.Dotted)
                            output += ".";
                        output += " ";
                    }
                }
            }

            return output;
        }
    }
}
