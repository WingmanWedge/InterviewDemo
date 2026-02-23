using InterviewDemo;
using Microsoft.Extensions.Logging;
using Moq;

namespace InterviewDemo.Test
{
    [TestClass]
    public class RecommendationManagerTests
    {
        private Mock<ILogger> _mockLogger = null!;
        private Mock<IMovieRepository> _mockRepo = null!;
        private RecommendationManager _manager = null!;

        // A fixed "today" to make age calculations deterministic.
        // BirthDate helpers below use this as the reference point.
        private static readonly DateTime Today = DateTime.Today;

        [TestInitialize]
        public void Init()
        {
            _mockLogger = new Mock<ILogger>();
            _mockRepo = new Mock<IMovieRepository>();
            _manager = new RecommendationManager(_mockLogger.Object, _mockRepo.Object);
        }

        // ── helpers ──────────────────────────────────────────────────────────

        private static DateTime AgeDate(int years) => Today.AddYears(-years);

        private static Movie MakeMovie(string name, string genre,
            MPAARating rating = MPAARating.G, DateTime? featureStart = null) =>
            new Movie
            {
                Name = name,
                Genre = genre,
                ReleaseDate = DateTime.Today,
                Rating = rating,
                FeatureStartDate = featureStart
            };

        private static Moviegoer MakeUser(int age, List<Movie>? history = null) =>
            new Moviegoer
            {
                Name = "Test User",
                BirthDate = AgeDate(age),
                ViewingHistory = history
            };

        // ── tests ─────────────────────────────────────────────────────────────

        /// <summary>The user parameter can be null.
        /// When this is the case we want to return an empty list.</summary>
        [TestMethod]
        public void GetRecommendations_ReturnsEmptyListIfUserIsNUll()
        {
            _mockRepo.Setup(r => r.GetActive()).Returns(new List<Movie>());

            var result = _manager.GetRecommendations(null);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        /// <summary>Every user, whether they have a viewing history or not,
        /// will always be recommended the most recent featured movie.</summary>
        [TestMethod]
        public void GetRecommendations_AlwaysReturnsTheLatestFeature()
        {
            var olderFeature = MakeMovie("Old Feature", "Drama",
                featureStart: Today.AddDays(-10));
            var latestFeature = MakeMovie("Latest Feature", "Action",
                featureStart: Today.AddDays(-1));
            var notFeatured = MakeMovie("Not Featured", "Comedy");

            _mockRepo.Setup(r => r.GetActive())
                     .Returns(new List<Movie> { olderFeature, latestFeature, notFeatured });

            // User with no viewing history – no genre match is possible
            var user = MakeUser(25, new List<Movie>());

            var result = _manager.GetRecommendations(user);

            Assert.IsTrue(result.Contains(latestFeature),
                "Expected the most recently featured movie to be included.");
            Assert.IsFalse(result.Contains(olderFeature),
                "Older featured movie should not be included via the feature rule.");
        }

        /// <summary>For the rest of the movie recommendations,
        /// we want to only include them if the user has a history of viewing that genre.</summary>
        [TestMethod]
        public void GetRecommendations_OnlyIncludesIfGenreWasVeiwed()
        {
            var actionMovie = MakeMovie("Action Movie", "Action");
            var dramaMovie  = MakeMovie("Drama Movie",  "Drama");
            var comedyMovie = MakeMovie("Comedy Movie", "Comedy");

            _mockRepo.Setup(r => r.GetActive())
                     .Returns(new List<Movie> { actionMovie, dramaMovie, comedyMovie });

            // User has only watched Action movies
            var viewedAction = MakeMovie("Old Action", "Action");
            var user = MakeUser(25, new List<Movie> { viewedAction });

            var result = _manager.GetRecommendations(user);

            Assert.IsTrue(result.Contains(actionMovie),
                "Movie in a viewed genre should be recommended.");
            Assert.IsFalse(result.Contains(dramaMovie),
                "Movie in an un-viewed genre should NOT be recommended.");
            Assert.IsFalse(result.Contains(comedyMovie),
                "Movie in an un-viewed genre should NOT be recommended.");
        }

        /// <summary>No movie should be recommended more than once.</summary>
        [TestMethod]
        public void GetRecommendations_NeverReturnsDuplicates()
        {
            // This movie is the latest feature AND matches the user's viewed genre
            var featuredAction = MakeMovie("Featured Action", "Action",
                featureStart: Today.AddDays(-1));

            _mockRepo.Setup(r => r.GetActive())
                     .Returns(new List<Movie> { featuredAction });

            var viewedAction = MakeMovie("Old Action", "Action");
            var user = MakeUser(25, new List<Movie> { viewedAction });

            var result = _manager.GetRecommendations(user);

            // Count occurrences of the featured movie
            var count = result.Count(m => m == featuredAction);
            Assert.AreEqual(1, count,
                "A movie that is both the latest feature and genre-matched should only appear once.");
        }

        /// <summary>MPAA ratings are an enumerator whose index doubles as the minimum age a moviegoer
        /// should be to be recommended that movie.</summary>
        [TestMethod]
        public void GetRecommendations_NeverRecommendsInapproriateAgeRatings()
        {
            var gMovie    = MakeMovie("G Movie",    "Action", MPAARating.G);
            var rMovie    = MakeMovie("R Movie",    "Action", MPAARating.R);    // min age 17
            var nc17Movie = MakeMovie("NC17 Movie", "Action", MPAARating.NC17); // min age 18

            _mockRepo.Setup(r => r.GetActive())
                     .Returns(new List<Movie> { gMovie, rMovie, nc17Movie });

            // User is 16 – too young for R and NC-17
            var viewedAction = MakeMovie("Old Action", "Action");
            var user = MakeUser(16, new List<Movie> { viewedAction });

            var result = _manager.GetRecommendations(user);

            Assert.IsTrue(result.Contains(gMovie),
                "G-rated movie should be recommended to a 16-year-old.");
            Assert.IsFalse(result.Contains(rMovie),
                "R-rated movie should NOT be recommended to a 16-year-old.");
            Assert.IsFalse(result.Contains(nc17Movie),
                "NC-17 movie should NOT be recommended to a 16-year-old.");
        }

        /// <summary>Errors should produce logs. Each step we perform should produce info level logs.</summary>
        [TestMethod]
        public void GetRecommendations_LogsErrorAndInfo()
        {
            _mockRepo.Setup(r => r.GetActive()).Returns(new List<Movie>());

            // 1. Null user → error log
            _manager.GetRecommendations(null);

            _mockLogger.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception?>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.AtLeastOnce,
                "Expected at least one error-level log when user is null.");

            // 2. Valid user → info logs
            var user = MakeUser(25, new List<Movie>());
            _manager.GetRecommendations(user);

            _mockLogger.Verify(
                l => l.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception?>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.AtLeastOnce,
                "Expected at least one info-level log for a valid user.");
        }
    }
}