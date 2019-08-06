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

        public bool RefreshUserPlaylistsArtwork(Guid appUserId)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity =
                    ctx
                    .Playlists
                    .Where(p => p.AppUserId == appUserId);

                foreach(var playlist in entity)
                {
                    
                }

                return true;
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
                return ctx.SaveChanges() == changeCount;
            }
        }

        public List<PlaylistIndex> GetPlaylistIndex(Guid appUserId)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var query =
                        ctx
                        .Playlists
                        .Where(p => p.AppUserId == appUserId)
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


    }
}
