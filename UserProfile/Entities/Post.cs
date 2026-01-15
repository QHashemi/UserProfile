using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserProfile.Entities
{
    public class Post
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty; // title is Required
        public string? Description { get; set; } // Description is optional

        public bool IsPublished { get; set; } = false;
        public bool IsDeleted { get; set; } = false;


        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }



        // One-to-one Relation with User Details
        // Foreign Key
        [Key, ForeignKey("User")]
        public Guid UserId { get; set; }

        // Navigation Property
        public User User { get; set; } = null!;

    }
}
