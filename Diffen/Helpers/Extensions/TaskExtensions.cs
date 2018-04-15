using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Diffen.Helpers.Extensions
{
	using Models;

	public static class TaskExtensions
	{
		public static void UpdateResults(this Task<bool> task, Message message, ICollection<Result> results)
		{
			var result = new Result
			{
				Type = task.IsFaulted ? ResultType.Failure : (ResultType)Convert.ToInt32(task.Result)
			};
			switch (result.Type)
			{
				case ResultType.Failure:
					result.Message = message.Failure;
					break;
				case ResultType.Success:
					result.Message = message.Success;
					break;
			}
			results.Add(result);
		}
	}
}
