using System.Threading.Tasks;
using System.Collections.Generic;

namespace Diffen.Database.Clients.Contracts
{
	using Helpers;
	using Entities.User;
	using Entities.Forum;
	using Entities.Squad;
	using Entities.Other;

	public interface IDiffenDbClient
	{
		// Post Related Requests
		Task<int> CountPostsAsync();
		Task<List<Post>> GetPostsAsync();
		Task<List<Post>> GetPagedPostsAsync(int pageNumber, int pageSize);
		Task<Paging<Post>> GetPostsOnFilterAsync(Models.Forum.Filter filter, int pageNumber, int pageSize);
		Task<List<Post>> GetPostsOnUserIdAsync(string userId);
		Task<List<Post>> GetSavedPostsOnUserIdAsync(string userId);
		Task<SavedPost> GetSavedPostsOnPostAndUserIdAsync(int postId, string userId);
		Task<Post> GetPostOnIdAsync(int postId);
		Task<List<Post>> GetConversationOnPostIdAsync(int postId);
		Task<List<UrlTip>> GetUrlTipsAsync();
		Task<UrlTip> GetUrlTipOnIdAsync(int tipId);
		Task<List<Vote>> GetVotesAsync();
		Task<List<Vote>> GetVotesOnPostIdAsync(int postId);
		Task<List<Vote>> GetVotesOnUserIdAsync(string userId);
		Task<bool> CreatePostAsync(Post post);
		Task<bool> UpdatePostAsync(Post post);
		Task<bool> DeletePostAsync(int postId);
		Task<bool> ScissorPostAsync(Scissored scissoredPost);
		Task<bool> SavePostForUserAsync(SavedPost savedPost);
		Task<bool> DeleteSavedPostForUserAsync(SavedPost savedPost);
		Task<bool> ConnectLineupToPostAsync(PostToLineup postToLineup);
		Task<bool> DeleteLineupConnectionToPostAsync(int postId);
		Task<bool> PostHasALineupConnectedToItAsync(int postId);
		Task<bool> PostHasAnUrlTipConnectedToItAsync(int postId);
		bool UrlTipEqualsCurrentUrlTipOnPostId(int postId, string urlTipHref);
		Task<bool> CreateUrlTipAsync(UrlTip urlTip);
		Task<bool> UpdateUrlTipAsync(UrlTip urlTip);
		Task<bool> DeleteUrlTipAsync(int postId);
		Task<bool> IncrementUrlTipClickCounterAsync(string subject, int postId);
		Task<bool> CreateVoteAsync(Vote vote);
		Task<bool> DeleteVoteAsync(int voteId);
		Task<bool> UserHasAlreadyVotedOnPostAsync(int postId, string userId);
		Task<List<Thread>> GetPostThreadsAsync();
		Task<int> GetNumberOfPostsOnThreadAsync(int threadId);
		Task<bool> AddThreadsToPostAsync(int postId, IEnumerable<int> threadIds);
		Task<bool> CreateThreadAsync(Thread thread);
		Task<bool> CreatePostThreadsAndConnectToNewPostWithIdAsync(int postId, List<string> threadNames);
		Task<bool> DeleteExistingThreadsOnPostAsync(int postId);

		// User Related Requests
		Task<List<AppUser>> GetUsersAsync();
		Task<List<AppUser>> GetUsersExceptForLoggedInUserAsync();
		Task<AppUser> GetUserOnIdAsync(string userId);
		Task<AppUser> GetUserOnEmailAsync(string userEmail);
		Task<FavoritePlayer> GetFavoritePlayerOnUserIdAsync(string userId);
		Task<string> GetCurrentNickNameOnUserIdAsync(string userId);
		Task<Filter> GetBaseFilterForForumOnUserIdAsync(string userId);
		Task<List<Invite>> GetInvitesAsync();
		Task<Invite> GetInviteOnUniqueCodeAsync(string code);
		Task<List<PersonalMessage>> GetPmsSentFromUserToUserAsync(string fromUserId, string toUserId);
		Task<List<AppUser>> GetUsersThatUserHasOngoingConversationWithAsync(string userId);
		Task<bool> CreatePersonalMessageAsync(PersonalMessage personalMessage);
		Task<bool> MarkPmAsReadByOnIdAsync(int personalMessageId);
		Task<List<PersonalMessage>> GetUnReadPersonalMessagesForUserWithIdAsync(string userId);
		Task<int> GetNumberOfUnReadPersonalMessagesForUserWithIdAsync(string userId);
		Task<int> GetNumberOfUnReadPersonalMessagesFromUserToUserAsync(string fromUserId, string toUserId);
		Task<bool> UpdateUserAsync(AppUser user);
		Task<bool> UpdateUserBioAsync(string userId, string newBio);
		Task<bool> UserHasAFavoritePlayerSelectedAsync(string userId);
		Task<bool> ConnectFavoritePlayerToUserAsync(FavoritePlayer favoritePlayer);
		Task<bool> DeleteFavoritePlayerConnectionToUserAsync(string userId);
		Task<bool> CreateNewNickNameForUserAsync(NickName nickName);
		Task<bool> NickNameIsAlreadyTakenByOtherUserAsync(string nickName);
		Task<bool> SetSelectedAvatarFileNameForUserAsync(string userId, string fileName);
		Task<bool> CreateBaseFilterForForumOnUserAsync(Filter filter);
		Task<bool> UpdateBaseFilterForForumOnUserAsync(Filter filter);
		Task<bool> AnActiveInviteExistsOnCodeAsync(string code);
		Task<bool> AnActiveAccountIsCreatedOnEmailUsingCodeAsync(string code, string email);
		Task<bool> CreateInviteAsync(Invite invite);
		Task<bool> UpdateInviteAsync(Invite invite);
		Task<bool> DeleteInviteAsync(int inviteId);

		// Squad Related Requests
		Task<Lineup> GetLineupOnIdAsync(int lineupId);
		Task<Lineup> GetLineupOnPostIdAsync(int postId);
		Task<List<Lineup>> GetLineupsCreatedByUserIdAsync(string userId);
		Task<List<Game>> GetGamesWherePlayerStartedButNoEventsAsync(int playerId);
		Task<List<Player>> GetPlayersAsync();
		Task<Player> GetPlayerOnIdAsync(int playerId);
		Task<List<Position>> GetPositionsAsync();
		Task<List<Formation>> GetFormationsAsync();
		Task<bool> CreateLineupAsync(Lineup lineup);
		Task<bool> CreatePlayerAsync(Player player);
		Task<bool> UpdatePlayerAsync(Player player);
		Task<bool> DeleteFavoritePlayerRelationToUserForPlayerAsync(int playerId);
		Task<bool> UpdateAvailablePositionsForPlayerAsync(int playerId, IEnumerable<int> positionIds);
		Task<List<Game>> GetGamesAsync();
		Task<Game> GetUpcomingGameAsync();
		Task<Game> GetGameOnIdAsync(int gameId);
		Task<List<PlayerEvent>> GetPlayerEventsAsync();
		Task<bool> CreateGameAsync(Game game);
		Task<bool> UpdateGameAsync(Game game);
		Task<bool> UpdateGameWithLineupAsync(int gameId, int lineupId);
		Task<bool> CreatePlayerEventsAsync(List<PlayerEvent> events);
		Task<bool> DeletePlayerEventsOnGameIdAsync(int gameId);
		Task<List<Title>> GetTitlesAsync();
		Task<List<GameResultGuess>> GetFinishedGameResultGuessesAsync();
		Task<List<GameResultGuess>> GetGameResultGuessesForGameWithIdAsync(int gameId);
		Task<List<GameResultGuess>> GetGameResultGuessesForUserWithIdAsync(string userId);
		Task<bool> CreateGameResultGuessAsync(GameResultGuess guess);
		Task<List<Season>> GetSeasonsAsync();

		// Poll Related Requests
		Task<List<Poll>> GetPollsAsync();
		Task<List<Poll>> GetActivePollsAsync();
		Task<List<Poll>> GetLastNthActivePollsAsync(int amount = 5);
		Task<List<Poll>> GetPollsOnUserIdAsync(string userId);
		Task<Poll> GetPollOnIdAsync(int pollId);
		Task<Poll> GetPollOnSlugAsync(string slug);
		Task<bool> UserHasAlreadyVotedOnPollAsync(Models.Other.CRUD.PollVote pollVote);
		Task<bool> CreatePollAsync(Poll poll);
		Task<bool> CreatePollSelectionsAsync(IEnumerable<PollSelection> selections);
		Task<bool> CreateVoteOnPollAsync(PollVote vote);

		// Chronicle Related Requests
		Task<List<Chronicle>> GetChroniclesAsync(int amount = 0);
		Task<List<Chronicle>> GetChroniclesOnUserIdAsync(string userId);
		Task<Chronicle> GetLastAddedChronicleAsync();
		Task<List<ChronicleCategory>> GetChronicleCategoriesAsync();
		Task<Chronicle> GetChronicleOnIdAsync(int chronicleId);
		Task<Chronicle> GetChronicleOnSlugAsync(string slug);
		Task<bool> CreateChronicleAsync(Chronicle chronicle);
		Task<bool> AddCategoriesToChronicleAsync(int chronicleId, IEnumerable<int> categoryIds);
		Task<bool> CreateNewChronicleCategoriesAndConnectToNewChronicleWithIdAsync(int chronicleId, List<string> categoryNames);
		Task<bool> AddOrRemoveCategoriesToExistingChronicleAsync(int chronicleId, IEnumerable<int> categoryIds);
		Task<bool> UpdateChronicleAsync(Chronicle chronicle);
		Task<bool> SetHeaderFileNameOnChronicleAsync(Chronicle chronicle);
		Task<bool> ChronicleIsCreatedBySelectedUserAsync(string slug, string userId);

		// Region Related Requests
		Task<List<Region>> GetRegionsAsync();
		Task<bool> UserHasRegionSelectedAsync(string userId);
		bool RegionWithSameNameAlreadyExists(string newRegionName);
		Task<bool> CreateRegionAsync(Region region);
		Task<bool> CreateRegionToUserAsync(string userId, int regionId);
		Task<bool> UpdateRegionForUserAsync(string userId, string newRegion);
		Task<bool> DeleteRegionForUserAsync(string userId);
	}
}
