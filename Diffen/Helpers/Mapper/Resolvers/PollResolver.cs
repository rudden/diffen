using System;
using System.Linq;
using System.Collections.Generic;

using AutoMapper;

namespace Diffen.Helpers.Mapper.Resolvers
{
	using Models;
	using Extensions;

	public class PollResolver : 
		ITypeConverter<Database.Entities.Other.Poll, Models.Other.Poll>,
		ITypeConverter<Database.Entities.Other.PollSelection, Models.Other.PollSelection>,
		ITypeConverter<Models.Other.CRUD.PollVote, Database.Entities.Other.PollVote>,
		ITypeConverter<Models.Other.CRUD.Poll, Database.Entities.Other.Poll>,
		ITypeConverter<string, Database.Entities.Other.PollSelection>
	{
		public Models.Other.Poll Convert(Database.Entities.Other.Poll source, Models.Other.Poll destination, ResolutionContext context)
		{
			return new Models.Other.Poll
			{
				Id = source.Id,
				Name = source.Name,
				Selections = context.Mapper.Map<IEnumerable<Models.Other.PollSelection>>(source.Selections),
				ByUser = new IdAndNickNameUser
				{
					Id = source.CreatedByUserId,
					NickName = source.CreatedByUser.NickNames.Current()
				},
				Created = source.Created.ToShortDateString(),
				IsOpen = source.Created.AddDays(7) > DateTime.Now
			};
		}

		public Models.Other.PollSelection Convert(Database.Entities.Other.PollSelection source, Models.Other.PollSelection destination, ResolutionContext context)
		{
			return new Models.Other.PollSelection
			{
				Id = source.Id,
				Name = source.Name,
				Votes = source.Votes?.Select(vote => new IdAndNickNameUser
				{
					Id = vote.VotedByUserId,
					NickName = vote.VotedByUser.NickNames.Current()
				}),
				IsWinner = IsWinner(source)
			};
		}

		public Database.Entities.Other.PollVote Convert(Models.Other.CRUD.PollVote source, Database.Entities.Other.PollVote destination, ResolutionContext context)
		{
			return new Database.Entities.Other.PollVote
			{
				PollSelectionId = source.PollSelectionId,
				VotedByUserId = source.VotedByUserId,
				Created = DateTime.Now
			};
		}


		public Database.Entities.Other.Poll Convert(Models.Other.CRUD.Poll source, Database.Entities.Other.Poll destination, ResolutionContext context)
		{
			return new Database.Entities.Other.Poll
			{
				Name = source.Name,
				CreatedByUserId = source.CreatedByUserId,
				Created = DateTime.Now
			};
		}

		public Database.Entities.Other.PollSelection Convert(string source, Database.Entities.Other.PollSelection destination, ResolutionContext context)
		{
			return new Database.Entities.Other.PollSelection
			{
				Name = source
			};
		}

		private static bool IsWinner(Database.Entities.Other.PollSelection pollSelection)
		{
			// no winner determined if poll is still open
			if (pollSelection.Poll.Created.AddDays(7) > DateTime.Now)
			{
				return false;
			}

			var highestValue = pollSelection.Poll.Selections.Select(x => 
				new KeyValuePair<int, int>(x.Id, x.Votes.Count))
				.Select(x => x.Value)
				.Distinct()
				.OrderByDescending(x => x).First();

			return pollSelection.Votes.Count == highestValue;
		}
	}
}
