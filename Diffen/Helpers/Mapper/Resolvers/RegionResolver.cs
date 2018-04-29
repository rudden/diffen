using System;
using System.Linq;

using AutoMapper;

namespace Diffen.Helpers.Mapper.Resolvers
{
	using Models;
	using Extensions;

	public class RegionResolver : 
		ITypeConverter<Database.Entities.Other.Region, Models.Other.Region>,
		ITypeConverter<Models.Other.CRUD.Region, Database.Entities.Other.Region>
	{
		public Models.Other.Region Convert(Database.Entities.Other.Region source, Models.Other.Region destination, ResolutionContext context)
		{
			return new Models.Other.Region
			{
				Id = source.Id,
				Name = source.Name,
				Longitud = source.Longitud,
				Latitud = source.Latitud,
				Users = source.UsersInRegion.Select(x => x.User).Select(user => new IdAndNickNameUser
				{
					Id = user.Id,
					NickName = user.NickNames.Current()
				})
			};
		}

		public Database.Entities.Other.Region Convert(Models.Other.CRUD.Region source, Database.Entities.Other.Region destination, ResolutionContext context)
		{
			return new Database.Entities.Other.Region
			{
				Name = source.Name,
				Longitud = source.Longitud,
				Latitud = source.Latitud,
				Created = DateTime.Now
			};
		}
	}
}
