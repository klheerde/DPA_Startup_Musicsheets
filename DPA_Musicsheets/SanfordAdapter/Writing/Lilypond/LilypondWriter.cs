using DPA_Musicsheets.SanfordAdapter.Reading.Lilypond.Handling;
using DPA_Musicsheets.SanfordAdapter.Tonal;
using PSAMControlLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
            private bool isFirstTrackPartAfterRelative;
            private int baseKeycode;
            private int prevTimeSig0;
            private int prevTimeSig1;
            private string output;

            public string Write()
            {
                Reset();

                try
                {
                    output += "\\relative ";
                    WriteBaseNote();
                    output += "{" + END;

                    //NOTE: all clefs handled so no out of bounds.
                    output += "\\clef " + CLEF_TO_STRING[Song.Clef] + END;
                    //TODO tempo by beat note
                    output += "\\tempo 4=" + Song.Tempo + END;

                    foreach (Track track in Song.Tracks)
                        WriteTrack(track);

                    output += CLOSE; //\\relative

                    return output;
                }
                catch (Exception e)
                {
                    string errorTitle = "Bad Song";
                    string errorMessage = "Reading Song failed."/* + System.Environment.NewLine/* + e.Message*/;
                    MessageBox.Show(errorMessage, errorTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return null;
                }
            }

            //NOTE: failsafe if multiple usage of same SongToLilypondWriter object.
            private void Reset()
            {
                //NOTE: make sure to (re)set output string.
                output = "";
                previous = null;
                isFirstTrackPartAfterRelative = true;
                baseKeycode = RelativeHandler.CENTER * 12;
                prevTimeSig0 = 0;
                prevTimeSig1 = 0;
            }

            private void WriteBaseNote()
            {
                Tonal.Note baseNote = null;
                if (Song.Tracks.Count > 0 && Song.Tracks.First().Parts.Count > 0 && 
                    (baseNote = Song.Tracks.First().Parts.First().BaseNote) != null)
                {
                    WriteNote(baseNote, baseKeycode, false);
                }
                else
                {
                    output += "c ";
                }
            }

            private void WriteTrack(Track track)
            {
                foreach (TrackPart trackPart in track.Parts)
                {
                    #region opening trackparts
                    if (trackPart.Repeat > 1)
                        output += "\\repeat volta " + trackPart.Repeat + " {" + END;
                    //NOTE: if first trackpart brace opened from \\relative.
                    else if (!isFirstTrackPartAfterRelative)
                        output += "{" + END;
                    #endregion

                    if (trackPart.TimeSignature(0) != prevTimeSig0 || trackPart.TimeSignature(1) != prevTimeSig1)
                    {
                        output += "\\time " + trackPart.TimeSignature(0) + "/" + trackPart.TimeSignature(1) + END;
                        prevTimeSig0 = trackPart.TimeSignature(0);
                        prevTimeSig1 = trackPart.TimeSignature(1);
                    }

                    //TODO bar lines
                    foreach (Tonal.Note note in trackPart.Notes)
                        //NOTE: only very first trackParts BaseOctave used.
                        WriteNote(note, trackPart.BaseNote == null ? this.baseKeycode : trackPart.BaseNote.Keycode);
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
                                    WriteNote(note, trackPart.BaseNote == null ? this.baseKeycode : trackPart.BaseNote.Keycode);
                                output += CLOSE; // { alt row
                            }
                            output += CLOSE; //\\alternative
                        }
                    }
                    else if (!isFirstTrackPartAfterRelative)
                    {
                        output += CLOSE; //{
                    }
                    #endregion

                    this.isFirstTrackPartAfterRelative = false;
                }
            }

            //@"^([a-gr])((?:is)*)((?:es)*)('*)(\,*)(\d{0,2})(\.*)$"
            private void WriteNote(Tonal.Note note, int baseKeycode, bool setPrevious = true)
            {
                output += note.Tone.ToString().ToLower();

                if (note.Tone != Tone.R)
                {
                    string raise = note.Raise > 0 ? "is" : "es";
                    output += string.Concat(Enumerable.Repeat(raise, Math.Abs(note.Raise)));
                    int octaveRaiseAmount = OctaveRaise(note, baseKeycode);
                    string octaveRaiseString = octaveRaiseAmount > 0 ? "'" : ",";
                    output += string.Concat(Enumerable.Repeat(octaveRaiseString, Math.Abs(octaveRaiseAmount)));
                }

                output += note.Count;
                //output += new string('.', note.Dots);
                if (note.Dotted)
                    output += ".";
                output += " ";

                if (setPrevious && note.Tone != Tone.R)
                    previous = note;
            }

            private int OctaveRaise(Tonal.Note note, int baseKeycode)
            {
                int diff;
                if (previous == null)
                {
                    //NOTE: baseOctave is C keycode.
                    //TODO custom base octave key
                    diff = note.Keycode - baseKeycode;
                }
                else
                {
                    int previousKeycode = previous.Keycode;
                    int noteKeycode = note.Keycode;
                    diff = noteKeycode - previousKeycode;
                }

                //NOTE: 12 notes in an octave.
                int octaves = diff / 12;
                int left = diff % 12;

                this.baseKeycode = baseKeycode;

                return octaves + (left > 6 ? 1 : left < -6 ? -1 : 0);

                //Tone currentTone = note.Tone;
                //Tone previousTone = previous.Tone;
                //int toneDiff = currentTone - previousTone;
                //int keyDiff = note.Keycode - previous.Keycode - toneDiff;

                ////int add = diff < -6 ? -1 : diff > 6 ? 1 : 0;
                //if (keyDiff < -6 || keyDiff > 6)
                //{

                //}

                //return note.Octave - previous.Octave;
            }
        }
    }
}
