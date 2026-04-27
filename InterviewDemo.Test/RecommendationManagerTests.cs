using Microsoft.Extensions.Logging;
using Moq;

namespace InterviewDemo.Test
{
    [TestClass]
    public class RecommendationManagerTests
    {
        private Mock<ILogger> _mockLogger = null!;
        private Mock<IMovieRepository> _mockRepo = null!;

        private Movie _featuredAction = null!;
        private Movie _featuredComedy = null!;
        private Movie _actionPG = null!;
        private Movie _dramaR = null!;
        private Movie _comedyG = null!;
        private Movie _horrorNC17 = null!;

        [TestInitialize]
        public void Init()
        {
            _mockLogger = new Mock<ILogger>();
            _mockRepo = new Mock<IMovieRepository>();

            _featuredAction = new Movie
            {
                Name = "Action Featured",
                Genre = "Action",
                ReleaseDate = new DateTime(2024, 6, 1),
                Rating = MPAARating.PG13,
                FeatureStartDate = new DateTime(2024, 12, 1)
            };

            _featuredComedy = new Movie
            {
                Name = "Comedy Featured",
                Genre = "Comedy",
                ReleaseDate = new DateTime(2024, 7, 1),
                Rating = MPAARating.PG,
                FeatureStartDate = new DateTime(2025, 1, 15)
            };

            _actionPG = new Movie
            {
                Name = "Action Regular",
                Genre = "Action",
                ReleaseDate = new DateTime(2024, 3, 1),
                Rating = MPAARating.PG
            };

            _dramaR = new Movie
            {
                Name = "Drama Mature",
                Genre = "Drama",
                ReleaseDate = new DateTime(2024, 5, 1),
                Rating = MPAARating.R
            };

            _comedyG = new Movie
            {
                Name = "Comedy Family",
                Genre = "Comedy",
                ReleaseDate = new DateTime(2024, 2, 1),
                Rating = MPAARating.G
            };

            _horrorNC17 = new Movie
            {
                Name = "Horror Extreme",
                Genre = "Horror",
                ReleaseDate = new DateTime(2024, 10, 1),
                Rating = MPAARating.NC17
            };
        }

        private RecommendationManager CreateManager()
        {
            return new RecommendationManager(_mockLogger.Object, _mockRepo.Object);
        }

        private Moviegoer CreateAdultUser(params Movie[] viewingHistory)
        {
            return new Moviegoer
            {
                Name = "Adult User",
                BirthDate = new DateTime(1990, 1, 1),
                ViewingHistory = viewingHistory.ToList()
            };
        }

        /// <summary>The user parameter can be null.
        /// When this is the case we want to return an empty list.</summary>
        [TestMethod]
        public void GetRecommendations_ReturnsEmptyListIfUserIsNUll()
        {
            _mockRepo.Setup(r => r.GetActive()).Returns(new List<Movie> { _featuredAction });
            var manager = CreateManager();

            var result = manager.GetRecommendations(null);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        /// <summary>Every user, whether they have a viewing history or not,
        /// will always be recommended the most recent featured movie.</summary>
        [TestMethod]
        public void GetRecommendations_AlwaysReturnsTheLatestFeature()
        {
            _mockRepo.Setup(r => r.GetActive())
                .Returns(new List<Movie> { _featuredAction, _featuredComedy, _actionPG });

            var userWithHistory = CreateAdultUser(_actionPG);
            var userWithoutHistory = new Moviegoer
            {
                Name = "New User",
                BirthDate = new DateTime(1990, 1, 1),
                ViewingHistory = null
            };

            var manager = CreateManager();

            var resultWithHistory = manager.GetRecommendations(userWithHistory);
            var resultWithoutHistory = manager.GetRecommendations(userWithoutHistory);

            Assert.IsTrue(resultWithHistory.Contains(_featuredComedy),
                "Latest featured movie should be included for user with history.");
            Assert.IsTrue(resultWithoutHistory.Contains(_featuredComedy),
                "Latest featured movie should be included for user without history.");

            Assert.IsFalse(resultWithoutHistory.Any(m => m == _featuredAction),
                "Older featured movie should not appear as the 'latest featured' for user with no matching genre.");
        }

        /// <summary>For the rest of the movie recommendations,
        /// we want to only include them if the user has a history of viewing that genre.</summary>
        [TestMethod]
        public void GetRecommendations_OnlyIncludesIfGenreWasVeiwed()
        {
            _mockRepo.Setup(r => r.GetActive())
                .Returns(new List<Movie> { _featuredComedy, _actionPG, _dramaR, _comedyG });

            var user = CreateAdultUser(
                new Movie { Name = "Old Action", Genre = "Action", ReleaseDate = new DateTime(2020, 1, 1), Rating = MPAARating.PG }
            );

            var manager = CreateManager();
            var result = manager.GetRecommendations(user);

            Assert.IsTrue(result.Contains(_actionPG),
                "Action movie should be included because user has watched Action genre.");
            Assert.IsFalse(result.Contains(_comedyG),
                "Comedy movie should be excluded because user has not watched Comedy genre.");
            Assert.IsFalse(result.Contains(_dramaR),
                "Drama movie should be excluded because user has not watched Drama genre.");
        }

        /// <summary>No movie should be recommended more than once.</summary>
        [TestMethod]
        public void GetRecommendations_NeverReturnsDuplicates()
        {
            _mockRepo.Setup(r => r.GetActive())
                .Returns(new List<Movie> { _featuredComedy, _comedyG, _featuredComedy });

            var user = CreateAdultUser(
                new Movie { Name = "Some Comedy", Genre = "Comedy", ReleaseDate = new DateTime(2020, 1, 1), Rating = MPAARating.G }
            );

            var manager = CreateManager();
            var result = manager.GetRecommendations(user);

            Assert.AreEqual(result.Count, result.Distinct().Count(),
                "Result should not contain duplicate movies.");
        }

        /// <summary>MPAA ratings are an enumerator whose index doubles as the minimum age a movigoer should be to be recommended that movie.
        /// We must assert that, give all of our other logic, that we also never return a movie for an appropriate age
        /// </summary>
        [TestMethod]
        public void GetRecommendations_NeverRecommendsInapproriateAgeRatings()
        {
            _mockRepo.Setup(r => r.GetActive())
                .Returns(new List<Movie> { _featuredComedy, _actionPG, _dramaR, _comedyG, _horrorNC17 });

            var youngUser = new Moviegoer
            {
                Name = "Teen User",
                BirthDate = DateTime.Today.AddYears(-14),
                ViewingHistory = new List<Movie>
                {
                    new Movie { Name = "Watched Action", Genre = "Action", ReleaseDate = new DateTime(2020, 1, 1), Rating = MPAARating.PG },
                    new Movie { Name = "Watched Drama", Genre = "Drama", ReleaseDate = new DateTime(2020, 1, 1), Rating = MPAARating.PG },
                    new Movie { Name = "Watched Horror", Genre = "Horror", ReleaseDate = new DateTime(2020, 1, 1), Rating = MPAARating.PG },
                    new Movie { Name = "Watched Comedy", Genre = "Comedy", ReleaseDate = new DateTime(2020, 1, 1), Rating = MPAARating.G }
                }
            };

            var manager = CreateManager();
            var result = manager.GetRecommendations(youngUser);

            Assert.IsTrue(result.Contains(_featuredComedy),
                "PG-rated featured movie should be included for a 14-year-old.");
            Assert.IsTrue(result.Contains(_actionPG),
                "PG-rated action movie should be included for a 14-year-old.");
            Assert.IsTrue(result.Contains(_comedyG),
                "G-rated comedy should be included for a 14-year-old.");
            Assert.IsFalse(result.Contains(_dramaR),
                "R-rated movie (min age 17) should be excluded for a 14-year-old.");
            Assert.IsFalse(result.Contains(_horrorNC17),
                "NC-17 movie (min age 18) should be excluded for a 14-year-old.");
        }

        /// <summary>errors should produce logs. Each step we perform should produce info level logs</summary>
        [TestMethod]
        public void GetRecommendations_LogsErrorAndInfo()
        {
            _mockRepo.Setup(r => r.GetActive())
                .Returns(new List<Movie> { _featuredComedy, _actionPG });

            _mockLogger.Setup(l => l.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

            var manager = CreateManager();

            manager.GetRecommendations(null);

            _mockLogger.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception?>(),
                    It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
                Times.AtLeastOnce,
                "Null user should produce an error-level log.");

            _mockLogger.Invocations.Clear();

            var user = CreateAdultUser(
                new Movie { Name = "Old Action", Genre = "Action", ReleaseDate = new DateTime(2020, 1, 1), Rating = MPAARating.PG }
            );
            manager.GetRecommendations(user);

            _mockLogger.Verify(
                l => l.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception?>(),
                    It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
                Times.AtLeastOnce,
                "Normal execution should produce info-level logs.");
        }
    }
}
