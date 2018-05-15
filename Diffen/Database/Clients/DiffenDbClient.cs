using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Diffen.Database.Clients
{
	using Contracts;
	using Entities.User;
	using Entities.Forum;
	using Entities.Squad;
	using Entities.Other;
	using Helpers.Extensions;

	public class DiffenDbClient : IDiffenDbClient
	{
		private readonly DiffenDbContext _dbContext;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public DiffenDbClient(DiffenDbContext dbContext, IHttpContextAccessor httpContextAccessor)
		{
			_dbContext = dbContext;
			_httpContextAccessor = httpContextAccessor;
		}

		public Task<int> CountPostsAsync()
		{
			return _dbContext.Posts.CountAsync();
		}

		public Task<List<Post>> GetPostsAsync()
		{
			return _dbContext.Posts.IncludeAll().ToListAsync();
		}

		public Task<List<Post>> GetPagedPostsAsync(int pageNumber, int pageSize)
		{
			return _dbContext.Posts.IncludeAll().ExceptScissored()
				.OrderByCreated().Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToListAsync();
		}

		public Task<List<Post>> GetPostsOnFilterAsync(Models.Forum.Filter filter)
		{
			var posts = _dbContext.Posts.IncludeAll().ExceptScissored();
			if (filter == null)
			{
				return posts.ToListAsync();
			}
			if (filter.ExcludedUsers != null && filter.ExcludedUsers.Any())
			{
				posts = posts.Where(x => !filter.ExcludedUsers.Select(y => y.Key).Contains(x.CreatedByUserId));
			}
			if (filter.FromDate != null)
			{
				posts = posts.Where(p => p.Created.Date >= Convert.ToDateTime(filter.FromDate).Date);
			}
			if (filter.ToDate != null)
			{
				posts = posts.Where(p => p.Created.Date <= Convert.ToDateTime(filter.ToDate).Date);
			}
			if (!string.IsNullOrEmpty(filter.MessageWildCard))
			{
				posts = posts.Where(p => p.Message.ToLower().Contains(filter.MessageWildCard.ToLower()));
			}
			switch (filter.StartingEleven)
			{
				case Models.Forum.StartingEleven.With:
					posts = posts.Where(x => x.Lineups.Any());
					break;
				case Models.Forum.StartingEleven.Without:
					posts = posts.Where(x => !x.Lineups.Any());
					break;
				case Models.Forum.StartingEleven.All:
					break;
			}
			if (filter.IncludedUsers != null && filter.IncludedUsers.Any())
			{
				posts = posts.Where(x => filter.IncludedUsers.Select(y => y.Key).Contains(x.CreatedByUserId));
			}
			return posts.OrderByCreated().ToListAsync();
		}

		public Task<List<Post>> GetPostsOnUserIdAsync(string userId)
		{
			return _dbContext.Posts.IncludeAll().ExceptScissored()
				.Where(post => post.CreatedByUserId == userId).OrderByCreated().ToListAsync();
		}

		public async Task<List<Post>> GetSavedPostsOnUserIdAsync(string userId)
		{
			var savedPosts = await _dbContext.SavedPosts.IncludeAll().ToListAsync();
			return savedPosts.Where(post => post.SavedByUserId == userId).Select(x => x.Post).OrderByDescending(x => x.Created).ToList();
		}

		public Task<Post> GetPostOnIdAsync(int postId)
		{
			return _dbContext.Posts.IncludeAll().FirstOrDefaultAsync(x => x.Id == postId);
		}

		public async Task<List<Post>> GetConversationOnPostIdAsync(int postId)
		{
			var list = new List<Post>();
			var initialPost = await _dbContext.Posts.IncludeAll().ExceptScissored().FirstOrDefaultAsync(x => x.Id == postId);
			if (initialPost == null)
				return new List<Post>();
			list.Add(initialPost);
			await LoadConversationAsync(initialPost, list);
			return list.OrderBy(x => x.Created).ToList();
		}

		#region Conversation Related Helpers
		private async Task LoadConversationAsync(Post post, ICollection<Post> posts)
		{
			await LoadPostAsync(post, posts, PostRelationType.Child);
			await LoadPostAsync(post, posts, PostRelationType.Parent);
		}

		private async Task LoadPostAsync(Post post, ICollection<Post> posts, PostRelationType relationType)
		{
			var relationPosts = await LoadPostsOnRelationTypeAsync(post, relationType);
			if (relationPosts == null || !relationPosts.Any())
				return;

			foreach (var relationPost in relationPosts)
			{
				await AddPostToCollectionAsync(relationPost, posts);
			}
		}

		private Task<List<Post>> LoadPostsOnRelationTypeAsync(Post post, PostRelationType relationType)
		{
			switch (relationType)
			{
				case PostRelationType.Child:
					return _dbContext.Posts.IncludeAll().ExceptScissored().Where(x => x.ParentPostId == post.Id).ToListAsync();
				case PostRelationType.Parent:
					return _dbContext.Posts.IncludeAll().ExceptScissored().Where(x => x.Id == post.ParentPostId).ToListAsync();
				default:
					return null;
			}
		}

		private async Task AddPostToCollectionAsync(Post post, ICollection<Post> posts)
		{
			var postIds = posts.Any() ? posts.Select(x => x.Id) : new List<int>();
			if (postIds.Contains(post.Id))
				return;
			posts.Add(post);
			await LoadConversationAsync(post, posts);
		}

		public enum PostRelationType
		{
			Child,
			Parent
		}
		#endregion

		public Task<List<UrlTip>> GetUrlTipsAsync()
		{
			return _dbContext.UrlTips.Include(x => x.Post).ToListAsync();
		}

		public Task<UrlTip> GetUrlTipOnIdAsync(int tipId)
		{
			return _dbContext.UrlTips.FindAsync(tipId);
		}

		public Task<List<Vote>> GetVotesAsync()
		{
			return _dbContext.Votes.OrderByDescending(x => x.Created).ToListAsync();
		}

		public Task<List<Vote>> GetVotesOnPostIdAsync(int postId)
		{
			return _dbContext.Votes.Include(x => x.User).ThenInclude(x => x.NickNames)
				.Where(x => x.PostId == postId).OrderByDescending(x => x.Created).ToListAsync();
		}

		public Task<List<Vote>> GetVotesOnUserIdAsync(string userId)
		{
			return _dbContext.Votes.Where(x => x.CreatedByUserId == userId).OrderByDescending(x => x.Created).ToListAsync();
		}

		public Task<bool> CreatePostAsync(Post post)
		{
			_dbContext.Posts.Add(post);
			return CommitedResultIsSuccessfulAsync();
		}

		public async Task<bool> UpdatePostAsync(Post post)
		{
			_dbContext.Posts.Update(post);
			_dbContext.Entry(post).State = EntityState.Modified;
			_dbContext.Entry(post).Property(x => x.Created).IsModified = false;
			var result = await CommitedResultIsSuccessfulAsync();
			_dbContext.Entry(post).State = EntityState.Detached;
			return result;
		}

		public Task<bool> DeletePostAsync(int postId)
		{
			var entity = _dbContext.Posts.Find(postId);

			if (entity == null)
				return Task.FromResult(false);

			_dbContext.Posts.Remove(entity);
			return CommitedResultIsSuccessfulAsync();
		}

		public Task<bool> ScissorPostAsync(Scissored scissoredPost)
		{
			_dbContext.ScissoredPosts.Add(scissoredPost);
			return CommitedResultIsSuccessfulAsync();
		}

		public Task<bool> SavePostForUserAsync(SavedPost savedPost)
		{
			_dbContext.SavedPosts.Add(savedPost);
			return CommitedResultIsSuccessfulAsync();
		}

		public Task<bool> ConnectLineupToPostAsync(PostToLineup postToLineup)
		{
			_dbContext.LineupsOnPosts.Add(postToLineup);
			return CommitedResultIsSuccessfulAsync();
		}

		public Task<bool> DeleteLineupConnectionToPostAsync(int postId)
		{
			var entity = _dbContext.LineupsOnPosts.FirstOrDefault(e => e.PostId == postId);

			if (entity == null)
				return Task.FromResult(false);

			_dbContext.LineupsOnPosts.Remove(entity);
			return CommitedResultIsSuccessfulAsync();
		}

		public async Task<bool> PostHasALineupConnectedToItAsync(int postId)
		{
			return await _dbContext.LineupsOnPosts.CountAsync(x => x.PostId == postId) > 0;
		}

		public async Task<bool> PostHasAnUrlTipConnectedToItAsync(int postId)
		{
			return await _dbContext.UrlTips.CountAsync(x => x.PostId == postId) > 0;
		}

		public bool UrlTipEqualsCurrentUrlTipOnPostId(int postId, string urlTipHref)
		{
			var urlTips = _dbContext.UrlTips.Where(x => x.PostId == postId).OrderByDescending(x => x.Created);
			return urlTips.FirstOrDefault()?.Href == urlTipHref;
		}

		public Task<bool> CreateUrlTipAsync(UrlTip urlTip)
		{
			_dbContext.UrlTips.Add(urlTip);
			return CommitedResultIsSuccessfulAsync();
		}

		public Task<bool> UpdateUrlTipAsync(UrlTip urlTip)
		{
			_dbContext.UrlTips.Update(urlTip);
			return CommitedResultIsSuccessfulAsync();
		}

		public Task<bool> DeleteUrlTipAsync(int postId)
		{
			var entities = _dbContext.UrlTips.Where(e => e.PostId == postId);

			if (!entities.Any())
				return Task.FromResult(false);

			foreach (var entity in entities)
			{
				entity.PostId = null;
				_dbContext.UrlTips.Update(entity);
			}

			return CommitedResultIsSuccessfulAsync();
		}

		public Task<bool> IncrementUrlTipClickCounterAsync(string subject, int id)
		{
			UrlTip tip = null;
			switch (subject.ToLower())
			{
				case "post":
					tip = _dbContext.UrlTips.Where(t => t.PostId == id).Current();
					break;
				case "tip":
					tip = _dbContext.UrlTips.Where(t => t.Id == id).Current();
					break;
			}
			if (tip == null)
				return Task.FromResult(false);

			tip.Clicks++;
			_dbContext.UrlTips.Update(tip);
			return CommitedResultIsSuccessfulAsync();
		}

		public Task<bool> CreateVoteAsync(Vote vote)
		{
			_dbContext.Votes.Add(vote);
			return CommitedResultIsSuccessfulAsync();
		}

		public async Task<bool> UserHasAlreadyVotedOnPostAsync(int postId, string userId)
		{
			return await _dbContext.Votes.CountAsync(x => x.PostId == postId && x.CreatedByUserId == userId) > 0;
		}

		public Task<List<AppUser>> GetUsersAsync()
		{
			return _dbContext.Users
				.Include(x => x.NickNames)
				.OrderByDescending(x => x.Joined)
				.ToListAsync();
		}

		public Task<List<AppUser>> GetUsersExceptForLoggedInUserAsync()
		{
			return _dbContext.Users
				.Include(x => x.NickNames)
				.Where(x => x.UserName != _httpContextAccessor.HttpContext.User.Identity.Name)
				.OrderByDescending(x => x.Joined)
				.ToListAsync();
		}

		public Task<AppUser> GetUserOnIdAsync(string userId)
		{
			return _dbContext.Users.IncludeAll().FirstOrDefaultAsync(user => user.Id == userId);
		}

		public Task<AppUser> GetUserOnEmailAsync(string userEmail)
		{
			return _dbContext.Users.IncludeAll().FirstOrDefaultAsync(user => user.Email == userEmail);
		}

		public Task<FavoritePlayer> GetFavoritePlayerOnUserIdAsync(string userId)
		{
			return _dbContext.FavoritePlayers.Include(x => x.Player).FirstOrDefaultAsync(x => x.UserId == userId);
		}

		public Task<string> GetCurrentNickNameOnUserIdAsync(string userId)
		{
			return _dbContext.NickNames.Where(x => x.UserId == userId).OrderByDescending(x => x.Created).Select(x => x.Nick).FirstOrDefaultAsync();
		}

		public Task<Filter> GetBaseFilterForForumOnUserIdAsync(string userId)
		{
			return _dbContext.UserFilters.FirstOrDefaultAsync(u => u.UserId == userId);
		}

		public Task<List<Invite>> GetInvitesAsync()
		{
			return _dbContext.Invites
				.Include(x => x.InvitedByUser).ThenInclude(x => x.NickNames)
				.Include(x => x.InviteUsedByUser).ThenInclude(x => x.NickNames)
				.OrderByDescending(x => x.InviteSent).ToListAsync();
		}

		public Task<Invite> GetInviteOnUniqueCodeAsync(string code)
		{
			return _dbContext.Invites.FirstOrDefaultAsync(x => x.UniqueCode.Equals(code));
		}

		public Task<List<PersonalMessage>> GetPmsSentFromUserToUserAsync(string fromUserId, string toUserId)
		{
			return _dbContext.PersonalMessages.IncludeAll()
				.Where(x => (x.FromUserId == fromUserId || x.FromUserId == toUserId) && (x.ToUserId == toUserId || x.ToUserId == fromUserId))
				.OrderByDescending(x => x.Created).ToListAsync();
		}

		public async Task<List<AppUser>> GetUsersThatUserHasOngoingConversationWithAsync(string userId)
		{
			var all = await _dbContext.PersonalMessages.IncludeAll()
				.Where(x => x.FromUserId == userId || x.ToUserId == userId).ToListAsync();
			var from = all.Select(x => x.FromUser);
			var to = all.Select(x => x.ToUser);

			var users = from.Distinct().Union(to.Distinct()).ToList();
			users.RemoveAll(x => x.Id == userId);
			return users;
		}

		public Task<bool> CreatePersonalMessageAsync(PersonalMessage personalMessage)
		{
			_dbContext.PersonalMessages.Add(personalMessage);
			return CommitedResultIsSuccessfulAsync();
		}

		public Task<bool> UpdateUserAsync(AppUser user)
		{
			_dbContext.Users.Update(user);
			return CommitedResultIsSuccessfulAsync();
		}

		public async Task<bool> UpdateUserBioAsync(string userId, string newBio)
		{
			var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
			user.Bio = newBio;
			_dbContext.Users.Update(user);
			return await CommitedResultIsSuccessfulAsync();
		}

		public async Task<bool> UserHasAFavoritePlayerSelectedAsync(string userId)
		{
			return await _dbContext.FavoritePlayers.CountAsync(x => x.UserId == userId) > 0;
		}

		public Task<bool> ConnectFavoritePlayerToUserAsync(FavoritePlayer favoritePlayer)
		{
			_dbContext.FavoritePlayers.Add(favoritePlayer);
			return CommitedResultIsSuccessfulAsync();
		}

		public Task<bool> DeleteFavoritePlayerConnectionToUserAsync(string userId)
		{
			var entity = _dbContext.FavoritePlayers.FirstOrDefault(e => e.UserId == userId);

			if (entity == null)
				return Task.FromResult(false);

			_dbContext.FavoritePlayers.Remove(entity);
			return CommitedResultIsSuccessfulAsync();
		}

		public Task<bool> CreateNewNickNameForUserAsync(NickName nickName)
		{
			_dbContext.NickNames.Add(nickName);
			return CommitedResultIsSuccessfulAsync();
		}

		public async Task<bool> NickNameIsAlreadyTakenByOtherUserAsync(string nickName)
		{
			var activeNicks = _dbContext.NickNames.OrderByDescending(x => x.Created).GroupBy(x => x.UserId).Select(x => x.FirstOrDefault().Nick);
			return await activeNicks.CountAsync(x => x == nickName) > 0;
		}

		public async Task<bool> SetSelectedAvatarFileNameForUserAsync(string userId, string fileName)
		{
			var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
			user.AvatarFileName = fileName;
			_dbContext.Users.Update(user);
			return await _dbContext.SaveChangesAsync() >= 0;
		}

		public Task<bool> CreateBaseFilterForForumOnUserAsync(Filter filter)
		{
			_dbContext.UserFilters.Add(filter);
			return CommitedResultIsSuccessfulAsync();
		}

		public Task<bool> UpdateBaseFilterForForumOnUserAsync(Filter filter)
		{
			_dbContext.UserFilters.Update(filter);
			return CommitedResultIsSuccessfulAsync();
		}

		public async Task<bool> AnActiveInviteExistsOnCodeAsync(string code)
		{
			return await _dbContext.Invites.CountAsync(x => x.UniqueCode.Equals(code) && !x.AccountIsCreated) > 0;
		}

		public async Task<bool> CreateInviteAsync(Invite invite)
		{
			invite.InviteSent = DateTime.Now;
			_dbContext.Invites.Add(invite);
			return await CommitedResultIsSuccessfulAsync();
		}

		public Task<bool> UpdateInviteAsync(Invite invite)
		{
			_dbContext.Invites.Update(invite);
			return CommitedResultIsSuccessfulAsync();
		}

		public Task<bool> DeleteInviteAsync(int inviteId)
		{
			var entity = _dbContext.Invites.Find(inviteId);

			if (entity == null)
				return Task.FromResult(false);

			_dbContext.Invites.Remove(entity);
			return CommitedResultIsSuccessfulAsync();
		}

		public Task<Lineup> GetLineupOnIdAsync(int lineupId)
		{
			return _dbContext.Lineups.IncludeAll().FirstOrDefaultAsync(l => l.Id == lineupId);
		}

		public Task<Lineup> GetLineupOnPostIdAsync(int postId)
		{
			var lineupToPost = _dbContext.LineupsOnPosts.FirstOrDefault(x => x.PostId == postId);
			return _dbContext.Lineups.IncludeAll().FirstOrDefaultAsync(l => l.Id == lineupToPost.LineupId);
		}

		public Task<List<Lineup>> GetLineupsCreatedByUserIdAsync(string userId)
		{
			return _dbContext.Lineups.IncludeAll().Where(l => l.CreatedByUserId == userId).ToListAsync();
		}

		public Task<List<Player>> GetPlayersAsync()
		{
			return _dbContext.Players.Include(x => x.AvailablePositions).ThenInclude(x => x.Position).Include(x => x.InLineups).Where(x => !x.IsSold).OrderBy(x => x.LastName).ToListAsync();
		}

		public Task<Player> GetPlayerOnIdAsync(int playerId)
		{
			return _dbContext.Players.Include(x => x.AvailablePositions).ThenInclude(x => x.Position).FirstOrDefaultAsync(x => x.Id == playerId);
		}

		public Task<List<Position>> GetPositionsAsync()
		{
			return _dbContext.Positions.ToListAsync();
		}

		public Task<List<Formation>> GetFormationsAsync()
		{
			return _dbContext.Formations.ToListAsync();
		}

		public Task<bool> CreateLineupAsync(Lineup lineup)
		{
			_dbContext.Lineups.Add(lineup);
			return CommitedResultIsSuccessfulAsync();
		}

		public Task<bool> CreatePlayerAsync(Player player)
		{
			_dbContext.Players.Add(player);
			return CommitedResultIsSuccessfulAsync();
		}

		public Task<bool> UpdatePlayerAsync(Player player)
		{
			_dbContext.Players.Update(player);
			return CommitedResultIsSuccessfulAsync();
		}

		public Task<bool> DeleteFavoritePlayerRelationToUserForPlayerAsync(int playerId)
		{
			_dbContext.FavoritePlayers.RemoveRange(_dbContext.FavoritePlayers.Where(x => x.PlayerId == playerId));
			return CommitedResultIsSuccessfulAsync();
		}

		public Task<bool> UpdateAvailablePositionsForPlayerAsync(int playerId, IEnumerable<int> positionIds)
		{
			var relations = _dbContext.PlayersToPositions.Where(x => x.PlayerId == playerId).ToList();
			if (relations.Any())
			{
				foreach (var relation in relations)
				{
					_dbContext.PlayersToPositions.Remove(relation);
				}
			}
			foreach (var positionId in positionIds)
			{
				_dbContext.PlayersToPositions.Add(new PlayerToPosition
				{
					PlayerId = playerId,
					PositionId = positionId
				});
			}
			return CommitedResultIsSuccessfulAsync();
		}

		public Task<List<Poll>> GetPollsAsync()
		{
			return _dbContext.Polls.IncludeAll().OrderByDescending(x => x.Created).ToListAsync();
		}

		public Task<List<Poll>> GetActivePollsAsync()
		{
			return _dbContext.Polls.IncludeAll().Where(poll => poll.Created.AddDays(7) >= DateTime.Now).OrderByDescending(x => x.Created).ToListAsync();
		}

		public Task<List<Poll>> GetLastNthActivePollsAsync(int amount = 5)
		{
			return _dbContext.Polls.IncludeAll().Where(poll => poll.Created.AddDays(7) >= DateTime.Now).OrderByDescending(x => x.Created).Take(amount).ToListAsync();
		}

		public Task<List<Poll>> GetPollsOnUserIdAsync(string userId)
		{
			return _dbContext.Polls.IncludeAll().Where(poll => poll.CreatedByUserId == userId).OrderByDescending(x => x.Created).ToListAsync();
		}

		public Task<Poll> GetPollOnIdAsync(int pollId)
		{
			return _dbContext.Polls.IncludeAll().FirstOrDefaultAsync(poll => poll.Id == pollId);
		}

		public Task<Poll> GetPollOnSlugAsync(string slug)
		{
			return _dbContext.Polls.IncludeAll().FirstOrDefaultAsync(poll => poll.Slug == slug);
		}

		public async Task<bool> UserHasAlreadyVotedOnPollAsync(Models.Other.CRUD.PollVote pollVote)
		{
			var poll = await _dbContext.Polls.IncludeAll().FirstOrDefaultAsync(x => x.Selections.Select(y => y.Id).Contains(pollVote.PollSelectionId));
			return poll != null && poll.Selections.Any(selection => selection.Votes.Select(x => x.VotedByUserId).Contains(pollVote.VotedByUserId));
		}

		public Task<bool> CreatePollAsync(Poll poll)
		{
			_dbContext.Polls.Add(poll);
			return CommitedResultIsSuccessfulAsync();
		}

		public Task<bool> CreatePollSelectionsAsync(IEnumerable<PollSelection> selections)
		{
			_dbContext.PollSelections.AddRange(selections);
			return CommitedResultIsSuccessfulAsync();
		}

		public Task<bool> CreateVoteOnPollAsync(PollVote vote)
		{
			_dbContext.PollVotes.Add(vote);
			return CommitedResultIsSuccessfulAsync();
		}

		public async Task<List<Chronicle>> GetChroniclesAsync(int amount = 0)
		{
			var chronicles = await _dbContext.Chronicles.IncludeAll().OrderByDescending(x => x.Created).ToListAsync();
			return amount == 0 ? chronicles : chronicles.Take(amount).ToList();
		}

		public Task<List<Chronicle>> GetChroniclesOnUserIdAsync(string userId)
		{
			return _dbContext.Chronicles.IncludeAll().Where(x => x.WrittenByUserId == userId).OrderByDescending(x => x.Created).ToListAsync();
		}

		public Task<Chronicle> GetChronicleOnIdAsync(int chronicleId)
		{
			return _dbContext.Chronicles.IncludeAll().FirstOrDefaultAsync(x => x.Id == chronicleId);
		}

		public Task<Chronicle> GetChronicleOnSlugAsync(string slug)
		{
			return _dbContext.Chronicles.IncludeAll().FirstOrDefaultAsync(x => x.Slug == slug);
		}

		public Task<Chronicle> GetLastAddedChronicleAsync()
		{
			return _dbContext.Chronicles.IncludeAll().OrderByDescending(x => x.Created).FirstOrDefaultAsync();
		}

		public Task<List<ChronicleCategory>> GetChronicleCategoriesAsync()
		{
			return _dbContext.ChronicleCategories.ToListAsync();
		}

		public Task<bool> CreateChronicleAsync(Chronicle chronicle)
		{
			chronicle.Created = DateTime.Now;
			if (_dbContext.Chronicles.Count(c=> c.Slug == chronicle.Slug) > 0)
			{
				chronicle.Slug = $"{chronicle.Slug}-{Guid.NewGuid().ToString().Substring(0, 8)}";
			}
			_dbContext.Chronicles.Add(chronicle);
			return CommitedResultIsSuccessfulAsync();
		}

		public Task<bool> AddCategoriesToChronicleAsync(int chronicleId, IEnumerable<int> categoryIds)
		{
			_dbContext.ChroniclesToCategories.AddRange(categoryIds.Select(id => new ChronicleToCategory
			{
				CategoryId = id,
				ChronicleId = chronicleId
			}));
			return CommitedResultIsSuccessfulAsync();
		}

		public async Task<bool> CreateNewChronicleCategoriesAndConnectToNewChronicleWithIdAsync(int chronicleId, List<string> categoryNames)
		{
			_dbContext.ChronicleCategories.AddRange(categoryNames.Select(name => new ChronicleCategory
			{
				Name = name
				}));
			await CommitedResultIsSuccessfulAsync();
			var newCategories = _dbContext.ChronicleCategories.Where(category => categoryNames.Contains(category.Name));
			_dbContext.ChroniclesToCategories.AddRange(newCategories.Select(category => new ChronicleToCategory
			{
				CategoryId = category.Id,
				ChronicleId = chronicleId
			}));
			return await CommitedResultIsSuccessfulAsync();
		}

		public async Task<bool> AddOrRemoveCategoriesToExistingChronicleAsync(int chronicleId, IEnumerable<int> categoryIds)
		{
			_dbContext.ChroniclesToCategories.RemoveRange(
				_dbContext.ChroniclesToCategories.Where(c => c.ChronicleId == chronicleId));
			await CommitedResultIsSuccessfulAsync();
			return await AddCategoriesToChronicleAsync(chronicleId, categoryIds);
		}

		public async Task<bool> UpdateChronicleAsync(Chronicle chronicle)
		{
			chronicle.Updated = DateTime.Now;
			_dbContext.Chronicles.Update(chronicle);
			_dbContext.Entry(chronicle).State = EntityState.Modified;
			_dbContext.Entry(chronicle).Property(x => x.HeaderFileName).IsModified = false;
			_dbContext.Entry(chronicle).Property(x => x.Created).IsModified = false;
			var result = await CommitedResultIsSuccessfulAsync();
			_dbContext.Entry(chronicle).State = EntityState.Detached;
			return result;
		}

		public async Task<bool> SetHeaderFileNameOnChronicleAsync(Chronicle chronicle)
		{
			_dbContext.Chronicles.Update(chronicle);
			_dbContext.Entry(chronicle).State = EntityState.Modified;
			_dbContext.Entry(chronicle).Property(x => x.Created).IsModified = false;
			var result = await CommitedResultIsSuccessfulAsync();
			_dbContext.Entry(chronicle).State = EntityState.Detached;
			return result;
		}

		public async Task<bool> ChronicleIsCreatedBySelectedUserAsync(string slug, string userId)
		{
			return await _dbContext.Chronicles.CountAsync(c => c.WrittenByUserId == userId && c.Slug == slug) > 0;
		}

		public async Task<List<Region>> GetRegionsAsync()
		{
			var regions = await _dbContext.Regions.IncludeAll().ToListAsync();
			return regions.OrderByDescending(x => x.UsersInRegion.Count).ToList();
		}

		public async Task<bool> UserHasRegionSelectedAsync(string userId)
		{
			var currentRegionSelection = await _dbContext.UsersToRegions.FirstOrDefaultAsync(x => x.UserId == userId);
			return currentRegionSelection != null;
		}

		public bool RegionWithSameNameAlreadyExists(string newRegionName)
		{
			return _dbContext.Regions.Any(r => string.Equals(r.Name, newRegionName, StringComparison.OrdinalIgnoreCase));
		}

		public Task<bool> CreateRegionAsync(Region region)
		{
			_dbContext.Regions.Add(region);
			return CommitedResultIsSuccessfulAsync();
		}

		public Task<bool> CreateRegionToUserAsync(string userId, int regionId)
		{
			var entity = new RegionToUser
			{
				UserId = userId,
				RegionId = regionId
			};
			_dbContext.UsersToRegions.Add(entity);
			return CommitedResultIsSuccessfulAsync();
		}

		public async Task<bool> UpdateRegionForUserAsync(string userId, string newRegion)
		{
			var currentRegionSelection = await _dbContext.UsersToRegions.FirstOrDefaultAsync(x => x.UserId == userId);
			if (currentRegionSelection != null)
			{
				_dbContext.UsersToRegions.Remove(currentRegionSelection);
				await _dbContext.SaveChangesAsync();
			}
			_dbContext.UsersToRegions.Add(new RegionToUser
			{
				RegionId = (await _dbContext.Regions.FirstOrDefaultAsync(x => x.Name == newRegion)).Id,
				UserId = userId
			});
			return await CommitedResultIsSuccessfulAsync();
		}

		public async Task<bool> DeleteRegionForUserAsync(string userId)
		{
			var currentRegionSelection = await _dbContext.UsersToRegions.FirstOrDefaultAsync(x => x.UserId == userId);
			if (currentRegionSelection == null)
			{
				return false;
			}
			_dbContext.UsersToRegions.Remove(currentRegionSelection);
			return await CommitedResultIsSuccessfulAsync();
		}

		public Task<List<Game>> GetGamesAsync()
		{
			return _dbContext.Games.IncludeAll().OrderByDescending(x => x.OnDate).ToListAsync();
		}

		public Task<Game> GetGameOnIdAsync(int gameId)
		{
			return _dbContext.Games.IncludeAll().FirstOrDefaultAsync(game => game.Id == gameId);
		}

		public Task<List<PlayerEvent>> GetPlayerEventsAsync()
		{
			return _dbContext.PlayerEvents.IncludeAll().ToListAsync();
		}

		public Task<bool> CreateGameAsync(Game game)
		{
			_dbContext.Games.Add(game);
			return CommitedResultIsSuccessfulAsync();
		}

		public Task<bool> CreatePlayerEventsAsync(List<PlayerEvent> events)
		{
			_dbContext.PlayerEvents.AddRange(events);
			return CommitedResultIsSuccessfulAsync();
		}

		private async Task<bool> CommitedResultIsSuccessfulAsync()
		{
			return await _dbContext.SaveChangesAsync() >= 0;
		}
	}
}
