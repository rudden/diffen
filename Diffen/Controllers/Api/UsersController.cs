using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

using Serilog;

namespace Diffen.Controllers.Api
{
	using Helpers;
	using Helpers.Authorize;
	using Models;
	using Models.User;
	using Models.Forum;

	using Repositories.Contracts;

	[Authorize]
	[Route("api/[controller]")]
	public class UsersController : ControllerBase
	{
		private readonly IPmRepository _pmRepository;
		private readonly IPostRepository _postRepository;
		private readonly IUserRepository _userRepository;

		private readonly RoleManager<IdentityRole> _roleManager;

		private readonly ILogger _logger = Log.ForContext<UsersController>();

		public UsersController(IPmRepository pmRepository, IPostRepository postRepository, IUserRepository userRepository, RoleManager<IdentityRole> roleManager)
		{
			_pmRepository = pmRepository;
			_postRepository = postRepository;
			_userRepository = userRepository;
			_roleManager = roleManager;
		}

		[HttpGet]
		public Task<List<KeyValuePair<string, string>>> Get()
		{
			_logger.Debug("Requesting all users except for logged in user in the form of key value pair with user id and nickname");
			return _userRepository.GetUsersAsKeyValuePairAsync();
		}

		[HttpGet("{userId}")]
		public Task<Models.User.User> Get(string userId)
		{
			_logger.Debug("Requesting user with id {userId}", userId);
			return _userRepository.GetUserOnIdAsync(userId);
		}

		[HttpGet("{userId}/posts/{pageNumber}")]
		public Task<Paging<Post>> GetPosts(string userId, int pageNumber)
		{
			_logger.Debug("Requesting to get paged posts with page number {pageNumber} for user with id {userId}", pageNumber, userId);
			return _postRepository.GetPagedPostsOnUserIdAsync(userId, pageNumber);
		}

		[HttpGet("{userId}/posts/saved/{pageNumber}")]
		public Task<Paging<Post>> GetSavedPosts(string userId, int pageNumber)
		{
			_logger.Debug("Requesting to get paged saved posts with page number {pageNumber} for user with id {userId}", pageNumber, userId);
			return _postRepository.GetPagedSavedPostsAsync(userId, pageNumber);
		}

		[Authorize(Policy = "IsManager")]
		[HttpPost("{userId}/seclude")]
		public Task<List<Result>> Seclude(string userId, string to)
		{
			_logger.Debug("Requesting to seclude user with id {userId} to date {toDate}", userId, to);
			return _userRepository.SecludeUserAsync(userId, to);
		}

		[VerifyInputToLoggedInUserId("userId")]
		[HttpPost("{userId}/update")]
		public Task<List<Result>> UpdateUser(string userId, [FromBody] Models.User.CRUD.User user)
		{
			_logger.Debug("Requesting to update user with id {userId}", userId);
			return _userRepository.UpdateUserAsync(userId, user);
		}

		[HttpGet("roles")]
		public Task<List<string>> GetRoles()
		{
			_logger.Debug("Requesting to get all roles");
			return _roleManager.Roles.Select(x => x.Name).ToListAsync();
		}

		[HttpGet("role")]
		public Task<List<KeyValuePair<string, string>>> GetUsersInRole(string roleName)
		{
			_logger.Debug("Requesting users in role with name {roleName} in the form of key value pair with user id and nickname", roleName);
			return _userRepository.GetUsersInRoleAsKeyValuePairAsync(roleName);
		}

		[HttpGet("invites")]
		public Task<List<Invite>> GetInvites()
		{
			_logger.Debug("Requesting to get all invites");
			return _userRepository.GetInvitesAsync();
		}

		[VerifyInputToLoggedInUserId("invite", "InvitedByUserId")]
		[HttpPost("invites/create")]
		public Task<List<string>> AddInvites([FromBody] Models.User.CRUD.Invite invite)
		{
			_logger.Debug("Requesting to create an invite");
			return _userRepository.CreateInvitesAsync(invite);
		}

		[VerifyInputToLoggedInUserId("filter", "UserId")]
		[HttpPost("filter")]
		public Task<List<Result>> ChangeFilter([FromBody] Models.User.Filter filter)
		{
			_logger.Debug("Requesting to change filter for user with id {userId}", filter.UserId);
			return _userRepository.UpdateUserFilterAsync(filter);
		}

		[VerifyInputToLoggedInUserId("userId")]
		[HttpGet, Route("{userId}/pm")]
		public Task<List<PersonalMessage>> GetPersonalMessages(string userId, string to = null)
		{
			_logger.Debug("Requesting to get personal messages between user with id {userId} and user with id {toUserId}", userId, to);
			return _pmRepository.GetPmsSentFromUserToUserAsync(userId, to);
		}

		[VerifyInputToLoggedInUserId("userId")]
		[HttpGet("{userId}/pm/users")]
		public Task<List<Conversation>> GetUsersThatUserHasConversationWith(string userId)
		{
			_logger.Debug("Requesting to get a list of users that the selected user has a conversation with (personal message)");
			return _pmRepository.GetUsersWithConversationForUserAsKeyValuePairAsync(userId);
		}

		[VerifyInputToLoggedInUserId("pm", "FromUserId")]
		[HttpPost("pm/create")]
		public Task<List<Result>> CreatePm([FromBody] Models.User.CRUD.PersonalMessage pm)
		{
			_logger.Debug("Requesting to create a personal message");
			return _pmRepository.CreatePersonalMessageAsync(pm);
		}

		[VerifyInputToLoggedInUserId("userId")]
		[HttpPost("{userId}/avatar/{fileName}")]
		public Task<List<Result>> UpdateAvatarFileName(string userId, string fileName)
		{
			_logger.Debug("Requesting to update avatar for user with id {userId}", userId);
			return _userRepository.UpdateAvatarFileNameForUserWithIdAsync(userId, fileName);
		}

		[VerifyInputToLoggedInUserId("userId")]
		[HttpDelete, Route("{userId}/avatar")]
		public Task<List<Result>> DeleteAvatarForUser(string userId)
		{
			_logger.Debug("Requesting to delete avatar for user with id {userId}", userId);
			return _userRepository.ResetUsersAvatarToGenericAsync(userId);
		}
	}
}
