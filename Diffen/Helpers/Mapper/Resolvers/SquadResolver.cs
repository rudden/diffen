using System.Linq;
using System.Collections.Generic;

using AutoMapper;

namespace Diffen.Helpers.Mapper.Resolvers
{
	using Business;

	using DbPlayer = Database.Entities.Squad.Player;
	using DbLineup = Database.Entities.Squad.Lineup;
	using DbPlayerToLineup = Database.Entities.Squad.PlayerToLineup;

	using ModelPlayer = Models.Squad.Player;
	using ModelLineup = Models.Squad.Lineup;
	using ModelPlayerToLineup = Models.Squad.PlayerToLineup;

	public class SquadResolver : 
		ITypeConverter<DbPlayer, ModelPlayer>, 
		ITypeConverter<DbLineup, ModelLineup>,
		ITypeConverter<DbPlayerToLineup, ModelPlayerToLineup>
	{
		public ModelPlayer Convert(DbPlayer source, ModelPlayer destination, ResolutionContext context)
		{
			return new ModelPlayer
			{
				Id = source.Id,
				FirstName = source.FirstName,
				LastName = source.LastName,
				KitNumber = source.KitNumber,
				IsOutOnLoan = source.IsOutOnLoan,
				IsHereOnLoan = source.IsHereOnLoan,
				IsCaptain = source.IsCaptain,
				IsSold = source.IsSold
			};
		}

		public ModelLineup Convert(DbLineup source, ModelLineup destination, ResolutionContext context)
		{
			return new ModelLineup
			{
				Id = source.Id,
				ComponentName = FormationList.All().FirstOrDefault(f => f.Name == source.Formation.Name)?.ComponentName,
				Players = context.Mapper.Map<List<ModelPlayerToLineup>>(source.Players),
				Created = source.Created.ToString("yyyy-MM-dd")
			};
		}

		public ModelPlayerToLineup Convert(DbPlayerToLineup source, ModelPlayerToLineup destination, ResolutionContext context)
		{
			return new ModelPlayerToLineup
			{
				Id = source.Id,
				Player = context.Mapper.Map<ModelPlayer>(source.Player),
				Position = source.Position.Name
			};
		}
	}
}
