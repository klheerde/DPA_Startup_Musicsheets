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
            string close = "}" + end;
            //Stack <string> append = new Stack<string>();

            //TODO base octave
            output += "\\relative c' {" + end;

            //TODO tempo by beat note
            output += "\\tempo 4=" + song.Tempo + end;

            foreach (Track track in song.Tracks)
            {
                foreach (TrackPart trackPart in track.Parts)
                {
                    if (trackPart.Repeat > 1)
                    {
                        output += "\\repeat volta " + trackPart.Repeat + " {" + end;
                    }

                    if (trackPart.TimeSignature(0) > 0)
                    {
                        output += "\\time " + trackPart.TimeSignature(0) + "/" + trackPart.TimeSignature(1) + end;
                    }

                    //TODO bar lines
                    foreach (Note note in trackPart.Notes)
                    {
                        output += NoteString(note);
                    }
                    output += end;

                    if (trackPart.Repeat > 1)
                    {
                        output += close; //\\repeat

                        if (trackPart.Alternatives.Count > 0)
                        {
                            output += "\\alternative {" + end;
                            foreach (List<Note> alternative in trackPart.Alternatives)
                            {
                                output += "{ ";
                                foreach (Note note in alternative)
                                {
                                    output += NoteString(note);
                                }
                                output += close; // { alt row
                            }

                            output += close; //\\alternative
                        }
                    }
                }
            }

            output += close; //\\relative

            return output;
        }

        private string NoteString(Note note)
        {
            string output = "";
            //public static readonly string REGEXSTRING = @"^([a-gr])((?:is)*)((?:es)*)('*)(\,*)(\d{0,2})(\.*)$";
            output += note.Tone.ToString().ToLower();
            string raise = note.Raise > 0 ? "is" : "es";
            output += string.Concat(Enumerable.Repeat(raise, Math.Abs(note.Raise)));
            //TODO octave raise or low
            output += note.Count;
            //output += new string('.', note.Dots);
            if (note.Dotted)
                output += ".";

            return output + " ";
        }
    }
}
