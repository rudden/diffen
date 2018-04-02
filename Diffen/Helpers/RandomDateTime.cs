using System;

namespace Diffen.Helpers
{
	public static class RandomDateTime
	{
		public static DateTime Get()
		{
			return Randomize(new DateTime(2017, 12, 1), new DateTime(2018, 6, 1));
		}

		public static DateTime Get(DateTime start, DateTime end)
		{
			return Randomize(start, end);
		}

		public static DateTime Randomize(DateTime start, DateTime end)
		{
			var rand = new Random();
			var range = (end - start).Days;
			return start.AddDays(rand.Next(range)).AddHours(rand.Next(0, 24)).AddMinutes(rand.Next(0, 60)).AddSeconds(rand.Next(0, 60));
		}
	}
}
