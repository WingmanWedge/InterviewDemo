using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using Moq;
using ILogger = Castle.Core.Logging.ILogger;

namespace InterviewDemo.Test
{
    [TestClass]
    public class RecommendationManagerTests
    {
        private RecommendationManager _testRecommendationManager;
        private TestMovieRepo movieRepo;

        private readonly Moviegoer _testViewer = new Moviegoer
        {
            Name = "Tony",
            BirthDate = new DateTime(1969, 2, 12),
            ViewingHistory =
            [
                new Movie
                {
                    Name = "FunnyOne",
                    FeatureStartDate = new DateTime(2024, 8, 21),
                    Rating = MPAARating.PG,
                    Genre = "Comedy",
                    ReleaseDate = new DateTime(2024, 8, 15)
                },

                new Movie
                {
                    Name = "ExcitingOne",
                    FeatureStartDate = new DateTime(2024, 8, 21),
                    Rating = MPAARating.PG,
                    Genre = "Action",
                    ReleaseDate = new DateTime(2024, 8, 15)
                },

                new Movie
                {
                    Name = "SadOne",
                    FeatureStartDate = new DateTime(2024, 8, 21),
                    Rating = MPAARating.PG,
                    Genre = "Romance",
                    ReleaseDate = new DateTime(2024, 8, 15)
                }
            ]
        };

        private Mock<ILogger<TestLogger>> mockLogger;
        private TestLogger logger;

        [TestInitialize]
        public void Init()
        {
            movieRepo = new TestMovieRepo();
            mockLogger = new Mock<ILogger<TestLogger>>();
            _testRecommendationManager = new RecommendationManager(mockLogger.Object, movieRepo);
        }

        /// <summary>The user parameter can be null.
        /// When this is the case we want to return an empty list.</summary>
        [TestMethod]
        public void GetRecommendations_ReturnsEmptyListIfUserIsNUll()
        {
            Assert.IsTrue(_testRecommendationManager.GetRecommendations(null).Count == 0);
        }

        /// <summary>Every user, whether they have a viewing history or not,
        /// will always be recommended the most recent featured movie.</summary>
        [TestMethod]
        public void GetRecommendations_AlwaysReturnsTheLatestFeature()
        {
            movieRepo.AddMovie(
                new Movie
                {
                    Name = "Alien",
                    FeatureStartDate = new DateTime(2024, 8, 22),
                    Rating = MPAARating.PG,
                    Genre = "Action",
                    ReleaseDate = new DateTime(2024, 8, 15)
                });

            var testMovieList = _testRecommendationManager.GetRecommendations(_testViewer);

            Assert.IsTrue(testMovieList.Exists(m => m.Name == "Alien"));
        }

        /// <summary>For the rest of the movie recommendations,
        /// we want to only include them if the user has a history of viewing that genre.</summary>
        [TestMethod]
        public void GetRecommendations_OnlyIncludesIfGenreWasVeiwed()
        {
            //arrange
            var viewedMovies = new List<Movie>
            {
                new Movie
                {
                    Name = "FunnyOne",
                    FeatureStartDate = new DateTime(2024, 8, 21),
                    Rating = MPAARating.PG,
                    Genre = "Comedy",
                    ReleaseDate = new DateTime(2024, 8, 15)
                },
                new Movie
                {
                    Name = "ExcitingOne",
                    FeatureStartDate = new DateTime(2024, 8, 21),
                    Rating = MPAARating.PG,
                    Genre = "Action",
                    ReleaseDate = new DateTime(2024, 8, 15)
                },
                new Movie
                {
                    Name = "SadOne",
                    FeatureStartDate = new DateTime(2024, 8, 21),
                    Rating = MPAARating.PG,
                    Genre = "Romance",
                    ReleaseDate = new DateTime(2024, 8, 15)
                }
            };

            _testViewer.ViewingHistory = viewedMovies;

            //act
            var testMovieList = _testRecommendationManager.GetRecommendations(_testViewer);

            //assert
            Assert.IsTrue(testMovieList.Any(m => m.Genre == "Comedy") || testMovieList.Any(m => m.Genre == "Action"));
        }

        /// <summary>No movie should be recommended more than once.</summary>
        [TestMethod]
        public void GetRecommendations_NeverReturnsDuplicates()
        {
            // arrange: add duplicates to the movie list that should be returned
            movieRepo.AddMany(new List<Movie>
            {
                new Movie
                {
                    Name = "Highlander",
                    Genre = "Action",
                    ReleaseDate = default,
                    Rating = MPAARating.G
                },
                new Movie
                {
                    Name = "Highlander",
                    Genre = "Action",
                    ReleaseDate = default,
                    Rating = MPAARating.G
                }
            });

            //arrange
            // select the recommendations
            var testMovieList = _testRecommendationManager.GetRecommendations(_testViewer);

            // assert that the added duplicates are only returned once
            Assert.IsTrue(testMovieList.Count(m => m.Name == "Highlander") == 1);
        }

        /// <summary>MPAA ratings are an enumerator whose index doubles as the minimum age a movigoer should be to be recommended that movie.
        /// We must assert that, give all of our other logic, that we also never return a movie for an appropriate age
        /// </summary>
        [TestMethod]
        public void GetRecommendations_NeverRecommendsInapproriateAgeRatings()
        {
            // arrange 
            // set viewer age to 12
            _testViewer.BirthDate = DateTime.Today - TimeSpan.FromDays(365 * 12);

            // add a movie inappropriate for this user
            movieRepo.AddMovie(new Movie
            {
                Name = "R-Rated Movie",
                Genre = "Action",
                ReleaseDate = default,
                Rating = MPAARating.R,
                FeatureStartDate = null
            });


            // act select movies
            var testMovieList = _testRecommendationManager.GetRecommendations(_testViewer);
            // assert: check that no PG-13, R, or NC-17 are returned
            var count = testMovieList.Count(m => m.Rating == MPAARating.R);
            Assert.IsTrue(count == 0);
        }

        /// <summary>errors should produce logs. Each step we perform should produce info level logs</summary>
        [TestMethod]
        public void GetRecommendations_LogsErrorAndInfo()
        {
            //arrange
            // pass logger to recommender enginge
            var testMovieList = _testRecommendationManager.GetRecommendations(_testViewer);


            //Assert.IsTrue(logger.);
            mockLogger.Verify(
                m => m.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Information")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}