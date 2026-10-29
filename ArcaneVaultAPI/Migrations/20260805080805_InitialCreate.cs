using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ArcaneVaultAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArcaneVaultUserRoles",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArcaneVaultUserRoles", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryCode = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    CategoryName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryCode);
                });

            migrationBuilder.CreateTable(
                name: "ArcaneVaultUsers",
                columns: table => new
                {
                    UserName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    RoleId = table.Column<int>(type: "INTEGER", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordSalt = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArcaneVaultUsers", x => x.UserName);
                    table.ForeignKey(
                        name: "FK_ArcaneVaultUsers_ArcaneVaultUserRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "ArcaneVaultUserRoles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CollectionItems",
                columns: table => new
                {
                    ItemId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ItemName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    StartingQuantity = table.Column<int>(type: "INTEGER", nullable: false),
                    CurrentQuantity = table.Column<int>(type: "INTEGER", nullable: false),
                    ImageData = table.Column<byte[]>(type: "BLOB", nullable: true),
                    ImageContentType = table.Column<string>(type: "TEXT", nullable: true),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionItems", x => x.ItemId);
                    table.ForeignKey(
                        name: "FK_CollectionItems_ArcaneVaultUsers_UserName",
                        column: x => x.UserName,
                        principalTable: "ArcaneVaultUsers",
                        principalColumn: "UserName",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoginAttempts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    AttemptedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Successful = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoginAttempts_ArcaneVaultUsers_UserName",
                        column: x => x.UserName,
                        principalTable: "ArcaneVaultUsers",
                        principalColumn: "UserName",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PasswordResetTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    TokenHash = table.Column<string>(type: "TEXT", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsUsed = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordResetTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PasswordResetTokens_ArcaneVaultUsers_UserName",
                        column: x => x.UserName,
                        principalTable: "ArcaneVaultUsers",
                        principalColumn: "UserName",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CollectionItemCategories",
                columns: table => new
                {
                    ItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    CategoryCode = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionItemCategories", x => new { x.ItemId, x.CategoryCode });
                    table.ForeignKey(
                        name: "FK_CollectionItemCategories_Categories_CategoryCode",
                        column: x => x.CategoryCode,
                        principalTable: "Categories",
                        principalColumn: "CategoryCode",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CollectionItemCategories_CollectionItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "CollectionItems",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ArcaneVaultUserRoles",
                columns: new[] { "RoleId", "RoleName" },
                values: new object[,]
                {
                    { 1, "Staff" },
                    { 2, "User" }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryCode", "CategoryName" },
                values: new object[,]
                {
                    { "ART", "Art & Prints" },
                    { "BOOKS", "Rare Books" },
                    { "CARDS", "Trading Cards" },
                    { "COINS", "Coins & Currency" },
                    { "FIGS", "Figurines & Models" },
                    { "STAMPS", "Stamps & Philately" }
                });

            migrationBuilder.InsertData(
                table: "ArcaneVaultUsers",
                columns: new[] { "UserName", "Email", "IsDeleted", "PasswordHash", "PasswordSalt", "RoleId" },
                values: new object[,]
                {
                    { "staff1", "staff1@arcanevault.test", false, "r1ca5lr9v2LhMV8rBVFg8xjxcy1xoVcG6v3QG/H54/syONQVF/20ZpXHzQseDmAGB/pOdDgoLFPclPxEiP0WXQ==", "qGZnuDvNK762Gli6AeeuTwUB3oTsafYgQnDd6oVjwQQ=", 1 },
                    { "user1", "user1@arcanevault.test", false, "pyQcnPqQV3WgTnCEgKJKdYWl3WkSgy0dRIai7nuA2LbVKnT/ybWMBaejfg9eGZQYvhZzwFlmKYAJxlvOGvGADQ==", "CX3b69o0zJ2SwxFAsaxwnV6WfPH+VJ5L8cYKPam5uTw=", 2 },
                    { "user2", "user2@arcanevault.test", false, "NTj0pc1gvNV4qwVJiMUNDp6JNAm7QCQtdF6GLfq13K9yCh3Z8Jde76NdacbWE5lIi5uK+fxYvbjtSVPtmOlqgw==", "E01ZFMHQy2C4S28HaArGCusedLOkm3XFZKP052gy/QQ=", 2 },
                    { "user3", "user3@arcanevault.test", false, "iUQ1IiqXAWh34+V3A+sV+cDmD2z1C+mj2sRxkvSCGmNUNx+X54DAvsZg0c423/TpWvOSanC1Uo7LB/rmg3GmWg==", "9bNSqpOIUX6LsdJkDDWm11XMm1h+qyrk+zYGXhrKjh0=", 2 },
                    { "user4", "user4@arcanevault.test", false, "IKJj+64Lkl51FH6NSsFgv8aTlE6fEUYLHqOL46CzTUt2B/9ZmmX9iVXE3Vv26D5nLYv1Cis5iF+VOgynQ0279Q==", "Lv9feLJchBI70LgEQ+5UJC9W7CD6m/kN5Q4CpsBdJ78=", 2 }
                });

            migrationBuilder.InsertData(
                table: "CollectionItems",
                columns: new[] { "ItemId", "CurrentQuantity", "ImageContentType", "ImageData", "IsDeleted", "ItemName", "StartingQuantity", "UserName" },
                values: new object[,]
                {
                    { 1, 3, null, null, false, "1909-S VDB Lincoln Cent", 5, "staff1" },
                    { 2, 1, null, null, false, "Ember Drake Holo Card", 4, "staff1" },
                    { 3, 6, null, null, false, "Penny Black Stamp", 6, "user1" },
                    { 4, 2, null, null, false, "Battle Sergeant Miniature", 8, "user1" },
                    { 5, 3, null, null, false, "Rare First Edition Notebook", 3, "user2" },
                    { 6, 1, null, null, false, "Abstract Canvas Original", 5, "user2" },
                    { 7, 2, null, null, false, "Inverted Jenny Stamp", 2, "user3" },
                    { 8, 0, null, null, false, "Anime Scale Figure", 4, "user3" },
                    { 9, 8, null, null, false, "1943 Steel Penny", 10, "user4" },
                    { 10, 5, null, null, false, "Signed Poetry Anthology", 5, "user4" }
                });

            migrationBuilder.InsertData(
                table: "CollectionItemCategories",
                columns: new[] { "CategoryCode", "ItemId" },
                values: new object[,]
                {
                    { "COINS", 1 },
                    { "CARDS", 2 },
                    { "STAMPS", 3 },
                    { "FIGS", 4 },
                    { "BOOKS", 5 },
                    { "ART", 6 },
                    { "BOOKS", 6 },
                    { "STAMPS", 7 },
                    { "ART", 8 },
                    { "FIGS", 8 },
                    { "COINS", 9 },
                    { "BOOKS", 10 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArcaneVaultUsers_RoleId",
                table: "ArcaneVaultUsers",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionItemCategories_CategoryCode",
                table: "CollectionItemCategories",
                column: "CategoryCode");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionItems_UserName",
                table: "CollectionItems",
                column: "UserName");

            migrationBuilder.CreateIndex(
                name: "IX_LoginAttempts_UserName",
                table: "LoginAttempts",
                column: "UserName");

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResetTokens_UserName",
                table: "PasswordResetTokens",
                column: "UserName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CollectionItemCategories");

            migrationBuilder.DropTable(
                name: "LoginAttempts");

            migrationBuilder.DropTable(
                name: "PasswordResetTokens");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "CollectionItems");

            migrationBuilder.DropTable(
                name: "ArcaneVaultUsers");

            migrationBuilder.DropTable(
                name: "ArcaneVaultUserRoles");
        }
    }
}
