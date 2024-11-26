using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fork.Migrations
{
    /// <inheritdoc />
    public partial class EntityFrameworkUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AutomationTime_SimpleTime_TimeId",
                table: "AutomationTime");

            migrationBuilder.DropForeignKey(
                name: "FK_ServerPlayer_PlayerSet_PlayerId",
                table: "ServerPlayer");

            migrationBuilder.DropForeignKey(
                name: "FK_ServerSet_JavaSettings_JavaSettingsId",
                table: "ServerSet");

            migrationBuilder.DropForeignKey(
                name: "FK_ServerSet_ServerVersion_VersionId",
                table: "ServerSet");

            migrationBuilder.AlterColumn<ulong>(
                name: "VersionId",
                table: "ServerSet",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul,
                oldClrType: typeof(ulong),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ServerSet",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<ulong>(
                name: "JavaSettingsId",
                table: "ServerSet",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul,
                oldClrType: typeof(ulong),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PlayerId",
                table: "ServerPlayer",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "PlayerSet",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Head",
                table: "PlayerSet",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "StartupParameters",
                table: "JavaSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "JavaPath",
                table: "JavaSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<ulong>(
                name: "TimeId",
                table: "AutomationTime",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul,
                oldClrType: typeof(ulong),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AutomationTime_SimpleTime_TimeId",
                table: "AutomationTime",
                column: "TimeId",
                principalTable: "SimpleTime",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServerPlayer_PlayerSet_PlayerId",
                table: "ServerPlayer",
                column: "PlayerId",
                principalTable: "PlayerSet",
                principalColumn: "Uid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServerSet_JavaSettings_JavaSettingsId",
                table: "ServerSet",
                column: "JavaSettingsId",
                principalTable: "JavaSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServerSet_ServerVersion_VersionId",
                table: "ServerSet",
                column: "VersionId",
                principalTable: "ServerVersion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AutomationTime_SimpleTime_TimeId",
                table: "AutomationTime");

            migrationBuilder.DropForeignKey(
                name: "FK_ServerPlayer_PlayerSet_PlayerId",
                table: "ServerPlayer");

            migrationBuilder.DropForeignKey(
                name: "FK_ServerSet_JavaSettings_JavaSettingsId",
                table: "ServerSet");

            migrationBuilder.DropForeignKey(
                name: "FK_ServerSet_ServerVersion_VersionId",
                table: "ServerSet");

            migrationBuilder.AlterColumn<ulong>(
                name: "VersionId",
                table: "ServerSet",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(ulong),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ServerSet",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<ulong>(
                name: "JavaSettingsId",
                table: "ServerSet",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(ulong),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "PlayerId",
                table: "ServerPlayer",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "PlayerSet",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Head",
                table: "PlayerSet",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "StartupParameters",
                table: "JavaSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "JavaPath",
                table: "JavaSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<ulong>(
                name: "TimeId",
                table: "AutomationTime",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(ulong),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_AutomationTime_SimpleTime_TimeId",
                table: "AutomationTime",
                column: "TimeId",
                principalTable: "SimpleTime",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ServerPlayer_PlayerSet_PlayerId",
                table: "ServerPlayer",
                column: "PlayerId",
                principalTable: "PlayerSet",
                principalColumn: "Uid");

            migrationBuilder.AddForeignKey(
                name: "FK_ServerSet_JavaSettings_JavaSettingsId",
                table: "ServerSet",
                column: "JavaSettingsId",
                principalTable: "JavaSettings",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ServerSet_ServerVersion_VersionId",
                table: "ServerSet",
                column: "VersionId",
                principalTable: "ServerVersion",
                principalColumn: "Id");
        }
    }
}
