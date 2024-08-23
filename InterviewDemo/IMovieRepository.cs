using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterviewDemo
{
    public interface IMovieRepository
    {
        /// <summary>get all active movies from the repository</summary>
        /// <returns>a list of movies that may be featured or not.</returns>
        List<Movie> GetActive();  
    }
}
