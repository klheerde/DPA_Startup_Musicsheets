using PSAMControlLibrary;
using PSAMWPFControlLibrary;
using Sanford.Multimedia.Midi;
using TrackX = Sanford.Multimedia.Midi.Track;
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
                int timeSig0 = trackPart.TimeSignature(0);
                int timeSig1 = trackPart.TimeSignature(1);
                int ticksPerBeat = trackPart.TimeSignature(2);
                int ticksPerBar = ticksPerBeat * timeSig0;

                viewer.AddMusicalSymbol(new TimeSignature(TimeSignatureType.Numbers, (uint)timeSig0, (uint)timeSig1));

                int currentCount = 0;
                foreach (Tonal.Note note in trackPart.Notes)
                {
                    if (currentCount >= ticksPerBar)
                    {
                        viewer.AddMusicalSymbol(new Barline());
                        currentCount = 0;
                    }

                    MusicalSymbol symbol;
                    MusicalSymbolDuration duration = (MusicalSymbolDuration)note.Count;

                    if (note.Tone == Tonal.Tone.Rest)
                    {
                        symbol = new Rest(duration);
                    }
                    else
                    {
                        string tone = note.Tone.ToString(); //default ToString("G");
                        int octave = note.Octave - 1; //centering octave
                        int raise = note.Raise;
                        symbol = new Note(tone, raise, octave, duration, NoteStemDirection.Up, NoteTieType.None, new List<NoteBeamType>() { NoteBeamType.Single })
                        {
                            //TODO multiple dots
                            NumberOfDots = note.Dotted ? 1 : 0
                        };
                    }

                    viewer.AddMusicalSymbol(symbol);
                    currentCount += note.Duration;
                }
            }
        }
    }
}
