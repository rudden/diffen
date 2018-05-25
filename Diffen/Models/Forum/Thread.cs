
namespace Diffen.Models.Forum
{
	using Helpers.Enum;

	public class Thread
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public ThreadType Type { get; set; }
		public int NumberOfPosts { get; set; }
		public string StartTime { get; set; }
		public string EndTime { get; set; }
	}
}
