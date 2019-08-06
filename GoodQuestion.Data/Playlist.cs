using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodQuestion.Data
{
    public class Playlist
    {
        public string OwnerId { get; set; }
        public Guid AppUserId { get; set; }
        public string PlaylistId { get; set; }
        public string PlaylistName { get; set; }
        public string TracksUrl { get; set; }
        public string ImageUrl { get; set; }
        public bool HasSongs { get; set; }

        public float Danceability
        {
            get
            {
                float count = 0;
                float danceability = 0;

                foreach (var song in this.Songs)
                {
                    danceability += song.Danceability;

                    count++;
                }

                return danceability / count;
            }
        }

        public float Energy
        {
            get
            {
                float count = 0;
                float energy = 0;

                foreach (var song in this.Songs)
                {
                    energy += song.Energy;

                    count++;
                }

                return energy / count;
            }
        }

        public int Key
        {
            get
            {
                int count = 0;

                var keyList = new int[this.Songs.Count()];

                foreach (var song in this.Songs)
                {
                    keyList[count] = song.Key;

                    count++;
                }

                var key = keyList.GroupBy(n => n)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key).FirstOrDefault();

                return key;
            }
        }

        public float Loudness
        {
            get
            {
                float count = 0;
                float Loudness = 0;

                foreach (var song in this.Songs)
                {
                    Loudness += song.Loudness;

                    count++;
                }

                return Loudness / count;
            }
        }

        public int Mode
        {
            get
            {
                int count = 0;

                var modeList = new int[this.Songs.Count()];

                foreach (var song in this.Songs)
                {
                    modeList[count] = song.Mode;

                    count++;
                }

                var mode = modeList.GroupBy(n => n)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key).FirstOrDefault();

                return mode;
            }
        }

        public float Speechiness
        {
            get
            {
                float count = 0;
                float speechiness = 0;

                foreach (var song in this.Songs)
                {
                    speechiness += song.Speechiness;

                    count++;
                }

                return speechiness / count;
            }
        }

        public float Acousticness
        {
            get
            {
                float count = 0;
                float acousticness = 0;

                foreach (var song in this.Songs)
                {
                    acousticness += song.Acousticness;

                    count++;
                }

                return acousticness / count;
            }
        }

        public float Instrumentalness
        {
            get
            {
                float count = 0;
                float instrumentalness = 0;

                foreach (var song in this.Songs)
                {
                    instrumentalness += song.Instrumentalness;

                    count++;
                }

                return instrumentalness / count;
            }
        }

        public float Liveness
        {
            get
            {
                float count = 0;
                float liveness = 0;

                foreach (var song in this.Songs)
                {
                    liveness += song.Liveness;

                    count++;
                }

                return liveness / count;
            }
        }

        public float Valence
        {
            get
            {
                float count = 0;
                float valence = 0;

                foreach (var song in this.Songs)
                {
                    valence += song.Valence;

                    count++;
                }

                return valence / count;
            }
        }

        public float Tempo
        {
            get
            {
                float count = 0;
                float tempo = 0;

                foreach (var song in this.Songs)
                {
                    tempo += song.Tempo;

                    count++;
                }

                return tempo / count;
            }
        }

        public int Duration_ms
        {
            get
            {
                int duration = 0;

                foreach (var song in this.Songs)
                {
                    duration += song.DurationMs;
                }

                return duration;
            }
        }

        public DateTime LastRefreshed { get; set; }
        public DateTime LastSyncedWithSpotify { get; set; }
        public virtual ICollection<Song> Songs { get; set; }
    }
}
