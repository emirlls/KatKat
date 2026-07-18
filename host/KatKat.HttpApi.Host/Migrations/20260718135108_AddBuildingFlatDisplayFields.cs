using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KatKat.Migrations
{
    /// <inheritdoc />
    public partial class AddBuildingFlatDisplayFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FlatId",
                table: "KatKatResourceReservations",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "FlatId",
                table: "KatKatP2PRequests",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BuildingId",
                table: "KatKatIssues",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_KatKatResourceReservations_FlatId",
                table: "KatKatResourceReservations",
                column: "FlatId");

            migrationBuilder.CreateIndex(
                name: "IX_KatKatP2PRequests_FlatId",
                table: "KatKatP2PRequests",
                column: "FlatId");

            migrationBuilder.CreateIndex(
                name: "IX_KatKatIssues_BuildingId",
                table: "KatKatIssues",
                column: "BuildingId");

            migrationBuilder.AddForeignKey(
                name: "FK_KatKatIssues_KatKatBuildings_BuildingId",
                table: "KatKatIssues",
                column: "BuildingId",
                principalTable: "KatKatBuildings",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_KatKatP2PRequests_KatKatFlats_FlatId",
                table: "KatKatP2PRequests",
                column: "FlatId",
                principalTable: "KatKatFlats",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_KatKatResourceReservations_KatKatFlats_FlatId",
                table: "KatKatResourceReservations",
                column: "FlatId",
                principalTable: "KatKatFlats",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KatKatIssues_KatKatBuildings_BuildingId",
                table: "KatKatIssues");

            migrationBuilder.DropForeignKey(
                name: "FK_KatKatP2PRequests_KatKatFlats_FlatId",
                table: "KatKatP2PRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_KatKatResourceReservations_KatKatFlats_FlatId",
                table: "KatKatResourceReservations");

            migrationBuilder.DropIndex(
                name: "IX_KatKatResourceReservations_FlatId",
                table: "KatKatResourceReservations");

            migrationBuilder.DropIndex(
                name: "IX_KatKatP2PRequests_FlatId",
                table: "KatKatP2PRequests");

            migrationBuilder.DropIndex(
                name: "IX_KatKatIssues_BuildingId",
                table: "KatKatIssues");

            migrationBuilder.DropColumn(
                name: "FlatId",
                table: "KatKatResourceReservations");

            migrationBuilder.DropColumn(
                name: "FlatId",
                table: "KatKatP2PRequests");

            migrationBuilder.DropColumn(
                name: "BuildingId",
                table: "KatKatIssues");
        }
    }
}
