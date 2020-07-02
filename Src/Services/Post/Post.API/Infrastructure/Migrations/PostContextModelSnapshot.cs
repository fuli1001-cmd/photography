﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Photography.Services.Post.Infrastructure;

namespace Photography.Services.Post.API.Infrastructure.Migrations
{
    [DbContext(typeof(PostContext))]
    partial class PostContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Photography.Services.Post.Domain.AggregatesModel.CircleAggregate.Circle", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("BackgroundImage")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("UserCount")
                        .HasColumnType("int");

                    b.Property<bool>("VerifyJoin")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("Name");

                    b.HasIndex("OwnerId");

                    b.HasIndex("UserCount");

                    b.ToTable("Circles");
                });

            modelBuilder.Entity("Photography.Services.Post.Domain.AggregatesModel.CommentAggregate.Comment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<double>("CreatedTime")
                        .HasColumnType("float");

                    b.Property<int>("Likes")
                        .HasColumnType("int");

                    b.Property<Guid?>("ParentCommentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("PostId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ParentCommentId");

                    b.HasIndex("PostId");

                    b.HasIndex("UserId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("Photography.Services.Post.Domain.AggregatesModel.PostAggregate.Post", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double?>("AppointedTime")
                        .HasColumnType("float");

                    b.Property<int?>("AppointmentDealStatus")
                        .HasColumnType("int");

                    b.Property<Guid?>("AppointmentedToPostId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("AppointmentedUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("CircleGood")
                        .HasColumnType("bit");

                    b.Property<Guid?>("CircleId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CityCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("CommentCount")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.Property<bool?>("Commentable")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<double>("CreatedTime")
                        .HasColumnType("float");

                    b.Property<int>("ForwardType")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.Property<Guid?>("ForwardedPostId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<double?>("Latitude")
                        .HasColumnType("float");

                    b.Property<int>("LikeCount")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.Property<string>("LocationName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double?>("Longitude")
                        .HasColumnType("float");

                    b.Property<int?>("PayerType")
                        .HasColumnType("int");

                    b.Property<int>("PostType")
                        .HasColumnType("int");

                    b.Property<decimal?>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("PrivateTag")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PublicTags")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Score")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.Property<int>("ShareCount")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.Property<int>("ShareType")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.Property<bool?>("ShowOriginalText")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<string>("Text")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("UpdatedTime")
                        .HasColumnType("float");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ViewPassword")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Visibility")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.HasKey("Id");

                    b.HasIndex("AppointmentedToPostId");

                    b.HasIndex("AppointmentedUserId");

                    b.HasIndex("CircleId");

                    b.HasIndex("ForwardedPostId");

                    b.HasIndex("UpdatedTime");

                    b.HasIndex("UserId");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("Photography.Services.Post.Domain.AggregatesModel.PostAggregate.PostAttachment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int?>("AttachmentStatus")
                        .HasColumnType("int");

                    b.Property<int>("AttachmentType")
                        .HasColumnType("int");

                    b.Property<bool>("IsPrivate")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("PostId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Text")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("PostId");

                    b.ToTable("PostAttachments");
                });

            modelBuilder.Entity("Photography.Services.Post.Domain.AggregatesModel.TagAggregate.Tag", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Count")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("Count");

                    b.HasIndex("Name");

                    b.HasIndex("UserId");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("Photography.Services.Post.Domain.AggregatesModel.UserAggregate.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Avatar")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Nickname")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Score")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.Property<int?>("UserType")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Photography.Services.Post.Domain.AggregatesModel.UserCircleRelationAggregate.UserCircleRelation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CircleId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<double>("JoinTime")
                        .HasColumnType("float");

                    b.Property<bool>("Topping")
                        .HasColumnType("bit");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("CircleId");

                    b.HasIndex("JoinTime");

                    b.HasIndex("UserId");

                    b.ToTable("UserCircleRelations");
                });

            modelBuilder.Entity("Photography.Services.Post.Domain.AggregatesModel.UserCommentRelationAggregate.UserCommentRelation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CommentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("CommentId");

                    b.HasIndex("UserId");

                    b.ToTable("UserCommentRelations");
                });

            modelBuilder.Entity("Photography.Services.Post.Domain.AggregatesModel.UserPostRelationAggregate.UserPostRelation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<double>("CreatedTime")
                        .HasColumnType("float");

                    b.Property<Guid?>("PostId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("UserPostRelationType")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PostId");

                    b.HasIndex("UserId");

                    b.ToTable("UserPostRelations");
                });

            modelBuilder.Entity("Photography.Services.Post.Domain.AggregatesModel.UserRelationAggregate.UserRelation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("FollowedUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("FollowerId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("FollowedUserId");

                    b.HasIndex("FollowerId");

                    b.ToTable("UserRelations");
                });

            modelBuilder.Entity("Photography.Services.Post.Domain.AggregatesModel.UserShareAggregate.UserShare", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<double>("CreatedTime")
                        .HasColumnType("float");

                    b.Property<Guid?>("PostId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("TagId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("UnSpecifiedTag")
                        .HasColumnType("bit");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("PostId");

                    b.HasIndex("TagId");

                    b.HasIndex("UserId", "PostId");

                    b.HasIndex("UserId", "TagId");

                    b.ToTable("UserShares");
                });

            modelBuilder.Entity("Photography.Services.Post.Domain.AggregatesModel.CircleAggregate.Circle", b =>
                {
                    b.HasOne("Photography.Services.Post.Domain.AggregatesModel.UserAggregate.User", "Owner")
                        .WithMany("Circles")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Photography.Services.Post.Domain.AggregatesModel.CommentAggregate.Comment", b =>
                {
                    b.HasOne("Photography.Services.Post.Domain.AggregatesModel.CommentAggregate.Comment", "ParentComment")
                        .WithMany("SubComments")
                        .HasForeignKey("ParentCommentId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Photography.Services.Post.Domain.AggregatesModel.PostAggregate.Post", "Post")
                        .WithMany("Comments")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Photography.Services.Post.Domain.AggregatesModel.UserAggregate.User", "User")
                        .WithMany("Comments")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Photography.Services.Post.Domain.AggregatesModel.PostAggregate.Post", b =>
                {
                    b.HasOne("Photography.Services.Post.Domain.AggregatesModel.PostAggregate.Post", "AppointmentedToPost")
                        .WithMany("AppointmentedFromPosts")
                        .HasForeignKey("AppointmentedToPostId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Photography.Services.Post.Domain.AggregatesModel.UserAggregate.User", "AppointmentedUser")
                        .WithMany("Appointments")
                        .HasForeignKey("AppointmentedUserId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Photography.Services.Post.Domain.AggregatesModel.CircleAggregate.Circle", "Circle")
                        .WithMany("Posts")
                        .HasForeignKey("CircleId");

                    b.HasOne("Photography.Services.Post.Domain.AggregatesModel.PostAggregate.Post", "ForwardedPost")
                        .WithMany("ForwardingPosts")
                        .HasForeignKey("ForwardedPostId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Photography.Services.Post.Domain.AggregatesModel.UserAggregate.User", "User")
                        .WithMany("Posts")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Photography.Services.Post.Domain.AggregatesModel.PostAggregate.PostAttachment", b =>
                {
                    b.HasOne("Photography.Services.Post.Domain.AggregatesModel.PostAggregate.Post", "Post")
                        .WithMany("PostAttachments")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Photography.Services.Post.Domain.AggregatesModel.TagAggregate.Tag", b =>
                {
                    b.HasOne("Photography.Services.Post.Domain.AggregatesModel.UserAggregate.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Photography.Services.Post.Domain.AggregatesModel.UserCircleRelationAggregate.UserCircleRelation", b =>
                {
                    b.HasOne("Photography.Services.Post.Domain.AggregatesModel.CircleAggregate.Circle", "Circle")
                        .WithMany("UserCircleRelations")
                        .HasForeignKey("CircleId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Photography.Services.Post.Domain.AggregatesModel.UserAggregate.User", "User")
                        .WithMany("UserCircleRelations")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Photography.Services.Post.Domain.AggregatesModel.UserCommentRelationAggregate.UserCommentRelation", b =>
                {
                    b.HasOne("Photography.Services.Post.Domain.AggregatesModel.CommentAggregate.Comment", "Comment")
                        .WithMany("UserCommentRelations")
                        .HasForeignKey("CommentId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Photography.Services.Post.Domain.AggregatesModel.UserAggregate.User", "User")
                        .WithMany("UserCommentRelations")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Photography.Services.Post.Domain.AggregatesModel.UserPostRelationAggregate.UserPostRelation", b =>
                {
                    b.HasOne("Photography.Services.Post.Domain.AggregatesModel.PostAggregate.Post", "Post")
                        .WithMany("UserPostRelations")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Photography.Services.Post.Domain.AggregatesModel.UserAggregate.User", "User")
                        .WithMany("UserPostRelations")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Photography.Services.Post.Domain.AggregatesModel.UserRelationAggregate.UserRelation", b =>
                {
                    b.HasOne("Photography.Services.Post.Domain.AggregatesModel.UserAggregate.User", "FollowedUser")
                        .WithMany("FollowedUsers")
                        .HasForeignKey("FollowedUserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Photography.Services.Post.Domain.AggregatesModel.UserAggregate.User", "Follower")
                        .WithMany("Followers")
                        .HasForeignKey("FollowerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Photography.Services.Post.Domain.AggregatesModel.UserShareAggregate.UserShare", b =>
                {
                    b.HasOne("Photography.Services.Post.Domain.AggregatesModel.PostAggregate.Post", "Post")
                        .WithMany("UserShares")
                        .HasForeignKey("PostId");

                    b.HasOne("Photography.Services.Post.Domain.AggregatesModel.TagAggregate.Tag", "Tag")
                        .WithMany("UserShares")
                        .HasForeignKey("TagId");

                    b.HasOne("Photography.Services.Post.Domain.AggregatesModel.UserAggregate.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
