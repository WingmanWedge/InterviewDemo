using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

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
                _logger.LogError("GetRecommendations called with null user");
                return [];
            }

            int age = (int)((DateTime.Today - user.BirthDate).TotalDays / 365.25);
            var activeMovies = _movieRepository.GetActive();

            _logger.LogInformation("Fetched {Count} active movies for user {User}", activeMovies.Count, user.Name);

            var latest = activeMovies
                .Where(m => m.FeatureStartDate.HasValue)
                .OrderByDescending(m => m.FeatureStartDate)
                .FirstOrDefault();

            var result = new List<Movie>();

            if (latest != null)
            {
                result.Add(latest);
                _logger.LogInformation("Added latest feature: {Movie}", latest.Name);
            }

            var genres = (user.ViewingHistory ?? []).Select(m => m.Genre).ToHashSet();

            var genreMovies = activeMovies
                .Where(m => genres.Contains(m.Genre) && !result.Contains(m) && (int)m.Rating <= age)
                .ToList();

            result.AddRange(genreMovies);

            _logger.LogInformation("Returning {Count} recommendations for user {User}", result.Count, user.Name);

            return result;
        }
    }
}
