using System;
using System.Linq;
using TSF_mustidisProj.Models;

namespace TSF_mustidisProj.Data
{
    public static class DataSeeder
    {
        public static void SeedData(ApplicationDbContext context)
        {
            // Ensure database is created
            context.Database.EnsureCreated();
            context.Feeds.RemoveRange(context.Feeds.ToList());
            context.Users.RemoveRange(context.Users.ToList());
            context.SaveChanges();

            // context.Database.ExecuteSqlRaw("ALTER TABLE Users AUTO_INCREMENT = 1");
            // // Reset auto-increment for Feeds table
            // context.Database.ExecuteSqlRaw("ALTER TABLE Feeds AUTO_INCREMENT = 1");
            
            if (!context.Users.Any())
            {
                // Add sample users
                var users = new[]
                {
                    new User { Id = 1, Username = "user1" },
                    new User { Id = 2, Username = "user2" }
                };

                context.Users.AddRange(users);
                context.SaveChanges();

                // Add sample feeds
                var feeds = new[]
                {
                    new Feed 
                    {
                        Id = 1,
                        Name = "AutoPump", 
                        Key = "autopump", 
                        LastValue = "OFF", 
                        
                        UserId = users[0].Id
                    },
                    new Feed 
                    { 
                        Id = 2,
                        Name = "light Intensity", 
                        Key = "light", 
                        LastValue = "1715", 
                        UserId = users[0].Id
                    },
                    new Feed
                    { 
                        Id = 3,
                        Name = "lights", 
                        Key = "lights", 
                        LastValue = "OFF", 
                        UserId = users[0].Id
                    },
                    new Feed 
                    { 
                        Id = 4,
                        Name = "moisture", 
                        Key = "moisture", 
                        LastValue = "57.9", 
                        UserId = users[0].Id
                    },
                    new Feed 
                    { 
                        Id = 5,
                        Name = "pumps", 
                        Key = "pumps", 
                        LastValue = "ON", 
                        UserId = users[0].Id
                    },
                    new Feed 
                    { 
                        Id = 6,
                        Name = "soimoi", 
                        Key = "soimoi", 
                        LastValue = "0", 
                        UserId = users[0].Id
                    },
                    new Feed 
                    { 
                        Id = 7,
                        Name = "soimoilimit", 
                        Key = "soimoilimit", 
                        LastValue = "30", 
                        UserId = users[0].Id
                    },
                    new Feed 
                    { 
                        Id = 8,
                        Name = "temperature", 
                        Key = "temperature", 
                        LastValue = "28.0", 
                        UserId = users[0].Id
                    },
                    
                };

                context.Feeds.AddRange(feeds);
                context.SaveChanges();

                Console.WriteLine("Sample data seeded successfully!");
            }
            else
            {
                Console.WriteLine("Database already contains data - skipping seed.");
            }
        }
    }
}