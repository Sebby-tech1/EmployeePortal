using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeePortal.API.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentStatusFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "SubmittedToSwift",
                table: "Payments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubmittedToSwift",
                table: "Payments");
        }
    }
}
