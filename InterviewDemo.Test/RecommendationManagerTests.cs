using Microsoft.Extensions.Logging;

namespace InterviewDemo.Test
{
    public class FakeMovieRepo : IMovieRepository
    {
        public List<Movie> Movies
        {
            get; set;
        } = new();
        public List<Movie> GetActive() => Movies;
    }

    public class FakeLogger : ILogger
    {
        public List<(LogLevel Level, string Message)> Logs { get; } = new();

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            => Logs.Add((logLevel, formatter(state, exception)));

        public bool IsEnabled(LogLevel logLevel) => true;

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
    }

    [TestClass]
    public class RecommendationManagerTests
    {

        private FakeMovieRepo _repo = null;
        private FakeLogger _logger = null;
        private RecommendationManager _sut = null;

        [TestInitialize]
        public void Init()
        {
            _repo = new FakeMovieRepo();
            _logger = new FakeLogger();
            _sut = new RecommendationManager(_logger, _repo);
        }

        /// <summary>The user parameter can be null.
        /// When this is the case we want to return an empty list.</summary>
        [TestMethod]
        public void GetRecommendations_ReturnsEmptyListIfUserIsNUll()
        {
            var result = _sut.GetRecommendations(null);
            Assert.AreEqual(result.Count, 0);
        }

        /// <summary>Every user, whether they have a viewing history or not,
        /// will always be recommended the most recent featured movie.</summary>
        [TestMethod]
        public void GetRecommendations_AlwaysReturnsTheLatestFeature()
        {

            // setup
            _repo.Movies = TestMovies.All;
            var user = TestUsers.AdultWithHistory;

            //action
            var result = _sut.GetRecommendations(user);

            Assert.AreEqual(TestMovies.LatestFeature, result[0]);
        }

        // NEW
        /// <summary>A user with no viewing history should receive only the latest featured movie,
        /// since genre-based recommendations require prior viewing history.</summary>
        [TestMethod]
        public void GetRecommendations_ReturnsOnlyLatestFeature_WhenUserHasNoHistory()
        {
            _repo.Movies = TestMovies.All;
            var user = TestUsers.AdultNoHistory;

            var result = _sut.GetRecommendations(user);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(TestMovies.LatestFeature, result[0]);
        }

        // NEW
        /// <summary>When the repo contains no featured movies, the result should be empty
        /// since there is no latest feature to anchor the list.</summary>
        [TestMethod]
        public void GetRecommendations_ReturnsEmpty_WhenNoFeaturedMovies()
        {
            _repo.Movies = [TestMovies.SciFiNonFeature, TestMovies.DramaNonFeature];
            var user = TestUsers.AdultNoHistory;

            var result = _sut.GetRecommendations(user);

            Assert.AreEqual(0, result.Count);
        }

        /// <summary>For the rest of the movie recommendations,
        /// we want to only include them if the user has a history of viewing that genre.</summary>
        [TestMethod]
        public void GetRecommendations_OnlyIncludesIfGenreWasVeiwed()
        {
            _repo.Movies = TestMovies.All;
            var user = TestUsers.AdultWithHistory;
            var genres = user.ViewingHistory.Select(m => m.Genre).ToHashSet();

            var result = _sut.GetRecommendations(user);

            Assert.IsTrue(result.Skip(1).All(m => genres.Contains(m.Genre)));
        }

        /// <summary>No movie should be recommended more than once.</summary>
        [TestMethod]
        public void GetRecommendations_NeverReturnsDuplicates()
        {
            // AdultWithHistory viewed Sci-Fi, which is also LatestFeature's genre — prime duplicate candidate
            _repo.Movies = TestMovies.All;
            var user = TestUsers.AdultWithHistory;

            var result = _sut.GetRecommendations(user);

            Assert.AreEqual(result.Distinct().Count(), result.Count);
        }

        /// <summary>MPAA ratings are an enumerator whose index doubles as the minimum age a movigoer should be to be recommended that movie.
        /// We must assert that, give all of our other logic, that we also never return a movie for an appropriate age
        /// </summary>
        [TestMethod]
        public void GetRecommendations_NeverRecommendsInapproriateAgeRatings()
        {
            // TeenWithHistory (born 2011-08-03) is ~14 — old enough for PG13 but not R or NC17
            _repo.Movies = TestMovies.All;
            var user = TestUsers.TeenWithHistory;
            int age = (int)((DateTime.Today - user.BirthDate).TotalDays / 365.25);

            var result = _sut.GetRecommendations(user);

            Assert.IsTrue(result.All(m => (int)m.Rating <= age));
        }

        // NEW
        /// <summary>Age-appropriate genre movies must appear in the result, confirming the age filter
        /// excludes only ineligible movies rather than filtering out everything.</summary>
        [TestMethod]
        public void GetRecommendations_IncludesAgeAppropriateGenreMovies()
        {
            // TeenWithHistory viewed Comedy (FamilyMovie, rated G) — should appear in results
            _repo.Movies = TestMovies.All;
            var user = TestUsers.TeenWithHistory;

            var result = _sut.GetRecommendations(user);

            Assert.IsTrue(result.Contains(TestMovies.FamilyMovie));
        }

        // NEW
        /// <summary>Each step of a successful recommendation run should produce at least one info-level log.</summary>
        [TestMethod]
        public void GetRecommendations_LogsInfo_OnSuccess()
        {
            _repo.Movies = TestMovies.All;
            var user = TestUsers.AdultWithHistory;

            _sut.GetRecommendations(user);

            Assert.IsTrue(_logger.Logs.Any(l => l.Level == LogLevel.Information));
        }

        // NEW
        /// <summary>When a null user is passed, an error-level log should be produced
        /// so that callers can diagnose unexpected null references in production.</summary>
        [TestMethod]
        public void GetRecommendations_LogsError_WhenUserIsNull()
        {
            _sut.GetRecommendations(null);

            Assert.IsTrue(_logger.Logs.Any(l => l.Level == LogLevel.Error));
        }
    }
}
