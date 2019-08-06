using GoodQuestion.Models.Playlist;
using GoodQuestion.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodQuestion.Services
{
    public class PlaylistServices
    {
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
    }
}
