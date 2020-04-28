using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MovieManager.Core.Validations;

namespace MovieManager.Core.Entities
{
    public partial class Movie : EntityObject
    {
        [Required]
        [StringLength(100, ErrorMessage = "The length of {0} must be between {2} and {1}.", MinimumLength = 2)]
        public string Title { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; }

        [Required]
        [Range(1,int.MaxValue, ErrorMessage = "CategoryId must be greater than zero")]
        public int CategoryId { get; set; }

        [Required]
        [Range(0, 300)]
        [ClassicMovieMaxDuration(isClassicMovieUntilYear: 1950, maxDurationForClassicMovie: 60)]
        public int Duration { get; set; } //in Minuten

        [Range(1900, 2099)]
        [Required]
        public int Year { get; set; }
    }
}
