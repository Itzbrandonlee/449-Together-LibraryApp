using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryManagement.Migrations
{
    /// <inheritdoc />
    public partial class RemoveBorrowDateConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_BorrowRecords_BorrowDate",
                table: "BorrowRecords");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_BorrowRecords_BorrowDate",
                table: "BorrowRecords",
                sql: "BorrowDate <= CURRENT_TIMESTAMP");
        }
    }
}
