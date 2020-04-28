using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieManager.Core.Contracts;
using MovieManager.Core.Entities;
using MovieManager.Persistence;
using MovieManager.WebApi.DataTransferObjects;

namespace MovieManager.WebApi.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public MoviesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/Movies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovieDto>>> GetMovies()
        {
            var movies = await _unitOfWork.Movies.GetAllAsync();
            return movies
                .Select(movie => new MovieDto(movie))
                .ToArray();
        }

        // GET: api/Movies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MovieDto>> GetMovie(int id)
        {
            var movie = await _unitOfWork.Movies.GetByIdAsync(id);

            if (movie == null)
            {
                return NotFound();
            }

            return new MovieDto(movie);
        }

        // PUT: api/Movies/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMovie(int id, MovieDto movieDto)
        {
            if (id != movieDto.Id)
            {
                return BadRequest();
            }
            var dbMovie = await _unitOfWork.Movies.GetByIdAsync(id);
            if (dbMovie == null)
            {
                return NotFound();
            }
            dbMovie.Title = movieDto.Title;
            dbMovie.Duration = movieDto.Duration;
            dbMovie.Year = movieDto.Year;
            dbMovie.CategoryId = movieDto.CategoryId;
            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return NoContent();
        }

        // POST: api/Movies
        /// <response code="201">Movie wurde erfolgreich erstellt.</response>
        /// <response code="400">Movie konnte nicht erstellt werden!</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Movie>> PostMovie(MovieDto movieDto)
        {
            Movie movie = movieDto.ToEntity();
            await _unitOfWork
                .Movies
                .AddAsync(movie);
            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            movieDto.Id = movie.Id;

            return CreatedAtAction(
                nameof(GetMovie),
                new { id = movieDto.Id },
                movieDto);
        }

        // DELETE: api/Movies/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Movie>> DeleteMovie(int id)
        {
            var movie = await _unitOfWork.Movies.GetByIdAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            _unitOfWork.Movies.Delete(movie);
            await _unitOfWork.SaveChangesAsync();

            return movie;
        }

    }
}
