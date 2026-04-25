using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddBookCheckConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Members_Email",
                table: "Members",
                column: "Email",
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Members_FullName",
                table: "Members",
                sql: "length(FullName) > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_BorrowRecords_BorrowDate",
                table: "BorrowRecords",
                sql: "BorrowDate <= CURRENT_TIMESTAMP");

            migrationBuilder.AddCheckConstraint(
                name: "CK_BorrowRecords_ReturnDate",
                table: "BorrowRecords",
                sql: "ReturnDate IS NULL OR ReturnDate >= BorrowDate");

            migrationBuilder.AddCheckConstraint(
                name: "CK_BorrowRecords_Status",
                table: "BorrowRecords",
                sql: "Status IN ('Borrowed', 'Returned')");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Books_AvailableCopies",
                table: "Books",
                sql: "AvailableCopies >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Books_AvailableNotExceedTotal",
                table: "Books",
                sql: "AvailableCopies <= TotalCopies");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Books_TotalCopies",
                table: "Books",
                sql: "TotalCopies > 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Members_Email",
                table: "Members");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Members_FullName",
                table: "Members");

            migrationBuilder.DropCheckConstraint(
                name: "CK_BorrowRecords_BorrowDate",
                table: "BorrowRecords");

            migrationBuilder.DropCheckConstraint(
                name: "CK_BorrowRecords_ReturnDate",
                table: "BorrowRecords");

            migrationBuilder.DropCheckConstraint(
                name: "CK_BorrowRecords_Status",
                table: "BorrowRecords");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Books_AvailableCopies",
                table: "Books");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Books_AvailableNotExceedTotal",
                table: "Books");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Books_TotalCopies",
                table: "Books");
        }
    }
}
