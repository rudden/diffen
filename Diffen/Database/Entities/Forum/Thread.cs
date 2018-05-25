using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Diffen.Database.Entities.Forum
{
	using Helpers.Enum;

	public class Thread
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		public string Name { get; set; }

		public ThreadType Type { get; set; }

		public DateTime Created { get; set; }

		public DateTime? StartTime { get; set; }
		public DateTime? EndTime { get; set; }
	}
}
