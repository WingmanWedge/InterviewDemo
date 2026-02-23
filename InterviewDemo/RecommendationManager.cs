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
            if (user == null)
            {
                _logger.LogError("GetRecommendations called with null user.");
                return new List<Movie>();
            }

            _logger.LogInformation("Getting recommendations");

            var allMovies = _movieRepository.GetActive();
            _logger.LogInformation("Retrieved {Count} active movies from repository.", allMovies.Count);

            var recommendations = new List<Movie>();

            // Always include the most recently featured movie
            var latestFeature = allMovies
                .Where(m => m.FeatureStartDate.HasValue)
                .OrderByDescending(m => m.FeatureStartDate)
                .FirstOrDefault();

            if (latestFeature != null)
            {
                _logger.LogInformation("Adding latest featured movie: {Name}.", latestFeature.Name);
                recommendations.Add(latestFeature);
            }

            // Determine genres the user has viewed
            var viewedGenres = user.ViewingHistory?
                .Select(m => m.Genre)
                .ToHashSet() ?? new HashSet<string>();

            _logger.LogInformation("User has viewed {Count} distinct genres.", viewedGenres.Count);

            // Calculate user age
            var today = DateTime.Today;
            var age = today.Year - user.BirthDate.Year;
            if (user.BirthDate.Date > today.AddYears(-age)) age--;

            // Add non-duplicate movies matching viewed genres and appropriate age rating
            foreach (var movie in allMovies)
            {
                if (recommendations.Contains(movie))
                    continue;

                if (!viewedGenres.Contains(movie.Genre))
                    continue;

                if ((int)movie.Rating > age)
                {
                    _logger.LogInformation("Skipping movie {Name} due to age restriction (rating {Rating}, user age {Age}).",
                        movie.Name, movie.Rating, age);
                    continue;
                }

                _logger.LogInformation("Adding recommended movie: {Name}.", movie.Name);
                recommendations.Add(movie);
            }

            return recommendations;
        }        
    }
}
