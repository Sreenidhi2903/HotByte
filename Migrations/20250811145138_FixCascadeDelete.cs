using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Case_HotPot.Migrations
{
    /// <inheritdoc />
    public partial class FixCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Carts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    PasswordSalt = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Branches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RestaurantId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Branches_Users_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShippingAddresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AddressLine = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Pincode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingAddresses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MenuItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    QuantityAvailable = table.Column<int>(type: "int", nullable: false),
                    IsCommon = table.Column<bool>(type: "bit", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuItems_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentMethod = table.Column<int>(type: "int", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    BranchId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_Branches_BranchId1",
                        column: x => x.BranchId1,
                        principalTable: "Branches",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Orders_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CartItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CartId = table.Column<int>(type: "int", nullable: false),
                    MenuItemId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartItems_Carts_CartId",
                        column: x => x.CartId,
                        principalTable: "Carts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartItems_MenuItems_MenuItemId",
                        column: x => x.MenuItemId,
                        principalTable: "MenuItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MenuItemId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_MenuItems_MenuItemId",
                        column: x => x.MenuItemId,
                        principalTable: "MenuItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviews_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    MenuItemId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_MenuItems_MenuItemId",
                        column: x => x.MenuItemId,
                        principalTable: "MenuItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "MenuItems",
                columns: new[] { "Id", "BranchId", "Category", "Description", "DiscountPercentage", "ImageUrl", "IsAvailable", "IsCommon", "Name", "Price", "QuantityAvailable" },
                values: new object[,]
                {
                    { 1, null, "Main Course", "Classic tomato and cheese pizza", null, "/images/margherita_pizza.jpg", true, true, "Margherita Pizza", 102m, 50 },
                    { 2, null, "Main Course", "Loaded with vegetables and cheese", null, "/images/farmhouse_pizza.jpg", true, true, "Farmhouse Pizza", 103m, 50 },
                    { 3, null, "Main Course", "Spicy rice with marinated chicken", null, "/images/chicken_biryani.jpg", true, true, "Chicken Biryani", 104m, 50 },
                    { 4, null, "Main Course", "Basmati rice with vegetables and spices", null, "/images/veg_biryani.jpg", true, true, "Veg Biryani", 105m, 50 },
                    { 5, null, "Main Course", "Creamy tomato-based chicken curry", null, "/images/butter_chicken.jpg", true, true, "Butter Chicken", 106m, 50 },
                    { 6, null, "Main Course", "Cottage cheese in rich butter gravy", null, "/images/paneer_butter_masala.jpg", true, true, "Paneer Butter Masala", 107m, 50 },
                    { 7, null, "Main Course", "South Indian rice crepe with potato filling", null, "/images/masala_dosa.jpg", true, true, "Masala Dosa", 108m, 50 },
                    { 8, null, "Breakfast", "Steamed rice cakes with sambar", null, "/images/idli_sambar.jpg", true, true, "Idli Sambar", 109m, 50 },
                    { 9, null, "Breakfast", "Fried bread with spiced potato curry", null, "/images/poori_bhaji.jpg", true, true, "Poori Bhaji", 110m, 50 },
                    { 10, null, "Snacks", "Spicy mashed vegetables with buttered bread", null, "/images/pav_bhaji.jpg", true, true, "Pav Bhaji", 111m, 50 },
                    { 11, null, "Snacks", "Deep-fried pastry with spicy potato filling", null, "/images/samosa.jpg", true, true, "Samosa", 112m, 50 },
                    { 12, null, "Snacks", "Crispy fried potato sticks", null, "/images/french_fries.jpg", true, true, "French Fries", 113m, 50 },
                    { 13, null, "Snacks", "Fried crispy chicken bites", null, "/images/chicken_nuggets.jpg", true, true, "Chicken Nuggets", 114m, 50 },
                    { 14, null, "Starter", "Lettuce with parmesan and croutons", null, "/images/caesar_salad.jpg", true, true, "Caesar Salad", 115m, 50 },
                    { 15, null, "Starter", "Vegetable-stuffed fried rolls", null, "/images/spring_rolls.jpg", true, true, "Spring Rolls", 116m, 50 },
                    { 16, null, "Starter", "Creamy tomato-flavored soup", null, "/images/tomato_soup.jpg", true, true, "Tomato Soup", 117m, 50 },
                    { 17, null, "Starter", "Soup made with sweet corn and veggies", null, "/images/sweet_corn_soup.jpg", true, true, "Sweet Corn Soup", 118m, 50 },
                    { 18, null, "Beverage", "Fresh lemon drink", null, "/images/lemonade.jpg", true, true, "Lemonade", 119m, 50 },
                    { 19, null, "Beverage", "Spiced Indian tea", null, "/images/masala_chai.jpg", true, true, "Masala Chai", 120m, 50 },
                    { 20, null, "Beverage", "Chilled coffee with milk and sugar", null, "/images/cold_coffee.jpg", true, true, "Cold Coffee", 121m, 50 },
                    { 21, null, "Beverage", "Milkshake with chocolate", null, "/images/chocolate_shake.jpg", true, true, "Chocolate Shake", 122m, 50 },
                    { 22, null, "Beverage", "Yogurt drink with mango flavor", null, "/images/mango_lassi.jpg", true, true, "Mango Lassi", 123m, 50 },
                    { 23, null, "Dessert", "Classic vanilla dessert", null, "/images/vanilla_ice_cream.jpg", true, true, "Vanilla Ice Cream", 124m, 50 },
                    { 24, null, "Dessert", "Sweet deep-fried dumplings", null, "/images/gulab_jamun.jpg", true, true, "Gulab Jamun", 125m, 50 },
                    { 25, null, "Dessert", "Spongy white syrupy sweet", null, "/images/rasgulla.jpg", true, true, "Rasgulla", 126m, 50 },
                    { 26, null, "Dessert", "Chocolate baked dessert", null, "/images/brownie.jpg", true, true, "Brownie", 127m, 50 },
                    { 27, null, "Main Course", "Spicy chickpeas with fried bread", null, "/images/chole_bhature.jpg", true, true, "Chole Bhature", 128m, 50 },
                    { 28, null, "Main Course", "Kidney beans curry with rice", null, "/images/rajma_chawal.jpg", true, true, "Rajma Chawal", 129m, 50 },
                    { 29, null, "Breakfast", "Potato stuffed flatbread", null, "/images/aloo_paratha.jpg", true, true, "Aloo Paratha", 130m, 50 },
                    { 30, null, "Breakfast", "South Indian thick pancake", null, "/images/uttapam.jpg", true, true, "Uttapam", 131m, 50 },
                    { 31, null, "Main Course", "Spicy fish gravy", null, "/images/fish_curry.jpg", true, true, "Fish Curry", 132m, 50 },
                    { 32, null, "Main Course", "Kashmiri-style mutton curry", null, "/images/mutton_rogan_josh.jpg", true, true, "Mutton Rogan Josh", 133m, 50 },
                    { 33, null, "Main Course", "Charcoal-grilled chicken", null, "/images/tandoori_chicken.jpg", true, true, "Tandoori Chicken", 134m, 50 },
                    { 34, null, "Main Course", "Flavored rice with vegetables", null, "/images/veg_pulao.jpg", true, true, "Veg Pulao", 135m, 50 },
                    { 35, null, "Main Course", "Fried rice with chicken", null, "/images/chicken_fried_rice.jpg", true, true, "Chicken Fried Rice", 136m, 50 },
                    { 36, null, "Main Course", "Spicy Indo-Chinese gravy", null, "/images/manchurian.jpg", true, true, "Manchurian", 137m, 50 },
                    { 37, null, "Main Course", "Stir-fried noodles", null, "/images/hakka_noodles.jpg", true, true, "Hakka Noodles", 138m, 50 },
                    { 38, null, "Dessert", "Lentil-based sweet dish", null, "/images/moong_dal_halwa.jpg", true, true, "Moong Dal Halwa", 139m, 50 },
                    { 39, null, "Dessert", "Rice pudding with milk", null, "/images/kheer.jpg", true, true, "Kheer", 140m, 50 }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "City", "Email", "FullName", "PasswordHash", "PasswordSalt", "PhoneNumber", "Role" },
                values: new object[,]
                {
                    { "restaurant-blr", "Bangalore", "blrrestaurant@hotbyte.com", "Bangalore Restaurant", new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, null, "Restaurant" },
                    { "restaurant-hyd", "Hyderabad", "hydrestaurant@hotbyte.com", "Hyderabad Restaurant", new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, null, "Restaurant" }
                });

            migrationBuilder.InsertData(
                table: "Branches",
                columns: new[] { "Id", "ContactNumber", "IsDeleted", "Location", "Name", "RestaurantId" },
                values: new object[,]
                {
                    { 101, "040-12345601", false, "Hyderabad", "Hitech City", "restaurant-hyd" },
                    { 102, "040-12345602", false, "Hyderabad", "Kukatpally", "restaurant-hyd" },
                    { 103, "040-12345603", false, "Hyderabad", "Jubilee Hills", "restaurant-hyd" },
                    { 104, "040-12345604", false, "Hyderabad", "Gachibowli", "restaurant-hyd" },
                    { 105, "040-12345605", false, "Hyderabad", "Secunderabad", "restaurant-hyd" },
                    { 201, "080-98765401", false, "Bangalore", "Koramangala", "restaurant-blr" },
                    { 202, "080-98765402", false, "Bangalore", "Indiranagar", "restaurant-blr" },
                    { 203, "080-98765403", false, "Bangalore", "Whitefield", "restaurant-blr" },
                    { 204, "080-98765404", false, "Bangalore", "MG Road", "restaurant-blr" },
                    { 205, "080-98765405", false, "Bangalore", "Jayanagar", "restaurant-blr" }
                });

            migrationBuilder.InsertData(
                table: "MenuItems",
                columns: new[] { "Id", "BranchId", "Category", "Description", "DiscountPercentage", "ImageUrl", "IsAvailable", "IsCommon", "Name", "Price", "QuantityAvailable" },
                values: new object[,]
                {
                    { 40, 101, "Special", "Andhra-style chicken with gongura", null, "/images/gongura_chicken.jpg", true, false, "Gongura Chicken", 121m, 30 },
                    { 41, 101, "Special", "Wheat, lentils, and meat stew", null, "/images/hyderabadi_haleem.jpg", true, false, "Hyderabadi Haleem", 121m, 30 },
                    { 42, 101, "Special", "Spicy dosa with red chutney", null, "/images/mysore_masala_dosa.jpg", true, false, "Mysore Masala Dosa", 121m, 30 },
                    { 43, 101, "Special", "Spiced rice-lentil dish", null, "/images/bangalore_bisi_bele_bath.jpg", true, false, "Bangalore Bisi Bele Bath", 121m, 30 },
                    { 44, 101, "Special", "Crispy spicy chicken fry", null, "/images/andhra_chicken_fry.jpg", true, false, "Andhra Chicken Fry", 121m, 30 },
                    { 45, 101, "Beverage", "South Indian style brewed coffee", null, "/images/filter_coffee.jpg", true, false, "Filter Coffee", 121m, 30 },
                    { 46, 101, "Special", "Soft dosa with butter", null, "/images/bangalore_benne_dosa.jpg", true, false, "Bangalore Benne Dosa", 121m, 30 },
                    { 47, 101, "Dessert", "Bread pudding dessert", null, "/images/hyderabad_double_ka_meetha.jpg", true, false, "Hyderabad Double Ka Meetha", 121m, 30 },
                    { 48, 101, "Special", "Mini idlis with spicy powder", null, "/images/karampodi_idli.jpg", true, false, "Karampodi Idli", 121m, 30 },
                    { 49, 101, "Main Course", "Millet balls with curry", null, "/images/ragi_mudde.jpg", true, false, "Ragi Mudde", 121m, 30 },
                    { 50, 102, "Special", "Andhra-style chicken with gongura", null, "/images/gongura_chicken.jpg", true, false, "Gongura Chicken", 122m, 30 },
                    { 51, 102, "Special", "Wheat, lentils, and meat stew", null, "/images/hyderabadi_haleem.jpg", true, false, "Hyderabadi Haleem", 122m, 30 },
                    { 52, 102, "Special", "Spicy dosa with red chutney", null, "/images/mysore_masala_dosa.jpg", true, false, "Mysore Masala Dosa", 122m, 30 },
                    { 53, 102, "Special", "Spiced rice-lentil dish", null, "/images/bangalore_bisi_bele_bath.jpg", true, false, "Bangalore Bisi Bele Bath", 122m, 30 },
                    { 54, 102, "Special", "Crispy spicy chicken fry", null, "/images/andhra_chicken_fry.jpg", true, false, "Andhra Chicken Fry", 122m, 30 },
                    { 55, 102, "Beverage", "South Indian style brewed coffee", null, "/images/filter_coffee.jpg", true, false, "Filter Coffee", 122m, 30 },
                    { 56, 102, "Special", "Soft dosa with butter", null, "/images/bangalore_benne_dosa.jpg", true, false, "Bangalore Benne Dosa", 122m, 30 },
                    { 57, 102, "Dessert", "Bread pudding dessert", null, "/images/hyderabad_double_ka_meetha.jpg", true, false, "Hyderabad Double Ka Meetha", 122m, 30 },
                    { 58, 102, "Special", "Mini idlis with spicy powder", null, "/images/karampodi_idli.jpg", true, false, "Karampodi Idli", 122m, 30 },
                    { 59, 102, "Main Course", "Millet balls with curry", null, "/images/ragi_mudde.jpg", true, false, "Ragi Mudde", 122m, 30 },
                    { 60, 103, "Special", "Andhra-style chicken with gongura", null, "/images/gongura_chicken.jpg", true, false, "Gongura Chicken", 123m, 30 },
                    { 61, 103, "Special", "Wheat, lentils, and meat stew", null, "/images/hyderabadi_haleem.jpg", true, false, "Hyderabadi Haleem", 123m, 30 },
                    { 62, 103, "Special", "Spicy dosa with red chutney", null, "/images/mysore_masala_dosa.jpg", true, false, "Mysore Masala Dosa", 123m, 30 },
                    { 63, 103, "Special", "Spiced rice-lentil dish", null, "/images/bangalore_bisi_bele_bath.jpg", true, false, "Bangalore Bisi Bele Bath", 123m, 30 },
                    { 64, 103, "Special", "Crispy spicy chicken fry", null, "/images/andhra_chicken_fry.jpg", true, false, "Andhra Chicken Fry", 123m, 30 },
                    { 65, 103, "Beverage", "South Indian style brewed coffee", null, "/images/filter_coffee.jpg", true, false, "Filter Coffee", 123m, 30 },
                    { 66, 103, "Special", "Soft dosa with butter", null, "/images/bangalore_benne_dosa.jpg", true, false, "Bangalore Benne Dosa", 123m, 30 },
                    { 67, 103, "Dessert", "Bread pudding dessert", null, "/images/hyderabad_double_ka_meetha.jpg", true, false, "Hyderabad Double Ka Meetha", 123m, 30 },
                    { 68, 103, "Special", "Mini idlis with spicy powder", null, "/images/karampodi_idli.jpg", true, false, "Karampodi Idli", 123m, 30 },
                    { 69, 103, "Main Course", "Millet balls with curry", null, "/images/ragi_mudde.jpg", true, false, "Ragi Mudde", 123m, 30 },
                    { 70, 104, "Special", "Andhra-style chicken with gongura", null, "/images/gongura_chicken.jpg", true, false, "Gongura Chicken", 124m, 30 },
                    { 71, 104, "Special", "Wheat, lentils, and meat stew", null, "/images/hyderabadi_haleem.jpg", true, false, "Hyderabadi Haleem", 124m, 30 },
                    { 72, 104, "Special", "Spicy dosa with red chutney", null, "/images/mysore_masala_dosa.jpg", true, false, "Mysore Masala Dosa", 124m, 30 },
                    { 73, 104, "Special", "Spiced rice-lentil dish", null, "/images/bangalore_bisi_bele_bath.jpg", true, false, "Bangalore Bisi Bele Bath", 124m, 30 },
                    { 74, 104, "Special", "Crispy spicy chicken fry", null, "/images/andhra_chicken_fry.jpg", true, false, "Andhra Chicken Fry", 124m, 30 },
                    { 75, 104, "Beverage", "South Indian style brewed coffee", null, "/images/filter_coffee.jpg", true, false, "Filter Coffee", 124m, 30 },
                    { 76, 104, "Special", "Soft dosa with butter", null, "/images/bangalore_benne_dosa.jpg", true, false, "Bangalore Benne Dosa", 124m, 30 },
                    { 77, 104, "Dessert", "Bread pudding dessert", null, "/images/hyderabad_double_ka_meetha.jpg", true, false, "Hyderabad Double Ka Meetha", 124m, 30 },
                    { 78, 104, "Special", "Mini idlis with spicy powder", null, "/images/karampodi_idli.jpg", true, false, "Karampodi Idli", 124m, 30 },
                    { 79, 104, "Main Course", "Millet balls with curry", null, "/images/ragi_mudde.jpg", true, false, "Ragi Mudde", 124m, 30 },
                    { 80, 105, "Special", "Andhra-style chicken with gongura", null, "/images/gongura_chicken.jpg", true, false, "Gongura Chicken", 125m, 30 },
                    { 81, 105, "Special", "Wheat, lentils, and meat stew", null, "/images/hyderabadi_haleem.jpg", true, false, "Hyderabadi Haleem", 125m, 30 },
                    { 82, 105, "Special", "Spicy dosa with red chutney", null, "/images/mysore_masala_dosa.jpg", true, false, "Mysore Masala Dosa", 125m, 30 },
                    { 83, 105, "Special", "Spiced rice-lentil dish", null, "/images/bangalore_bisi_bele_bath.jpg", true, false, "Bangalore Bisi Bele Bath", 125m, 30 },
                    { 84, 105, "Special", "Crispy spicy chicken fry", null, "/images/andhra_chicken_fry.jpg", true, false, "Andhra Chicken Fry", 125m, 30 },
                    { 85, 105, "Beverage", "South Indian style brewed coffee", null, "/images/filter_coffee.jpg", true, false, "Filter Coffee", 125m, 30 },
                    { 86, 105, "Special", "Soft dosa with butter", null, "/images/bangalore_benne_dosa.jpg", true, false, "Bangalore Benne Dosa", 125m, 30 },
                    { 87, 105, "Dessert", "Bread pudding dessert", null, "/images/hyderabad_double_ka_meetha.jpg", true, false, "Hyderabad Double Ka Meetha", 125m, 30 },
                    { 88, 105, "Special", "Mini idlis with spicy powder", null, "/images/karampodi_idli.jpg", true, false, "Karampodi Idli", 125m, 30 },
                    { 89, 105, "Main Course", "Millet balls with curry", null, "/images/ragi_mudde.jpg", true, false, "Ragi Mudde", 125m, 30 },
                    { 90, 201, "Special", "Andhra-style chicken with gongura", null, "/images/gongura_chicken.jpg", true, false, "Gongura Chicken", 121m, 30 },
                    { 91, 201, "Special", "Wheat, lentils, and meat stew", null, "/images/hyderabadi_haleem.jpg", true, false, "Hyderabadi Haleem", 121m, 30 },
                    { 92, 201, "Special", "Spicy dosa with red chutney", null, "/images/mysore_masala_dosa.jpg", true, false, "Mysore Masala Dosa", 121m, 30 },
                    { 93, 201, "Special", "Spiced rice-lentil dish", null, "/images/bangalore_bisi_bele_bath.jpg", true, false, "Bangalore Bisi Bele Bath", 121m, 30 },
                    { 94, 201, "Special", "Crispy spicy chicken fry", null, "/images/andhra_chicken_fry.jpg", true, false, "Andhra Chicken Fry", 121m, 30 },
                    { 95, 201, "Beverage", "South Indian style brewed coffee", null, "/images/filter_coffee.jpg", true, false, "Filter Coffee", 121m, 30 },
                    { 96, 201, "Special", "Soft dosa with butter", null, "/images/bangalore_benne_dosa.jpg", true, false, "Bangalore Benne Dosa", 121m, 30 },
                    { 97, 201, "Dessert", "Bread pudding dessert", null, "/images/hyderabad_double_ka_meetha.jpg", true, false, "Hyderabad Double Ka Meetha", 121m, 30 },
                    { 98, 201, "Special", "Mini idlis with spicy powder", null, "/images/karampodi_idli.jpg", true, false, "Karampodi Idli", 121m, 30 },
                    { 99, 201, "Main Course", "Millet balls with curry", null, "/images/ragi_mudde.jpg", true, false, "Ragi Mudde", 121m, 30 },
                    { 100, 202, "Special", "Andhra-style chicken with gongura", null, "/images/gongura_chicken.jpg", true, false, "Gongura Chicken", 122m, 30 },
                    { 101, 202, "Special", "Wheat, lentils, and meat stew", null, "/images/hyderabadi_haleem.jpg", true, false, "Hyderabadi Haleem", 122m, 30 },
                    { 102, 202, "Special", "Spicy dosa with red chutney", null, "/images/mysore_masala_dosa.jpg", true, false, "Mysore Masala Dosa", 122m, 30 },
                    { 103, 202, "Special", "Spiced rice-lentil dish", null, "/images/bangalore_bisi_bele_bath.jpg", true, false, "Bangalore Bisi Bele Bath", 122m, 30 },
                    { 104, 202, "Special", "Crispy spicy chicken fry", null, "/images/andhra_chicken_fry.jpg", true, false, "Andhra Chicken Fry", 122m, 30 },
                    { 105, 202, "Beverage", "South Indian style brewed coffee", null, "/images/filter_coffee.jpg", true, false, "Filter Coffee", 122m, 30 },
                    { 106, 202, "Special", "Soft dosa with butter", null, "/images/bangalore_benne_dosa.jpg", true, false, "Bangalore Benne Dosa", 122m, 30 },
                    { 107, 202, "Dessert", "Bread pudding dessert", null, "/images/hyderabad_double_ka_meetha.jpg", true, false, "Hyderabad Double Ka Meetha", 122m, 30 },
                    { 108, 202, "Special", "Mini idlis with spicy powder", null, "/images/karampodi_idli.jpg", true, false, "Karampodi Idli", 122m, 30 },
                    { 109, 202, "Main Course", "Millet balls with curry", null, "/images/ragi_mudde.jpg", true, false, "Ragi Mudde", 122m, 30 },
                    { 110, 203, "Special", "Andhra-style chicken with gongura", null, "/images/gongura_chicken.jpg", true, false, "Gongura Chicken", 123m, 30 },
                    { 111, 203, "Special", "Wheat, lentils, and meat stew", null, "/images/hyderabadi_haleem.jpg", true, false, "Hyderabadi Haleem", 123m, 30 },
                    { 112, 203, "Special", "Spicy dosa with red chutney", null, "/images/mysore_masala_dosa.jpg", true, false, "Mysore Masala Dosa", 123m, 30 },
                    { 113, 203, "Special", "Spiced rice-lentil dish", null, "/images/bangalore_bisi_bele_bath.jpg", true, false, "Bangalore Bisi Bele Bath", 123m, 30 },
                    { 114, 203, "Special", "Crispy spicy chicken fry", null, "/images/andhra_chicken_fry.jpg", true, false, "Andhra Chicken Fry", 123m, 30 },
                    { 115, 203, "Beverage", "South Indian style brewed coffee", null, "/images/filter_coffee.jpg", true, false, "Filter Coffee", 123m, 30 },
                    { 116, 203, "Special", "Soft dosa with butter", null, "/images/bangalore_benne_dosa.jpg", true, false, "Bangalore Benne Dosa", 123m, 30 },
                    { 117, 203, "Dessert", "Bread pudding dessert", null, "/images/hyderabad_double_ka_meetha.jpg", true, false, "Hyderabad Double Ka Meetha", 123m, 30 },
                    { 118, 203, "Special", "Mini idlis with spicy powder", null, "/images/karampodi_idli.jpg", true, false, "Karampodi Idli", 123m, 30 },
                    { 119, 203, "Main Course", "Millet balls with curry", null, "/images/ragi_mudde.jpg", true, false, "Ragi Mudde", 123m, 30 },
                    { 120, 204, "Special", "Andhra-style chicken with gongura", null, "/images/gongura_chicken.jpg", true, false, "Gongura Chicken", 124m, 30 },
                    { 121, 204, "Special", "Wheat, lentils, and meat stew", null, "/images/hyderabadi_haleem.jpg", true, false, "Hyderabadi Haleem", 124m, 30 },
                    { 122, 204, "Special", "Spicy dosa with red chutney", null, "/images/mysore_masala_dosa.jpg", true, false, "Mysore Masala Dosa", 124m, 30 },
                    { 123, 204, "Special", "Spiced rice-lentil dish", null, "/images/bangalore_bisi_bele_bath.jpg", true, false, "Bangalore Bisi Bele Bath", 124m, 30 },
                    { 124, 204, "Special", "Crispy spicy chicken fry", null, "/images/andhra_chicken_fry.jpg", true, false, "Andhra Chicken Fry", 124m, 30 },
                    { 125, 204, "Beverage", "South Indian style brewed coffee", null, "/images/filter_coffee.jpg", true, false, "Filter Coffee", 124m, 30 },
                    { 126, 204, "Special", "Soft dosa with butter", null, "/images/bangalore_benne_dosa.jpg", true, false, "Bangalore Benne Dosa", 124m, 30 },
                    { 127, 204, "Dessert", "Bread pudding dessert", null, "/images/hyderabad_double_ka_meetha.jpg", true, false, "Hyderabad Double Ka Meetha", 124m, 30 },
                    { 128, 204, "Special", "Mini idlis with spicy powder", null, "/images/karampodi_idli.jpg", true, false, "Karampodi Idli", 124m, 30 },
                    { 129, 204, "Main Course", "Millet balls with curry", null, "/images/ragi_mudde.jpg", true, false, "Ragi Mudde", 124m, 30 },
                    { 130, 205, "Special", "Andhra-style chicken with gongura", null, "/images/gongura_chicken.jpg", true, false, "Gongura Chicken", 125m, 30 },
                    { 131, 205, "Special", "Wheat, lentils, and meat stew", null, "/images/hyderabadi_haleem.jpg", true, false, "Hyderabadi Haleem", 125m, 30 },
                    { 132, 205, "Special", "Spicy dosa with red chutney", null, "/images/mysore_masala_dosa.jpg", true, false, "Mysore Masala Dosa", 125m, 30 },
                    { 133, 205, "Special", "Spiced rice-lentil dish", null, "/images/bangalore_bisi_bele_bath.jpg", true, false, "Bangalore Bisi Bele Bath", 125m, 30 },
                    { 134, 205, "Special", "Crispy spicy chicken fry", null, "/images/andhra_chicken_fry.jpg", true, false, "Andhra Chicken Fry", 125m, 30 },
                    { 135, 205, "Beverage", "South Indian style brewed coffee", null, "/images/filter_coffee.jpg", true, false, "Filter Coffee", 125m, 30 },
                    { 136, 205, "Special", "Soft dosa with butter", null, "/images/bangalore_benne_dosa.jpg", true, false, "Bangalore Benne Dosa", 125m, 30 },
                    { 137, 205, "Dessert", "Bread pudding dessert", null, "/images/hyderabad_double_ka_meetha.jpg", true, false, "Hyderabad Double Ka Meetha", 125m, 30 },
                    { 138, 205, "Special", "Mini idlis with spicy powder", null, "/images/karampodi_idli.jpg", true, false, "Karampodi Idli", 125m, 30 },
                    { 139, 205, "Main Course", "Millet balls with curry", null, "/images/ragi_mudde.jpg", true, false, "Ragi Mudde", 125m, 30 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Branches_RestaurantId",
                table: "Branches",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_CartId",
                table: "CartItems",
                column: "CartId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_MenuItemId",
                table: "CartItems",
                column: "MenuItemId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_BranchId",
                table: "MenuItems",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_MenuItemId",
                table: "OrderItems",
                column: "MenuItemId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_BranchId",
                table: "Orders",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_BranchId1",
                table: "Orders",
                column: "BranchId1");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_MenuItemId",
                table: "Reviews",
                column: "MenuItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserId",
                table: "Reviews",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingAddresses_UserId",
                table: "ShippingAddresses",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartItems");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "ShippingAddresses");

            migrationBuilder.DropTable(
                name: "Carts");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "MenuItems");

            migrationBuilder.DropTable(
                name: "Branches");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
