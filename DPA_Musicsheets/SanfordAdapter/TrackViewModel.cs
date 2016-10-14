using DPA_Musicsheets.SanfordAdapter.Tonal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.SanfordAdapter
{
    class TrackViewModel
    {
        public ObservableCollection<string> Messages { get; private set; }


        public Track Track { get; private set; }
        public string TrackName { get { return Track.Name; } }

        public TrackViewModel(Track track)
        {
            Messages = new ObservableCollection<string>();
            Track = track;
            Process();
        }

        public void Process()
        {
            Messages.Clear();

            foreach (TrackPart trackPart in Track.Parts)
            {
                Messages.Add("New TrackPart:");
                Messages.Add("\tStartTime: " + trackPart.StartTime);
                Messages.Add("\tTimeSignature: " + trackPart.TimeSignature(0) + "/" + trackPart.TimeSignature(1));
                Messages.Add("\tNotes:");

                foreach (Note note in trackPart.Notes)
                {
                    //TODO add more display info
                    string message = "\t\t";
                    message += note.Tone == Tone.R ? "Rest, " : String.Format("Keycode: {0}, ", note.Keycode);
                    message += String.Format("note: {0}, counts: {1}, dots: {2}", note.Tone, note.Count, note.Dotted ? 1 : 0);
                    Messages.Add(message);
                }
            }
        }
    }
}
