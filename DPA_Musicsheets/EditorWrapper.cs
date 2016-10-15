using DPA_Musicsheets.SanfordAdapter;
using DPA_Musicsheets.SanfordAdapter.Reading;
using DPA_Musicsheets.SanfordAdapter.Reading.Lilypond;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

namespace DPA_Musicsheets
{
    class EditorWrapper
    {
        public Song Song { get; private set; }

        private Timer timer;
        private bool running; //defaults to false
        private bool saved; //defaults to false
        private TextBox textBox;

        private List<Action<Song>> handlers = new List<Action<Song>>();

        //NOTE: string is reference type.
        private List<string> history = new List<string>();
        private int historyIndex = 0;

        public EditorWrapper(TextBox textBox, int milliseconds = 1500)
        {
            timer = new Timer(milliseconds);
            timer.AutoReset = false;
            this.textBox = textBox;
            timer.Elapsed += new ElapsedEventHandler(Timer_Elapsed);
            //NOTE: initialte function to be called when text changed.
            textBox.TextChanged += TextBox_TextChanged;
        }

        public void Read()
        {
            LilypondReader lilyReader = MusicReader.Singleton.Readers["ly"] as LilypondReader;
            Application.Current.Dispatcher.Invoke(() =>
            {
                string text = textBox.Text;
                history.Add(text);
                historyIndex = history.Count - 1;
                Song = lilyReader.ReadFromString(text);
                foreach (Action<Song> handler in handlers)
                    handler(Song);
                saved = false;
            });
        }

        public void Undo()
        {
            if (historyIndex <= 0)
                return;
            --historyIndex;
            //NOTE: we want to fire textChanged.
            textBox.Text = history.ElementAt(historyIndex);
            saved = false;
        }
        public void Redo()
        {
            if (historyIndex >= history.Count)
                return;
            ++historyIndex;
            //NOTE: we want to fire textChanged.
            textBox.Text = history.ElementAt(historyIndex);
            saved = false;
        }

        public void Save()
        {
            //TODO save to file
            saved = true;
            MessageBox.Show("eeeh");
        }

        #region text change timer
        //NOTE: when timer finishes, call Read().
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Read();
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

        public void AddHandler(Action<Song> handler)
        {
            handlers.Add(handler);
        }
        #endregion
    }
}
