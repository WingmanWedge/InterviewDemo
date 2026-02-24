using Moq;
using Microsoft.Extensions.Logging;

namespace InterviewDemo.Test
{
    [TestClass]
    public class RecommendationManagerTests
    {
        private Mock<ILogger<RecommendationManager>> _loggerMock = null;
        private Mock<IMovieRepository> _movieRepositoryMock = null;
        private RecommendationManager _recommendationManager = null;

        [TestInitialize]
        public void Init()
        {
            _loggerMock = new Mock<ILogger<RecommendationManager>>();
            _movieRepositoryMock = new Mock<IMovieRepository>();
            _recommendationManager = new RecommendationManager(_loggerMock.Object, _movieRepositoryMock.Object);
        }

        /// <summary>The user parameter can be null.
        /// When this is the case we want to return an empty list.</summary>
        [TestMethod]
        public void GetRecommendations_ReturnsEmptyListIfUserIsNUll()
        {
            Moviegoer? user = null;

            var result = _recommendationManager.GetRecommendations(user);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        /// <summary>Every user, whether they have a viewing history or not,
        /// will always be recommended the most recent featured movie.</summary>
        [TestMethod]
        public void GetRecommendations_AlwaysReturnsTheLatestFeature()
        {
            Moviegoer? user = new Moviegoer
            { BirthDate = DateTime.Now.AddYears(-30), Name = "Test User", ViewingHistory = null };

            var movies = new List<Movie>
            {
                new Movie { Name = "Old Featured Movie", Genre = "Action", ReleaseDate = DateTime.Now.AddYears(-1), Rating = MPAARating.PG, FeatureStartDate = DateTime.Now.AddMonths(-2) },
                new Movie { Name = "New Featured Movie", Genre = "Comedy", ReleaseDate = DateTime.Now.AddMonths(-6), Rating = MPAARating.PG13, FeatureStartDate = DateTime.Now.AddMonths(-1) },
                new Movie { Name = "Non-Featured Movie", Genre = "Drama", ReleaseDate = DateTime.Now.AddYears(-2), Rating = MPAARating.R }
            };

            _movieRepositoryMock.Setup(repo => repo.GetActive()).Returns(movies);
            var result = _recommendationManager.GetRecommendations(user);

            Assert.IsTrue(result.Any(m => m.Name == "New Featured Movie"));
        }

        /// <summary>For the rest of the movie recommendations,
        /// we want to only include them if the user has a history of viewing that genre.</summary>
        [TestMethod]
        public void GetRecommendations_OnlyIncludesIfGenreWasVeiwed()
        {
            var user = new Moviegoer
            { BirthDate = DateTime.Now.AddYears(-30), Name = "Test User", ViewingHistory = new List<Movie>
                {
                    new Movie { Name = "Action Movie 1", Genre = "Action", ReleaseDate = DateTime.Now.AddYears(-3), Rating = MPAARating.PG },
                    new Movie { Name = "Action Movie 2", Genre = "Action", ReleaseDate = DateTime.Now.AddYears(-2), Rating = MPAARating.PG13 },
                    new Movie { Name = "Comedy Movie 1", Genre = "Comedy", ReleaseDate = DateTime.Now.AddYears(-1), Rating = MPAARating.R }
                }
            };

            var movies = new List<Movie>
            {
                new Movie { Name = "Action Movie 3", Genre = "Action", ReleaseDate = DateTime.Now.AddMonths(-6), Rating = MPAARating.PG },
                new Movie { Name = "Comedy Movie 2", Genre = "Comedy", ReleaseDate = DateTime.Now.AddMonths(-5), Rating = MPAARating.PG13 },
                new Movie { Name = "Drama Movie 1", Genre = "Drama", ReleaseDate = DateTime.Now.AddMonths(-4), Rating = MPAARating.R }
            };

            _movieRepositoryMock.Setup(repo => repo.GetActive()).Returns(movies);
            var result = _recommendationManager.GetRecommendations(user);
    
            Assert.IsTrue(result.Any(m => m.Name == "Action Movie 3"));
            Assert.IsTrue(result.Any(m => m.Name == "Comedy Movie 2"));
            Assert.IsFalse(result.Any(m => m.Name == "Drama Movie 1"));
        }

        /// <summary>No movie should be recommended more than once.</summary>
        [TestMethod]
        public void GetRecommendations_NeverReturnsDuplicates()
        {
            var user = new Moviegoer
            { BirthDate = DateTime.Now.AddYears(-30), Name = "Test User", ViewingHistory = new List<Movie>
                {
                    new Movie { Name = "Action Movie 1", Genre = "Action", ReleaseDate = DateTime.Now.AddYears(-3), Rating = MPAARating.PG },
                    new Movie { Name = "Action Movie 2", Genre = "Action", ReleaseDate = DateTime.Now.AddYears(-2), Rating = MPAARating.PG13 },
                    new Movie { Name = "Comedy Movie 1", Genre = "Comedy", ReleaseDate = DateTime.Now.AddYears(-1), Rating = MPAARating.R }
                }
            };
            var movies = new List<Movie>
            {
                new Movie { Name = "Action Movie 1", Genre = "Action", ReleaseDate = DateTime.Now.AddYears(-3), Rating = MPAARating.PG },
                new Movie { Name = "Action Movie 2", Genre = "Action", ReleaseDate = DateTime.Now.AddYears(-2), Rating = MPAARating.PG13, FeatureStartDate = DateTime.Now.AddDays(-1) },
                new Movie { Name = "Comedy Movie 1", Genre = "Comedy", ReleaseDate = DateTime.Now.AddYears(-1), Rating = MPAARating.R }
            };

            _movieRepositoryMock.Setup(repo => repo.GetActive()).Returns(movies);

            var result = _recommendationManager.GetRecommendations(user);

            var duplicateMovies = result.GroupBy(m => m.Name).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
            Assert.IsTrue(duplicateMovies.Count == 0, $"The following movies were recommended more than once: {string.Join(", ", duplicateMovies)}");
            //Assert.IsTrue(result.Count(m => m.Name == "Action Movie 2") == 1, "Action Movie 2 was recommended more than once.");
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