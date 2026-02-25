using Microsoft.Extensions.Logging;
using InterviewDemo;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace InterviewDemoNUnitTest
{
    [TestFixture]       
    public class Tests
    {

        private Mock<IMovieRepository> _movieRepositoryMock = null;
        private ILogger _logger = null;



        [SetUp]
        public void Setup()
        {
            _movieRepositoryMock = new Mock<IMovieRepository>();
            
        }

        /// <summary>The user parameter can be null.
        /// When this is the case we want to return an empty list.</summary>
        [Test]
        public void GetRecommendations_ReturnsEmptyListIfUserIsNUll()
        {
            var manager = new RecommendationManager(NullLogger.Instance, _movieRepositoryMock.Object);

            var result = manager.GetRecommendations(null!);

            Assert.That(result, Is.Empty);
            Assert.That(result, Is.Not.Null);
        }

        /// <summary>Every user, whether they have a viewing history or not,
        /// will always be recommended the most recent featured movie.</summary>
        [Test]
        public void GetRecommendations_AlwaysReturnsTheLatestFeature()
        {
            var latestFeature = new Movie
            {
                Name = "latest feature",
                Genre = "Comedy",
                ReleaseDate = new DateTime(2025, 1, 1),
                Rating = MPAARating.PG13
                ,
                FeatureStartDate = new DateTime(2025, 6, 1)
            };

            var oldFeature = new Movie
            {
                Name = "old feature",
                Genre = "Action",
                ReleaseDate = new DateTime(2024, 1, 1),
                Rating = MPAARating.PG13
                ,
                FeatureStartDate = new DateTime(2024, 6, 1)
            };

            _movieRepositoryMock.Setup(x=>x.GetActive()).Returns(new List<Movie> { latestFeature,oldFeature });

            var user = new Moviegoer { Name = "Test user 1", BirthDate = DateTime.Today.AddYears(-20), ViewingHistory = null };

            var manager = new RecommendationManager(NullLogger.Instance, _movieRepositoryMock.Object);
            var result = manager.GetRecommendations(user);

            Assert.That(result.Any(x => x.Name == "latest feature"), Is.True, "Latest movie recommend");
            Assert.That(result.First(), Is.SameAs(latestFeature), "latest feature should be first");
        }

        /// <summary>For the rest of the movie recommendations,
        /// we want to only include them if the user has a history of viewing that genre.</summary>
        [Test]
        public void GetRecommendations_OnlyIncludesIfGenreWasVeiwed()
        {
            var actionMovie = new Movie { Name = "Action 1", Genre = "Action", ReleaseDate = new DateTime(2024, 1, 1), Rating = MPAARating.PG13, FeatureStartDate = new DateTime(2024,6,1) };
            var comedyMovie = new Movie { Name = "Comedy 1", Genre = "Comedy", ReleaseDate = new DateTime(2024, 2, 1), Rating = MPAARating.PG13, FeatureStartDate = new DateTime(2024, 6, 1) };

            _movieRepositoryMock.Setup(x => x.GetActive()).Returns(new List<Movie> { actionMovie, comedyMovie });

            var user = new Moviegoer { 
                Name = "Test user 1",
                BirthDate = DateTime.Today.AddYears(-25)
                , ViewingHistory = new List<Movie> { new Movie { Name = "Action 2", Genre = "Action", ReleaseDate = DateTime.MinValue, 
                    Rating = MPAARating.PG13 }
                } };

            var manager = new RecommendationManager(NullLogger.Instance, _movieRepositoryMock.Object);
            var result = manager.GetRecommendations(user);

            Assert.That(result.Any(x => x.Genre == "Action"), Is.True, "recommend Action");
            Assert.That(result.Any(x => x.Genre == "Comedy"), Is.False, "don't recommend comedy");
        }

        /// <summary>No movie should be recommended more than once.</summary>
        [Test]
        public void GetRecommendations_NeverReturnsDuplicates()
        {
            var actionMovie = new Movie { Name = "Action 1", Genre = "Action", ReleaseDate = new DateTime(2024, 1, 1),
                Rating = MPAARating.PG13, FeatureStartDate = new DateTime(2024, 6, 1) };

            _movieRepositoryMock.Setup(x => x.GetActive()).Returns(new List<Movie> { actionMovie });

            var user = new Moviegoer
            {
                Name = "Test user 1",
                BirthDate = DateTime.Today.AddYears(-25),
                ViewingHistory = new List<Movie> {  actionMovie}
            };

            var manager = new RecommendationManager(NullLogger.Instance, _movieRepositoryMock.Object);
            var result = manager.GetRecommendations(user);

            var count = result.Count(x=>x.Name == "Action 1");
            Assert.That(count, Is.EqualTo(1), "Recommend only once");

        }

        /// <summary>MPAA ratings are an enumerator whose index doubles as the minimum age a movigoer should be to be recommended that movie.
        /// We must assert that, give all of our other logic, that we also never return a movie for an appropriate age
        /// </summary>
        [Test]
        public void GetRecommendations_NeverRecommendsInapproriateAgeRatings()
        {
            var actionMovie = new Movie { Name = "R Action 1", Genre = "Action", ReleaseDate = new DateTime(2024, 1, 1), Rating = MPAARating.R, FeatureStartDate = new DateTime(2024, 6, 1) };
            var comedyMovie = new Movie { Name = "PG Comedy 1", Genre = "Comedy", ReleaseDate = new DateTime(2024, 2, 1), Rating = MPAARating.PG, FeatureStartDate = new DateTime(2024, 6, 1) };

            _movieRepositoryMock.Setup(x => x.GetActive()).Returns(new List<Movie> { actionMovie, comedyMovie });

            var user = new Moviegoer
            {
                Name = "Test user 1",
                BirthDate = DateTime.Today.AddYears(-10),
                ViewingHistory = new List<Movie> { actionMovie, comedyMovie }
            };

            var manager = new RecommendationManager(NullLogger.Instance, _movieRepositoryMock.Object);
            var result = manager.GetRecommendations(user);

            Assert.That(result.Any(x=>x.Rating == MPAARating.R), Is.False, "Should not recommend");
            Assert.That(result.Any(x => x.Rating == MPAARating.PG), Is.True, "Recommend");
        }

        /// <summary>errors should produce logs. Each step we perform should produce info level logs</summary>
        [Test]
        public void GetRecommendations_LogsErrorAndInfo()
        {
            var _logger = new TestingLogger();
            _movieRepositoryMock.Setup(x => x.GetActive()).Returns(new List<Movie>());
            var user = new Moviegoer
            {
                Name = "Test user 1",
                BirthDate = DateTime.Today.AddYears(-10),
                ViewingHistory = new List<Movie> ()
            };

            var manager = new RecommendationManager(_logger, _movieRepositoryMock.Object);

            Assert.DoesNotThrow(() => manager.GetRecommendations(user), "not throw");

            _movieRepositoryMock.Setup(x => x.GetActive()).Throws(new InvalidOperationException("repo error"));
            var result = manager.GetRecommendations(user);

            Assert.That(result, Is.Not.Null, "result not be null");
            Assert.That(result, Is.Empty, "result should empty");
            Assert.IsTrue(_logger.ErrorMessages.Count > 0, "Expected at least one error");
        }
    }

    public class TestingLogger : ILogger
    {
        public List<string> InfoMessages { get; set; } = new List<string>();
        public List<string> ErrorMessages { get; set; } = new List<string>();
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            throw new NotImplementedException();
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if(logLevel == LogLevel.Information)
                InfoMessages.Add(eventId.ToString());
            if(logLevel == LogLevel.Error)
                ErrorMessages.Add(eventId.ToString());

        }
    }


}