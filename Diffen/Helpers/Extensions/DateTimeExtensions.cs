using System;

namespace Diffen.Helpers.Extensions
{
	public static class DateTimeExtensions
	{
		public static string GetSinceStamp(this DateTime dt)
		{
			return dt.ToString("yyyy-MM-dd HH:mm");
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
