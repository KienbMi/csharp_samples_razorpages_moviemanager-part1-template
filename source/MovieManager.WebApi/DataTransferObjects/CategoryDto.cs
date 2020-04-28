using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MovieManager.Core.Contracts;
using MovieManager.Core.Entities;
using MovieManager.Core.Validations;

namespace MovieManager.WebApi.DataTransferObjects
{
    public class CategoryDto
    {
        public int? Id { get; set; }

        [Required]
        public string CategoryName { get; set; }

        /// <summary>
        /// Konstruktor für die JSON-Serialisierung
        /// </summary>
        public CategoryDto()
        {
        }

        public CategoryDto(int? id, string categoryName)
        {
            Id = id;
            CategoryName = categoryName;
        }

        public CategoryDto(Category category)
        {
            Id = category.Id;
            CategoryName = category.CategoryName;
        }

    }
}
