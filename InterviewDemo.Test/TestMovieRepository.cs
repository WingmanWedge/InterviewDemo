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
    }
}
