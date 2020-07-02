using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Photography.Services.Post.API.Infrastructure.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Nickname = table.Column<string>(nullable: true),
                    Avatar = table.Column<string>(nullable: true),
                    UserType = table.Column<int>(nullable: true),
                    Score = table.Column<int>(nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Circles",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    VerifyJoin = table.Column<bool>(nullable: false),
                    BackgroundImage = table.Column<string>(nullable: true),
                    UserCount = table.Column<int>(nullable: false),
                    OwnerId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Circles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Circles_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Count = table.Column<int>(nullable: false),
                    UserId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tags_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserRelations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    FollowerId = table.Column<Guid>(nullable: false),
                    FollowedUserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRelations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRelations_Users_FollowedUserId",
                        column: x => x.FollowedUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRelations_Users_FollowerId",
                        column: x => x.FollowerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Text = table.Column<string>(nullable: true),
                    CreatedTime = table.Column<double>(nullable: false),
                    UpdatedTime = table.Column<double>(nullable: false),
                    PostType = table.Column<int>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    Latitude = table.Column<double>(nullable: true),
                    Longitude = table.Column<double>(nullable: true),
                    LocationName = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    CityCode = table.Column<string>(nullable: true),
                    LikeCount = table.Column<int>(nullable: false, defaultValue: 0),
                    ShareCount = table.Column<int>(nullable: false, defaultValue: 0),
                    CommentCount = table.Column<int>(nullable: false, defaultValue: 0),
                    Score = table.Column<int>(nullable: false, defaultValue: 0),
                    Commentable = table.Column<bool>(nullable: true, defaultValue: true),
                    ForwardType = table.Column<int>(nullable: false, defaultValue: 0),
                    ShareType = table.Column<int>(nullable: false, defaultValue: 0),
                    Visibility = table.Column<int>(nullable: false, defaultValue: 0),
                    ViewPassword = table.Column<string>(nullable: true),
                    ShowOriginalText = table.Column<bool>(nullable: true, defaultValue: true),
                    PublicTags = table.Column<string>(nullable: true),
                    PrivateTag = table.Column<string>(nullable: true),
                    CircleGood = table.Column<bool>(nullable: false),
                    CircleId = table.Column<Guid>(nullable: true),
                    ForwardedPostId = table.Column<Guid>(nullable: true),
                    AppointedTime = table.Column<double>(nullable: true),
                    Price = table.Column<decimal>(nullable: true),
                    PayerType = table.Column<int>(nullable: true),
                    AppointmentDealStatus = table.Column<int>(nullable: true),
                    AppointmentedUserId = table.Column<Guid>(nullable: true),
                    AppointmentedToPostId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Posts_Posts_AppointmentedToPostId",
                        column: x => x.AppointmentedToPostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Posts_Users_AppointmentedUserId",
                        column: x => x.AppointmentedUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Posts_Circles_CircleId",
                        column: x => x.CircleId,
                        principalTable: "Circles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Posts_Posts_ForwardedPostId",
                        column: x => x.ForwardedPostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Posts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserCircleRelations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    CircleId = table.Column<Guid>(nullable: false),
                    JoinTime = table.Column<double>(nullable: false),
                    Topping = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCircleRelations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCircleRelations_Circles_CircleId",
                        column: x => x.CircleId,
                        principalTable: "Circles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserCircleRelations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Text = table.Column<string>(nullable: false),
                    Likes = table.Column<int>(nullable: false),
                    CreatedTime = table.Column<double>(nullable: false),
                    PostId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    ParentCommentId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_Comments_ParentCommentId",
                        column: x => x.ParentCommentId,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comments_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PostAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Text = table.Column<string>(nullable: true),
                    AttachmentType = table.Column<int>(nullable: false),
                    AttachmentStatus = table.Column<int>(nullable: true),
                    IsPrivate = table.Column<bool>(nullable: false),
                    PostId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostAttachments_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPostRelations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PostId = table.Column<Guid>(nullable: true),
                    UserId = table.Column<Guid>(nullable: true),
                    CreatedTime = table.Column<double>(nullable: false),
                    UserPostRelationType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPostRelations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPostRelations_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserPostRelations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserShares",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    PostId = table.Column<Guid>(nullable: true),
                    PrivateTag = table.Column<string>(nullable: true),
                    CreatedTime = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserShares", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserShares_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserShares_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserCommentRelations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CommentId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCommentRelations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCommentRelations_Comments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserCommentRelations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Circles_Name",
                table: "Circles",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Circles_OwnerId",
                table: "Circles",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Circles_UserCount",
                table: "Circles",
                column: "UserCount");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ParentCommentId",
                table: "Comments",
                column: "ParentCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_PostId",
                table: "Comments",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_UserId",
                table: "Comments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PostAttachments_PostId",
                table: "PostAttachments",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_AppointmentedToPostId",
                table: "Posts",
                column: "AppointmentedToPostId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_AppointmentedUserId",
                table: "Posts",
                column: "AppointmentedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_CircleId",
                table: "Posts",
                column: "CircleId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_ForwardedPostId",
                table: "Posts",
                column: "ForwardedPostId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_UpdatedTime",
                table: "Posts",
                column: "UpdatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_UserId",
                table: "Posts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Count",
                table: "Tags",
                column: "Count");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Name",
                table: "Tags",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_UserId",
                table: "Tags",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCircleRelations_CircleId",
                table: "UserCircleRelations",
                column: "CircleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCircleRelations_JoinTime",
                table: "UserCircleRelations",
                column: "JoinTime");

            migrationBuilder.CreateIndex(
                name: "IX_UserCircleRelations_UserId",
                table: "UserCircleRelations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCommentRelations_CommentId",
                table: "UserCommentRelations",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCommentRelations_UserId",
                table: "UserCommentRelations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPostRelations_PostId",
                table: "UserPostRelations",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPostRelations_UserId",
                table: "UserPostRelations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRelations_FollowedUserId",
                table: "UserRelations",
                column: "FollowedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRelations_FollowerId",
                table: "UserRelations",
                column: "FollowerId");

            migrationBuilder.CreateIndex(
                name: "IX_UserShares_PostId",
                table: "UserShares",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_UserShares_PrivateTag",
                table: "UserShares",
                column: "PrivateTag");

            migrationBuilder.CreateIndex(
                name: "IX_UserShares_UserId",
                table: "UserShares",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PostAttachments");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "UserCircleRelations");

            migrationBuilder.DropTable(
                name: "UserCommentRelations");

            migrationBuilder.DropTable(
                name: "UserPostRelations");

            migrationBuilder.DropTable(
                name: "UserRelations");

            migrationBuilder.DropTable(
                name: "UserShares");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "Circles");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
