using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Diffen.Database.Entities.User
{
	public class Filter
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		public int PostsPerPage { get; set; }

		// comma separated
		public string ExcludedUserIds { get; set; }
		public string ExcludedThreadIds { get; set; }

		[ForeignKey("UserId")]
		public AppUser User { get; set; }
		public string UserId { get; set; }

		public bool HideLeftMenu { get; set; }
		public bool HideRightMenu { get; set; }
	}
}
