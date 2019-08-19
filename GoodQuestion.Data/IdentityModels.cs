using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GoodQuestion.Data;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace GoodQuestion.Data
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string SpotifyUserId { get; set; }
        public string SpotifyAuthToken { get; set; }
        public string SpotifyRefreshToken { get; set; }
        public bool HasPlaylists { get; set; }
        public DateTime? TokenExpiration { get; set; }
        public float Danceability
        {
            get
            {
                float count = 0;
                float danceability = 0;

                foreach (var playlist in this.Playlists)
                {
                    danceability += playlist.Danceability;

                    count++;
                }

                return danceability / count;
            }
        }

        public float Energy
        {
            get
            {
                float count = 0;
                float energy = 0;

                foreach (var playlist in this.Playlists)
                {
                    energy += playlist.Energy;

                    count++;
                }

                return energy / count;
            }
        }

        public int Key
        {
            get
            {
                int count = 0;

                var keyList = new int[this.Playlists.Count()];

                foreach (var playlist in this.Playlists)
                {
                    keyList[count] = playlist.Key;

                    count++;
                }

                var key = keyList.GroupBy(n => n)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key).FirstOrDefault();

                return key;
            }
        }
        public float Loudness
        {
            get
            {
                float count = 0;
                float Loudness = 0;

                foreach (var playlist in this.Playlists)
                {
                    Loudness += playlist.Loudness;

                    count++;
                }

                return Loudness / count;
            }
        }

        public int Mode
        {
            get
            {
                int count = 0;

                var modeList = new int[this.Playlists.Count()];

                foreach (var playlist in this.Playlists)
                {
                    modeList[count] = playlist.Mode;

                    count++;
                }

                var mode = modeList.GroupBy(n => n)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key).FirstOrDefault();

                return mode;
            }
        }

        public float Speechiness
        {
            get
            {
                float count = 0;
                float speechiness = 0;

                foreach (var playlist in this.Playlists)
                {
                    speechiness += playlist.Speechiness;

                    count++;
                }

                return speechiness / count;
            }
        }

        public float Acousticness
        {
            get
            {
                float count = 0;
                float acousticness = 0;

                foreach (var playlist in this.Playlists)
                {
                    acousticness += playlist.Acousticness;

                    count++;
                }

                return acousticness / count;
            }
        }

        public float Instrumentalness
        {
            get
            {
                float count = 0;
                float instrumentalness = 0;

                foreach (var playlist in this.Playlists)
                {
                    instrumentalness += playlist.Instrumentalness;

                    count++;
                }

                return instrumentalness / count;
            }
        }
        public float Liveness
        {
            get
            {
                float count = 0;
                float liveness = 0;

                foreach (var playlist in this.Playlists)
                {
                    liveness += playlist.Liveness;

                    count++;
                }

                return liveness / count;
            }
        }

        public float Valence
        {
            get
            {
                float count = 0;
                float valence = 0;

                foreach (var playlist in this.Playlists)
                {
                    valence += playlist.Valence;

                    count++;
                }

                return valence / count;
            }
        }

        public float Tempo
        {
            get
            {
                float count = 0;
                float tempo = 0;

                foreach (var playlist in this.Playlists)
                {
                    tempo += playlist.Tempo;

                    count++;
                }

                return tempo / count;
            }
        }

        public int Duration_ms
        {
            get
            {
                int duration = 0;

                foreach (var playlist in this.Playlists)
                {
                    duration += playlist.Duration_ms;
                }

                return duration;
            }
        }

        public virtual ICollection<Playlist> Playlists { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<Song> Songs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder
                .Conventions
                .Remove<PluralizingTableNameConvention>();

            modelBuilder
                .Configurations
                .Add(new IdentityUserLoginConfiguration())
                .Add(new IdentityUserRoleConfiguration());

            modelBuilder.Entity<Playlist>()
                .HasMany(c => c.Songs).WithMany(i => i.Playlists)
                .Map(t => t.MapLeftKey("PlaylistId")
                    .MapRightKey("SongId")
                    .ToTable("PlaylistSong"));

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(c => c.Playlists).WithMany(i => i.AppUsers)
                .Map(t => t.MapLeftKey("AppUserId")
                    .MapRightKey("PlaylistId")
                    .ToTable("UserPlaylist"));
        }
    }
    public class IdentityUserLoginConfiguration : EntityTypeConfiguration<IdentityUserLogin>
    {
        public IdentityUserLoginConfiguration()
        {
            HasKey(iul => iul.UserId);
        }
    }

    public class IdentityUserRoleConfiguration : EntityTypeConfiguration<IdentityUserRole>
    {
        public IdentityUserRoleConfiguration()
        {
            HasKey(iur => iur.UserId);
        }
    }
}