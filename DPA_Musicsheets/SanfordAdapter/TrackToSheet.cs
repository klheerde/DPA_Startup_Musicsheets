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

            foreach (TrackPart trackPart in track.Parts)
            {
                //NOTE: [2] = ticksPerBeat, [3] = ticksPerBar.
                int[] timeSignatures = GetTimeSignatures(trackPart, track, song);
                double countsPerBar = timeSignatures[0] * (1.0 / timeSignatures[1]);

                viewer.AddMusicalSymbol(new TimeSignature(TimeSignatureType.Numbers, (uint)timeSignatures[0], (uint)timeSignatures[1]));

                double currentCount = 0.0;
                foreach (Tonal.Note note in trackPart.Notes)
                {
                    double counts = 1.0 / note.Count;
                    currentCount += counts;
                    currentCount += (counts / 2) * (note.Dotted ? 1 : 0); //TODO dotted to dots int

                    double countDiffFromBar = currentCount - countsPerBar;
                    //TODO sound through bar if note longer
                    //MusicalSymbolDuration duration = (MusicalSymbolDuration)(countDiffFromBar <= 0 ? note.Count : 1.0 / (1.0 / note.Count - countDiffFromBar));
                    MusicalSymbolDuration duration = (MusicalSymbolDuration)note.Count;

                    MusicalSymbol symbol = BuildSymbol(note, duration);
                    viewer.AddMusicalSymbol(symbol);

                    if (currentCount >= countsPerBar) //was ticksPerBar
                    {
                        viewer.AddMusicalSymbol(new Barline());

                        //if (countDiffFromBar > 0)
                        //{
                        //    MusicalSymbolDuration duration2 = (MusicalSymbolDuration)(1 / countDiffFromBar);
                        //    //NOTE: only uses tone, octave and raise from note.
                        //    MusicalSymbol symbol2 = BuildSymbol(note, duration2);
                        //}

                        currentCount = 0;
                    }
                }
            }
        }

        private static int[] GetTimeSignatures(TrackPart currentDrawingTrackPart, Track parentTrack, Song parentSong)
        {
            int timeSig0 = currentDrawingTrackPart.TimeSignature(0);
            int timeSig1 = currentDrawingTrackPart.TimeSignature(1);
            int ticksPerBeat = currentDrawingTrackPart.TimeSignature(2);

            if (timeSig0 == 0 || timeSig1 == 0)
            {
                //NOTE: different order of execution
                try
                {
                    int startTime = parentSong.TimeSignatureStartTimes.First(s => s >= currentDrawingTrackPart.StartTime);
                    int[] timeSig = parentSong.TimeSignature(startTime);
                    timeSig0 = timeSig[0];
                    timeSig1 = timeSig[1];
                    ticksPerBeat = timeSig[2];
                }
                catch (InvalidOperationException e)
                {
                    int index = parentTrack.Parts.IndexOf(currentDrawingTrackPart);
                    //NOTE: throws exception if -1.
                    TrackPart firstTrackPart = parentTrack.Parts.ElementAt(index - 1);
                    timeSig0 = firstTrackPart.TimeSignature(0);
                    timeSig1 = firstTrackPart.TimeSignature(1);
                    ticksPerBeat = firstTrackPart.TimeSignature(2);
                }
            }

            int ticksPerBar = ticksPerBeat * timeSig0;
            
            return new int[] { timeSig0, timeSig1, ticksPerBeat, ticksPerBar };
        }

        private static MusicalSymbol BuildSymbol(Tonal.Note note, MusicalSymbolDuration duration)
        {
            if (note.Tone == Tonal.Tone.R)
            {
                return new Rest(duration);
            }

            string tone = note.Tone.ToString(); //default ToString("G");
            int octave = note.Octave - 1; //centering octave
            int raise = note.Raise;
            return new Note(tone, raise, octave, duration, NoteStemDirection.Up, NoteTieType.None, new List<NoteBeamType>() { NoteBeamType.Single })
            {
                //TODO multiple dots
                NumberOfDots = note.Dotted ? 1 : 0
            };
        }
    }
}
