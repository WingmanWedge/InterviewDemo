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
            List<Movie> movies = new List<Movie>();
            if(user == null) return movies;

            movies.Add(GetRecentFeature());
            
            if(user.ViewingHistory == null)
            {
                return movies;
            }

            movies.concat(GetMoviesBasedOnGenres(user));
            
        
        }   

       

        private List<Movie> GetMoviesBasedOnGenres(MovieGoer user){
            List<string> genres = user.ViewingHistory.Select(x=>x.Genre).Distinct();
            return this._movieRepository.GetActive().Where(x => genres.Contains(x.Genre));
        }

        private Movie GetRecentFeature(){
            return this._movieRepository.GetActive()
                .Where(x => x.FeatureStartDate != null)
                .OrderByDescending(x=>x.FeatureStartDate)
                .FirstOrDefault();
        }
            
    }
}
