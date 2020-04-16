using MovieManager.Core.Entities;
using MovieManager.Core.DataTransferObjects;
using System.Collections.Generic;
using System.Linq;

namespace MovieManager.Core.Contracts
{
    public interface ICategoryRepository
    {
        (Category category, int count) GetCategoryWithMostMovies();
        IEnumerable<MovieCategoryDto> GetMovieCategoryDtos();
    }
}
