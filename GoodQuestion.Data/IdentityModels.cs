using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Security.Claims;
using System.Threading.Tasks;
using GoodQuestion.Data;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace GoodQuestion.WebAPI.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string SpotifyUserId { get; set; }
        public string SpotifyAuthToken { get; set; }
        public string SpotifyRefreshToken { get; set; }
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
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<Playlist>()
                .HasMany(c => c.Songs).WithMany(i => i.Playlists)
                .Map(t => t.MapLeftKey("PlaylistId")
                    .MapRightKey("SongId")
                    .ToTable("PlaylistSong"));
        }
    }
}