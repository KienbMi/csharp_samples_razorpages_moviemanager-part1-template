using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieManager.Core.Contracts;
using MovieManager.Core.Entities;
using MovieManager.Persistence;

namespace MovieManager.Tests
{
    [TestClass]
    public class UnitOfWorkValidationTests
    {
        private static IUnitOfWork GetInMemoryUnitOfWork()
        {
            // Build the ApplicationDbContext 
            //  - with InMemory-DB
            var dbContext = new ApplicationDbContext(
                new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase(typeof(UnitOfWorkValidationTests).ToString())
                    .EnableSensitiveDataLogging()
                    .Options);
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
            return new UnitOfWork(dbContext);
        }

        [TestMethod]
        public async Task Movie_AddSameTitleTwiceInTwoYears_ShouldSaveCorrectly()
        {
            Category category = new Category { CategoryName = "Drama" };
            var movieA = new Movie { Category = category, Duration = 50, Year = 2019, Title = "Title" };
            var movieB = new Movie { Category = category, Duration = 50, Year = 2018, Title = "Title" };
            await using IUnitOfWork unitOfWork = GetInMemoryUnitOfWork();
            await unitOfWork.Movies.AddAsync(movieA);
            await unitOfWork.Movies.AddAsync(movieB);
            int count = await unitOfWork.SaveChangesAsync();
            Assert.AreEqual(-1, count);  // 2 movies 1 category
        }

        [TestMethod]
        public async Task Movie_AddSameTitleTwiceInSameYear_ShouldThrowException()
        {
            Category category = new Category { CategoryName = "Drama" };
            var movieA = new Movie { Category = category, Duration = 50, Year = 2019, Title = "Title" };
            var movieB = new Movie { Category = category, Duration = 50, Year = 2019, Title = "Title" };
            await using IUnitOfWork unitOfWork = GetInMemoryUnitOfWork();
            await unitOfWork.Movies.AddAsync(movieA);
            int count = await unitOfWork.SaveChangesAsync();
            Assert.AreEqual(2, count);

            await unitOfWork.Movies.AddAsync(movieB);
            try
            {
                await unitOfWork.SaveChangesAsync();
                Assert.Fail("Should throw exception");
            }
            catch (ValidationException ex)
            {
                Assert.AreEqual("Movie Title existiert bereits im Jahr 2019!", ex.ValidationResult.ErrorMessage);
                Assert.AreEqual(2, ex.ValidationResult.MemberNames.Count());
            }
        }

        [TestMethod]
        public async Task Movie_ManyErrors_ShouldAggregationOfErrors()
        {
            var movie = new Movie
            {
                CategoryId = 0, 
                Duration = 200, 
                Year = 1899, 
                Title = "X"
            };
            await using IUnitOfWork unitOfWork = GetInMemoryUnitOfWork();
            await unitOfWork.Movies.AddAsync(movie);
            try
            {
                await unitOfWork.SaveChangesAsync();
                Assert.Fail("Should throw exception");
            }
            catch (ValidationException validationException)
            {
                Assert.AreEqual("Entity validation failed for Title, CategoryId, Duration, Year",
                    validationException.Message);
                var aggregateException = validationException.InnerException as AggregateException;
                var messages = aggregateException
                    ?.InnerExceptions
                    .Select(ie => ie.Message)
                    .OrderBy(m=>m)
                    .ToArray();
                Assert.IsNotNull(messages);
                Assert.AreEqual(4,messages.Length);
                Assert.AreEqual("CategoryId must be greater than zero", messages[0]);
                Assert.AreEqual("Classical Movies (until year '1950') may not last longer than 60 minutes!", messages[1]);
                Assert.AreEqual("The field Year must be between 1900 and 2099.", messages[2]);
                Assert.AreEqual("The length of Title must be between 2 and 100.", messages[3]);
            }
        }


    }
}
