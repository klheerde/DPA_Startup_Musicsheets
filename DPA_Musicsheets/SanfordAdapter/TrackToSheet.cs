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
            new TrackToSheetWriter(song, track, viewer).Write();
        }

        private sealed class TrackToSheetWriter
        {
            private Song Song { get; }
            private Track Track { get; }
            private IncipitViewerWPF Viewer { get; }
            public TrackToSheetWriter(Song song, Track track, IncipitViewerWPF viewer)
            {
                Song = song;
                Track = track;
                Viewer = viewer;
            }

            private int previousTimeSig0;
            private int previousTimeSig1;

            public void Write()
            {
                Viewer.ClearMusicalIncipit();
                Viewer.AddMusicalSymbol(new Clef(Song.Clef, 2));

                foreach (TrackPart trackPart in Track.Parts)
                {
                    if (trackPart.Repeat > 0)
                        RepeatHack("<REPEAT>");

                    WriteNoteList(trackPart, trackPart.Notes);

                    int i = 1;
                    foreach (List<Tonal.Note> alternative in trackPart.Alternatives)
                    {
                        RepeatHack("<ALT " + i + ">");
                        WriteNoteList(trackPart, alternative);
                        RepeatHack("</ALT " + i + ">");
                        //NOTE: music is notated as first alternative, followed by end repeat and other alternatives.
                        if (i++ == 1)
                            RepeatHack("</REPEAT>");
                    }
                }
            }

            private void WriteNoteList(TrackPart trackPart, List<Tonal.Note> notes)
            {
                //NOTE: [2] = ticksPerBeat, [3] = ticksPerBar.
                int[] timeSignatures = GetTimeSignatures(trackPart);
                double countsPerBar = timeSignatures[0] * (1.0 / timeSignatures[1]);

                if (timeSignatures[0] != previousTimeSig0 && timeSignatures[1] != previousTimeSig1)
                {
                    Viewer.AddMusicalSymbol(new TimeSignature(TimeSignatureType.Numbers, (uint)timeSignatures[0], (uint)timeSignatures[1]));
                    previousTimeSig0 = timeSignatures[0];
                    previousTimeSig1 = timeSignatures[1];
                }

                double currentCount = 0.0;
                foreach (Tonal.Note note in notes)
                {
                    double counts = 1.0 / note.Count;
                    currentCount += counts;
                    currentCount += (counts / 2) * (note.Dotted ? 1 : 0); //TODO dotted to dots int

                    double countDiffFromBar = currentCount - countsPerBar;
                    //TODO sound through bar if note longer
                    //MusicalSymbolDuration duration = (MusicalSymbolDuration)(countDiffFromBar <= 0 ? note.Count : 1.0 / (1.0 / note.Count - countDiffFromBar));
                    MusicalSymbolDuration duration = (MusicalSymbolDuration)note.Count;

                    MusicalSymbol symbol = BuildSymbol(note, duration);
                    Viewer.AddMusicalSymbol(symbol);

                    if (currentCount >= countsPerBar) //was ticksPerBar
                    {
                        Viewer.AddMusicalSymbol(new Barline());

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

            private int[] GetTimeSignatures(TrackPart trackPart)
            {
                int timeSig0 = trackPart.TimeSignature(0);
                int timeSig1 = trackPart.TimeSignature(1);
                int ticksPerBeat = trackPart.TimeSignature(2);

                if (timeSig0 == 0 || timeSig1 == 0)
                {
                    //NOTE: different order of execution.
                    try
                    {
                        //TODO to previous timeSig when ly. fix for midi.
                        int startTime = Song.TimeSignatureStartTimes.First(s => s >= trackPart.StartTime);
                        int[] timeSig = Song.TimeSignature(startTime);
                        timeSig0 = timeSig[0];
                        timeSig1 = timeSig[1];
                        ticksPerBeat = timeSig[2];
                    }
                    catch (InvalidOperationException)
                    {
                        int index = Track.Parts.IndexOf(trackPart);
                        //NOTE: throws exception if -1.
                        TrackPart firstTrackPart = Track.Parts.ElementAt(index - 1);
                        timeSig0 = firstTrackPart.TimeSignature(0);
                        timeSig1 = firstTrackPart.TimeSignature(1);
                        ticksPerBeat = firstTrackPart.TimeSignature(2);
                    }
                }

                int ticksPerBar = ticksPerBeat * timeSig0;

                return new int[] { timeSig0, timeSig1, ticksPerBeat, ticksPerBar };
            }

            private MusicalSymbol BuildSymbol(Tonal.Note note, MusicalSymbolDuration duration)
            {
                if (note.Tone == Tonal.Tone.R)
                {
                    return new Rest(duration);
                }

                string tone = note.Tone.ToString(); //default ToString("G");
                int octave = note.Octave + 1; //was - 1; centering octave
                int raise = note.Raise;
                return new Note(tone, raise, octave, duration, NoteStemDirection.Up, NoteTieType.None, new List<NoteBeamType>() { NoteBeamType.Single })
                {
                    //TODO multiple dots
                    NumberOfDots = note.Dotted ? 1 : 0
                };
            }

            private List<LyricsType> _lyricType = new List<LyricsType> { LyricsType.Middle };
            private void RepeatHack(string text)
            {
                //NOTE: set note to bar length.
                MusicalSymbolDuration duration = (MusicalSymbolDuration) (previousTimeSig0 * previousTimeSig1);
                Note repeatHack = new Note("G", 0, 5, duration, NoteStemDirection.Down, NoteTieType.None, new List<NoteBeamType>() { NoteBeamType.Single });
                repeatHack.Lyrics = _lyricType;
                repeatHack.LyricTexts = new List<string> { text };
                Viewer.AddMusicalSymbol(repeatHack);
                Viewer.AddMusicalSymbol(new Barline());
            }
        }
    }
}
