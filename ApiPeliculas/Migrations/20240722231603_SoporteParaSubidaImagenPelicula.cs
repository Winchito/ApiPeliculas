using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiPeliculas.Migrations
{
    /// <inheritdoc />
    public partial class SoporteParaSubidaImagenPelicula : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ImageRoute",
                table: "Peliculas",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "RutaLocalImagen",
                table: "Peliculas",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RutaLocalImagen",
                table: "Peliculas");

            migrationBuilder.AlterColumn<string>(
                name: "ImageRoute",
                table: "Peliculas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
