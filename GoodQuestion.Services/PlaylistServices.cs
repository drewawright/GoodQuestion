using GoodQuestion.Data;
using GoodQuestion.Models.Playlist;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;

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
            AccessToken = "BQB7FyIj2RuHA1CkoUhCQCD8AYCBYyLDk3HIZ1pjVK18MtcIooNFkp-vc-HE4cDHjRN27hv0nN61yHIUWlgUZiClplurHGV26wAG0s091IYt4LsGETt48A2vNk14UAS-898zeXuqmP08GBbTcH52lrdI6dTliKPxBW8M5BGq2Whvpke__rWMKlbAIeZHrnb7JhljF1NKrhmchT39gpNo3hj0BAivL4F2lAl4OsYBhKUEYY5v0QZCRHWdWcStOHBeHtxUVb7anzus9KBqK9Qx",
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
                    .Users
                    .Where(u => u.Id == _userId.ToString())
                    .Single();

                int changeCount = 0;
                foreach (var playlist in entity.Playlists)
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
            var playlists = _api.GetUserPlaylists(spotifyId, 50, 0);
            var count = playlists.Total;
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
                var query = ctx
                    .Users
                    .Single(u => u.Id == _userId.ToString());

                foreach (Playlist playlist in playlistsToAdd)
                {
                    var queryPlaylist = ctx
                        .Playlists
                        .Where(u => u.PlaylistId == playlist.PlaylistId)
                        .SingleOrDefault();

                    if (!CheckIfPlaylistExists(playlist.PlaylistId))
                    {
                        ctx.Playlists.Add(playlist);
                        changeCount++;
                    }

                    if (!query.Playlists.Contains(queryPlaylist))
                    {
                        query.Playlists.Add(playlist);
                        changeCount++;
                    }

                    if (changeCount >= 1)
                    {
                        query.HasPlaylists = true;
                    }
                }

                return ctx.SaveChanges() == changeCount;
            }
        }

        public List<PlaylistIndex> GetPlaylistIndex()
        {
            using (var ctx = new ApplicationDbContext())
            {
                var userQuery =
                        ctx
                        .Users
                        .Where(u => u.Id == _userId.ToString())
                        .Single();
                List<PlaylistIndex> playlistIndexList = new List<PlaylistIndex>();
                foreach (var playlist in userQuery.Playlists)
                {
                    PlaylistIndex newIndexItem = new PlaylistIndex
                    {
                        PlaylistId = playlist.PlaylistId,
                        PlaylistName = playlist.PlaylistName,
                        ImageUrl = playlist.ImageUrl,
                        OwnerId = playlist.OwnerId,
                        LastRefreshed = playlist.LastRefreshed,
                        LastSyncedWithSpotify = playlist.LastSyncedWithSpotify,
                        Danceability = playlist.Danceability,
                        Energy = playlist.Energy,
                        Key = playlist.Key,
                        Loudness = playlist.Loudness,
                        Mode = playlist.Mode,
                        Speechiness = playlist.Speechiness,
                        Acousticness = playlist.Acousticness,
                        Instrumentalness = playlist.Instrumentalness,
                        Liveness = playlist.Liveness,
                        Valence = playlist.Valence,
                        Tempo = playlist.Tempo,
                        Duration_ms = playlist.Duration_ms,
                    };
                    playlistIndexList.Add(newIndexItem);
                }
                return playlistIndexList;
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
            int changeCount = 1;
            using (var ctx = new ApplicationDbContext())
            {
                var entity = ctx
                    .Playlists
                    .Single(e => e.PlaylistId == playlistId);

                List<string> songsToDelete = new List<string>();

                foreach (var song in entity.Songs)
                {
                    if(song.Playlists.Count == 1)
                    {
                        songsToDelete.Add(song.SongId);
                    }
                }

                foreach (var songId in songsToDelete)
                {
                    var songEntity = ctx
                        .Songs
                        .Single(se => se.SongId == songId);
                        ctx.Songs.Remove(songEntity);
                        changeCount = changeCount + 2;
                }

                ctx.Playlists.Remove(entity);
                var actual = ctx.SaveChanges();
                return actual == changeCount;
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

                /*                var playlists =
                                    ctx
                                        .Users
                                        .Where(u => u.Id.ToString() == entity.Id)
                                        .Single();*/

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

                foreach (var playlist in entity.Playlists)
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

        public bool UpdateDbPlaylist(string playlistId)
        {
            var spotifyPlaylist = _api.GetPlaylist(playlistId);

            using (var db = new ApplicationDbContext())
            {
                var entity = db
                    .Playlists
                    .Single(e => e.PlaylistId == playlistId);

                entity.PlaylistName = spotifyPlaylist.Name;
                entity.ImageUrl = spotifyPlaylist.Images[0].Url;
                entity.OwnerId = spotifyPlaylist.Owner.Id;
                entity.LastRefreshed = DateTime.Now;
                entity.LastSyncedWithSpotify = DateTime.Now;

                return db.SaveChanges() == 1;
            }
        }
    }
}
