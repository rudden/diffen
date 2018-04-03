using System;
using System.Globalization;

namespace Diffen.Helpers.Extensions
{
	public static class DateTimeExtensions
	{
		public static string GetSinceStamp(this System.DateTime dt)
		{
			if (dt < System.DateTime.Now.AddDays(-1))
			{
				if (Convert.ToInt32((System.DateTime.Now - dt).Days) > 7)
				{
					return dt.ToString("yyyy-MM-dd HH:mm");
				}
				return (System.DateTime.Now - dt).Days.ToString(CultureInfo.InvariantCulture) + " d sedan";
			}
			if (dt > System.DateTime.Now.AddHours(-1))
			{
				var minutes = (System.DateTime.Now - dt).Minutes;
				if (minutes <= 0)
				{
					return "alldeles nyss";
				}
				return (System.DateTime.Now - dt).Minutes.ToString(CultureInfo.InvariantCulture) + " m sedan";
			}
			if (dt > System.DateTime.Now.AddDays(-1))
			{
				return (System.DateTime.Now - dt).Hours.ToString(CultureInfo.InvariantCulture) + " t sedan";
			}
			return "";
		}

		public static string GetSinceStamp(this System.DateTime? dtn)
		{
			return dtn == null ? null : Convert.ToDateTime(dtn).GetSinceStamp();
		}

		public static string GetSecluded(this System.DateTime dt)
		{
			return dt < System.DateTime.Now ? "" : dt.ToString("yyyy-MM-dd");
		}
	}
}
