using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fork.Migrations
{
    /// <inheritdoc />
    public partial class NullabilityUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AutomationTime_SimpleTime_TimeId",
                table: "AutomationTime");

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

            migrationBuilder.AlterColumn<DateTime>(
                name: "ResourcePackHashAge",
                table: "ServerSet",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AutomationTime_SimpleTime_TimeId",
                table: "AutomationTime");

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

            migrationBuilder.AlterColumn<DateTime>(
                name: "ResourcePackHashAge",
                table: "ServerSet",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
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
    }
}
