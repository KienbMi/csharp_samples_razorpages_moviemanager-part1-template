using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieManager.Core.Contracts;
using MovieManager.Core.Entities;
using MovieManager.WebApi.DataTransferObjects;

namespace MovieManager.WebApi.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoriesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Liefert alle existierenden Kategorien
        /// </summary>
        /// <response code="200">Die Abfrage war erfolgreich.</response>
        // GET: api/Categories
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<MovieManager.WebApi.DataTransferObjects.CategoryDto[]> GetCategories()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();
            //object[] apiCategories = categories
            //    .Select(c => new
            //    {
            //        CategorieName = c.CategoryName,
            //        Movies = c.Movies
            //            .Select(m => new
            //            {
            //                m.Title,
            //                m.Duration,
            //                m.Year
            //            })
            //    })
            //    .ToArray();
            CategoryDto[] apiCategories = categories
                .Select(c => new CategoryDto(c.Id, c.CategoryName))
                .ToArray();
            //var categoryNames = categories.Select(c => c.CategoryName).ToArray();
            return apiCategories; //apiCategories;
        }

        //// GET: api/Categories
        //[HttpGet]
        //public async Task<string[]> GetCategories()
        //{
        //    var categories = await _unitOfWork.Categories.GetAllAsync();
        //    var categoryNames = categories.Select(c => c.CategoryName).ToArray();
        //    return categoryNames;
        //}

        /// <summary>
        /// Liefert eine spezifische Kategorie
        /// </summary>
        /// <param name="id">Die Id der Kategorie</param>
        /// <response code="200">Die Abfrage war erfolgreich.</response>
        /// <response code="404">Unbekannte Id!</response>
        // GET: api/Categories/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoryDto>> GetCategory(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return new CategoryDto(category.Id, category.CategoryName );
        }

        /// <summary>
        /// Liefert die Filme zu einer Kateogie
        /// </summary>
        /// <param name="id">Die Id der Kategorie</param>
        /// <response code="200">Die Abfrage war erfolgreich.</response>
        /// <response code="400">Die Id konnte nicht verarbeitet werden!</response>
        /// <response code="404">Unbekannte Id!</response>
        // GET: api/Categories/5/movies
        [HttpGet]
        [Route("{id}/movies")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<object[]>> GetMoviesByCategory(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            var movies = await _unitOfWork.Movies.GetAllByCatIdAsync(id);
            var resources = movies.Select(m => new
            {
                m.Title,
                m.Duration,
                m.Year
            }).ToArray();

            return resources;
        }


        /// <summary>
        /// Erstellt eine neue Kategorie
        /// </summary>
        /// <param name="categoryDto">Neuen Kategorie</param>
        /// <response code="201">Die Kategorie wurde erfolgreich erstellt.</response>
        /// <response code="400">Die Daten der neuen Kategorie konnten nicht verarbeitet werden!</response>
        // POST: api/Categories
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CategoryDto>> AddCategory(CategoryDto categoryDto)
        {
            Category category = new Category{ CategoryName = categoryDto.CategoryName };
            await _unitOfWork
                .Categories
                .AddAsync(category);

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            categoryDto.Id = category.Id;

            return CreatedAtAction(
                nameof(GetCategory),
                new { id = categoryDto.Id },
                categoryDto);
        }

        /// <summary>
        /// Ändert eine bestehende Kategorie
        /// </summary>
        /// <param name="id">Die Id der Kategorie</param>
        /// <param name="categoryDto">Dto für Kategorie</param>
        /// <response code="204">Die Kategorie wurde erfolgreich aktualisiert.</response>
        /// <response code="400">Die übergebenen Daten konnten nicht verarbeitet werden!</response>
        /// <response code="404">Unbekannte Id!</response>
        // PUT: api/Categories/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateCategory(int id, CategoryDto categoryDto)
        {
            var categoryInDb = await _unitOfWork.Categories.GetByIdAsync(id);
            if (categoryInDb == null)
            {
                return NotFound();
            }

            categoryInDb.CategoryName = categoryDto.CategoryName;

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }

            return NoContent();
        }

        /// <summary>
        /// Löscht eine bestehende Kategorie
        /// </summary>
        /// <param name="id">Die Id der Kategorie</param>
        /// <response code="204">Die Kategorie wurde erfolgreich gelöscht.</response>
        /// <response code="400">Die Id konnten nicht verarbeitet werden!</response>
        /// <response code="404">Unbekannte Id!</response>
        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            var categoryInDb = await _unitOfWork.Categories.GetByIdAsync(id);
            if (categoryInDb == null)
            {
                return NotFound();
            }

            _unitOfWork.Categories.Delete(categoryInDb);
            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }

            return NoContent();
        }


    }
}
