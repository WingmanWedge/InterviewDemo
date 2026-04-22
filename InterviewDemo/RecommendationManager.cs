using InterviewDemo;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterviewDemo
{
    public class RecommendationManager
    {
        private readonly ILogger _logger;
        private readonly IMovieRepository _movieRepository;

        public RecommendationManager(ILogger logger
            , IMovieRepository movieRepository)
        {
            this._movieRepository = movieRepository;
            this._logger = logger;
        }

        public List<Movie> GetRecommendations(Moviegoer? user)
        {
            var movies = new List<Movie>();

            if (user == null)
            {
             
                _logger.LogInformation("User was null, returning empty list");
                return movies;
            }
            else
            {

            }

            return movies;
        }        
    }
}
