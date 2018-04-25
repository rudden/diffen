using System;

using Slugify;
using AutoMapper;

namespace Diffen.Helpers.Mapper.Resolvers
{
	using Models;
	using Extensions;

	public class ChronicleResolver : 
		ITypeConverter<Database.Entities.Other.Chronicle, Models.Other.Chronicle>,
		ITypeConverter<Models.Other.CRUD.Chronicle, Database.Entities.Other.Chronicle>
	{
		public Models.Other.Chronicle Convert(Database.Entities.Other.Chronicle source, Models.Other.Chronicle destination, ResolutionContext context)
		{
			return new Models.Other.Chronicle
			{
				Id = source.Id,
				Title = source.Title,
				Text = source.Text,
				HeaderFileName = source.HeaderFileName ?? "banner.jpg",
				Slug = source.Slug,
				WrittenByUser = new IdAndNickNameUser
				{
					Id = source.WrittenByUserId,
					NickName = source.WrittenByUser.NickNames.Current()
				},
				Created = source.Created.ToShortDateString(),
				Updated = source.Updated.ToShortDateString()
			};
		}

		public Database.Entities.Other.Chronicle Convert(Models.Other.CRUD.Chronicle source, Database.Entities.Other.Chronicle destination, ResolutionContext context)
		{
			return new Database.Entities.Other.Chronicle
			{
				Id = source.Id,
				Title = source.Title,
				Text = source.Text,
				Slug = new SlugHelper().GenerateSlug(source.Title),
				WrittenByUserId = source.WrittenByUserId,
				Created = DateTime.Now
			};
		}
	}
}
