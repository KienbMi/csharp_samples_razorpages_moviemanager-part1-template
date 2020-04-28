using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using MovieManager.Core.Contracts;
using MovieManager.Core.Entities;
using MovieManager.Core.Validations;
using Namotion.Reflection;

namespace MovieManager.WebApi.DataTransferObjects
{
    public class MovieDto // : IDatabaseValidatableObject
    {
        public MovieDto()
        {
        }

        public MovieDto(Movie movie)
        {
            Id = movie.Id; 
            Title = movie.Title;
            Duration = movie.Duration;
            Year = movie.Year;
            CategoryId = movie.CategoryId;
        }

        public Movie ToEntity()
        {
            return new Movie
            {
                Id = this.Id.GetValueOrDefault(),
                Title = this.Title,
                Duration = this.Duration,
                Year = this.Year,
                CategoryId = this.CategoryId
            };

        }

        public int? Id { get; set; }

        //[Required]
        //[StringLength(100, ErrorMessage = "The length of {0} must be between {2} and {1}.", MinimumLength = 2)]
        public string Title { get; set; }

        //[Required]
        //[Range(1, int.MaxValue, ErrorMessage = "CategoryId must be greater than zero")]
        public int CategoryId { get; set; }

        //[Required]
        //[Range(0, 300)]
        public int Duration { get; set; } //in Minuten

        //[Range(1900, 2099)]
        //[Required]
        public int Year { get; set; }


        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    var unitOfWork = validationContext.GetService(typeof(IUnitOfWork)) as IUnitOfWork;
        //    if (unitOfWork == null)
        //    {
        //        throw new AccessViolationException("UnitOfWork is not injected!");
        //    }
        //    var validationResults = DatabaseValidations.ExistsMovieTwiceThisYearAsync(ToEntity(), unitOfWork).Result;
        //    if (validationResults != ValidationResult.Success)
        //    {
        //        yield return validationResults;
        //    }
        //}


    }
}
