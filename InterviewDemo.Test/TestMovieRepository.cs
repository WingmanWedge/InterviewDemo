using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace InterviewDemo.Test
{
    public class TestMovieRepository : IMovieRepository
    {
        private List<Movie> _movieList;

        public TestMovieRepository()
        {
            _movieList = new List<Movie>();
        }

        public List<Movie> GetActive()
        {
            return _movieList;
        }

        public void AddMovie(List<Movie> movies)
        {
            _movieList.AddRange(movies);
        }

        public List<Movie> GetMoviesSample()
        {
            var movieList = new List<Movie>();
            var sample1 = new Movie()
            {
                Name = "Star Wars",
                Genre = "Sci Fi",
                ReleaseDate = DateTime.Now,
                Rating = MPAARating.PG13,
                FeatureStartDate = DateTime.Parse("2024-07-01")
            };
            var sample2 = new Movie()
            {
                Name = "Star Wars2",
                Genre = "Sci Fi",
                ReleaseDate = DateTime.Now,
                Rating = MPAARating.PG13,
                FeatureStartDate = DateTime.Parse("2024-08-01")
            };

            var movieGoer = new Moviegoer()
            {
                Name = "Ronald",
                BirthDate = DateTime.Now,
                ViewingHistory = null
            };

            movieList.Add(sample1);
            movieList.Add(sample2);

            return movieList;
        }

        public Moviegoer GetMoviegoer()
        {
            var sample3 = new Movie()
            {
                Name = "Star Wars3",
                Genre = "Sci Fi",
                ReleaseDate = DateTime.Now,
                Rating = MPAARating.PG13,
                FeatureStartDate = DateTime.Parse("2024-07-01")
            };

            var movieList = new List<Movie>();
            movieList.Add(sample3);

            var movieGoer = new Moviegoer()
            {
                Name = "Ronald",
                BirthDate = DateTime.Now,
                ViewingHistory = movieList
            };

            return movieGoer;

        }

        public Moviegoer GetNullMoviegoer()
        {
            var movieGoer = new Moviegoer()
            {
                Name = "Ronald",
                BirthDate = DateTime.Now,
                ViewingHistory = null
            };

            return movieGoer;

        }

    }
}
