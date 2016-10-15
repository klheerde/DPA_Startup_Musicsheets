using DPA_Musicsheets.KeyHandling;
using DPA_Musicsheets.SanfordAdapter;
using DPA_Musicsheets.SanfordAdapter.Reading;
using DPA_Musicsheets.SanfordAdapter.Reading.Lilypond;
using DPA_Musicsheets.SanfordAdapter.Writing.Lilypond;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DPA_Musicsheets
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<TrackViewModel> trackViewModels = new ObservableCollection<TrackViewModel>();
        private Player player = new Player();
        private Song song;

        private EditorWrapper editorWrapper;

        private KeyBinder keyBinder;

        //TODO not saved message when close
        public MainWindow()
        {
            InitializeComponent();
            DataContext = trackViewModels;
            
            editorWrapper = new EditorWrapper(editor);
            //NOTE: set function to be called when Song created after auto compile.
            editorWrapper.AddHandler(new Action<Song>(Editor_TimerElapsed));
            SetMainWindowKeyBindings();
        }

        private void SetMainWindowKeyBindings()
        {
            //keyBinder = new KeyBinder(this);
            ////NOTE: keybinding applied to general app.
            //keyBinder.Handlers.Add(new SaveHandler(editorWrapper));
            //keyBinder.Handlers.Add(new OpenHandler(Open));
            //keyBinder.Handlers.Add(new PdfHandler());
        }

        private void Open()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog() { Filter = "Music Files|*.mid;*.ly" };

            if (openFileDialog.ShowDialog() != true)
                return;
            string filePath = openFileDialog.FileName;
            //NOTE: show the selected file in textbox
            txt_MidiFilePath.Text = openFileDialog.SafeFileName;

            song = MusicReader.Singleton.Read(filePath);
            if (song == null)
                return;
            string lilypond = new LilypondWriter().Write(song);
            if (lilypond != null)
            {
                editor.Text = lilypond;
                //NOTE: dont file text change event. after assign because started when text changed.
                editorWrapper.Stop();
            }
            ShowMidiTracks();
        }

        private void ShowMidiTracks()
        {
            trackViewModels.Clear();
            foreach (Track track in song.Tracks)
            {
                TrackViewModel viewModel = new TrackViewModel(track);
                trackViewModels.Add(viewModel);
            }
            tabCtrl_MidiContent.SelectedIndex = 0;
        }

        //NOTE: when editor created Song, read and show.
        private void Editor_TimerElapsed(Song song)
        {
            this.song = song;
            if (song != null)
                ShowMidiTracks();
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            Open();
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            editorWrapper.Save();
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (player != null)
                player.Dispose();
            if (song != null)
                player.Play(song);
        }
        private void btn_Stop_Click(object sender, RoutedEventArgs e)
        {
            if (player != null)
                player.Dispose();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //TODO is saved?
            if (player != null)
                player.Dispose();
        }
        private void tabCtrl_MidiContent_SelectedIndexChanged(object sender, EventArgs e)
        {
            TrackViewModel viewModel = tabCtrl_MidiContent.SelectedItem as TrackViewModel;
            if (viewModel == null)
                return;
            Track track = viewModel.Track;
            TrackToSheet.Write(song, track, staff);
        }
    }
}
