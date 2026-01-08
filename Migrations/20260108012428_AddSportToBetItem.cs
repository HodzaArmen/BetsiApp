using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BetsiApp.Migrations
{
    /// <inheritdoc />
    public partial class AddSportToBetItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Sport",
                table: "BetItems",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sport",
                table: "BetItems");
        }
    }
}
