using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using Serilog;

namespace Diffen.Helpers.Authorize
{
	using Extensions;

	public class VerifyInputToLoggedInUserIdAttribute : ActionFilterAttribute
	{
		private readonly string _key;
		private readonly string _value;
		private readonly ILogger _logger = Log.ForContext<VerifyInputToLoggedInUserIdAttribute>();

		public VerifyInputToLoggedInUserIdAttribute(string key, string value = null)
		{
			_key = key;
			_value = value;
		}

		public override void OnActionExecuting(ActionExecutingContext context)
		{
			context.ActionArguments.TryGetValue(_key, out var value);

			string requestUserId;
			if (!string.IsNullOrEmpty(_value))
			{
				requestUserId = value?.GetType().GetProperty(_value).GetValue(value, null) as string;
			}
			else
			{
				requestUserId = value as string;
			}

			var loggedInUserId = context.HttpContext.User.GetUserId();
			if (!string.IsNullOrWhiteSpace(requestUserId) && !string.IsNullOrWhiteSpace(loggedInUserId))
			{
				if (requestUserId.Equals(loggedInUserId))
				{
					base.OnActionExecuting(context);
					return;
				}
			}
			_logger.Information("LoggedInUserId {@loggedInUserId} does not match RequestUserId {@requestUserId}", loggedInUserId, requestUserId);
			context.Result = new BadRequestResult();
		}
	}
}