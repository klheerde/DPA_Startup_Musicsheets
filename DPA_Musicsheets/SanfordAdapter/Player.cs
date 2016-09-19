using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.SanfordAdapter
{
    public class Player : IDisposable
    {
        private OutputDevice outputDevice;
        private Sequence sequence;
        private Sequencer sequencer;

        public Player() : this(new OutputDevice(0)) { }
        public Player(OutputDevice outputDevice)
        {
            this.outputDevice = outputDevice;
            sequencer = new Sequencer();
            sequencer.ChannelMessagePlayed += (object sender, ChannelMessageEventArgs e) =>
            {
                outputDevice.Send(e.Message);
            };
            sequencer.PlayingCompleted += (playingSender, playingEvent) =>
            {
                sequencer.Stop();
            };
        }

        public void Play(Song song)
        {
            //TODO
        }

        public void Play(string filePath)
        {
            sequence = new Sequence();
            sequence.LoadCompleted += (object sender, AsyncCompletedEventArgs e) =>
            {
                Play(sequence);
            };
            sequence.LoadAsync(filePath);
        }

        public void Play(Sequence sequence)
        {
            sequencer.Sequence = this.sequence = sequence;
            StartPlaying();
        }

        private void StartPlaying()
        {
            sequencer.Start();
        }

        public void Dispose()
        {
            outputDevice.Close();
            sequencer.Stop();
        }
    }
}
