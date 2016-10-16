using DPA_Musicsheets.KeyHandling;
using DPA_Musicsheets.SanfordAdapter;
using DPA_Musicsheets.SanfordAdapter.Reading;
using DPA_Musicsheets.SanfordAdapter.Reading.Lilypond;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DPA_Musicsheets
{
    class EditorWrapper
    {
        public Song Song { get; private set; }
        public bool Saved { get; private set; } = false;
        public bool running { get; private set; } = false;

        private Timer timer;
        private TextBox textBox;
        private KeyBinder keyBinder;
        private string savedFile = null;

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
            //textBox.UndoLimit = 0;
            textBox.AcceptsReturn = true;
            //NOTE: initialte function to be called when text changed.
            textBox.TextChanged += TextBox_TextChanged;
            SetEditorKeyBindings();
        }

        public void Read()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                string text = textBox.Text;
                Build(text);
                Saved = false;
                //NOTE: only create new history point if lilypond is valid.
                if (Song == null)
                    return;
                if (historyIndex < history.Count - 1)
                    //NOTE: remove all following current history point.
                    history.RemoveRange(historyIndex, history.Count - historyIndex);
                history.Add(text);
                //NOTE: set current history point to last.
                historyIndex = history.Count - 1;
            });
        }

        public void Build(string text)
        {
            LilypondReader lilyReader = MusicReader.Singleton.Readers["ly"] as LilypondReader;
            Song = lilyReader.ReadFromString(text);
            foreach (Action<Song> handler in handlers)
                handler(Song);
        }

        public void Undo()
        {
            if (historyIndex <= 0)
                return;
            --historyIndex;
            //NOTE: we want to fire textChanged.
            textBox.Text = history.ElementAt(historyIndex);
            //NOTE: stop timer from firing Read().
            Stop();
            //NOTE: only build, without set history point.
            Build(textBox.Text);
            Saved = false;
        }

        public void Redo()
        {
            if (historyIndex >= history.Count)
                return;
            ++historyIndex;
            //NOTE: we want to fire textChanged.
            textBox.Text = history.ElementAt(historyIndex);
            //NOTE: stop timer from firing Read().
            Stop();
            //NOTE: only build, without set history point.
            Build(textBox.Text);
            Saved = false;
        }

        public void Save()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "lilypond files |*.ly";
            saveFileDialog.FilterIndex = 2;
            saveFileDialog.RestoreDirectory = true;
            if (savedFile != null)
                saveFileDialog.FileName = savedFile;
            if (saveFileDialog.ShowDialog() != true)
                return;
            System.IO.File.WriteAllText(saveFileDialog.FileName, textBox.Text);
            savedFile = saveFileDialog.SafeFileName;
            Saved = true;
        }

        private void SetEditorKeyBindings()
        {
            keyBinder = new KeyBinder(textBox);
            keyBinder.Handlers.Add(new KeyHandler(new[]{ Key.LeftCtrl, Key.Z }, Undo));
            keyBinder.Handlers.Add(new KeyHandler(new[]{ Key.LeftCtrl, Key.LeftShift, Key.Z }, Redo));
            keyBinder.Handlers.Add(new KeyHandler(new[]{ Key.LeftAlt, Key.C }, InsertClef));
            keyBinder.Handlers.Add(new KeyHandler(new[]{ Key.LeftAlt, Key.S }, InsertTempo));
            keyBinder.Handlers.Add(new KeyHandler(new[]{ Key.LeftAlt, Key.T }, InsertTime));
            keyBinder.Handlers.Add(new KeyHandler(new[]{ Key.LeftAlt, Key.T, Key.D4 }, InsertTime));
            keyBinder.Handlers.Add(new KeyHandler(new[]{ Key.LeftAlt, Key.T, Key.D3 }, InsertTime3));
            keyBinder.Handlers.Add(new KeyHandler(new[]{ Key.LeftAlt, Key.T, Key.D6 }, InsertTime6));
        }

        private void InsertText(string text)
        {
            int selectionIndex = textBox.SelectionStart;
            textBox.Text = textBox.Text.Insert(selectionIndex, text);
            textBox.SelectionStart = selectionIndex + text.Length;
        }

        private void InsertClef() { InsertText("\\cleff treble"); }
        private void InsertTempo() { InsertText("\\tempo 4=120"); }
        private void InsertTime() { InsertText("\\time 4/4"); }
        private void InsertTime3() { InsertText("\\time 3/4"); }
        private void InsertTime6() { InsertText("\\time 6/8"); }

        //TODO barlines where neccesary

        #region text change timer
        //NOTE: when timer finishes, call Read().
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Read();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Saved = false;
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


        //public event PropertyChangedEventHandler PropertyChanged;
        //protected void OnPropertyChanged(string propertyName)
        //{
        //     PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}
    }
}
