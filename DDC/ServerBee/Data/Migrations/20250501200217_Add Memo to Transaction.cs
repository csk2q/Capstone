using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServerBee.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMemotoTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "memo",
                table: "transaction",
                type: "character varying(250)",
                maxLength: 250,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "memo",
                table: "transaction");
        }
    }
}
