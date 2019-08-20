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
        private string _spotifyId;

        public PlaylistServices(Guid userId)
        {
            _userId = userId;
        }

        private SpotifyWebAPI _api = new SpotifyWebAPI
        {
            TokenType = "Bearer"
        };

        public void SetToken()
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity =
                    ctx
                    .Users
                    .Where(u => u.Id == _userId.ToString())
                    .Single();

                _api.AccessToken = entity.SpotifyAuthToken;
                _spotifyId = entity.SpotifyUserId;
            }
        }
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
                    playlist.LastRefreshed = DateTime.Now;
                    changeCount++;
                    if (apiPlaylist.Images.Count != 0)
                    {
                        playlist.ImageUrl = apiPlaylist.Images[0].Url;
                    }

                }

                return ctx.SaveChanges() == changeCount;
            }
        }

        public bool GetAllUserPlaylistsSpotify()
        {
            List<Playlist> userPlaylists = new List<Playlist>();
            var playlistsSpotify = _api.GetUserPlaylists(_spotifyId, 50, 0);
            var count = playlistsSpotify.Total;
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
                    var additionalPlaylists = _api.GetUserPlaylists(_spotifyId, 50, offset);
                    foreach (var playlist in additionalPlaylists.Items)
                    {
                        playlistsSpotify.Items.Add(playlist);
                    }
                }
            }
            foreach (var playlist in playlistsSpotify.Items)
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
                userPlaylists.Add(userPlaylist);
            }

            var changeCount = 0;
            using (var ctx = new ApplicationDbContext())
            {
                var userQuery = ctx
                    .Users
                    .Single(u => u.Id == _userId.ToString());

                foreach (Playlist playlist in userPlaylists)
                {
                    try
                    {
                        var playlistQuery = ctx
                        .Playlists
                        .Single(e => e.PlaylistId == playlist.PlaylistId);

                        changeCount++;
                        if (!userQuery.Playlists.Contains(playlistQuery))
                        {
                            userQuery.Playlists.Add(playlistQuery);
                            changeCount++;
                        }

                        //return true;
                    }
                    catch (InvalidOperationException)
                    {
                        ctx.Playlists.Add(playlist);
                        userQuery.Playlists.Add(playlist);
                        changeCount++;
                        changeCount++;
                    }
                    catch (ArgumentNullException)
                    {
                        ctx.Playlists.Add(playlist);
                        userQuery.Playlists.Add(playlist);
                        changeCount++;
                        changeCount++;
                    }

                    if (changeCount >= 1)
                    {
                        userQuery.HasPlaylists = true;
                    }
                }

                int actual = ctx.SaveChanges();
                if (actual == changeCount || actual == changeCount + 1)
                {
                    return true;
                }
                else return false;
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

        public bool DeleteTable()
        {
            using (var ctx = new ApplicationDbContext())
            {
                ctx.Database.ExecuteSqlCommand("DELETE FROM [Playlist]");
                ctx.SaveChanges();
            }
            return true;
        }
    }
}
