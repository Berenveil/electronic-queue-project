using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Migrations
{
    /// <inheritdoc />
    public partial class FixModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Service_ServiceId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeServices_Service_ServiceId",
                table: "EmployeeServices");

            migrationBuilder.DropForeignKey(
                name: "FK_Service_Professions_ProfessionId",
                table: "Service");

            migrationBuilder.DropIndex(
                name: "IX_Schedules_EmployeeId",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_EmployeeId",
                table: "Appointments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Service",
                table: "Service");

            migrationBuilder.RenameTable(
                name: "Service",
                newName: "Services");

            migrationBuilder.RenameIndex(
                name: "IX_Service_ProfessionId",
                table: "Services",
                newName: "IX_Services_ProfessionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Services",
                table: "Services",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_EmployeeId_WorkDate",
                table: "Schedules",
                columns: new[] { "EmployeeId", "WorkDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clients_Phone",
                table: "Clients",
                column: "Phone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_EmployeeId_ScheduledAt",
                table: "Appointments",
                columns: new[] { "EmployeeId", "ScheduledAt" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_TicketNumber",
                table: "Appointments",
                column: "TicketNumber",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Services_ServiceId",
                table: "Appointments",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeServices_Services_ServiceId",
                table: "EmployeeServices",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Services_Professions_ProfessionId",
                table: "Services",
                column: "ProfessionId",
                principalTable: "Professions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Services_ServiceId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeServices_Services_ServiceId",
                table: "EmployeeServices");

            migrationBuilder.DropForeignKey(
                name: "FK_Services_Professions_ProfessionId",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Schedules_EmployeeId_WorkDate",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_Clients_Phone",
                table: "Clients");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_EmployeeId_ScheduledAt",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_TicketNumber",
                table: "Appointments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Services",
                table: "Services");

            migrationBuilder.RenameTable(
                name: "Services",
                newName: "Service");

            migrationBuilder.RenameIndex(
                name: "IX_Services_ProfessionId",
                table: "Service",
                newName: "IX_Service_ProfessionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Service",
                table: "Service",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_EmployeeId",
                table: "Schedules",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_EmployeeId",
                table: "Appointments",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Service_ServiceId",
                table: "Appointments",
                column: "ServiceId",
                principalTable: "Service",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeServices_Service_ServiceId",
                table: "EmployeeServices",
                column: "ServiceId",
                principalTable: "Service",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Service_Professions_ProfessionId",
                table: "Service",
                column: "ProfessionId",
                principalTable: "Professions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
