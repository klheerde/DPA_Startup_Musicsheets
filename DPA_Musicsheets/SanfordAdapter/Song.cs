using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.SanfordAdapter
{
    public class Song
    {
        private int bpm;
        private int[] timeSignature = new int[2];

        private Sequence sequence;

        private List<Track> tracks = new List<Track>();

        private Song() { }
        public Song(Sequence sequence)
        {
            //TODO sequence used?
            this.sequence = sequence;
        }

        public void AddTrack(Track track)
        {
            tracks.Add(track);
        }
        public Track GetTrack(int index)
        {
            return tracks[index];
        }

        
        public class Builder : IBuilder<Song>
        {
            private Song buildee;
            public Builder() : this(new Song()) { }
            public Builder(Song song)
            {
                buildee = song;
            }

            public Builder SetMetaData(MetaMessage metaMessage)
            {
                byte[] bytes = metaMessage.GetBytes();

                switch (metaMessage.MetaType)
                {
                    case MetaType.Tempo:
                        // Bitshifting is nodig om het tempo in BPM te be
                        int tempo = (bytes[0] & 0xff) << 16 | (bytes[1] & 0xff) << 8 | (bytes[2] & 0xff);
                        buildee.bpm = 60000000 / tempo;
                        break;
                    //case MetaType.SmpteOffset:
                    //    break;
                    case MetaType.TimeSignature:
                        //NOTE: kwart = 1 / 0.25 = 4
                        buildee.timeSignature[0] = bytes[0];
                        buildee.timeSignature[1] = (int)(1 / Math.Pow(bytes[1], -2));
                        break;
                    //case MetaType.KeySignature:
                    //    break;
                    //case MetaType.ProprietaryEvent:
                    //    break;
                    //case MetaType.TrackName:
                    //    name = Encoding.Default.GetString(bytes);
                    //    break;
                    default:
                        //return metaMessage.MetaType + ": " + Encoding.Default.GetString(metaMessage.GetBytes());
                        break;
                }

                return this;
            }

            public Song Build()
            {
                return buildee;
            }
        }
    }
}
