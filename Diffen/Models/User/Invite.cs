namespace Diffen.Models.User
{
	public class Invite
	{
		public string UniqueCode { get; set; }
		public InvitedBy InvitedBy { get; set; }
		public InvitedBy InviteUsedBy { get; set; }

		public bool AccountIsCreated { get; set; }

		public string InviteSent { get; set; }
		public string AccountCreated { get; set; }
	}

	public class InvitedBy
	{
		public string Id { get; set; }
		public string NickName { get; set; }
	}
}
