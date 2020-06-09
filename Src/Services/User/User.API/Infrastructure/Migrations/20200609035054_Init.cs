using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Photography.Services.User.API.Infrastructure.Migrations
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
                    BackgroundImage = table.Column<string>(nullable: true),
                    Gender = table.Column<int>(nullable: true),
                    Birthday = table.Column<double>(nullable: true),
                    UserType = table.Column<int>(nullable: true),
                    Province = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    Sign = table.Column<string>(nullable: true),
                    FollowingCount = table.Column<int>(nullable: false, defaultValue: 0),
                    FollowerCount = table.Column<int>(nullable: false, defaultValue: 0),
                    PostCount = table.Column<int>(nullable: false, defaultValue: 0),
                    AppointmentCount = table.Column<int>(nullable: false, defaultValue: 0),
                    LikedCount = table.Column<int>(nullable: false, defaultValue: 0),
                    LikedPostCount = table.Column<int>(nullable: false, defaultValue: 0),
                    OngoingOrderCount = table.Column<int>(nullable: false, defaultValue: 0),
                    Score = table.Column<int>(nullable: false, defaultValue: 0),
                    Code = table.Column<string>(nullable: false),
                    RealNameRegistrationStatus = table.Column<int>(nullable: false, defaultValue: 0),
                    ChatServerUserId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Notice = table.Column<string>(nullable: true),
                    Avatar = table.Column<string>(nullable: true),
                    Muted = table.Column<bool>(nullable: false),
                    ModifyMemberEnabled = table.Column<bool>(nullable: false),
                    CreatedTime = table.Column<double>(nullable: false),
                    ChatServerGroupId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwnerId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Groups_Users_OwnerId",
                        column: x => x.OwnerId,
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
                    FollowedUserId = table.Column<Guid>(nullable: false),
                    MutedFollowedUser = table.Column<bool>(nullable: false, defaultValue: false)
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
                name: "GroupUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    GroupId = table.Column<Guid>(nullable: true),
                    UserId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupUsers_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GroupUsers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Groups_OwnerId",
                table: "Groups",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupUsers_GroupId",
                table: "GroupUsers",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupUsers_UserId",
                table: "GroupUsers",
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
                name: "GroupUsers");

            migrationBuilder.DropTable(
                name: "UserRelations");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
