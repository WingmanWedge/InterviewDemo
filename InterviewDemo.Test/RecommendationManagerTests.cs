using Microsoft.Extensions.Logging;

namespace InterviewDemo.Test
{
    [TestClass]
    public class RecommendationManagerTests
    {
        ILogger _logger;
        TestMovieRepository _movieRepository;
        RecommendationManager _manager;

        [TestInitialize]
        public void Init()
        {
            _logger = new TestLogger();
            _movieRepository = new TestMovieRepository();
            _manager = new RecommendationManager( _logger, _movieRepository);

        }

        /// <summary>The user parameter can be null.
        /// When this is the case we want to return an empty list.</summary>
        [TestMethod]
        public void GetRecommendations_ReturnsEmptyListIfUserIsNUll()
        {
            //Arrange
            Moviegoer movieGoer = null;
            var expectedResult = new List<Movie>();

            //Act
            var result = _manager.GetRecommendations(null);
            
            //Assert
            Assert.AreEqual(expectedResult.Count, result.Count);
            
        }

        /// <summary>Every user, whether they have a viewing history or not,
        /// will always be recommended the most recent featured movie.</summary>
        [TestMethod]
        public void GetRecommendations_AlwaysReturnsTheLatestFeature()
        {
            //Arrange
            var movieList = _movieRepository.GetMoviesSample();
            var movieGoer = _movieRepository.GetMoviegoer();

            //Act
            _movieRepository.AddMovie(movieList);
            var result = _manager.GetRecommendations(movieGoer);

            //Assert
            Assert.AreEqual(movieList.Select(x => x.Name = "Star Wars2").First(), result.First().Name);

        }

        /// <summary>For the rest of the movie recommendations,
        /// we want to only include them if the user has a history of viewing that genre.</summary>
        [TestMethod]
        public void GetRecommendations_OnlyIncludesIfGenreWasVeiwed()
        {
            //Arrange
            var movieList = _movieRepository.GetMoviesSample();
            var movieGoer = _movieRepository.GetNullMoviegoer();
            var expectedResult = new List<Movie>();

            //Act
            _movieRepository.AddMovie(movieList);
            var result = _manager.GetRecommendations(movieGoer);

            //Assert
            Assert.AreEqual(expectedResult.Count, result.Count);
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