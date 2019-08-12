using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GoodQuestion.WebAPI.Models
{
    public class AccountDetailModel
    {
        public string UserName { get; set; }
        public string Id { get; set; }
        public bool HasPlaylists { get; set; }
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