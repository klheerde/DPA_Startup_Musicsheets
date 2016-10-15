using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Controls;

namespace DPA_Musicsheets.SanfordAdapter
{
    class EditorWrapper
    {
        private Timer timer;
        private bool running; //defaults to false

        public EditorWrapper(TextBox textBox, ElapsedEventHandler handler, int milliseconds = 1500)
        {
            timer = new Timer(milliseconds);
            timer.AutoReset = false;
            OnElapsed(handler);
            textBox.TextChanged += TextBox_TextChanged;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Go();
        }

        public void Go()
        {
            if (running)
                timer.Stop();
            timer.Start();
            running = true;
        }

        public void Stop()
        {
            timer.Stop();
            running = false;
        }

        public void OnElapsed(ElapsedEventHandler handler)
        {
            timer.Elapsed += handler;
        }
    }
}
