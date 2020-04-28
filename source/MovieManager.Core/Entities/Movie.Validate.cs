using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MovieManager.Core.Contracts;
using MovieManager.Core.Validations;

namespace MovieManager.Core.Entities
{
    public partial class Movie : IDatabaseValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var unitOfWork = validationContext.GetService(typeof(IUnitOfWork)) as IUnitOfWork;
            if (unitOfWork == null)
            {
                throw new AccessViolationException("UnitOfWork is not injected!");
            }
            var validationResults = DatabaseValidations.ExistsMovieTwiceThisYearAsync(this, 
                unitOfWork).Result;
            if (validationResults != ValidationResult.Success)
            {
                yield return validationResults;
            }
        }
    }
}
