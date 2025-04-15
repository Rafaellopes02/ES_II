using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrabalhoESII.Migrations
{
    public partial class AddEorganizadorColumn_Manual : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "eorganizador",
                table: "organizadoreseventos",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "eorganizador",
                table: "organizadoreseventos");
        }
    }
}