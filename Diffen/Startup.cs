using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using AutoMapper;

namespace Diffen
{
	using Database;
	using Database.Entities.User;
	using Repositories;
	using Repositories.Contracts;

	public class Startup
	{
		public IConfigurationRoot Configuration { get; }

		public Startup(IHostingEnvironment environment)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(environment.ContentRootPath)
				.AddJsonFile($"appsettings.{environment.EnvironmentName}.json")
				.AddEnvironmentVariables();

			Configuration = builder.Build();
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton(Configuration);

			services.AddDbContext<DiffenDbContext>();

			services.AddIdentity<AppUser, IdentityRole>(c =>
			{
				c.User.RequireUniqueEmail = true;
				c.Password.RequiredLength = 8;
			}).AddEntityFrameworkStores<DiffenDbContext>();

			services.AddAutoMapper();

			services.AddMvc()
				.AddJsonOptions(o =>
				{
					o.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
				});

			services.AddScoped<IPmRepository, PmRepository>();
			services.AddScoped<IPostRepository, PostRepository>();
			services.AddScoped<IUserRepository, UserRepository>();
			services.AddScoped<ISquadRepository, SquadRepository>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("Home/Error");
			}

			app.UseMvc(r =>
			{
				r.MapRoute(name: "default", template: "{controller=home}/{action=index}/{id?}");
			});
		}
	}
}
