using System.Collections.Generic;

namespace Diffen.Models.Other
{
	public class Chronicle
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string HeaderFileName { get; set; }
		public string Text { get; set; }
		public string Slug { get; set; }
		public User.User WrittenByUser { get; set; }
		public IEnumerable<ChronicleCategory> Categories { get; set; }
		public string Created { get; set; }
		public string Updated { get; set; }
		public string Published { get; set; }
	}
}
