using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadManagementSystemLMS.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLoadAndDocumentModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Vehicles_PlateNumber",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_VIN",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "DeliveryLocation",
                table: "Loads");

            migrationBuilder.DropColumn(
                name: "PickupLocation",
                table: "Loads");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Loads");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "Loads");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "TrackingNumber",
                table: "Loads",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Remarks",
                table: "Loads",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PickupDate",
                table: "Loads",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "Loads",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                table: "Loads",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Loads",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Loads",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UploadedAt",
                table: "Documents",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "FileType",
                table: "Documents",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FilePath",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "FileContent",
                table: "Documents",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FileSize",
                table: "Documents",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UploadedBy",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Stops",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoadId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Company = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InitialDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FinalDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ZipCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PickupNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoadedMiles = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EmptyMiles = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stops", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stops_Loads_LoadId",
                        column: x => x.LoadId,
                        principalTable: "Loads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_PlateNumber",
                table: "Vehicles",
                column: "PlateNumber",
                unique: true,
                filter: "[PlateNumber] IS NOT NULL AND [PlateNumber] <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_VIN",
                table: "Vehicles",
                column: "VIN",
                unique: true,
                filter: "[VIN] IS NOT NULL AND [VIN] <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_Stops_LoadId",
                table: "Stops",
                column: "LoadId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Stops");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_PlateNumber",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_VIN",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Loads");

            migrationBuilder.DropColumn(
                name: "IsPaid",
                table: "Loads");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Loads");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Loads");

            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "FileContent",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "FileSize",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "UploadedBy",
                table: "Documents");

            migrationBuilder.AlterColumn<string>(
                name: "TrackingNumber",
                table: "Loads",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Remarks",
                table: "Loads",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "PickupDate",
                table: "Loads",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryLocation",
                table: "Loads",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PickupLocation",
                table: "Loads",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Loads",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Weight",
                table: "Loads",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UploadedAt",
                table: "Documents",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FileType",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "FilePath",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_PlateNumber",
                table: "Vehicles",
                column: "PlateNumber",
                unique: true,
                filter: "[PlateNumber] IS NOT NULL AND [PlateNumber] != ''");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_VIN",
                table: "Vehicles",
                column: "VIN",
                unique: true,
                filter: "[VIN] IS NOT NULL AND [VIN] != ''");
        }
    }
}
