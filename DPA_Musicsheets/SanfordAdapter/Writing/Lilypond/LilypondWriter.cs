using DPA_Musicsheets.SanfordAdapter.Tonal;
using PSAMControlLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.SanfordAdapter.Writing.Lilypond
{
    class LilypondWriter : IMusicWriter
    {
        public string Write(Song song)
        {
            return new SongToLilypondWriter(song).Write();
        }

        #region hide
        ////TODO write using registered handlers
        //public string Write(Song song)
        //{
        //    string output = "";
        //    string end = " " + System.Environment.NewLine;
        //    string close = "}" + end;
        //    //Stack <string> append = new Stack<string>();

        //    //TODO base octave
        //    output += "\\relative c' {" + end;

        //    //NOTE: all clefs handled so no out of bounds.
        //    output += "\\clef " + CLEF_TO_STRING[song.Clef] + end;

        //    //TODO tempo by beat note
        //    output += "\\tempo 4=" + song.Tempo + end;

        //    foreach (Track track in song.Tracks)
        //    {
        //        //NOTE: previous set here so previous note from previous trackpart stored.
        //        Tonal.Note previous = null;
        //        foreach (TrackPart trackPart in track.Parts)
        //        {
        //            if (trackPart.Repeat > 1)
        //            {
        //                output += "\\repeat volta " + trackPart.Repeat + " {" + end;
        //            }

        //            if (trackPart.TimeSignature(0) > 0)
        //            {
        //                output += "\\time " + trackPart.TimeSignature(0) + "/" + trackPart.TimeSignature(1) + end;
        //            }

        //            //TODO bar lines
        //            foreach (Tonal.Note note in trackPart.Notes)
        //            {
        //                //NOTE: only very first trackParts BaseOctave used.
        //                output += NoteString(note, previous, trackPart.BaseOctave);
        //                previous = note;
        //            }
        //            output += end;

        //            if (trackPart.Repeat > 1)
        //            {
        //                output += close; //\\repeat

        //                if (trackPart.Alternatives.Count > 0)
        //                {
        //                    output += "\\alternative {" + end;
        //                    foreach (List<Tonal.Note> alternative in trackPart.Alternatives)
        //                    {
        //                        output += "{ ";
        //                        foreach (Tonal.Note note in alternative)
        //                        {
        //                            //NOTE: only very first trackParts BaseOctave used.
        //                            output += NoteString(note, previous, trackPart.BaseOctave);
        //                        }
        //                        output += close; // { alt row
        //                    }

        //                    output += close; //\\alternative
        //                }
        //            }
        //        }
        //    }

        //    output += close; //\\relative

        //    return output;
        //}

        ////@"^([a-gr])((?:is)*)((?:es)*)('*)(\,*)(\d{0,2})(\.*)$"
        //private string NoteString(Tonal.Note note, Tonal.Note previous, int baseOctave)
        //{
        //    string output = "";
        //    output += note.Tone.ToString().ToLower();
        //    string raise = note.Raise > 0 ? "is" : "es";
        //    output += string.Concat(Enumerable.Repeat(raise, Math.Abs(note.Raise)));
        //    int octaveRaise = OctaveRaise(note, previous, baseOctave);

        //    output += note.Count;
        //    //output += new string('.', note.Dots);
        //    if (note.Dotted)
        //        output += ".";

        //    if (note.Tone != Tone.R)
        //        previous = note;

        //    return output + " ";
        //}

        //private int OctaveRaise(Tonal.Note note, Tonal.Note previous, int baseOctave)
        //{
        //    if (previous == null)
        //    {
        //        //NOTE: not entirely true, but good for now.
        //        return note.Octave - baseOctave;
        //    }

        //    Tone currentTone = note.Tone;
        //    Tone previousTone = previous.Tone;
        //    int toneDiff = currentTone - previousTone;
        //    int keyDiff = note.Keycode - previous.Keycode;

        //    return note.Octave - previous.Octave;
        //}
        #endregion

        private sealed class SongToLilypondWriter
        {
            public static readonly string END = " " + System.Environment.NewLine;
            public static readonly string CLOSE = "}" + END;
            private static Dictionary<ClefType, string> CLEF_TO_STRING = new Dictionary<ClefType, string>()
            {
                { ClefType.GClef, "treble" },
                { ClefType.FClef, "bass" },
                { ClefType.CClef, "alto" },
            };

            private Song Song { get; }
            public SongToLilypondWriter(Song song) { Song = song; }

            private Tonal.Note previous;
            private bool firstTrackPart;
            private int prevTimeSig0;
            private int prevTimeSig1;
            private string output;

            public string Write()
            {
                Reset();

                //TODO base octave
                output += "\\relative c' {" + END;
                //NOTE: all clefs handled so no out of bounds.
                output += "\\clef " + CLEF_TO_STRING[Song.Clef] + END;
                //TODO tempo by beat note
                output += "\\tempo 4=" + Song.Tempo + END;

                foreach (Track track in Song.Tracks)
                    WriteTrack(track);

                output += CLOSE; //\\relative

                return output;
            }

            //NOTE: failsafe if multiple usage of same SongToLilypondWriter object.
            private void Reset()
            {
                //NOTE: make sure to (re)set output string.
                output = "";
                previous = null;
                firstTrackPart = true;
                prevTimeSig0 = 0;
                prevTimeSig1 = 0;
            }

            private void WriteTrack(Track track)
            {
                bool isFirstTrackPart = firstTrackPart;
                foreach (TrackPart trackPart in track.Parts)
                {
                    #region opening trackparts
                    if (trackPart.Repeat > 1)
                        output += "\\repeat volta " + trackPart.Repeat + " {" + END;
                    //NOTE: if first trackpart brace opened from \\relative.
                    else if (!isFirstTrackPart)
                    {
                        output += "{" + END;
                        //NOTE: set this var to false, but not stack var. That happens below.
                        firstTrackPart = false;
                    }
                    #endregion

                    if (trackPart.TimeSignature(0) != prevTimeSig0 && trackPart.TimeSignature(1) != prevTimeSig1)
                    {
                        output += "\\time " + trackPart.TimeSignature(0) + "/" + trackPart.TimeSignature(1) + END;
                        prevTimeSig0 = trackPart.TimeSignature(0);
                        prevTimeSig1 = trackPart.TimeSignature(1);
                    }

                    //TODO bar lines
                    foreach (Tonal.Note note in trackPart.Notes)
                        //NOTE: only very first trackParts BaseOctave used.
                        WriteNote(note, trackPart.BaseOctave);
                    output += END;

                    #region closing trackparts
                    if (trackPart.Repeat > 1)
                    {
                        output += CLOSE; //\\repeat

                        if (trackPart.Alternatives.Count > 0)
                        {
                            output += "\\alternative {" + END;
                            foreach (List<Tonal.Note> alternative in trackPart.Alternatives)
                            {
                                output += "{ ";
                                foreach (Tonal.Note note in alternative)
                                    //NOTE: only very first trackParts BaseOctave used.
                                    WriteNote(note, trackPart.BaseOctave);
                                output += CLOSE; // { alt row
                            }
                            output += CLOSE; //\\alternative
                        }
                    }
                    else if (!isFirstTrackPart)
                    {
                        output += CLOSE; //{
                        isFirstTrackPart = false;
                    }
                    #endregion
                }
            }

            //@"^([a-gr])((?:is)*)((?:es)*)('*)(\,*)(\d{0,2})(\.*)$"
            private void WriteNote(Tonal.Note note, int baseOctave)
            {
                output += note.Tone.ToString().ToLower();
                string raise = note.Raise > 0 ? "is" : "es";
                output += string.Concat(Enumerable.Repeat(raise, Math.Abs(note.Raise)));
                int octaveRaise = OctaveRaise(note, baseOctave);
                output += note.Count;
                //output += new string('.', note.Dots);
                if (note.Dotted)
                    output += ".";
                output += " ";

                if (note.Tone != Tone.R)
                    previous = note;
            }

            private int OctaveRaise(Tonal.Note note, int baseOctave)
            {
                if (previous == null)
                    //NOTE: not entirely true, but good for now.
                    return note.Octave - baseOctave;

                Tone currentTone = note.Tone;
                Tone previousTone = previous.Tone;
                int toneDiff = currentTone - previousTone;
                int keyDiff = note.Keycode - previous.Keycode;

                return note.Octave - previous.Octave;
            }
        }
    }
}
