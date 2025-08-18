using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountService.DatabaseAccess.Migrations
{
    /// <inheritdoc />
    public partial class newFormula : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE OR REPLACE PROCEDURE accrue_interest(account_id UUID)
                LANGUAGE plpgsql
                AS $$
                BEGIN
                    UPDATE ""Accounts""
                    SET ""Balance"" = ""Balance"" + (""Balance"" * ""InterestRate"" / 100.0) / 365.0
                    WHERE ""Id"" = account_id;
                END;
                $$;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
