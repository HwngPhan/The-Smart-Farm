namespace TSF_mustidisProj.Models
{
    public class Feed
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        public string LastValue { get; set; }
        public DateTime RecordedAt { get; set; }
        
        // Foreign key
        public int UserId { get; set; }
        
        // Navigation property
        public User User { get; set; }
    }
}