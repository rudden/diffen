using System;
using System.Linq;

using Slugify;
using AutoMapper;

namespace Diffen.Helpers.Mapper.Resolvers
{
	public class ChronicleResolver : 
		ITypeConverter<Database.Entities.Other.Chronicle, Models.Other.Chronicle>,
		ITypeConverter<Database.Entities.Other.ChronicleCategory, Models.Other.ChronicleCategory>,
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
				WrittenByUser = context.Mapper.Map<Models.User.User>(source.WrittenByUser),
				Categories = source.Categories.Select(x => x.Category).Select(context.Mapper.Map<Models.Other.ChronicleCategory>),
				Created = source.Created.ToString("yyyy-MM-dd"),
				Updated = source.Updated.ToString("yyyy-MM-dd"),
				Published = source.Published.ToString("yyyy-MM-dd")
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
				Published = !string.IsNullOrEmpty(source.Published) ? System.Convert.ToDateTime(source.Published) : DateTime.Now
			};
		}

		public Models.Other.ChronicleCategory Convert(Database.Entities.Other.ChronicleCategory source, Models.Other.ChronicleCategory destination, ResolutionContext context)
		{
			return new Models.Other.ChronicleCategory
			{
				Id = source.Id,
				Name = source.Name
			};
		}
	}
}
