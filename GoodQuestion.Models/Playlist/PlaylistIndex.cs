using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodQuestion.Models.Playlist
{
    public class PlaylistIndex
    {
        public string PlaylistId { get; set; }
        public string PlaylistName { get; set; }
        public string ImageUrl { get; set; }
        public string OwnerId { get; set; }
        public DateTime LastRefreshed { get; set; }
        public DateTime LastSyncedWithSpotify { get; set; }
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
    }
}
