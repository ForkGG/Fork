using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Fork.Migrations
{
    public partial class AddPlayers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Player",
                columns: table => new
                {
                    Uid = table.Column<string>(type: "TEXT", nullable: false),
                    IsOfflinePlayer = table.Column<bool>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Head = table.Column<string>(type: "TEXT", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Player", x => x.Uid);
                });

            migrationBuilder.CreateTable(
                name: "ServerPlayer",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PlayerUid = table.Column<string>(type: "TEXT", nullable: true),
                    ServerId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerPlayer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServerPlayer_Player_PlayerUid",
                        column: x => x.PlayerUid,
                        principalTable: "Player",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServerPlayer_ServerSet_ServerId",
                        column: x => x.ServerId,
                        principalTable: "ServerSet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServerPlayer_PlayerUid",
                table: "ServerPlayer",
                column: "PlayerUid");

            migrationBuilder.CreateIndex(
                name: "IX_ServerPlayer_ServerId",
                table: "ServerPlayer",
                column: "ServerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServerPlayer");

            migrationBuilder.DropTable(
                name: "Player");
        }
    }
}
