using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "barberTable",
                columns: table => new
                {
                    BarberId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Specialty = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ContactInfo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_barberTable", x => x.BarberId);
                });

            migrationBuilder.CreateTable(
                name: "customerTable",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ContactInfo = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customerTable", x => x.CustomerId);
                });

            migrationBuilder.CreateTable(
                name: "serviceTable",
                columns: table => new
                {
                    ServiceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ServicePrice = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_serviceTable", x => x.ServiceId);
                });

            migrationBuilder.CreateTable(
                name: "appointmentTable",
                columns: table => new
                {
                    AppointmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AppointmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    ServiceId = table.Column<int>(type: "int", nullable: false),
                    BarberId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appointmentTable", x => x.AppointmentId);
                    table.ForeignKey(
                        name: "FK_appointmentTable_barberTable_BarberId",
                        column: x => x.BarberId,
                        principalTable: "barberTable",
                        principalColumn: "BarberId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_appointmentTable_customerTable_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "customerTable",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_appointmentTable_serviceTable_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "serviceTable",
                        principalColumn: "ServiceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "barberServiceTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BarberId = table.Column<int>(type: "int", nullable: false),
                    ServiceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_barberServiceTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_barberServiceTable_barberTable_BarberId",
                        column: x => x.BarberId,
                        principalTable: "barberTable",
                        principalColumn: "BarberId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_barberServiceTable_serviceTable_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "serviceTable",
                        principalColumn: "ServiceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_appointmentTable_BarberId",
                table: "appointmentTable",
                column: "BarberId");

            migrationBuilder.CreateIndex(
                name: "IX_appointmentTable_CustomerId",
                table: "appointmentTable",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_appointmentTable_ServiceId",
                table: "appointmentTable",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_barberServiceTable_BarberId_ServiceId",
                table: "barberServiceTable",
                columns: new[] { "BarberId", "ServiceId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_barberServiceTable_ServiceId",
                table: "barberServiceTable",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_barberTable_Username",
                table: "barberTable",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_customerTable_Username",
                table: "customerTable",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "appointmentTable");

            migrationBuilder.DropTable(
                name: "barberServiceTable");

            migrationBuilder.DropTable(
                name: "customerTable");

            migrationBuilder.DropTable(
                name: "barberTable");

            migrationBuilder.DropTable(
                name: "serviceTable");
        }
    }
}
