using MovieManager.Core.Contracts;
using MovieManager.Core.Entities;
using System.Linq;
using System.Collections.Generic;

namespace MovieManager.Persistence
{
    public class MovieRepository : IMovieRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public MovieRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void AddRange(Movie[] movies) => _dbContext.Movies.AddRange(movies);

        public Movie GetLongestMovie() => _dbContext.Movies
                                                .OrderByDescending(m => m.Duration)
                                                .ThenBy(m => m.Title).First();
        public int GetYearWhereMostMoviesWereReleased(string genre)
        {
            Dictionary<int, int> yearWithAmountOfMovies = new Dictionary<int, int>();

            var tmp = _dbContext
                            .Movies
                            .Where(m => m.Category.CategoryName == genre);

            foreach (var item in tmp)
            {
                if(!yearWithAmountOfMovies.ContainsKey(item.Year))
                {
                    yearWithAmountOfMovies.Add(item.Year, 1);
                }
                else
                {
                    yearWithAmountOfMovies[item.Year]++;
                }
            }

            return yearWithAmountOfMovies.OrderByDescending(_ => _.Value).Select(_ => _.Key).First();
        } 
            
    }
}