using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeautyStudioSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class UniqueIndexOfReservationChanged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reservations_EmployeeId",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_ServiceId_Date_StartTime",
                table: "Reservations");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_EmployeeId_Date_StartTime",
                table: "Reservations",
                columns: new[] { "EmployeeId", "Date", "StartTime" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_ServiceId",
                table: "Reservations",
                column: "ServiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reservations_EmployeeId_Date_StartTime",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_ServiceId",
                table: "Reservations");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_EmployeeId",
                table: "Reservations",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_ServiceId_Date_StartTime",
                table: "Reservations",
                columns: new[] { "ServiceId", "Date", "StartTime" },
                unique: true);
        }
    }
}
