using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BetsiApp.Migrations
{
    /// <inheritdoc />
    public partial class AddBettingModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BetSlips",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: true),
                    Stake = table.Column<decimal>(type: "decimal(10, 2)", nullable: false),
                    TotalOdd = table.Column<decimal>(type: "decimal(10, 2)", nullable: false),
                    PlacementTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BetSlips", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Odds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MatchId = table.Column<int>(type: "INTEGER", nullable: false),
                    HomeWin = table.Column<decimal>(type: "decimal(5, 2)", nullable: false),
                    Draw = table.Column<decimal>(type: "decimal(5, 2)", nullable: false),
                    AwayWin = table.Column<decimal>(type: "decimal(5, 2)", nullable: false),
                    MarketType = table.Column<string>(type: "TEXT", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Odds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BetItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MatchId = table.Column<int>(type: "INTEGER", nullable: false),
                    MatchDescription = table.Column<string>(type: "TEXT", nullable: true),
                    SelectedOutcome = table.Column<string>(type: "TEXT", nullable: true),
                    PlacedOdd = table.Column<decimal>(type: "decimal(5, 2)", nullable: false),
                    BetSlipId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BetItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BetItems_BetSlips_BetSlipId",
                        column: x => x.BetSlipId,
                        principalTable: "BetSlips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BetItems_BetSlipId",
                table: "BetItems",
                column: "BetSlipId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BetItems");

            migrationBuilder.DropTable(
                name: "Odds");

            migrationBuilder.DropTable(
                name: "BetSlips");
        }
    }
}
