using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ArcaneVaultAPI.Migrations
{
    /// <inheritdoc />
    public partial class SeedPokemonData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CollectionItemCategories",
                keyColumns: new[] { "CategoryCode", "ItemId" },
                keyValues: new object[] { "COINS", 1 });

            migrationBuilder.DeleteData(
                table: "CollectionItemCategories",
                keyColumns: new[] { "CategoryCode", "ItemId" },
                keyValues: new object[] { "CARDS", 2 });

            migrationBuilder.DeleteData(
                table: "CollectionItemCategories",
                keyColumns: new[] { "CategoryCode", "ItemId" },
                keyValues: new object[] { "STAMPS", 3 });

            migrationBuilder.DeleteData(
                table: "CollectionItemCategories",
                keyColumns: new[] { "CategoryCode", "ItemId" },
                keyValues: new object[] { "FIGS", 4 });

            migrationBuilder.DeleteData(
                table: "CollectionItemCategories",
                keyColumns: new[] { "CategoryCode", "ItemId" },
                keyValues: new object[] { "BOOKS", 5 });

            migrationBuilder.DeleteData(
                table: "CollectionItemCategories",
                keyColumns: new[] { "CategoryCode", "ItemId" },
                keyValues: new object[] { "ART", 6 });

            migrationBuilder.DeleteData(
                table: "CollectionItemCategories",
                keyColumns: new[] { "CategoryCode", "ItemId" },
                keyValues: new object[] { "BOOKS", 6 });

            migrationBuilder.DeleteData(
                table: "CollectionItemCategories",
                keyColumns: new[] { "CategoryCode", "ItemId" },
                keyValues: new object[] { "STAMPS", 7 });

            migrationBuilder.DeleteData(
                table: "CollectionItemCategories",
                keyColumns: new[] { "CategoryCode", "ItemId" },
                keyValues: new object[] { "ART", 8 });

            migrationBuilder.DeleteData(
                table: "CollectionItemCategories",
                keyColumns: new[] { "CategoryCode", "ItemId" },
                keyValues: new object[] { "FIGS", 8 });

            migrationBuilder.DeleteData(
                table: "CollectionItemCategories",
                keyColumns: new[] { "CategoryCode", "ItemId" },
                keyValues: new object[] { "COINS", 9 });

            migrationBuilder.DeleteData(
                table: "CollectionItemCategories",
                keyColumns: new[] { "CategoryCode", "ItemId" },
                keyValues: new object[] { "BOOKS", 10 });

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryCode",
                keyValue: "ART");

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryCode",
                keyValue: "BOOKS");

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryCode",
                keyValue: "CARDS");

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryCode",
                keyValue: "COINS");

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryCode",
                keyValue: "FIGS");

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryCode",
                keyValue: "STAMPS");

            migrationBuilder.UpdateData(
                table: "ArcaneVaultUsers",
                keyColumn: "UserName",
                keyValue: "staff1",
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { "i4kuspS/08Ns/njbrJ/xIVO10cX3aeJ217j12CXwUkTfhyOB4Qcf2YfIJYF5d6m6u2rjD/7IwJnfye1eCDTpyQ==", "Gg1Jl8awJYueVJeTIazeW9UHZ4CPFNhVlfSfvFbVGw8=" });

            migrationBuilder.UpdateData(
                table: "ArcaneVaultUsers",
                keyColumn: "UserName",
                keyValue: "user1",
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { "lu/ShE8mhg0ncEuKFVnuG1cIMJS4eFgYp63TsgQ+nJJWMwqAxjnuEgWr7FJSMTDMM4T2ae7FLn1xal6br+YGMA==", "juhakR416nlF5yN8sLBxZ4w+3rk61jIIz4AYev3ehr4=" });

            migrationBuilder.UpdateData(
                table: "ArcaneVaultUsers",
                keyColumn: "UserName",
                keyValue: "user2",
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { "GCgoCQGz/CHbRAIg78toeKNgOfQQAFeQ9sjJsq2GW+GEMZF/uffSL3fD9lCuk0fAcGf9YvhUTqs71LA0IUjfRA==", "1od3GZycQ37doxIBAkCMZ1k9Wa/Lv2D7RqXPz5DGiUs=" });

            migrationBuilder.UpdateData(
                table: "ArcaneVaultUsers",
                keyColumn: "UserName",
                keyValue: "user3",
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { "0rarmgQtgwCE93c9zE+K4no/KW1WpSRjtHUzK0tjpUyjB+8O1L6Bh/tdJz/sHh7zRQenigX+Pq9s30PUtRGOnQ==", "xhsc0JcgLeQdHQClcTuJI7DumldKTfOyKZwJ758dv6c=" });

            migrationBuilder.UpdateData(
                table: "ArcaneVaultUsers",
                keyColumn: "UserName",
                keyValue: "user4",
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { "illQ+LvTh3mQb/kFz9mXDr1AW1pGXaAiSuns3awuOA09bvSE9/ynl0O09Levi8LCRPTjHbRqU+9uIyJ3djU/+A==", "TS3KdNajwimckt00qYr9QX68mufBmb0sbTEqMi8xRg0=" });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryCode", "CategoryName" },
                values: new object[,]
                {
                    { "ELECTRIC", "Electric Type" },
                    { "FIRE", "Fire Type" },
                    { "FLYING", "Flying Type" },
                    { "GRASS", "Grass Type" },
                    { "NORMAL", "Normal Type" },
                    { "POISON", "Poison Type" },
                    { "PSYCHIC", "Psychic Type" },
                    { "WATER", "Water Type" }
                });

            migrationBuilder.UpdateData(
                table: "CollectionItems",
                keyColumn: "ItemId",
                keyValue: 1,
                column: "ItemName",
                value: "Charizard Base Set");

            migrationBuilder.UpdateData(
                table: "CollectionItems",
                keyColumn: "ItemId",
                keyValue: 2,
                column: "ItemName",
                value: "Arcanine Promo");

            migrationBuilder.UpdateData(
                table: "CollectionItems",
                keyColumn: "ItemId",
                keyValue: 3,
                column: "ItemName",
                value: "Squirtle First Edition");

            migrationBuilder.UpdateData(
                table: "CollectionItems",
                keyColumn: "ItemId",
                keyValue: 4,
                column: "ItemName",
                value: "Gyarados Holo");

            migrationBuilder.UpdateData(
                table: "CollectionItems",
                keyColumn: "ItemId",
                keyValue: 5,
                column: "ItemName",
                value: "Bulbasaur Base Set");

            migrationBuilder.UpdateData(
                table: "CollectionItems",
                keyColumn: "ItemId",
                keyValue: 6,
                column: "ItemName",
                value: "Venusaur EX");

            migrationBuilder.UpdateData(
                table: "CollectionItems",
                keyColumn: "ItemId",
                keyValue: 7,
                column: "ItemName",
                value: "Pikachu Illustrator");

            migrationBuilder.UpdateData(
                table: "CollectionItems",
                keyColumn: "ItemId",
                keyValue: 8,
                column: "ItemName",
                value: "Raichu Holo");

            migrationBuilder.UpdateData(
                table: "CollectionItems",
                keyColumn: "ItemId",
                keyValue: 9,
                column: "ItemName",
                value: "Mewtwo GX");

            migrationBuilder.UpdateData(
                table: "CollectionItems",
                keyColumn: "ItemId",
                keyValue: 10,
                column: "ItemName",
                value: "Eevee Promo");

            migrationBuilder.InsertData(
                table: "CollectionItemCategories",
                columns: new[] { "CategoryCode", "ItemId" },
                values: new object[,]
                {
                    { "FIRE", 1 },
                    { "FLYING", 1 },
                    { "FIRE", 2 },
                    { "WATER", 3 },
                    { "FLYING", 4 },
                    { "WATER", 4 },
                    { "GRASS", 5 },
                    { "POISON", 5 },
                    { "GRASS", 6 },
                    { "POISON", 6 },
                    { "ELECTRIC", 7 },
                    { "ELECTRIC", 8 },
                    { "PSYCHIC", 9 },
                    { "NORMAL", 10 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CollectionItemCategories",
                keyColumns: new[] { "CategoryCode", "ItemId" },
                keyValues: new object[] { "FIRE", 1 });

            migrationBuilder.DeleteData(
                table: "CollectionItemCategories",
                keyColumns: new[] { "CategoryCode", "ItemId" },
                keyValues: new object[] { "FLYING", 1 });

            migrationBuilder.DeleteData(
                table: "CollectionItemCategories",
                keyColumns: new[] { "CategoryCode", "ItemId" },
                keyValues: new object[] { "FIRE", 2 });

            migrationBuilder.DeleteData(
                table: "CollectionItemCategories",
                keyColumns: new[] { "CategoryCode", "ItemId" },
                keyValues: new object[] { "WATER", 3 });

            migrationBuilder.DeleteData(
                table: "CollectionItemCategories",
                keyColumns: new[] { "CategoryCode", "ItemId" },
                keyValues: new object[] { "FLYING", 4 });

            migrationBuilder.DeleteData(
                table: "CollectionItemCategories",
                keyColumns: new[] { "CategoryCode", "ItemId" },
                keyValues: new object[] { "WATER", 4 });

            migrationBuilder.DeleteData(
                table: "CollectionItemCategories",
                keyColumns: new[] { "CategoryCode", "ItemId" },
                keyValues: new object[] { "GRASS", 5 });

            migrationBuilder.DeleteData(
                table: "CollectionItemCategories",
                keyColumns: new[] { "CategoryCode", "ItemId" },
                keyValues: new object[] { "POISON", 5 });

            migrationBuilder.DeleteData(
                table: "CollectionItemCategories",
                keyColumns: new[] { "CategoryCode", "ItemId" },
                keyValues: new object[] { "GRASS", 6 });

            migrationBuilder.DeleteData(
                table: "CollectionItemCategories",
                keyColumns: new[] { "CategoryCode", "ItemId" },
                keyValues: new object[] { "POISON", 6 });

            migrationBuilder.DeleteData(
                table: "CollectionItemCategories",
                keyColumns: new[] { "CategoryCode", "ItemId" },
                keyValues: new object[] { "ELECTRIC", 7 });

            migrationBuilder.DeleteData(
                table: "CollectionItemCategories",
                keyColumns: new[] { "CategoryCode", "ItemId" },
                keyValues: new object[] { "ELECTRIC", 8 });

            migrationBuilder.DeleteData(
                table: "CollectionItemCategories",
                keyColumns: new[] { "CategoryCode", "ItemId" },
                keyValues: new object[] { "PSYCHIC", 9 });

            migrationBuilder.DeleteData(
                table: "CollectionItemCategories",
                keyColumns: new[] { "CategoryCode", "ItemId" },
                keyValues: new object[] { "NORMAL", 10 });

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryCode",
                keyValue: "ELECTRIC");

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryCode",
                keyValue: "FIRE");

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryCode",
                keyValue: "FLYING");

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryCode",
                keyValue: "GRASS");

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryCode",
                keyValue: "NORMAL");

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryCode",
                keyValue: "POISON");

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryCode",
                keyValue: "PSYCHIC");

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryCode",
                keyValue: "WATER");

            migrationBuilder.UpdateData(
                table: "ArcaneVaultUsers",
                keyColumn: "UserName",
                keyValue: "staff1",
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { "r1ca5lr9v2LhMV8rBVFg8xjxcy1xoVcG6v3QG/H54/syONQVF/20ZpXHzQseDmAGB/pOdDgoLFPclPxEiP0WXQ==", "qGZnuDvNK762Gli6AeeuTwUB3oTsafYgQnDd6oVjwQQ=" });

            migrationBuilder.UpdateData(
                table: "ArcaneVaultUsers",
                keyColumn: "UserName",
                keyValue: "user1",
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { "pyQcnPqQV3WgTnCEgKJKdYWl3WkSgy0dRIai7nuA2LbVKnT/ybWMBaejfg9eGZQYvhZzwFlmKYAJxlvOGvGADQ==", "CX3b69o0zJ2SwxFAsaxwnV6WfPH+VJ5L8cYKPam5uTw=" });

            migrationBuilder.UpdateData(
                table: "ArcaneVaultUsers",
                keyColumn: "UserName",
                keyValue: "user2",
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { "NTj0pc1gvNV4qwVJiMUNDp6JNAm7QCQtdF6GLfq13K9yCh3Z8Jde76NdacbWE5lIi5uK+fxYvbjtSVPtmOlqgw==", "E01ZFMHQy2C4S28HaArGCusedLOkm3XFZKP052gy/QQ=" });

            migrationBuilder.UpdateData(
                table: "ArcaneVaultUsers",
                keyColumn: "UserName",
                keyValue: "user3",
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { "iUQ1IiqXAWh34+V3A+sV+cDmD2z1C+mj2sRxkvSCGmNUNx+X54DAvsZg0c423/TpWvOSanC1Uo7LB/rmg3GmWg==", "9bNSqpOIUX6LsdJkDDWm11XMm1h+qyrk+zYGXhrKjh0=" });

            migrationBuilder.UpdateData(
                table: "ArcaneVaultUsers",
                keyColumn: "UserName",
                keyValue: "user4",
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { "IKJj+64Lkl51FH6NSsFgv8aTlE6fEUYLHqOL46CzTUt2B/9ZmmX9iVXE3Vv26D5nLYv1Cis5iF+VOgynQ0279Q==", "Lv9feLJchBI70LgEQ+5UJC9W7CD6m/kN5Q4CpsBdJ78=" });

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

            migrationBuilder.UpdateData(
                table: "CollectionItems",
                keyColumn: "ItemId",
                keyValue: 1,
                column: "ItemName",
                value: "1909-S VDB Lincoln Cent");

            migrationBuilder.UpdateData(
                table: "CollectionItems",
                keyColumn: "ItemId",
                keyValue: 2,
                column: "ItemName",
                value: "Ember Drake Holo Card");

            migrationBuilder.UpdateData(
                table: "CollectionItems",
                keyColumn: "ItemId",
                keyValue: 3,
                column: "ItemName",
                value: "Penny Black Stamp");

            migrationBuilder.UpdateData(
                table: "CollectionItems",
                keyColumn: "ItemId",
                keyValue: 4,
                column: "ItemName",
                value: "Battle Sergeant Miniature");

            migrationBuilder.UpdateData(
                table: "CollectionItems",
                keyColumn: "ItemId",
                keyValue: 5,
                column: "ItemName",
                value: "Rare First Edition Notebook");

            migrationBuilder.UpdateData(
                table: "CollectionItems",
                keyColumn: "ItemId",
                keyValue: 6,
                column: "ItemName",
                value: "Abstract Canvas Original");

            migrationBuilder.UpdateData(
                table: "CollectionItems",
                keyColumn: "ItemId",
                keyValue: 7,
                column: "ItemName",
                value: "Inverted Jenny Stamp");

            migrationBuilder.UpdateData(
                table: "CollectionItems",
                keyColumn: "ItemId",
                keyValue: 8,
                column: "ItemName",
                value: "Anime Scale Figure");

            migrationBuilder.UpdateData(
                table: "CollectionItems",
                keyColumn: "ItemId",
                keyValue: 9,
                column: "ItemName",
                value: "1943 Steel Penny");

            migrationBuilder.UpdateData(
                table: "CollectionItems",
                keyColumn: "ItemId",
                keyValue: 10,
                column: "ItemName",
                value: "Signed Poetry Anthology");

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
        }
    }
}
