using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoutiqueEducation.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddFileUrlToTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SubmissionFileUrl",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubmissionNotes",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubmissionFileUrl",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "SubmissionNotes",
                table: "Tasks");
        }
    }
}
