namespace Diffen.Models
{
	public class Result
	{
		public ResultType Type { get; set; }
		public string Message { get; set; }

		public Result() {}

		public Result(bool outcome)
		{
			Type = outcome ? ResultType.Success : ResultType.Failure;
		}

		public Result ComplementWithMessageAndReturn(Message message)
		{
			switch (Type)
			{
				case ResultType.Failure:
					Message = message.Failure;
					break;
				case ResultType.Success:
					Message = message.Success;
					break;
				default:
					Message = "";
					break;
			}
			return this;
		}
	}

	public enum ResultType
	{
		Failure, Success
	}

	public static class ResultMessages
	{
		public static Message CreatePost { get; } = new Message
		{
			Success = "Inlägget skapades",
			Failure = "Kunde inte skapa inlägget"
		};

		public static Message UpdatePost { get; } = new Message
		{
			Success = "Inlägget uppdaterades",
			Failure = "Kunde inte uppdatera	 inlägget"
		};

		public static Message CreateUrlTip { get; } = new Message
		{
			Success = "",
			Failure = "Kunde inte lägga till länktipset"
		};

		public static Message CreateLineupToPost { get; } = new Message
		{
			Success = "",
			Failure = "Kunde inte koppla startelvan till inlägget"
		};

		public static Message CreateLineup { get; } = new Message
		{
			Success = "Skapade startelvan",
			Failure = "Kunde inte koppla startelvan till inlägget"
		};

		public static Message CreateFavoritePlayer { get; } = new Message
		{
			Success = "La till favoritspelaren",
			Failure = "Kunde inte lägga till spelaren som favorit"
		};

		public static Message CreateNick { get; } = new Message
		{
			Success = "Skapade ett nytt nick",
			Failure = "Kunde inte skapa ett nytt nick"
		};

		public static Message UpdateBio { get; } = new Message
		{
			Success = "Ändrade bio",
			Failure = "Kunde inte ändra bio"
		};

		public static Message CreateInvite { get; } = new Message
		{
			Success = "Skickade invite",
			Failure = "Kunde inte skicka invite"
		};

		public static Message CreatePm { get; } = new Message
		{
			Success = "Skickade pm",
			Failure = "Kunde inte skicka pm"
		};

		public static Message CreatePlayer { get; } = new Message
		{
			Success = "Skapade spelaren",
			Failure = "Kunde inte skapa spelaren"
		};

		public static Message UpdatePlayer { get; } = new Message
		{
			Success = "Uppdaterade spelaren",
			Failure = "Kunde inte uppdatera spelaren"
		};

		public static Message UpdateRoles { get; } = new Message
		{
			Success = "Uppdaterade användarens rollen",
			Failure = "Kunde inte uppdatera användarens roller"
		};

		public static Message ChangeFilter { get; } = new Message
		{
			Success = "Uppdaterade filter för forumet",
			Failure = "Kunde inte uppdatera filter för forumet"
		};

		public static Message RemovedFavoritePlayer { get; } = new Message
		{
			Success = "Tog bort favoritspelaren",
			Failure = "Kunde inte ta bort favoritspelaren"
		};

		public static Message CreateSeclude { get; } = new Message
		{
			Success = "Spärrade användaren",
			Failure = "Kunde inte spärra användaren"
		};
	}

	public class Message
	{
		public string Success { get; set; }
		public string Failure { get; set; }
	}
}
