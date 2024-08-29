using Microsoft.Extensions.Logging;

namespace InterviewDemo.Test
{
    [TestClass]
    public class RecommendationManagerTests
    {

        [TestInitialize]
        public void Init()
        {
        }

        /// <summary>The user parameter can be null.
        /// When this is the case we want to return an empty list.</summary>
        [TestMethod]
        public void GetRecommendations_ReturnsEmptyListIfUserIsNUll()
        {
            //Arrange
            Moviegoer movieGoer = null;
            ILogger logger = new TestLogger();
            IMovieRepository movieRepository = new TestMovieRepository();
            var recommendationManager = new RecommendationManager(logger, movieRepository);
            var expectedResult = new List<Movie>();


            //Act
            var result = recommendationManager.GetRecommendations(null);
            

            //Assert
            Assert.AreEqual(expectedResult.Count, result.Count);
            
        }

        /// <summary>Every user, whether they have a viewing history or not,
        /// will always be recommended the most recent featured movie.</summary>
        [TestMethod]
        public void GetRecommendations_AlwaysReturnsTheLatestFeature()
        {
            //Arrange
            ILogger logger = new TestLogger();
            var movieRepository = new TestMovieRepository();
            var recommendationManager = new RecommendationManager(logger, movieRepository);
            var movieList = new List<Movie>();
            var sample1 = new Movie()
            {
                Name = "Star Wars",
                Genre = "Sci Fi",
                ReleaseDate = DateTime.Now,
                Rating = MPAARating.PG13,
                FeatureStartDate = DateTime.Parse("2024-07-01")
            };
            var sample2 = new Movie()
            {
                Name = "Star Wars2",
                Genre = "Sci Fi",
                ReleaseDate = DateTime.Now,
                Rating = MPAARating.PG13,
                FeatureStartDate = DateTime.Parse("2024-08-01")
            };

            var movieGoer = new Moviegoer()
            {
                Name = "Ronald",
                BirthDate = DateTime.Now,
                ViewingHistory = null
            };

            movieList.Add(sample1);
            movieList.Add(sample2);


            //Act
            movieRepository.AddMovie(movieList);
            var result = recommendationManager.GetRecommendations(movieGoer);

            //Assert
            Assert.AreEqual(sample2.Name, result.First().Name);


        }

        /// <summary>For the rest of the movie recommendations,
        /// we want to only include them if the user has a history of viewing that genre.</summary>
        [TestMethod]
        public void GetRecommendations_OnlyIncludesIfGenreWasVeiwed()
        {
            Assert.Fail();
        }

        /// <summary>No movie should be recommended more than once.</summary>
        [TestMethod]
        public void GetRecommendations_NeverReturnsDuplicates()
        {
            Assert.Fail();
        }

        /// <summary>MPAA ratings are an enumerator whose index doubles as the minimum age a movigoer should be to be recommended that movie.
        /// We must assert that, give all of our other logic, that we also never return a movie for an appropriate age
        /// </summary>
        [TestMethod]
        public void GetRecommendations_NeverRecommendsInapproriateAgeRatings()
        {
            Assert.Fail();
        }

        /// <summary>errors should produce logs. Each step we perform should produce info level logs</summary>
        [TestMethod]
        public void GetRecommendations_LogsErrorAndInfo()
        {
            Assert.Fail();
        }
    }
}