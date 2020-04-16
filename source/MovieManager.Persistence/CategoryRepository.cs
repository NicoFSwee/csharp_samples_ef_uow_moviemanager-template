using MovieManager.Core.Contracts;
using MovieManager.Core.DataTransferObjects;
using MovieManager.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MovieManager.Persistence
{
    internal class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CategoryRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public (Category category, int count) GetCategoryWithMostMovies()
        {
            var tmp = _dbContext.Categories.OrderByDescending(c => c.Movies.Count()).Select(_ => new { Category = _, Count = _.Movies.Count() }).First();

            return new ValueTuple<Category, int>(tmp.Category, tmp.Count);
        }

        public IEnumerable<MovieCategoryDto> GetMovieCategoryDtos() => 
            _dbContext
            .Categories
            .Select(c => new MovieCategoryDto 
            { 
                Category = c, 
                Count = c.Movies.Count(), 
                OverallDuration = (double)c.Movies.Sum(m => m.Duration) 
            })
            .OrderBy(_ => _.Category.CategoryName);
    }
}