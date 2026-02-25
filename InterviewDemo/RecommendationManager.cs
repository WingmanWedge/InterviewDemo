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
            if(user == null)
            {
                _logger.LogError("User is null");
                return new List<Movie>();
            }

            List<Movie> allActive;

            try
            {
                allActive = _movieRepository.GetActive();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString(),"Failed to retrieve movies");
                return new List<Movie>();
            }

            int userAge = CalculateAge(user.BirthDate);

            var recommended = new List<Movie>();

            var latestFeatured = allActive.Where(x => x.FeatureStartDate.HasValue && (int)x.Rating <= userAge)
                .OrderByDescending(x => x.FeatureStartDate!.Value).FirstOrDefault();

            if (latestFeatured != null)
            {
                recommended.Add(latestFeatured);
            }
            else {
                _logger.LogInformation("Age is notappropriate");
            }

            var viewGenre = user.ViewingHistory?.Select(x=>x.Genre).ToHashSet(StringComparer.OrdinalIgnoreCase) ?? new HashSet<string>();

            var alreadyRecommended = new HashSet<String>(recommended.Select(x => x.Name), StringComparer.OrdinalIgnoreCase);

            foreach (var item in allActive)
            {
                if (alreadyRecommended.Contains(item.Name))
                    continue;

                if((int)item.Rating > userAge)
                {
                    continue;
                }

                if (!viewGenre.Contains(item.Genre)) { 
                    continue;
                }

                recommended.Add(item);
                alreadyRecommended.Add(item.Name);

            }

            return recommended;
        }

        private static int CalculateAge(DateTime birthDate)
        {
            int age = DateTime.Today.Year - birthDate.Year;
            if (birthDate.Date > DateTime.Today.AddYears(-age)) {
                age--;
            }
            return age;
        }
    }
}
