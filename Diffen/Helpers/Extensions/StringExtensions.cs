using System.Linq;

namespace Diffen.Helpers.Extensions
{
	public static class StringExtensions
	{
		public static string TrimStartAndEnd(this string str)
		{
			return str.TrimStart();
		}

		public static string FirstUpperCase(this string str)
		{
			return char.ToUpper(str[0]) + str.Substring(1);
		}

		public static string FirstLetterOfWords(this string str)
		{
			return string.Join(string.Empty, str.Split(' ').Select(w => $"{w.First()}. ")).Trim().TrimEnd('.');
		}
	}
}
