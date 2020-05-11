using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Photography.Services.Post.API.Infrastructure.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Text = table.Column<string>(nullable: true),
                    Likes = table.Column<int>(nullable: false),
                    Shares = table.Column<int>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    Commentable = table.Column<bool>(nullable: false),
                    Forwardable = table.Column<bool>(nullable: false),
                    ShareType = table.Column<int>(nullable: false),
                    Visibility = table.Column<int>(nullable: false),
                    ViewPassword = table.Column<string>(nullable: true),
                    Location_Latitude = table.Column<double>(nullable: true),
                    Location_Longitude = table.Column<double>(nullable: true),
                    Location_Name = table.Column<string>(nullable: true),
                    Location_Address = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Text = table.Column<string>(nullable: true),
                    Likes = table.Column<int>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    PostId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PostFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Url = table.Column<string>(nullable: true),
                    Text = table.Column<string>(nullable: true),
                    PostFileType = table.Column<int>(nullable: false),
                    PostId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostFiles_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_PostId",
                table: "Comments",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_PostFiles_PostId",
                table: "PostFiles",
                column: "PostId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "PostFiles");

            migrationBuilder.DropTable(
                name: "Posts");
        }
    }
}
