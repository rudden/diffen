﻿// <auto-generated />
using Diffen.Database;
using Diffen.Helpers.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace Diffen.Migrations
{
    [DbContext(typeof(DiffenDbContext))]
    [Migration("20180402085459_initial")]
    partial class initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Diffen.Database.Entities.Forum.Post", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedByUserId");

                    b.Property<DateTime?>("Edited");

                    b.Property<string>("Message");

                    b.HasKey("Id");

                    b.HasIndex("CreatedByUserId");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("Diffen.Database.Entities.Forum.PostToLineup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("LineupId");

                    b.Property<int>("PostId");

                    b.HasKey("Id");

                    b.HasIndex("LineupId");

                    b.HasIndex("PostId");

                    b.ToTable("LineupsOnPosts");
                });

            modelBuilder.Entity("Diffen.Database.Entities.Forum.PostToPost", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("ParentPostId");

                    b.Property<int?>("PostId");

                    b.HasKey("Id");

                    b.HasIndex("ParentPostId");

                    b.HasIndex("PostId");

                    b.ToTable("Conversations");
                });

            modelBuilder.Entity("Diffen.Database.Entities.Forum.UrlTip", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Clicks");

                    b.Property<DateTime>("Created");

                    b.Property<string>("Href");

                    b.Property<int>("PostId");

                    b.HasKey("Id");

                    b.HasIndex("PostId");

                    b.ToTable("UrlTips");
                });

            modelBuilder.Entity("Diffen.Database.Entities.Forum.Vote", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedByUserId");

                    b.Property<int>("PostId");

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.HasIndex("CreatedByUserId");

                    b.HasIndex("PostId");

                    b.ToTable("Votes");
                });

            modelBuilder.Entity("Diffen.Database.Entities.Squad.Formation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Formations");
                });

            modelBuilder.Entity("Diffen.Database.Entities.Squad.Lineup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedByUserId");

                    b.Property<int>("FormationId");

                    b.HasKey("Id");

                    b.HasIndex("CreatedByUserId");

                    b.HasIndex("FormationId");

                    b.ToTable("Lineups");
                });

            modelBuilder.Entity("Diffen.Database.Entities.Squad.Player", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("FirstName");

                    b.Property<bool>("IsCaptain");

                    b.Property<bool>("IsHereOnLoan");

                    b.Property<bool>("IsOutOnLoan");

                    b.Property<bool>("IsSold");

                    b.Property<int>("KitNumber");

                    b.Property<string>("LastName");

                    b.HasKey("Id");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("Diffen.Database.Entities.Squad.PlayerToLineup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("LineupId");

                    b.Property<int>("PlayerId");

                    b.Property<int>("PositionId");

                    b.HasKey("Id");

                    b.HasIndex("LineupId");

                    b.HasIndex("PlayerId");

                    b.HasIndex("PositionId");

                    b.ToTable("PlayersToLineups");
                });

            modelBuilder.Entity("Diffen.Database.Entities.Squad.Position", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Positions");
                });

            modelBuilder.Entity("Diffen.Database.Entities.User.AppUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("AvatarFileName");

                    b.Property<string>("Bio");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<DateTime>("Joined");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<DateTime>("SecludedUntil");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Diffen.Database.Entities.User.FavoritePlayer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("PlayerId");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("PlayerId");

                    b.HasIndex("UserId");

                    b.ToTable("FavoritePlayers");
                });

            modelBuilder.Entity("Diffen.Database.Entities.User.Filter", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ExcludedUserIds");

                    b.Property<int>("PostsPerPage");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserFilters");
                });

            modelBuilder.Entity("Diffen.Database.Entities.User.Invite", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("AccountCreated");

                    b.Property<bool>("AccountIsCreated");

                    b.Property<string>("Email");

                    b.Property<DateTime>("InviteSent");

                    b.Property<string>("InvitedByUserId");

                    b.HasKey("Id");

                    b.HasIndex("InvitedByUserId");

                    b.ToTable("Invites");
                });

            modelBuilder.Entity("Diffen.Database.Entities.User.NickName", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("Nick");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("NickNames");
                });

            modelBuilder.Entity("Diffen.Database.Entities.User.PersonalMessage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("FromUserId");

                    b.Property<string>("Message");

                    b.Property<string>("ToUserId");

                    b.HasKey("Id");

                    b.HasIndex("FromUserId");

                    b.HasIndex("ToUserId");

                    b.ToTable("PersonalMessages");
                });

            modelBuilder.Entity("Diffen.Database.Entities.User.SavedPost", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<int>("PostId");

                    b.Property<string>("SavedByUserId");

                    b.HasKey("Id");

                    b.HasIndex("PostId");

                    b.HasIndex("SavedByUserId");

                    b.ToTable("SavedPosts");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("Diffen.Database.Entities.Forum.Post", b =>
                {
                    b.HasOne("Diffen.Database.Entities.User.AppUser", "User")
                        .WithMany()
                        .HasForeignKey("CreatedByUserId");
                });

            modelBuilder.Entity("Diffen.Database.Entities.Forum.PostToLineup", b =>
                {
                    b.HasOne("Diffen.Database.Entities.Squad.Lineup", "Lineup")
                        .WithMany()
                        .HasForeignKey("LineupId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Diffen.Database.Entities.Forum.Post", "Post")
                        .WithMany()
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Diffen.Database.Entities.Forum.PostToPost", b =>
                {
                    b.HasOne("Diffen.Database.Entities.Forum.Post", "ParentPost")
                        .WithMany()
                        .HasForeignKey("ParentPostId");

                    b.HasOne("Diffen.Database.Entities.Forum.Post", "Post")
                        .WithMany()
                        .HasForeignKey("PostId");
                });

            modelBuilder.Entity("Diffen.Database.Entities.Forum.UrlTip", b =>
                {
                    b.HasOne("Diffen.Database.Entities.Forum.Post", "Post")
                        .WithMany()
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Diffen.Database.Entities.Forum.Vote", b =>
                {
                    b.HasOne("Diffen.Database.Entities.User.AppUser", "User")
                        .WithMany()
                        .HasForeignKey("CreatedByUserId");

                    b.HasOne("Diffen.Database.Entities.Forum.Post", "Post")
                        .WithMany("Votes")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Diffen.Database.Entities.Squad.Lineup", b =>
                {
                    b.HasOne("Diffen.Database.Entities.User.AppUser", "User")
                        .WithMany()
                        .HasForeignKey("CreatedByUserId");

                    b.HasOne("Diffen.Database.Entities.Squad.Formation", "Formation")
                        .WithMany()
                        .HasForeignKey("FormationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Diffen.Database.Entities.Squad.PlayerToLineup", b =>
                {
                    b.HasOne("Diffen.Database.Entities.Squad.Lineup")
                        .WithMany("Players")
                        .HasForeignKey("LineupId");

                    b.HasOne("Diffen.Database.Entities.Squad.Player", "Player")
                        .WithMany()
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Diffen.Database.Entities.Squad.Position", "Position")
                        .WithMany()
                        .HasForeignKey("PositionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Diffen.Database.Entities.User.FavoritePlayer", b =>
                {
                    b.HasOne("Diffen.Database.Entities.Squad.Player", "Player")
                        .WithMany()
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Diffen.Database.Entities.User.AppUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Diffen.Database.Entities.User.Filter", b =>
                {
                    b.HasOne("Diffen.Database.Entities.User.AppUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Diffen.Database.Entities.User.Invite", b =>
                {
                    b.HasOne("Diffen.Database.Entities.User.AppUser", "User")
                        .WithMany()
                        .HasForeignKey("InvitedByUserId");
                });

            modelBuilder.Entity("Diffen.Database.Entities.User.NickName", b =>
                {
                    b.HasOne("Diffen.Database.Entities.User.AppUser", "User")
                        .WithMany("NickNames")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Diffen.Database.Entities.User.PersonalMessage", b =>
                {
                    b.HasOne("Diffen.Database.Entities.User.AppUser", "FromUser")
                        .WithMany()
                        .HasForeignKey("FromUserId");

                    b.HasOne("Diffen.Database.Entities.User.AppUser", "ToUser")
                        .WithMany()
                        .HasForeignKey("ToUserId");
                });

            modelBuilder.Entity("Diffen.Database.Entities.User.SavedPost", b =>
                {
                    b.HasOne("Diffen.Database.Entities.Forum.Post", "Post")
                        .WithMany()
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Diffen.Database.Entities.User.AppUser", "User")
                        .WithMany()
                        .HasForeignKey("SavedByUserId");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Diffen.Database.Entities.User.AppUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Diffen.Database.Entities.User.AppUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Diffen.Database.Entities.User.AppUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Diffen.Database.Entities.User.AppUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
