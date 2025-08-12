using HotByteAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace HotByteAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ShippingAddress> ShippingAddresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed Restaurant Users with explicit string Ids and valid Base64 PasswordSalt/Hash
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = "restaurant-hyd",
                    FullName = "Hyderabad Restaurant",
                    Email = "hydrestaurant@hotbyte.com",
                    Role = "Restaurant",
                    City = "Hyderabad",
                    PasswordSalt = Convert.FromBase64String("AAAAAAAAAAAAAAAAAAAAAA=="), // 16 bytes of zero
                    PasswordHash = Convert.FromBase64String("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA=") // 24 bytes of zero (replace with real if needed)
                },
                new User
                {
                    Id = "restaurant-blr",
                    FullName = "Bangalore Restaurant",
                    Email = "blrrestaurant@hotbyte.com",
                    Role = "Restaurant",
                    City = "Bangalore",
                    PasswordSalt = Convert.FromBase64String("AAAAAAAAAAAAAAAAAAAAAA=="),
                    PasswordHash = Convert.FromBase64String("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA=")
                }
            );

            // Seed branches
            var branches = new List<Branch>
            {
                new Branch { Id = 101, Name = "Hitech City", Location = "Hyderabad", ContactNumber = "040-12345601", RestaurantId = "restaurant-hyd" },
                new Branch { Id = 102, Name = "Kukatpally", Location = "Hyderabad", ContactNumber = "040-12345602", RestaurantId = "restaurant-hyd" },
                new Branch { Id = 103, Name = "Jubilee Hills", Location = "Hyderabad", ContactNumber = "040-12345603", RestaurantId = "restaurant-hyd" },
                new Branch { Id = 104, Name = "Gachibowli", Location = "Hyderabad", ContactNumber = "040-12345604", RestaurantId = "restaurant-hyd" },
                new Branch { Id = 105, Name = "Secunderabad", Location = "Hyderabad", ContactNumber = "040-12345605", RestaurantId = "restaurant-hyd" },
                new Branch { Id = 201, Name = "Koramangala", Location = "Bangalore", ContactNumber = "080-98765401", RestaurantId = "restaurant-blr" },
                new Branch { Id = 202, Name = "Indiranagar", Location = "Bangalore", ContactNumber = "080-98765402", RestaurantId = "restaurant-blr" },
                new Branch { Id = 203, Name = "Whitefield", Location = "Bangalore", ContactNumber = "080-98765403", RestaurantId = "restaurant-blr" },
                new Branch { Id = 204, Name = "MG Road", Location = "Bangalore", ContactNumber = "080-98765404", RestaurantId = "restaurant-blr" },
                new Branch { Id = 205, Name = "Jayanagar", Location = "Bangalore", ContactNumber = "080-98765405", RestaurantId = "restaurant-blr" }
            };
            modelBuilder.Entity<Branch>().HasData(branches);

            // Configure Branch.RestaurantId FK (no cascade delete)
            modelBuilder.Entity<Branch>()
                .HasOne(b => b.Restaurant)
                .WithMany()
                .HasForeignKey(b => b.RestaurantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Order.UserId FK to Restrict delete (fixes multiple cascade paths error)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Order.BranchId FK to Restrict delete (fixes multiple cascade paths error)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Branch)
                .WithMany()
                .HasForeignKey(o => o.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed Menu Items
            var menuItems = new List<MenuItem>();
            int menuItemId = 1;

            var commonMenuItems = new List<(string Name, string Description, string Category)>
            {
                ("Margherita Pizza", "Classic tomato and cheese pizza", "Main Course"),
                ("Farmhouse Pizza", "Loaded with vegetables and cheese", "Main Course"),
                ("Chicken Biryani", "Spicy rice with marinated chicken", "Main Course"),
                ("Veg Biryani", "Basmati rice with vegetables and spices", "Main Course"),
                ("Butter Chicken", "Creamy tomato-based chicken curry", "Main Course"),
                ("Paneer Butter Masala", "Cottage cheese in rich butter gravy", "Main Course"),
                ("Masala Dosa", "South Indian rice crepe with potato filling", "Main Course"),
                ("Idli Sambar", "Steamed rice cakes with sambar", "Breakfast"),
                ("Poori Bhaji", "Fried bread with spiced potato curry", "Breakfast"),
                ("Pav Bhaji", "Spicy mashed vegetables with buttered bread", "Snacks"),
                ("Samosa", "Deep-fried pastry with spicy potato filling", "Snacks"),
                ("French Fries", "Crispy fried potato sticks", "Snacks"),
                ("Chicken Nuggets", "Fried crispy chicken bites", "Snacks"),
                ("Caesar Salad", "Lettuce with parmesan and croutons", "Starter"),
                ("Spring Rolls", "Vegetable-stuffed fried rolls", "Starter"),
                ("Tomato Soup", "Creamy tomato-flavored soup", "Starter"),
                ("Sweet Corn Soup", "Soup made with sweet corn and veggies", "Starter"),
                ("Lemonade", "Fresh lemon drink", "Beverage"),
                ("Masala Chai", "Spiced Indian tea", "Beverage"),
                ("Cold Coffee", "Chilled coffee with milk and sugar", "Beverage"),
                ("Chocolate Shake", "Milkshake with chocolate", "Beverage"),
                ("Mango Lassi", "Yogurt drink with mango flavor", "Beverage"),
                ("Vanilla Ice Cream", "Classic vanilla dessert", "Dessert"),
                ("Gulab Jamun", "Sweet deep-fried dumplings", "Dessert"),
                ("Rasgulla", "Spongy white syrupy sweet", "Dessert"),
                ("Brownie", "Chocolate baked dessert", "Dessert"),
                ("Chole Bhature", "Spicy chickpeas with fried bread", "Main Course"),
                ("Rajma Chawal", "Kidney beans curry with rice", "Main Course"),
                ("Aloo Paratha", "Potato stuffed flatbread", "Breakfast"),
                ("Uttapam", "South Indian thick pancake", "Breakfast"),
                ("Fish Curry", "Spicy fish gravy", "Main Course"),
                ("Mutton Rogan Josh", "Kashmiri-style mutton curry", "Main Course"),
                ("Tandoori Chicken", "Charcoal-grilled chicken", "Main Course"),
                ("Veg Pulao", "Flavored rice with vegetables", "Main Course"),
                ("Chicken Fried Rice", "Fried rice with chicken", "Main Course"),
                ("Manchurian", "Spicy Indo-Chinese gravy", "Main Course"),
                ("Hakka Noodles", "Stir-fried noodles", "Main Course"),
                ("Moong Dal Halwa", "Lentil-based sweet dish", "Dessert"),
                ("Kheer", "Rice pudding with milk", "Dessert")
            };

            foreach (var item in commonMenuItems)
            {
                menuItems.Add(new MenuItem
                {
                    Id = menuItemId++,
                    Name = item.Name,
                    Description = item.Description,
                    Category = item.Category,
                    Price = 100 + menuItemId % 50,
                    QuantityAvailable = 50,
                    BranchId = null,
                    IsCommon = true,
                    IsAvailable = true,
                    ImageUrl = $"/images/{item.Name.Replace(" ", "_").ToLower()}.jpg"
                });
            }

            // Special branch-specific items
            var specialItems = new List<(string Name, string Description, string Category)>
            {
                ("Gongura Chicken", "Andhra-style chicken with gongura", "Special"),
                ("Hyderabadi Haleem", "Wheat, lentils, and meat stew", "Special"),
                ("Mysore Masala Dosa", "Spicy dosa with red chutney", "Special"),
                ("Bangalore Bisi Bele Bath", "Spiced rice-lentil dish", "Special"),
                ("Andhra Chicken Fry", "Crispy spicy chicken fry", "Special"),
                ("Filter Coffee", "South Indian style brewed coffee", "Beverage"),
                ("Bangalore Benne Dosa", "Soft dosa with butter", "Special"),
                ("Hyderabad Double Ka Meetha", "Bread pudding dessert", "Dessert"),
                ("Karampodi Idli", "Mini idlis with spicy powder", "Special"),
                ("Ragi Mudde", "Millet balls with curry", "Main Course")
            };

            var branchIds = new[] { 101, 102, 103, 104, 105, 201, 202, 203, 204, 205 };

            foreach (var branchId in branchIds)
            {
                foreach (var item in specialItems)
                {
                    menuItems.Add(new MenuItem
                    {
                        Id = menuItemId++,
                        Name = item.Name,
                        Description = item.Description,
                        Category = item.Category,
                        Price = 120 + branchId % 50,
                        QuantityAvailable = 30,
                        BranchId = branchId,
                        IsCommon = false,
                        IsAvailable = true,
                        ImageUrl = $"/images/{item.Name.Replace(" ", "_").ToLower()}.jpg"
                    });
                }
            }

            modelBuilder.Entity<MenuItem>().HasData(menuItems);
        }
    }
}
