using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HostalIslaAzul.Migrations
{
    /// <inheritdoc />
    public partial class ChangeHabitacionNumeroToHabitacionId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_HabitacionesAmasDeLlaves",
                table: "HabitacionesAmasDeLlaves");

            migrationBuilder.DropColumn(
                name: "HabitacionNumero",
                table: "HabitacionesAmasDeLlaves");

            migrationBuilder.AlterColumn<string>(
                name: "Turno",
                table: "HabitacionesAmasDeLlaves",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "AmaDeLlavesId1",
                table: "HabitacionesAmasDeLlaves",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HabitacionId1",
                table: "HabitacionesAmasDeLlaves",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_HabitacionesAmasDeLlaves",
                table: "HabitacionesAmasDeLlaves",
                columns: new[] { "AmaDeLlavesId", "HabitacionId", "Turno" });

            migrationBuilder.CreateIndex(
                name: "IX_HabitacionesAmasDeLlaves_AmaDeLlavesId1",
                table: "HabitacionesAmasDeLlaves",
                column: "AmaDeLlavesId1");

            migrationBuilder.CreateIndex(
                name: "IX_HabitacionesAmasDeLlaves_HabitacionId1",
                table: "HabitacionesAmasDeLlaves",
                column: "HabitacionId1");

            migrationBuilder.AddForeignKey(
                name: "FK_HabitacionesAmasDeLlaves_AmasDeLlaves_AmaDeLlavesId1",
                table: "HabitacionesAmasDeLlaves",
                column: "AmaDeLlavesId1",
                principalTable: "AmasDeLlaves",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HabitacionesAmasDeLlaves_Habitaciones_HabitacionId1",
                table: "HabitacionesAmasDeLlaves",
                column: "HabitacionId1",
                principalTable: "Habitaciones",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HabitacionesAmasDeLlaves_AmasDeLlaves_AmaDeLlavesId1",
                table: "HabitacionesAmasDeLlaves");

            migrationBuilder.DropForeignKey(
                name: "FK_HabitacionesAmasDeLlaves_Habitaciones_HabitacionId1",
                table: "HabitacionesAmasDeLlaves");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HabitacionesAmasDeLlaves",
                table: "HabitacionesAmasDeLlaves");

            migrationBuilder.DropIndex(
                name: "IX_HabitacionesAmasDeLlaves_AmaDeLlavesId1",
                table: "HabitacionesAmasDeLlaves");

            migrationBuilder.DropIndex(
                name: "IX_HabitacionesAmasDeLlaves_HabitacionId1",
                table: "HabitacionesAmasDeLlaves");

            migrationBuilder.DropColumn(
                name: "AmaDeLlavesId1",
                table: "HabitacionesAmasDeLlaves");

            migrationBuilder.DropColumn(
                name: "HabitacionId1",
                table: "HabitacionesAmasDeLlaves");

            migrationBuilder.AlterColumn<string>(
                name: "Turno",
                table: "HabitacionesAmasDeLlaves",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "HabitacionNumero",
                table: "HabitacionesAmasDeLlaves",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HabitacionesAmasDeLlaves",
                table: "HabitacionesAmasDeLlaves",
                columns: new[] { "AmaDeLlavesId", "HabitacionNumero" });
        }
    }
}
