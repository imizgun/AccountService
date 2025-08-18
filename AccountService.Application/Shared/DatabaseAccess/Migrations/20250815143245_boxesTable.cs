using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountService.DatabaseAccess.Migrations
{
    /// <inheritdoc />
    public partial class boxesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OutboxMessage",
                table: "OutboxMessage");

            migrationBuilder.RenameTable(
                name: "OutboxMessage",
                newName: "OutboxMessages");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OutboxMessages",
                table: "OutboxMessages",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OutboxMessages",
                table: "OutboxMessages");

            migrationBuilder.RenameTable(
                name: "OutboxMessages",
                newName: "OutboxMessage");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OutboxMessage",
                table: "OutboxMessage",
                column: "Id");
        }
    }
}
