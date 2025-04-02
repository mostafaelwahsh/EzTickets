using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class orderChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ticket_Order_OrderID",
                table: "Ticket");

            migrationBuilder.RenameColumn(
                name: "OrderID",
                table: "Ticket",
                newName: "OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_Ticket_OrderID",
                table: "Ticket",
                newName: "IX_Ticket_OrderId");

            migrationBuilder.RenameColumn(
                name: "OrderID",
                table: "Order",
                newName: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Ticket_Order_OrderId",
                table: "Ticket",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ticket_Order_OrderId",
                table: "Ticket");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "Ticket",
                newName: "OrderID");

            migrationBuilder.RenameIndex(
                name: "IX_Ticket_OrderId",
                table: "Ticket",
                newName: "IX_Ticket_OrderID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Order",
                newName: "OrderID");

            migrationBuilder.AddForeignKey(
                name: "FK_Ticket_Order_OrderID",
                table: "Ticket",
                column: "OrderID",
                principalTable: "Order",
                principalColumn: "OrderID");
        }
    }
}
