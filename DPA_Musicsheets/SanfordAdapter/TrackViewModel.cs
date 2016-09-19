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
        public ObservableCollection<string> Messages = new ObservableCollection<string>();

        private Track track;

        public TrackViewModel(Track track)
        {
            this.track = track;
            Process();
        }

        public void Process()
        {
            Messages.Clear();

            for (int i = 0; i < track.EventCount; i++)
            {
                Rest rest = track.GetEvent(i);
                string message = "";
                if (rest is Note)
                    message += String.Format("keycode: {0}, ", (rest as Note).Keycode);
                message += String.Format("start: {0}, duration {1}", rest.StartTime, rest.Duration);
                Messages.Add(message);
            }
        }
    }
}
