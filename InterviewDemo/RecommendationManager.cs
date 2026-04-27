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
                _logger.LogError("User parameter was null; returning empty recommendations.");
                return new List<Movie>();
            }

            _logger.LogInformation("Getting recommendations for user {UserName}.", user.Name);

            var activeMovies = _movieRepository.GetActive();
            _logger.LogInformation("Retrieved {Count} active movies.", activeMovies.Count);

            var results = new List<Movie>();

            var latestFeatured = activeMovies
                .Where(m => m.FeatureStartDate.HasValue)
                .OrderByDescending(m => m.FeatureStartDate)
                .FirstOrDefault();

            int userAge = DateTime.Today.Year - user.BirthDate.Year;
            if (user.BirthDate.Date > DateTime.Today.AddYears(-userAge))
                userAge--;

            _logger.LogInformation("User age calculated as {Age}.", userAge);

            if (latestFeatured != null && userAge >= (int)latestFeatured.Rating)
            {
                results.Add(latestFeatured);
                _logger.LogInformation("Added latest featured movie: {MovieName}.", latestFeatured.Name);
            }

            var viewedGenres = user.ViewingHistory?
                .Select(m => m.Genre)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToHashSet(StringComparer.OrdinalIgnoreCase)
                ?? new HashSet<string>();

            _logger.LogInformation("User has viewed genres: {Genres}.", string.Join(", ", viewedGenres));

            var genreMatches = activeMovies
                .Where(m => viewedGenres.Contains(m.Genre))
                .Where(m => userAge >= (int)m.Rating)
                .Where(m => !results.Contains(m));

            results.AddRange(genreMatches);
            _logger.LogInformation("Returning {Count} total recommendations.", results.Count);

            return results.Distinct().ToList();
        }
    }
}
