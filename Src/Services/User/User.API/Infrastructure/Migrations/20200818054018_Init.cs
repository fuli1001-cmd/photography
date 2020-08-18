using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Photography.Services.User.API.Infrastructure.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserRelations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    FromUserId = table.Column<Guid>(nullable: false),
                    ToUserId = table.Column<Guid>(nullable: false),
                    Muted = table.Column<bool>(nullable: false),
                    Followed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRelations", x => x.Id);
                });

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
                    OngoingOrderCount = table.Column<int>(nullable: false),
                    WaitingForConfirmOrderCount = table.Column<int>(nullable: false),
                    Score = table.Column<int>(nullable: false, defaultValue: 0),
                    Code = table.Column<string>(nullable: false),
                    RealNameRegistrationStatus = table.Column<int>(nullable: false, defaultValue: 0),
                    IdCardFront = table.Column<string>(nullable: true),
                    IdCardBack = table.Column<string>(nullable: true),
                    IdCardHold = table.Column<string>(nullable: true),
                    ViewFollowersAllowed = table.Column<bool>(nullable: false),
                    ViewFollowedUsersAllowed = table.Column<bool>(nullable: false),
                    CreatedTime = table.Column<double>(nullable: false),
                    UpdatedTime = table.Column<double>(nullable: false),
                    DisabledTime = table.Column<DateTime>(nullable: true),
                    DisabledCount = table.Column<int>(nullable: false),
                    OrgType = table.Column<int>(nullable: true),
                    OrgSchoolName = table.Column<string>(nullable: true),
                    OrgName = table.Column<string>(nullable: true),
                    OrgDesc = table.Column<string>(nullable: true),
                    OrgOperatorName = table.Column<string>(nullable: true),
                    OrgOperatorPhoneNumber = table.Column<string>(nullable: true),
                    OrgImage = table.Column<string>(nullable: true),
                    OrgAuthStatus = table.Column<int>(nullable: false),
                    ChatServerUserId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RegistrationId = table.Column<string>(nullable: true),
                    ClientType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Albums",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    CreatedTime = table.Column<double>(nullable: false),
                    UpdatedTime = table.Column<double>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Albums", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Albums_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Feedbacks",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Text = table.Column<string>(nullable: true),
                    Image1 = table.Column<string>(nullable: true),
                    Image2 = table.Column<string>(nullable: true),
                    Image3 = table.Column<string>(nullable: true),
                    CreatedTime = table.Column<double>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedbacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Feedbacks_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Notice = table.Column<string>(nullable: true),
                    Avatar = table.Column<string>(nullable: true),
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
                name: "AlbumPhotos",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    DisplayName = table.Column<string>(nullable: false),
                    CreatedTime = table.Column<double>(nullable: false),
                    UpdatedTime = table.Column<double>(nullable: false),
                    AlbumId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlbumPhotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlbumPhotos_Albums_AlbumId",
                        column: x => x.AlbumId,
                        principalTable: "Albums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    GroupId = table.Column<Guid>(nullable: true),
                    UserId = table.Column<Guid>(nullable: true),
                    Muted = table.Column<bool>(nullable: false)
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
                name: "IX_AlbumPhotos_AlbumId",
                table: "AlbumPhotos",
                column: "AlbumId");

            migrationBuilder.CreateIndex(
                name: "IX_AlbumPhotos_CreatedTime",
                table: "AlbumPhotos",
                column: "CreatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_AlbumPhotos_UpdatedTime",
                table: "AlbumPhotos",
                column: "UpdatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_Albums_CreatedTime",
                table: "Albums",
                column: "CreatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_Albums_UpdatedTime",
                table: "Albums",
                column: "UpdatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_Albums_UserId",
                table: "Albums",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_UserId",
                table: "Feedbacks",
                column: "UserId");

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlbumPhotos");

            migrationBuilder.DropTable(
                name: "Feedbacks");

            migrationBuilder.DropTable(
                name: "GroupUsers");

            migrationBuilder.DropTable(
                name: "UserRelations");

            migrationBuilder.DropTable(
                name: "Albums");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
