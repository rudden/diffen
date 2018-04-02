using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Diffen.Database.Entities.Squad
{
	public class Player
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		public string FirstName { get; set; }
		public string LastName { get; set; }
		public int KitNumber { get; set; }
		public bool IsCaptain { get; set; }
		public bool IsOutOnLoan { get; set; }
		public bool IsHereOnLoan { get; set; }
		public bool IsSold { get; set; }
	}
}