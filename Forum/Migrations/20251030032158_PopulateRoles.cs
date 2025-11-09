using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NETForum.Migrations
{
    /// <inheritdoc />
    public partial class PopulateRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                values: new object[,]
                {
                    { "1", "Owner", "OWNER", "a1b2c3d4-e5f6-4a5b-8c9d-0e1f2a3b4c5d" },
                    { "2", "Admin", "ADMIN", "b2c3d4e5-f6a7-5b6c-9d0e-1f2a3b4c5d6e" },
                    { "3", "Moderator", "MODERATOR", "c3d4e5f6-a7b8-6c7d-0e1f-2a3b4c5d6e7f" },
                    { "4", "Member", "MEMBER", "d4e5f6a7-b8c9-7d8e-1f2a-3b4c5d6e7f8a" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValues: new object[] 
                { 
                    "1",
                    "2",
                    "3",
                    "4"
                });
        }
    }
}
