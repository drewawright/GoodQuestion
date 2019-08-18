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
        private readonly Guid _userId;

        public SongServices(Guid userId)
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
            }
        }

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
                    AlbumName = query.AlbumName,
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
                if (CheckIfSongExists(songId))
                {
                var entity = ctx
                    .Songs
                    .Single(e => e.SongId == songId);
                ctx.Songs.Remove(entity);

                return ctx.SaveChanges() == 1;
                }
                return false;
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
                        AlbumName = song.AlbumName,
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

        private void GetSongAudioFeatures(List<Song> songList)
        {
            var count = songList.Count();

            int loops = count / 100;

            int remainder = count % 100;
            if(count == 100)
            {
                remainder = 100;
            }

            if (count % 100 != 0)
            {
                loops++;
            }

            Dictionary<string, Song> songDict = songList.ToDictionary(s => s.SongId);

            for (int i = 0; i < loops; i++)
            {
                List<Song> songFeaturesList;

                if (i != loops - 1)
                {
                    songFeaturesList = songDict.Values.Skip(i * 100).Take(100).ToList();
                }
                else
                {
                    songFeaturesList = songDict.Values.Skip(i * 100).Take(remainder).ToList();
                }

                List<string> songIds = new List<string>();

                foreach (var song in songFeaturesList)
                {
                    songIds.Add(song.SongId);
                }

                SeveralAudioFeatures features = _api.GetSeveralAudioFeatures(songIds);
                foreach (var song in features.AudioFeatures)
                {
                    songDict[song.Id].DurationMs = song.DurationMs;
                    songDict[song.Id].Danceability = song.Danceability;
                    songDict[song.Id].Energy = song.Energy;
                    songDict[song.Id].Key = song.Key;
                    songDict[song.Id].Loudness = song.Loudness;
                    songDict[song.Id].Mode = song.Mode;
                    songDict[song.Id].Speechiness = song.Speechiness;
                    songDict[song.Id].Acousticness = song.Acousticness;
                    songDict[song.Id].Instrumentalness = song.Instrumentalness;
                    songDict[song.Id].Liveness = song.Liveness;
                    songDict[song.Id].Valence = song.Valence;
                    songDict[song.Id].Tempo = song.Tempo;
                    songDict[song.Id].HasAudioFeatures = true;
                }
            }
        }

        public void GetAllUserSongs()
        {
            using (var ctx = new ApplicationDbContext())
            {
                var query =
                    ctx
                    .Users
                    .Where(u => u.Id == _userId.ToString())
                    .Single();
                foreach (var playlist in query.Playlists)
                {
                    GetSongsInPlaylist(playlist.PlaylistId);
                }
            }
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
                if (!track.IsLocal && track.Track.Name != null)
                {

                    Song song = new Song
                    {
                        Name = track.Track.Name,
                        SongId = track.Track.Id,
                        Artists = track.Track.Artists.First().Name,
                        AlbumName = track.Track.Album.Name,
                        PlayerUrl = track.Track.ExternUrls["spotify"],
                        LastRefreshed = DateTime.Now,
                        Playlists = new List<Playlist>()
                    };

                    // GetSongAudioFeatures(song);

                    if (track.Track.Album.Images.Count != 0)
                    {
                        song.ImageUrl = track.Track.Album.Images[0].Url;
                    }

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
                    if (!CheckIfSongExists(track.SongId))
                    {
                        newSongs.Add(track);
                        track.Playlists.Add(query);
                    }
                    else
                    {
                        var querySong =
                            db
                            .Songs
                            .Where(s => s.SongId == track.SongId)
                            .Single();


                        querySong.Playlists.Add(query);
                    }
                }

                newSongs = newSongs.GroupBy(s => s.SongId).Select(s => s.First()).ToList();

                //Get audio features for new songs
                GetSongAudioFeatures(newSongs);

                foreach (Song song in newSongs)
                {
                    db.Songs.Add(song);
                    changeCount = changeCount + 2;
                }
                query.HasSongs = true;
                int actual = db.SaveChanges();
                if (actual == changeCount || actual == changeCount + 1)
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

        public bool DeleteTable()
        {
            using (var ctx = new ApplicationDbContext())
            {
                ctx.Database.ExecuteSqlCommand("DELETE FROM [Song]");
                ctx.SaveChanges();
            }
            return true;
        }
    }
}
