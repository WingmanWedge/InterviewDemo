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
            var result = new List<Movie>();
            if (user == null) return result;


            //adding latest feature
            result = _movieRepository.GetActive();
            var latest = result.OrderByDescending(m => m.FeatureStartDate).FirstOrDefault();
            result.Add(latest);

            //get all types of movies
            var genres = user.ViewingHistory.Select(m => m.Genre).ToHashSet();

            //filter out all list
            var genreMovies = _movieRepository.GetActive().Where(m => genres.Contains(m.Genre)).ToList<Movie>();
            result.AddRange(genreMovies);


            return result;
        }
    }
}
