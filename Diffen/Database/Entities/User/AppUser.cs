using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Identity;

namespace Diffen.Database.Entities.User
{
	using Forum;
	using Squad;
	using Other;

	public class AppUser : IdentityUser
	{
		public string Bio { get; set; }
		public string AvatarFileName { get; set; }
		public DateTime Joined { get; set; }
		public DateTime? SecludedUntil { get; set; }

		// Linked Tables
		public Filter Filter { get; set; }
		public FavoritePlayer FavoritePlayer { get; set; }
		public ICollection<Vote> Votes { get; set; }
		public ICollection<NickName> NickNames { get; set; }
		public ICollection<Post> Posts { get; set; }
		public ICollection<SavedPost> SavedPosts { get; set; }
		public ICollection<Lineup> Lineups { get; set; }
		public RegionToUser Region { get; set; }
		public ICollection<GameResultGuess> GameResultGuesses { get; set; }
	}
}