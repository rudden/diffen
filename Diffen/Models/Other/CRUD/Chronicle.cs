namespace Diffen.Models.Other.CRUD
{
	public class Chronicle
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string Text { get; set; }
		public string WrittenByUserId { get; set; }
		public string Published { get; set; }
	}
}
