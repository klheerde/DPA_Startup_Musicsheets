﻿using DPA_Musicsheets.SanfordAdapter.Reading.Midi;
using DPA_Musicsheets.SanfordAdapter.Reading.Lilypond;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sanford.Multimedia.Midi;
using System.Windows;

namespace DPA_Musicsheets.SanfordAdapter.Reading
{
    class MusicReader
    {
        static private MusicReader singleton;
        static public MusicReader Singleton {
            get {
                if (singleton == null)
                    singleton = new MusicReader();
                return singleton;
            }
        }

        public Dictionary<string, IMusicReader> Readers { get; private set; }

        MusicReader()
        {
            Readers = new Dictionary<string, IMusicReader>();

            //NOTE: key is file extension
            Readers.Add("mid", new MidiReader());
            Readers.Add("ly", new LilypondReader());
        }

        public Song Read(string filePath)
        {
            int extensionIndex = filePath.LastIndexOf('.');
            if (extensionIndex < 0)
                return null;

            string fileExtension = filePath.Substring(extensionIndex + 1);

            if (Readers.Keys.Contains(fileExtension))
            {
                try
                {
                    IMusicReader reader = Readers[fileExtension];
                    return reader.Read(filePath);
                }
                catch (Exception)
                {
                    string errorTitle = "Bad file";
                    string errorMessage = "Reading file failed."/* + System.Environment.NewLine/* + e.Message*/;
                    MessageBox.Show(errorMessage, errorTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return null;
                }
            }

            return null;
        }
    }
}
