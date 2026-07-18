using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KatKat.Migrations
{
    /// <inheritdoc />
    public partial class AddResourceReservationRejectionReason : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "KatKatResourceReservations",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "KatKatResourceReservations");
        }
    }
}
