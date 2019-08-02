using System;
using System.Collections.Generic;
using GoodQuestion.Data.Song;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodQuestion.Data
{
    public class Playlist
    {
        public string OwnerId { get; set; }
        public int PlaylistId { get; set; }
        public string PlaylistName { get; set; }
        public string TracksUrl { get; set; }
        public virtual ICollection<Song> Songs { get; set; }
        public string ImageUrl { get; set; }
        public bool HasSongs { get; set; }
        public float Danceability { get; set; }
        public float Energy { get; set; }
        public int Key { get; set; }
        public float Loudness { get; set; }
        public int Mode { get; set; }
        public float Speechiness { get; set; }
        public float Acousticness { get; set; }
        public float Instrumentalness { get; set; }
        public float Liveness { get; set; }
        public float Valence { get; set; }
        public float Tempo { get; set; }
        public int Duration_ms { get; set; }
        public DateTime LastRefreshed { get; set; }
    }
}
