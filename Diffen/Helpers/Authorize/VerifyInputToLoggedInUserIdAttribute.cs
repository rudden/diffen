using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Diffen.Helpers.Authorize
{
	using Extensions;

	public class VerifyInputToLoggedInUserIdAttribute : ActionFilterAttribute
	{
		private readonly string _requestUserId;

		public VerifyInputToLoggedInUserIdAttribute(string requestUserId)
		{
			_requestUserId = requestUserId;
		}

		public override void OnActionExecuting(ActionExecutingContext context)
		{
			context.ActionArguments.TryGetValue(_requestUserId, out var value);
			var requestUserId = value as string;

			var loggedInUserId = context.HttpContext.User.GetUserId();
			if (!string.IsNullOrWhiteSpace(requestUserId) && !string.IsNullOrWhiteSpace(loggedInUserId))
			{
				if (requestUserId.Equals(loggedInUserId))
				{
					base.OnActionExecuting(context);
					return;
				}
			}
			context.Result = new ForbidResult();
		}
	}
}
