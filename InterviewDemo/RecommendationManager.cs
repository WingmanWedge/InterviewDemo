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
                _logger.LogInformation("User is null. Returning empty recommendation list.");
                return [];
            }

            _logger.LogInformation("Getting recommendations for user {UserName}", user.Name);

            var activeMovies = _movieRepository.GetActive();
            var recommendations = new List<Movie>();
            
            var latestFeatured = activeMovies.Where(m => m.FeatureStartDate.HasValue).OrderByDescending(m => m.ReleaseDate).FirstOrDefault();

            if (latestFeatured != null)
            {
                if (user.Age >= ((int)latestFeatured.Rating))
                {
                    _logger.LogInformation("Adding latest featured movie {MovieName} to recommendations.", latestFeatured.Name);
                    recommendations.Add(latestFeatured);
                }
                else
                    _logger.LogInformation("Latest featured movie {MovieName} is not suitable for user age {UserAge}.", latestFeatured.Name, user.Age);
            }

            var viewedGenres = user.ViewingHistory?.Select(m => m.Genre).Distinct() ?? [];

            _logger.LogInformation("User has viewed genres: {Genres}", string.Join(", ", viewedGenres));

            var genreRecommendations = activeMovies.Where(m => !recommendations.Contains(m)).Where(m => viewedGenres.Contains(m.Genre)).ToList();

            recommendations.AddRange(genreRecommendations);
            _logger.LogInformation("Returning {Count} genre-based recommendations.", recommendations.Count);

            return recommendations;
        }        
    }
}
