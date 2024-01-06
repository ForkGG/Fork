using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Fork.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppSettingsSet",
                columns: table => new
                {
                    Key = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSettingsSet", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "JavaSettings",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MaxRam = table.Column<int>(type: "INTEGER", nullable: false),
                    JavaPath = table.Column<string>(type: "TEXT", nullable: true),
                    StartupParameters = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JavaSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServerVersion",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Version = table.Column<string>(type: "TEXT", nullable: true),
                    Build = table.Column<int>(type: "INTEGER", nullable: false),
                    JarLink = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerVersion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SimpleTime",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Hours = table.Column<int>(type: "INTEGER", nullable: false),
                    Minutes = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SimpleTime", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServerSet",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AutoSetSha1 = table.Column<bool>(type: "INTEGER", nullable: false),
                    ResourcePackHashAge = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    VersionId = table.Column<ulong>(type: "INTEGER", nullable: true),
                    JavaSettingsId = table.Column<ulong>(type: "INTEGER", nullable: true),
                    Initialized = table.Column<bool>(type: "INTEGER", nullable: false),
                    StartWithFork = table.Column<bool>(type: "INTEGER", nullable: false),
                    ServerIconId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerSet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServerSet_JavaSettings_JavaSettingsId",
                        column: x => x.JavaSettingsId,
                        principalTable: "JavaSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServerSet_ServerVersion_VersionId",
                        column: x => x.VersionId,
                        principalTable: "ServerVersion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AutomationTime",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Enabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    TimeId = table.Column<ulong>(type: "INTEGER", nullable: true),
                    ServerId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutomationTime", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AutomationTime_ServerSet_ServerId",
                        column: x => x.ServerId,
                        principalTable: "ServerSet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AutomationTime_SimpleTime_TimeId",
                        column: x => x.TimeId,
                        principalTable: "SimpleTime",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AutomationTime_ServerId",
                table: "AutomationTime",
                column: "ServerId");

            migrationBuilder.CreateIndex(
                name: "IX_AutomationTime_TimeId",
                table: "AutomationTime",
                column: "TimeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServerSet_JavaSettingsId",
                table: "ServerSet",
                column: "JavaSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_ServerSet_VersionId",
                table: "ServerSet",
                column: "VersionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppSettingsSet");

            migrationBuilder.DropTable(
                name: "AutomationTime");

            migrationBuilder.DropTable(
                name: "ServerSet");

            migrationBuilder.DropTable(
                name: "SimpleTime");

            migrationBuilder.DropTable(
                name: "JavaSettings");

            migrationBuilder.DropTable(
                name: "ServerVersion");
        }
    }
}
