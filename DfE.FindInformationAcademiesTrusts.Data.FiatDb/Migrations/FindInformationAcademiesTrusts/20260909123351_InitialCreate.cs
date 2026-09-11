using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DfE.FindInformationAcademiesTrusts.Data.FiatDb.Migrations.FindInformationAcademiesTrusts
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Watchlist",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReadableId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EstablishmentId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    TrustId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsTrust = table.Column<bool>(type: "bit", nullable: false),
                    User = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Watchlist", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Watchlist_User_EstablishmentId",
                table: "Watchlist",
                columns: new[] { "User", "EstablishmentId" });

            migrationBuilder.CreateIndex(
                name: "IX_Watchlist_User_TrustId",
                table: "Watchlist",
                columns: new[] { "User", "TrustId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Watchlist");
        }
    }
}
