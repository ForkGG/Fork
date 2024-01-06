using Microsoft.EntityFrameworkCore.Migrations;

namespace Fork.Migrations
{
    public partial class PlayerLogic : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServerPlayer_Player_PlayerUid",
                table: "ServerPlayer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Player",
                table: "Player");

            migrationBuilder.RenameTable(
                name: "Player",
                newName: "PlayerSet");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerSet",
                table: "PlayerSet",
                column: "Uid");

            migrationBuilder.AddForeignKey(
                name: "FK_ServerPlayer_PlayerSet_PlayerUid",
                table: "ServerPlayer",
                column: "PlayerUid",
                principalTable: "PlayerSet",
                principalColumn: "Uid",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServerPlayer_PlayerSet_PlayerUid",
                table: "ServerPlayer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerSet",
                table: "PlayerSet");

            migrationBuilder.RenameTable(
                name: "PlayerSet",
                newName: "Player");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Player",
                table: "Player",
                column: "Uid");

            migrationBuilder.AddForeignKey(
                name: "FK_ServerPlayer_Player_PlayerUid",
                table: "ServerPlayer",
                column: "PlayerUid",
                principalTable: "Player",
                principalColumn: "Uid",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
