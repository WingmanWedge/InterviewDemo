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
			var recommendations = new List<Movie>();
			var genreRecs = new List<Movie>();
			if (user is null)
			{
				return recommendations;
			}

			if (user.ViewingHistory != null)
			{
				// append by genre
				genreRecs = GetGenreRecommendations(user.ViewingHistory);
				recommendations.AddRange(genreRecs);
			}

			
			// append by Featured
			var featuredRecs = GetFeatureRecommendation();
			recommendations.Add(featuredRecs);


			var maxAllowedRating = GetMaxAllowedRating(user.BirthDate);
			
			_logger.LogInformation("Information");
			return recommendations.Where(m => m.Rating <= maxAllowedRating).DistinctBy(m => m.Name).ToList();
		}

		private MPAARating GetMaxAllowedRating(DateTime viewerBirthDate)
		{
			var yearsOld = DateTime.Today.Year - viewerBirthDate.Year;
			var ratingsYears = Enum.GetValues(typeof(MPAARating)).Cast<int>();
			
			var maxAllowedRating = ratingsYears.Last(r => r <= yearsOld);
			return (MPAARating)maxAllowedRating;
		}

		private Movie GetFeatureRecommendation()
		{
			return _movieRepository.GetActive().OrderByDescending(m => m.FeatureStartDate).First();
		}

		private List<Movie> GetGenreRecommendations(List<Movie> userViewingHistory)
		{
			var eligibleMovies = userViewingHistory.Select(m => m.Genre).Distinct().ToList();
			return _movieRepository.GetActive().Where(m => eligibleMovies.Contains(m.Genre)).DistinctBy(m => m.Name).ToList();
		}
	}
}



