using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoodQuestion.Data;
using GoodQuestion.Models.Song;
using GoodQuestion.WebAPI.Models;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;

namespace GoodQuestion.Services
{
    public class SongServices
    {
        private SpotifyWebAPI _api = new SpotifyWebAPI
        {
            AccessToken = "BQAu0mW3ne5pg89O1GWpqLCKOAS6p3mRVlzOSiHcwwYyPZhJugeJH7Oym05t2mlm8PWTpxAX9wQgWMv_O3MVqK1FPrN8amq_NDzewVBljSXZVEpNChYAtibNMyHP6TKN78pi91MaZ7K2wRh1ksEW0ZUkaPJ1jnMZWN6expIA5GwY6f5lYcK-RQV2W9RT1xYzoHO4qKVfh0dZdDZLDoa9lC_dnX9tD_A3UcxvJteHZDev1kFihT_q5_YEEVQ3r_ekpEPuicvoZGjJJNoX0AAH8w-vPehQnIB9",
            TokenType = "Bearer"
        };
        private string _accountId = "38vdur0tacvhr9wud418mvzqh";

        public bool CheckIfSongExists(string songId)
        {
            using (var ctx = new ApplicationDbContext())
            {
                try
                {
                    var query = ctx
                    .Songs
                    .Single(e => e.SongId == songId);
                    return true;
                }
                catch (InvalidOperationException)
                {
                    return false;
                }
                catch (ArgumentNullException)
                {
                    return false;
                }
            }
        }

        public SongDetail GetSongDetail(string songId)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var query = ctx
                    .Songs
                    .Single(s => s.SongId == songId);

                var songDetail = new SongDetail {
                    Name = query.Name,
                    SongId = query.SongId,
                    Artists = query.Artists,
                    ImageUrl = query.ImageUrl,
                    PlayerUrl = query.PlayerUrl,
                    DurationMs = query.DurationMs,
                    HasAudioFeatures = query.HasAudioFeatures,
                    LastRefreshed = query.LastRefreshed,
                    Danceability = query.Danceability,
                    Energy = query.Energy,
                    Key = query.Key,
                    Loudness = query.Loudness,
                    Mode = query.Mode,
                    Speechiness = query.Speechiness,
                    Acousticness = query.Acousticness,
                    Instrumentalness = query.Instrumentalness,
                    Liveness = query.Liveness,
                    Valence = query.Valence,
                    Tempo = query.Tempo
                    };
                return songDetail;
                    
            }
        }

        public bool DeleteSongDb(string songId)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity = ctx
                    .Songs
                    .Single(e => e.SongId == songId);
                ctx.Songs.Remove(entity);

                return ctx.SaveChanges() == 1;
            }
        }

        public List<SongIndex> GetSongIndexDb(string playlistId)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var query = ctx
                    .Playlists
                    .Where(p => p.PlaylistId == playlistId)
                    .Single();

                List<SongIndex> songIndex = new List<SongIndex>();

                foreach(var song in query.Songs)
                {
                    var songItem = new SongIndex
                    {
                        Name = song.Name,
                        SongId = song.SongId,
                        Artists = song.Artists,
                        ImageUrl = song.ImageUrl,
                        PlayerUrl = song.PlayerUrl,
                        DurationMs = song.DurationMs,
                        LastRefreshed = song.LastRefreshed,
                    };
                    songIndex.Add(songItem);
                }

                return songIndex;
            };
        }
    }
}
