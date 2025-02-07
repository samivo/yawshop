using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendApp.Migrations
{
    /// <inheritdoc />
    public partial class mig21 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "Events");

            migrationBuilder.AddColumn<string>(
                name: "ClientCodes",
                table: "Events",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientCodes",
                table: "Events");

            migrationBuilder.AddColumn<int>(
                name: "ClientId",
                table: "Events",
                type: "int",
                nullable: true);
        }
    }
}
