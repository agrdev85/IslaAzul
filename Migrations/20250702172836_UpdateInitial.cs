using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HostalIslaAzul.Migrations
{
    /// <inheritdoc />
    public partial class UpdateInitial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClienteId1",
                table: "Reservas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HabitacionId1",
                table: "Reservas",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CI",
                table: "AmasDeLlaves",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_ClienteId1",
                table: "Reservas",
                column: "ClienteId1");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_HabitacionId1",
                table: "Reservas",
                column: "HabitacionId1");

            migrationBuilder.CreateIndex(
                name: "IX_AmasDeLlaves_CI",
                table: "AmasDeLlaves",
                column: "CI",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Clientes_ClienteId1",
                table: "Reservas",
                column: "ClienteId1",
                principalTable: "Clientes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Habitaciones_HabitacionId1",
                table: "Reservas",
                column: "HabitacionId1",
                principalTable: "Habitaciones",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Clientes_ClienteId1",
                table: "Reservas");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Habitaciones_HabitacionId1",
                table: "Reservas");

            migrationBuilder.DropIndex(
                name: "IX_Reservas_ClienteId1",
                table: "Reservas");

            migrationBuilder.DropIndex(
                name: "IX_Reservas_HabitacionId1",
                table: "Reservas");

            migrationBuilder.DropIndex(
                name: "IX_AmasDeLlaves_CI",
                table: "AmasDeLlaves");

            migrationBuilder.DropColumn(
                name: "ClienteId1",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "HabitacionId1",
                table: "Reservas");

            migrationBuilder.AlterColumn<string>(
                name: "CI",
                table: "AmasDeLlaves",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
