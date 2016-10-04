using PSAMControlLibrary;
using PSAMWPFControlLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.SanfordAdapter
{
    class TrackToSheet
    {
        public static void Write(Song song, Track track, IncipitViewerWPF viewer)
        {
            viewer.ClearMusicalIncipit();
            viewer.AddMusicalSymbol(new Clef(ClefType.GClef, 2));

            //only show first for now.
            foreach (TrackPart trackPart in track.Parts)
            {
                foreach (Tonal.Note note in trackPart.Notes)
                {
                    MusicalSymbol symbol;
                    MusicalSymbolDuration duration = (MusicalSymbolDuration)note.Count;

                    if (note.Tone == Tonal.Tone.Rest)
                    {
                        symbol = new Rest(duration);
                    }
                    else
                    {
                        string tone = note.Tone.ToString(); //default ToString("G");
                        int octave = note.Octave;
                        int raise = note.Raise;
                        symbol = new Note(tone, raise, octave, duration, NoteStemDirection.Up, NoteTieType.None, new List<NoteBeamType>() { NoteBeamType.Single });
                    }
                    viewer.AddMusicalSymbol(symbol);
                }
            }

            //int countDuration = song.TimeSignature(1);
            //int amountOfCountDurationInBar = song.TimeSignature(0) * countDuration;
            //for (int i = 0; i < track.NoteCount; i++)
            //{
            //    var note = track.GetNote(i);

            //    MusicalSymbol symbol;
            //    if (note.Tone == Tonal.Tone.Rest)
            //        symbol = new Rest(MusicalSymbolDuration.Quarter);
            //    else
            //    {
            //        string tone = note.Tone.ToString(); //default ToString("G");
            //        int octave = note.Octave;
            //        int raise = note.Raise;
            //        double counts = song.NoteDurationInCounts(note);
            //        double duration = countDuration / counts;
            //        MusicalSymbolDuration d = (MusicalSymbolDuration) duration;
            //        symbol = new Note(tone, raise, octave, d, NoteStemDirection.Up, NoteTieType.None, new List<NoteBeamType>() { NoteBeamType.Start, NoteBeamType.Start });
            //    }

            //    viewer.AddMusicalSymbol(symbol);
            //}
        }
    }
}
