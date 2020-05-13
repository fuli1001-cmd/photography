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
                    UserName = table.Column<string>(nullable: true),
                    Nickname = table.Column<string>(nullable: true),
                    Phonenumber = table.Column<string>(nullable: true),
                    Avatar = table.Column<string>(nullable: true),
                    Gender = table.Column<bool>(nullable: true),
                    Birthday = table.Column<DateTime>(nullable: true),
                    UserType = table.Column<int>(nullable: false, defaultValue: 0),
                    Province = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    Sign = table.Column<string>(nullable: true),
                    LikedCount = table.Column<int>(nullable: true),
                    FollowingCount = table.Column<int>(nullable: true),
                    FollowerCount = table.Column<int>(nullable: true),
                    Points = table.Column<int>(nullable: false, defaultValue: 0),
                    Code = table.Column<string>(nullable: false),
                    RealNameRegistered = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Text = table.Column<string>(nullable: true),
                    LikeCount = table.Column<int>(nullable: false, defaultValue: 0),
                    ShareCount = table.Column<int>(nullable: false, defaultValue: 0),
                    CommentCount = table.Column<int>(nullable: false, defaultValue: 0),
                    Points = table.Column<int>(nullable: false, defaultValue: 0),
                    Timestamp = table.Column<DateTime>(nullable: false, defaultValue: new DateTime(2020, 5, 13, 7, 48, 12, 556, DateTimeKind.Utc).AddTicks(6953)),
                    Commentable = table.Column<bool>(nullable: true, defaultValue: true),
                    ForwardType = table.Column<int>(nullable: false, defaultValue: 0),
                    ShareType = table.Column<int>(nullable: false, defaultValue: 0),
                    Visibility = table.Column<int>(nullable: false, defaultValue: 0),
                    ViewPassword = table.Column<string>(nullable: true),
                    Location_Latitude = table.Column<double>(nullable: true),
                    Location_Longitude = table.Column<double>(nullable: true),
                    Location_Name = table.Column<string>(nullable: true),
                    Location_Address = table.Column<string>(nullable: true),
                    ForwardedPostId = table.Column<Guid>(nullable: true),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
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
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Text = table.Column<string>(nullable: false),
                    Likes = table.Column<int>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false, defaultValue: new DateTime(2020, 5, 13, 7, 48, 12, 593, DateTimeKind.Utc).AddTicks(3141)),
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
                    Url = table.Column<string>(nullable: false),
                    Text = table.Column<string>(nullable: true),
                    PostAttachmentType = table.Column<int>(nullable: false),
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
                name: "IX_Posts_ForwardedPostId",
                table: "Posts",
                column: "ForwardedPostId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_UserId",
                table: "Posts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRelations_FollowedUserId",
                table: "UserRelations",
                column: "FollowedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRelations_FollowerId",
                table: "UserRelations",
                column: "FollowerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "PostAttachments");

            migrationBuilder.DropTable(
                name: "UserRelations");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
