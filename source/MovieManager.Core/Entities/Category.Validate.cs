using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MovieManager.Core.Contracts;

namespace MovieManager.Core.Entities
{
    public partial class Category : IDatabaseValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var unitOfWork = validationContext.GetService(typeof(IUnitOfWork)) as IUnitOfWork;
            if (unitOfWork == null)
            {
                throw new AccessViolationException("UnitOfWork is not injected!");
            }

            var validationResults = ExistsCategoryNameTwice(this, unitOfWork).Result;
            if (validationResults != ValidationResult.Success)
            {
                yield return validationResults;
            }
        }

        static async Task<ValidationResult> ExistsCategoryNameTwice(Category category, IUnitOfWork unitOfWork)
        {
            if (await unitOfWork.Categories.ExistsCategoryName(category.CategoryName, category.Id))
            {
                return new ValidationResult($"Category {category.CategoryName} existiert bereits!",
                    new List<string> {nameof(category.CategoryName)});
            }

            return ValidationResult.Success;
        }
    }

}
