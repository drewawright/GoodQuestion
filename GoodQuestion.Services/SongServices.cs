using System;
using System.Collections.Generic;
using System.Linq;
using GoodQuestion.Data;
using GoodQuestion.Models.Song;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Models;

namespace GoodQuestion.Services
{
    public class SongServices
    {
        private SpotifyWebAPI _api = new SpotifyWebAPI
        {
            AccessToken = "BQB7FyIj2RuHA1CkoUhCQCD8AYCBYyLDk3HIZ1pjVK18MtcIooNFkp-vc-HE4cDHjRN27hv0nN61yHIUWlgUZiClplurHGV26wAG0s091IYt4LsGETt48A2vNk14UAS-898zeXuqmP08GBbTcH52lrdI6dTliKPxBW8M5BGq2Whvpke__rWMKlbAIeZHrnb7JhljF1NKrhmchT39gpNo3hj0BAivL4F2lAl4OsYBhKUEYY5v0QZCRHWdWcStOHBeHtxUVb7anzus9KBqK9Qx",
            TokenType = "Bearer"
        };
        private string _accountId = "chillpill9623";

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

        public bool CheckIfSongHasPlaylists(string songId)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var query = ctx
                    .Songs
                    .Where(s => s.SongId == songId)
                    .Single();

                if (query.Playlists.Count != 0)
                {
                    return true;
                }
                else return false;
            };
        }

        public SongDetail GetSongDetail(string songId)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var query = ctx
                    .Songs
                    .Single(s => s.SongId == songId);

                var songDetail = new SongDetail
                {
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

        public bool RefreshSongArtwork(string songId)
        {
            FullTrack track = _api.GetTrack(songId, null);

            using (var ctx = new ApplicationDbContext())
            {
                var entity = ctx
                    .Songs
                    .Single(s => s.SongId == songId);

                entity.ImageUrl = track.Album.Images[0].Url;
                entity.LastRefreshed = DateTime.Now;
                return ctx.SaveChanges() == 1;
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

                foreach (var song in query.Songs)
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

        private void GetSongAudioFeatures(Song song)
        {
            AudioFeatures features = _api.GetAudioFeatures(song.SongId);

            song.DurationMs = features.DurationMs;
            song.Danceability = features.Danceability;
            song.Energy = features.Energy;
            song.Key = features.Key;
            song.Loudness = features.Loudness;
            song.Mode = features.Mode;
            song.Speechiness = features.Speechiness;
            song.Acousticness = features.Acousticness;
            song.Instrumentalness = features.Instrumentalness;
            song.Liveness = features.Liveness;
            song.Valence = features.Valence;
            song.Tempo = features.Tempo;
            song.HasAudioFeatures = true;
        }

        public bool GetSongsInPlaylist(string playlistId)
        {
            List<Song> songs = new List<Song>();

            var tracks = _api.GetPlaylistTracks(playlistId);
            Playlist playlist;

            if (CheckIfPlaylistExists(playlistId))
            {
                playlist = GetPlaylistFromDb(playlistId);
            }
            else
            {
                playlist = new Playlist();
            }

            var count = tracks.Total;

            if (count > 100)
            {
                var loops = count / 100;

                if (count % 100 != 0)
                {
                    loops++;
                }

                for (int i = 1; i < loops; i++)
                {
                    var offset = i * 100;

                    var additionalTracks = _api.GetPlaylistTracks(playlistId, null, 100, offset, null);

                    foreach (var track in additionalTracks.Items)
                    {
                        tracks.Items.Add(track);
                    }
                }
            }

            foreach (var track in tracks.Items)
            {
                if (!track.IsLocal)
                {

                    Song song = new Song
                    {
                        Name = track.Track.Name,
                        SongId = track.Track.Id,
                        Artists = track.Track.Artists.First().Name,
                        ImageUrl = track.Track.Album.Images[0].Url,
                        PlayerUrl = track.Track.ExternUrls["spotify"],
                        LastRefreshed = DateTime.Now,
                        Playlists = new List<Playlist>()
                    };

                    songs.Add(song);
                }
            }

            var newSongs = new List<Song>();
            var changeCount = 0;
            using (var db = new ApplicationDbContext())
            {

                var query =
                        db
                        .Playlists
                        .Where(p => p.PlaylistId == playlistId)
                        .Single();
                //Add playlist to all songs, add new songs to list of new songs
                foreach (var track in songs)
                {

                    track.Playlists.Add(query);
                    if (!CheckIfSongExists(track.SongId))
                    {
                        newSongs.Add(track);
                    }
                }

                //Get audio features for new songs
                foreach (var song in newSongs)
                {
                    GetSongAudioFeatures(song);
                }

                foreach (Song song in newSongs)
                {
                    db.Songs.Add(song);
                    changeCount = changeCount + 2;
                }
                query.HasSongs = true;
                int actual = db.SaveChanges();
                if (actual == changeCount || actual == changeCount++)
                {
                    return true;
                }
                else return false;
            }
        }

        public bool RefreshPlaylistSongsArtwork(string playlistId)
        {
            List<Song> updateSongs = new List<Song>();

            var tracks = _api.GetPlaylistTracks(playlistId);

            var count = tracks.Items.Count();

            if (count > 100)
            {
                var loops = count / 100;

                if (count % 100 != 0)
                {
                    loops++;
                }

                for (int i = 1; i < loops; i++)
                {
                    var offset = i * 100;

                    var additionalTracks = _api.GetPlaylistTracks(playlistId, null, 100, offset, null);

                    foreach (var track in additionalTracks.Items)
                    {
                        tracks.Items.Add(track);
                    }
                }
            }

            var changeCount = 0;
            var loopCount = 0;

            using (var db = new ApplicationDbContext())
            {
                var entity =
                    db
                    .Playlists
                    .Where(p => p.PlaylistId == playlistId)
                    .Single();

                foreach (var song in entity.Songs)
                {
                    if (song.SongId == tracks.Items[loopCount].Track.Id)
                    {
                        song.LastRefreshed = DateTime.Now;
                        song.ImageUrl = tracks.Items[loopCount].Track.Album.Images[0].Url;
                        changeCount++;
                    }
                    loopCount++;
                }
                return db.SaveChanges() == changeCount;
            }
        }

        private Playlist GetPlaylistFromDb(string playlistId)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var query =
                    ctx
                    .Playlists
                    .Where(p => p.PlaylistId == playlistId)
                    .Single();
                return query;
            }
        }

        private bool CheckIfPlaylistExists(string playlistId)
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
    }
}
