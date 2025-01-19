using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lighter_SocialMediaService.Migrations
{
    /// <inheritdoc />
    public partial class AddReplyToField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReplyTo",
                table: "Comments",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReplyTo",
                table: "Comments");
        }
    }
}
