using InterviewDemo;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
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
            List<Movie> data = _movieRepository.GetActive();
           List<Movie> result = new List<Movie>();
            if (user == null)
            {
                return result;
            }
            else
            {
               result.Add(data.Where(movie => movie.FeatureStartDate.HasValue).OrderByDescending(movie => movie.FeatureStartDate).First());
                List<string> genres = user.ViewingHistory.Select(movie => movie.Genre).ToList();
                result.AddRange(data.Where(movie => genres.Contains(movie.Genre) && !result.Contains(movie)));
               return result;
            }
        }        
    }
}



