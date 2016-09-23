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


        public string TrackName { get; set; }
        public Track Track { get; private set; }

        public TrackViewModel(Track track)
        {
            Messages = new ObservableCollection<string>();
            TrackName = track.Name;
            Track = track;
            Process();
        }

        public void Process()
        {
            Messages.Clear();

            for (int i = 0; i < Track.NoteCount; i++)
            {
                Note note = Track.GetNote(i);
                string message = "";
                if (note.Tone == Tone.Rest)
                    message += "Rest, ";
                else
                    message += String.Format("Keycode: {0}, ", note.Keycode);
                message += String.Format("start: {0}, duration {1}", note.StartTime, note.Duration);
                Messages.Add(message);
            }
        }
    }
}
