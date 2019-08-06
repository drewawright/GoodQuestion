using GoodQuestion.Data;
using GoodQuestion.Models.Playlist;
using GoodQuestion.WebAPI.Models;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodQuestion.Services
{
    public class PlaylistServices
    {

        private readonly Guid _userId;

        public PlaylistServices(Guid userId)
        {
            _userId = userId;
        }

        private SpotifyWebAPI _api = new SpotifyWebAPI
        {
            AccessToken = "",
            TokenType = "Bearer"
        };

        private string _accountId = "38vdur0tacvhr9wud418mvzqh";

        private bool CheckUserHasPlaylists()
        {
            using (var ctx = new ApplicationDbContext())
            {
                try
                {
                    var query = ctx
                    .Users
                    .Single(u => u.Id == _userId.ToString());
                    if (query.HasPlaylists == true) { return true; }
                    else return false;
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

        public bool CheckIfPlaylistExists(string playlistId)
        {
            using (var ctx = new ApplicationDbContext())
            {
                try
                {
                    var query = ctx
                    .Playlists
                    .Single(e => e.PlaylistId == playlistId);
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

        public bool RefreshUserPlaylistsArtwork()
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity =
                    ctx
                    .Playlists
                    .Where(p => p.AppUserId == _userId);

                int changeCount = 0;
                foreach(var playlist in entity)
                {
                    var apiPlaylist = _api.GetPlaylist(playlist.PlaylistId);
                    playlist.ImageUrl = apiPlaylist.Images[0].Url;
                    playlist.LastRefreshed = DateTime.Now;
                    changeCount++;
                }

                return ctx.SaveChanges() == changeCount;
            }
        }

        public bool GetAllUserPlaylistsSpotify(string spotifyId)
        {
            List<Playlist> playlistsToAdd = new List<Playlist>();

            var playlists = _api.GetUserPlaylists(spotifyId);

            var count = playlists.Items.Count;

            if (count > 50)
            {
                var loops = count / 50;

                if (count % 50 != 0)
                {
                    loops++;
                }

                for (int i = 1; i < loops; i++)
                {
                    var offset = i * 50;

                    var additionalPlaylists = _api.GetUserPlaylists(spotifyId, 50, offset);

                    foreach (var playlist in additionalPlaylists.Items)
                    {
                        playlists.Items.Add(playlist);
                    }
                }
            }

            foreach (var playlist in playlists.Items)
            {
                Playlist userPlaylist = new Playlist
                {
                    OwnerId = playlist.Owner.Id,
                    AppUserId = _userId,
                    PlaylistId = playlist.Id,
                    PlaylistName = playlist.Name,
                    TracksUrl = playlist.Tracks.Href,
                    ImageUrl = playlist.Images[0].Url,
                    LastRefreshed = DateTime.Now,
                    LastSyncedWithSpotify = DateTime.Now
                };

                playlistsToAdd.Add(userPlaylist);
            }

            var changeCount = 0;

            using (var ctx = new ApplicationDbContext())
            {
                foreach (Playlist playlist in playlistsToAdd)
                {
                    ctx.Playlists.Add(playlist);
                    changeCount++;
                }
                if (changeCount >= 1)
                {
                    var query = ctx
                        .Users
                        .Single(u => u.Id == _userId.ToString());
                    query.HasPlaylists = true;
                    changeCount++;
                }
                return ctx.SaveChanges() == changeCount;
            }
        }

        public List<PlaylistIndex> GetPlaylistIndex()
        {
            using (var ctx = new ApplicationDbContext())
            {
                var query =
                        ctx
                        .Playlists
                        .Where(p => p.AppUserId == _userId)
                        .Select(p =>
                            new PlaylistIndex
                            {
                                PlaylistId = p.PlaylistId,
                                PlaylistName = p.PlaylistName,
                                ImageUrl = p.ImageUrl,
                                OwnerId = p.OwnerId,
                                LastRefreshed = p.LastRefreshed,
                                LastSyncedWithSpotify = p.LastSyncedWithSpotify,
                                Danceability = p.Danceability,
                                Energy = p.Energy,
                                Key = p.Key,
                                Loudness = p.Loudness,
                                Mode = p.Mode,
                                Speechiness = p.Speechiness,
                                Acousticness = p.Acousticness,
                                Instrumentalness = p.Instrumentalness,
                                Liveness = p.Liveness,
                                Valence = p.Valence,
                                Tempo = p.Tempo,
                                Duration_ms = p.Duration_ms,
                            });

                return query.ToList();
            }
        }

        public PlaylistDetail GetPlaylistDetail(string playlistId)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var query = ctx
                    .Playlists
                    .Single(s => s.PlaylistId == playlistId);

                var playlistDetail = new PlaylistDetail
                {
                    PlaylistName = query.PlaylistName,
                    ImageUrl = query.ImageUrl,
                    OwnerId = query.OwnerId,
                    LastRefreshed = query.LastRefreshed,
                    LastSyncedWithSpotify = query.LastSyncedWithSpotify,
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
                    Tempo = query.Tempo,
                    Duration_ms = query.Duration_ms
                };
                return playlistDetail;
            }
        }

        public bool DeletePlaylistDb(string playlistId)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity = ctx
                    .Playlists
                    .Single(e => e.PlaylistId == playlistId);
                ctx.Playlists.Remove(entity);

                return ctx.SaveChanges() == 1;
            }
        }

        public bool GetUserAudioFeatures()
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity =
                    ctx
                        .Users
                        .Where(u => u.Id == _userId.ToString())
                        .Single();

                var playlists =
                    ctx
                        .Playlists
                        .Where(p => p.AppUserId.ToString() == entity.Id);

                float count = 0;
                float danceability = 0;
                float energy = 0;
                float loudness = 0;
                float speechiness = 0;
                float acousticness = 0;
                float instrumentalness = 0;
                float liveness = 0;
                float valence = 0;
                float tempo = 0;
                var key = new List<int>();
                var mode = new List<int>();
                int duration_ms = 0;

                foreach (var playlist in playlists)
                {
                    danceability += playlist.Danceability;
                    energy += playlist.Energy;
                    loudness += playlist.Loudness;
                    speechiness += playlist.Speechiness;
                    acousticness += playlist.Acousticness;
                    instrumentalness += playlist.Instrumentalness;
                    liveness += playlist.Liveness;
                    valence += playlist.Valence;
                    tempo += playlist.Tempo;
                    key.Add(playlist.Key);
                    mode.Add(playlist.Mode);
                    duration_ms += playlist.Duration_ms;
                    count++;
                }

                var keyMode = key
                    .GroupBy(n => n)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key).FirstOrDefault();

                var modeMode = mode
                    .GroupBy(n => n)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key).FirstOrDefault();

                entity.Danceability = danceability / count;
                entity.Energy = energy / count;
                entity.Loudness = loudness / count;
                entity.Speechiness = speechiness / count;
                entity.Acousticness = acousticness / count;
                entity.Instrumentalness = instrumentalness / count;
                entity.Liveness = liveness / count;
                entity.Valence = valence / count;
                entity.Tempo = tempo / count;
                entity.Key = keyMode;
                entity.Mode = modeMode;
                entity.Duration_ms = duration_ms;

                return ctx.SaveChanges() == 1;
            }
        }
    }
}
