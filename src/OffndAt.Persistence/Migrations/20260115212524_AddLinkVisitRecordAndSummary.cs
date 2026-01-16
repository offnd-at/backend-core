using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace OffndAt.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLinkVisitRecordAndSummary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Visits",
                table: "Link");

            migrationBuilder.CreateTable(
                name: "LinkVisitLogEntry",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LinkId = table.Column<Guid>(type: "uuid", nullable: false),
                    VisitedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IpAddress = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    Referrer = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LinkVisitLogEntry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LinkVisitLogEntry_Link_LinkId",
                        column: x => x.LinkId,
                        principalTable: "Link",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LinkVisitSummary",
                columns: table => new
                {
                    LinkId = table.Column<Guid>(type: "uuid", nullable: false),
                    TotalVisits = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LinkVisitSummary", x => x.LinkId);
                    table.ForeignKey(
                        name: "FK_LinkVisitSummary_Link_LinkId",
                        column: x => x.LinkId,
                        principalTable: "Link",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LinkVisitLogEntry_LinkId",
                table: "LinkVisitLogEntry",
                column: "LinkId");

            migrationBuilder.CreateIndex(
                name: "IX_LinkVisitLogEntry_VisitedAt",
                table: "LinkVisitLogEntry",
                column: "VisitedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LinkVisitLogEntry");

            migrationBuilder.DropTable(
                name: "LinkVisitSummary");

            migrationBuilder.AddColumn<int>(
                name: "Visits",
                table: "Link",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
