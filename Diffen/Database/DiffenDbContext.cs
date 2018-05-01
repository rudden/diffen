using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Diffen.Database
{
	using Entities.User;
	using Entities.Forum;
	using Entities.Squad;
	using Entities.Other;

	public sealed class DiffenDbContext : IdentityDbContext<AppUser>
	{
		private readonly IConfigurationRoot _configuration;

		public DbSet<Post> Posts { get; set; }
		public DbSet<UrlTip> UrlTips { get; set; }
		public DbSet<Vote> Votes { get; set; }
		public DbSet<PostToLineup> LineupsOnPosts { get; set; }
		public DbSet<Scissored> ScissoredPosts { get; set; }

		public DbSet<NickName> NickNames { get; set; }
		public DbSet<Invite> Invites { get; set; }
		public DbSet<FavoritePlayer> FavoritePlayers { get; set; }
		public DbSet<SavedPost> SavedPosts { get; set; }
		public DbSet<PersonalMessage> PersonalMessages { get; set; }
		public DbSet<Filter> UserFilters { get; set; }

		public DbSet<Player> Players { get; set; }
		public DbSet<Position> Positions { get; set; }
		public DbSet<Formation> Formations { get; set; }
		public DbSet<Lineup> Lineups { get; set; }
		public DbSet<PlayerToLineup> PlayersToLineups { get; set; }
		public DbSet<PlayerToPosition> PlayersToPositions { get; set; }

		public DbSet<Poll> Polls { get; set; }
		public DbSet<PollSelection> PollSelections { get; set; }
		public DbSet<PollVote> PollVotes { get; set; }
		public DbSet<Chronicle> Chronicles { get; set; }
		public DbSet<Region> Regions { get; set; }
		public DbSet<RegionToUser> UsersToRegions { get; set; }

		public DiffenDbContext(IConfigurationRoot configuration, DbContextOptions options) : base(options)
		{
			_configuration = configuration;
			Database.Migrate();
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);
			optionsBuilder.UseSqlServer(_configuration["ConnectionStrings:DiffenDb"]);
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
			builder.Entity<UrlTip>().HasIndex(b => b.PostId).IsUnique(false);
			builder.Entity<NickName>().HasIndex(b => b.UserId).IsUnique(false);
		}
	}
}
