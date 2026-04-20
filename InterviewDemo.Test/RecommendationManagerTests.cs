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

        /// <summary>For the rest of the movie recommendations,
        /// we want to only include them if the user has a history of viewing that genre.</summary>
        [TestMethod]
        public void GetRecommendations_OnlyIncludesIfGenreWasVeiwed()
        {
            _repo.Movies = TestMovies.All;
            var user = TestUsers.AdultWithHistory;
            var genres = user.ViewingHistory.Select(m => m.Genre).ToHashSet();

            var result = _sut.GetRecommendations(user);

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
