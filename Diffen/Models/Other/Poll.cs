using System.Collections.Generic;

namespace Diffen.Models.Other
{
	public class Poll
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Slug { get; set; }
		public IEnumerable<PollSelection> Selections { get; set; }
		public IdAndNickNameUser ByUser { get; set; }
		public string Created { get; set; }
		public bool IsOpen { get; set; }
	}

	public class PollSelection
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public IEnumerable<IdAndNickNameUser> Votes { get; set; }
		public bool IsWinner { get; set; }
	}
}
