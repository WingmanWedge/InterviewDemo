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
        private IMovieRepository _movieRepository;


        public RecommendationManager(ILogger logger
            , IMovieRepository movieRepository)
        {
            this._movieRepository = movieRepository;
            this._logger = logger;
        }

        public List<Movie> GetRecommendations(Moviegoer? user)
        {
            // Get list of movies - ALL 100 movies
            // Check FeatureStartDate in future. Top 1 list of movies
            // Plus
            // user.ViewingHistory, user.Birthday
            // 20 movies (10 of comedy, 10 of action) viewing history
            // 50 Filter only the Genre that user viewed.
            // 30 movies that is to be recommeded.
            // G+PG (ALL) + Age Appropriate
            // Combine two lists and remove duplicates out from final list.
            if(user == null)
            {
                return [];
            }
            // Get the list of movies from the repository
            var movies = _movieRepository.GetActive();

            // Check FeatureStartDate on the Movie object and pick all recent movies releasing
            // Find the most recent FeatureStartDate
            var topDate = movies
                .Where(movie => movie.FeatureStartDate >= DateTime.Now)
                .OrderBy(movie => movie.FeatureStartDate)
                .Select(movie => movie.FeatureStartDate)
                .FirstOrDefault();

            // Get all movies releasing on the top date
            var topDateMovies = movies
                .Where(movie => movie.FeatureStartDate!.GetValueOrDefault().Date == topDate!.GetValueOrDefault().Date)
                .ToList();

            if (user.ViewingHistory == null || user.ViewingHistory.Count == 0)
                return topDateMovies;

            var viewedGenres = user.ViewingHistory!.Select(movie => movie.Genre).Distinct();
            var genreFilteredMovies = movies
                .Where(movie => viewedGenres.Contains(movie.Genre))
                .ToList();

            // Combine the lists
            var combinedMovies = topDateMovies.Concat(genreFilteredMovies).Distinct().ToList();

            return combinedMovies;
        }        
    }
}



