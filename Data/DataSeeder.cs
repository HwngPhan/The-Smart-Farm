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

            // Check if there are any users
            if (!context.Users.Any())
            {
                // Add sample users
                var users = new[]
                {
                    new User { Username = "user1" },
                    new User { Username = "user2" }
                };

                context.Users.AddRange(users);
                context.SaveChanges();

                // Add sample feeds
                var feeds = new[]
                {
                    new Feed 
                    { 
                        Name = "Temperature", 
                        Key = "temp-feed", 
                        LastValue = "24.5", 
                        RecordedAt = DateTime.Now.AddHours(-1),
                        UserId = users[0].Id
                    },
                    new Feed 
                    { 
                        Name = "Humidity", 
                        Key = "humidity-feed", 
                        LastValue = "65.2", 
                        RecordedAt = DateTime.Now.AddHours(-2),
                        UserId = users[0].Id
                    },
                    new Feed 
                    { 
                        Name = "Light", 
                        Key = "light-feed", 
                        LastValue = "345", 
                        RecordedAt = DateTime.Now.AddHours(-1),
                        UserId = users[1].Id
                    }
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