using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Diffen.Helpers.Extensions
{
	public static class JsonExtensions
	{
		public static string AsJson(this object obj)
		{
			return JsonConvert.SerializeObject(obj, new JsonSerializerSettings
			{
				ContractResolver = new CamelCasePropertyNamesContractResolver()
			});
		}
	}
}
