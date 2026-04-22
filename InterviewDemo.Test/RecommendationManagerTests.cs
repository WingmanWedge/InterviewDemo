using Microsoft.Extensions.Logging;
using NSubstitute;
using InterviewDemo;

namespace InterviewDemo.Test
{
    [TestClass]
    public class RecommendationManagerTests
    {

        [TestInitialize]
        public void Init()
        {
            _logger = Substitute.For<ILogger>();
            _movieRepository = Substitute.For<IMovieRepository>();
        }

        private ILogger _logger;
        private IMovieRepository _movieRepository;

        /// <summary>The user parameter can be null.
        /// When this is the case we want to return an empty list.</summary>
        [TestMethod]
        public void GetRecommendations_ReturnsEmptyListIfUserIsNUll()
        {
            //arrange
            var stuRec = new RecommendationManager(_logger, _movieRepository);

            //act
            Assert.AreEqual(0, stuRec.GetRecommendations(null).Count);            

        }

        /// <summary>Every user, whether they have a viewing history or not,
        /// will always be recommended the most recent featured movie.</summary>
        [TestMethod]
        public void GetRecommendations_AlwaysReturnsTheLatestFeature()
        {
            Assert.Fail();
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