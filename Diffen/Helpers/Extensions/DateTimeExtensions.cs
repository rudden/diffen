using System;
using System.Globalization;

namespace Diffen.Helpers.Extensions
{
	public static class DateTimeExtensions
	{
		public static string GetSinceStamp(this DateTime dt)
		{
			if (dt < DateTime.Now.AddDays(-1))
			{
				if (Convert.ToInt32((DateTime.Now - dt).Days) > 7)
				{
					return dt.ToString("yyyy-MM-dd HH:mm");
				}
				return (DateTime.Now - dt).Days.ToString(CultureInfo.InvariantCulture) + " d sedan";
			}
			if (dt > DateTime.Now.AddHours(-1))
			{
				var minutes = (DateTime.Now - dt).Minutes;
				if (minutes <= 0)
				{
					return "alldeles nyss";
				}
				return (DateTime.Now - dt).Minutes.ToString(CultureInfo.InvariantCulture) + " m sedan";
			}
			if (dt > DateTime.Now.AddDays(-1))
			{
				return (DateTime.Now - dt).Hours.ToString(CultureInfo.InvariantCulture) + " t sedan";
			}
			return "";
		}

		public static string GetSinceStamp(this DateTime? dtn)
		{
			return dtn == null ? null : Convert.ToDateTime(dtn).GetSinceStamp();
		}

		public static string GetSecluded(this DateTime dt)
		{
			return dt < DateTime.Now ? "" : dt.ToString("yyyy-MM-dd");
		}
	}
}
