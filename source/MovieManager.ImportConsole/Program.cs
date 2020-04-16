using MovieManager.Core;
using MovieManager.Core.Entities;
using MovieManager.Persistence;
using System;
using System.Linq;

namespace MovieManager.ImportConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            InitData();
            AnalyzeData();

            Console.WriteLine();
            Console.Write("Beenden mit Eingabetaste ...");
            Console.ReadLine();
        }

        private static void InitData()
        {
            Console.WriteLine("***************************");
            Console.WriteLine("          Import");
            Console.WriteLine("***************************");

            Console.WriteLine("Import der Movies und Categories in die Datenbank");
            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                Console.WriteLine("Datenbank löschen");
                unitOfWork.DeleteDatabase();

                Console.WriteLine("Datenbank migrieren");
                unitOfWork.MigrateDatabase();

                Console.WriteLine("Movies/Categories werden eingelesen");

                var movies = ImportController.ReadFromCsv().ToArray();
                if (movies.Length == 0)
                {
                    Console.WriteLine("!!! Es wurden keine Movies eingelesen");
                    return;
                }

                var categories = movies.GroupBy(m => m.Category);

                Console.WriteLine($"  Es wurden {movies.Count()} Movies in {categories.Count()} Kategorien eingelesen!");

                unitOfWork.MovieRepository.AddRange(movies);

                unitOfWork.Save();

                Console.WriteLine();
            }
        }

        private static void AnalyzeData()
        {
            Console.WriteLine("***************************");
            Console.WriteLine("        Statistik");
            Console.WriteLine("***************************");

            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                // Längster Film: Bei mehreren gleichlangen Filmen, soll jener angezeigt werden, dessen Titel im Alphabet am weitesten vorne steht.
                // Die Dauer des längsten Films soll in Stunden und Minuten angezeigt werden!
                //TODO
                var longestMovie = unitOfWork.MovieRepository.GetLongestMovie();
                Console.WriteLine($"Längster Film: {longestMovie.Title}; Länge: {longestMovie.Duration}");
                Console.WriteLine();

                // Top Kategorie:
                //   - Jene Kategorie mit den meisten Filmen.
                //TODO
                var categoryWithMostMovies = unitOfWork.CategoryRepository.GetCategoryWithMostMovies();
                Console.WriteLine($"Kategorie mit den meisten Filmen: '{categoryWithMostMovies.category.CategoryName}'; Filme: {categoryWithMostMovies.count}");
                Console.WriteLine();

                // Jahr der Kategorie "Action":
                //  - In welchem Jahr wurden die meisten Action-Filme veröffentlicht?
                //TODO
                int yearWhereMostActionMoviesWereReleased = unitOfWork.MovieRepository.GetYearWhereMostMoviesWereReleased("Action");
                Console.WriteLine($"Jahr der Action-Filme: {yearWhereMostActionMoviesWereReleased}");
                Console.WriteLine();

                // Kategorie Auswertung (Teil 1):
                //   - Eine Liste in der je Kategorie die Anzahl der Filme und deren Gesamtdauer dargestellt wird.
                //   - Sortiert nach dem Namen der Kategorie (aufsteigend).
                //   - Die Gesamtdauer soll in Stunden und Minuten angezeigt werden!
                //TODO
                Console.WriteLine();
                var movieCategorieDtos = unitOfWork.CategoryRepository.GetMovieCategoryDtos();

                Console.WriteLine("{0, -13}{1, -8}{2, -10}", "Kategorie", "Anzahl", "Gesamtdauer");
                Console.WriteLine("================================");
                foreach(var dto in movieCategorieDtos)
                {
                    Console.WriteLine($"{dto.Category.CategoryName, -13}{dto.Count, -8}{GetDurationAsString(dto.OverallDuration), -10}");
                }

                // Kategorie Auswertung (Teil 2):
                //   - Alle Kategorien und die durchschnittliche Dauer der Filme der Kategorie
                //   - Absteigend sortiert nach der durchschnittlichen Dauer der Filme.
                //     Bei gleicher Dauer dann nach dem Namen der Kategorie aufsteigend sortieren.
                //   - Die Gesamtdauer soll in Stunden, Minuten und Sekunden angezeigt werden!
                //TODO
                movieCategorieDtos = movieCategorieDtos.OrderByDescending(_ => _.OverallDuration / _.Count);
                Console.WriteLine();

                Console.WriteLine("{0, -15}{1, -20}", "Kategorie", "durchschn. Gesamtdauer");
                Console.WriteLine("=====================================");
                foreach (var dto in movieCategorieDtos)
                {
                    Console.WriteLine($"{dto.Category.CategoryName, -15}{GetAverageDurationAsString(dto.OverallDuration, dto.Count), -20}");
                }
            }

        }

        private static string GetDurationAsString(double minutes, bool withSeconds = true)
        {
            int hours = (int)minutes / 60;
            int restMinutes = (int)minutes - hours * 60;

            return $"{hours:D2} h {restMinutes:D2} min";
        }

        private static string GetAverageDurationAsString(double minutes, int movieCount)
        {
            double avgTime = minutes * 60 / movieCount;
            int avgHours = (int)avgTime / 3600;
            int avgMinutes = ((int)avgTime - avgHours * 3600) / 60;
            int avgSeconds = ((int)avgTime - avgHours * 3600) - avgMinutes * 60;

            return $"{avgHours:D2} h {avgMinutes:D2} min {avgSeconds:D2} sec";
        }
    }
}
