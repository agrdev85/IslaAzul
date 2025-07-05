using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HostalIslaAzul.Migrations
{
    /// <inheritdoc />
    public partial class AddTurnoToHabitacionAmaDeLlaves : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Turno",
                table: "HabitacionesAmasDeLlaves",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Turno",
                table: "HabitacionesAmasDeLlaves");
        }
    }
}
