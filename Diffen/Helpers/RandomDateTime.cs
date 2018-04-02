using System;

namespace Diffen.Helpers
{
	public static class RandomDateTime
	{
		public static DateTime Get()
		{
			var start = new DateTime(2017, 12, 1);
			var end = new DateTime(2018, 6, 1);

			var rand = new Random();
			var range = (end - start).Days;

			return start.AddDays(rand.Next(range)).AddHours(rand.Next(0, 24)).AddMinutes(rand.Next(0, 60)).AddSeconds(rand.Next(0, 60));
		}

		public static DateTime Get(DateTime start, DateTime end)
		{
			var rand = new Random();
			var range = (end - start).Days;

			return start.AddDays(rand.Next(range)).AddHours(rand.Next(0, 24)).AddMinutes(rand.Next(0, 60)).AddSeconds(rand.Next(0, 60));
		}
	}
}
