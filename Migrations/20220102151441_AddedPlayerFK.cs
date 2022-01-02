using Microsoft.EntityFrameworkCore.Migrations;

namespace Fork.Migrations
{
    public partial class AddedPlayerFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServerPlayer_PlayerSet_PlayerUid",
                table: "ServerPlayer");

            migrationBuilder.RenameColumn(
                name: "PlayerUid",
                table: "ServerPlayer",
                newName: "PlayerId");

            migrationBuilder.RenameIndex(
                name: "IX_ServerPlayer_PlayerUid",
                table: "ServerPlayer",
                newName: "IX_ServerPlayer_PlayerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServerPlayer_PlayerSet_PlayerId",
                table: "ServerPlayer",
                column: "PlayerId",
                principalTable: "PlayerSet",
                principalColumn: "Uid",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServerPlayer_PlayerSet_PlayerId",
                table: "ServerPlayer");

            migrationBuilder.RenameColumn(
                name: "PlayerId",
                table: "ServerPlayer",
                newName: "PlayerUid");

            migrationBuilder.RenameIndex(
                name: "IX_ServerPlayer_PlayerId",
                table: "ServerPlayer",
                newName: "IX_ServerPlayer_PlayerUid");

            migrationBuilder.AddForeignKey(
                name: "FK_ServerPlayer_PlayerSet_PlayerUid",
                table: "ServerPlayer",
                column: "PlayerUid",
                principalTable: "PlayerSet",
                principalColumn: "Uid",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
