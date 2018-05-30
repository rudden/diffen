using System;

namespace Diffen.Helpers.Extensions
{
	public static class DateTimeExtensions
	{
		public static string GetSinceStamp(this DateTime dt)
		{
			if (dt <= DateTime.Now.AddHours(-1))
			{
				return dt.ToString("yyyy-MM-dd HH:mm");
			}
			var minutes = (DateTime.Now - dt).Minutes;
			return minutes <= 0 ? "alldeles nyss" : dt.ToString("yyyy-MM-dd HH:mm");
		}

		public static string GetSinceStamp(this DateTime? dtn)
		{
			return dtn == null ? null : Convert.ToDateTime(dtn).GetSinceStamp();
		}

		public static string GetSecluded(this DateTime? dt)
		{
			if (dt != null)
				return dt.Value < DateTime.Now ? "" : dt.Value.ToString("yyyy-MM-dd");
			return "";
		}
	}
}
