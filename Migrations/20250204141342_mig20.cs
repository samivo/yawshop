using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendApp.Migrations
{
    /// <inheritdoc />
    public partial class mig20 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GiftcardCodes",
                table: "CheckoutItem",
                newName: "GiftcardCode");

            migrationBuilder.RenameColumn(
                name: "DiscountCodes",
                table: "CheckoutItem",
                newName: "DiscountCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GiftcardCode",
                table: "CheckoutItem",
                newName: "GiftcardCodes");

            migrationBuilder.RenameColumn(
                name: "DiscountCode",
                table: "CheckoutItem",
                newName: "DiscountCodes");
        }
    }
}
