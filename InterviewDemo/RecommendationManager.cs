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


			return recommendations;
		}

		private Movie GetFeatureRecommendation()
		{
			return _movieRepository.GetActive().OrderByDescending(m => m.FeatureStartDate).First();
		}

		private List<Movie> GetGenreRecommendations(List<Movie> userViewingHistory)
		{
			var eligibleMovies = userViewingHistory.Select(m => m.Genre).Distinct().ToList();
			return _movieRepository.GetActive().Where(m => eligibleMovies.Contains(m.Genre)).Distinct().ToList();
		}
	}
}



