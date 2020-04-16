using MovieManager.Core.Entities;
using System;
using System.Linq;
using System.Collections.Generic;
using Utils;

namespace MovieManager.Core
{
    public class ImportController
    {
        const string Filename = "movies.csv";
        const int TITLE_IDX = 0;
        const int YEAR_IDX = 1;
        const int CATEGORYNAME_IDX = 2;
        const int DURATION_IDX = 3;

        /// <summary>
        /// Liefert die Movies mit den dazugehörigen Kategorien
        /// </summary>
        public static IEnumerable<Movie> ReadFromCsv()
        {
            string[][] data = MyFile.ReadStringMatrixFromCsv(Filename, true);
            Dictionary<string, Category> categories = new Dictionary<string, Category>();
            List<Movie> movies = new List<Movie>();

            categories = data.GroupBy(c => c[CATEGORYNAME_IDX])
                            .Select(c => new Category 
                            { 
                                CategoryName = c.Key, 
                                Movies = new List<Movie>() 
                            })
                            .ToDictionary(_ => _.CategoryName);

            movies = data
                        .Select(m => new Movie
                        {
                            Title = m[TITLE_IDX],
                            Category = categories[m[CATEGORYNAME_IDX]],
                            Duration = int.Parse(m[DURATION_IDX]),
                            Year = int.Parse(m[YEAR_IDX])
                        }).ToList();

            foreach(var m in movies)
            {
                categories[m.Category.CategoryName].Movies.Add(m);
            }

            return movies;
        }

    }
}
