
using Microsoft.Extensions.Logging;
using Moq;

namespace InterviewDemo.Test
{
    [TestClass]
    public class RecommendationManagerTests
    {
        Mock<ILogger> mockLogger = new Mock<ILogger>();
        Mock<IMovieRepository> mockMovieRepository = new Mock<IMovieRepository>();
        [TestInitialize]
        public void Init()
        {


        }

        /// <summary>The user parameter can be null.
        /// When this is the case we want to return an empty list.</summary>
        [TestMethod]
        public void GetRecommendations_ReturnsEmptyListIfUserIsNUll()
        {
            // Arrange

            var recommendationManager = new RecommendationManager(mockLogger.Object, mockMovieRepository.Object);

            // Act
            List<Movie> result = recommendationManager.GetRecommendations(null);

            // Assert
            Assert.AreEqual(result.Count, 0);
           
        }

        /// <summary>Every user, whether they have a viewing history or not,
        /// will always be recommended the most recent featured movie.</summary>
        [TestMethod]
        public void GetRecommendations_AlwaysReturnsTheLatestFeature()
        {
            var movies = new List<Movie>
        {
            new() { Genre="comedy", Name="Robert", Rating=MPAARating.G, FeatureStartDate = DateTime.Now.AddDays(1), ReleaseDate=DateTime.Now.AddDays(1) },
            new() { Genre="comedy", Name="AAA", Rating=MPAARating.G, FeatureStartDate = DateTime.Now.AddDays(1), ReleaseDate=DateTime.Now },
            new() {Genre = "comedy", Name = "Inter", Rating = MPAARating.G, FeatureStartDate = DateTime.Now.AddDays(2), ReleaseDate=DateTime.Now.AddDays(1)}
        };

            mockMovieRepository.Setup(repo => repo.GetActive()).Returns(movies);

            var recommendationManager = new RecommendationManager(mockLogger.Object, mockMovieRepository.Object);

            // Act
            var result = recommendationManager.GetRecommendations(new Moviegoer { BirthDate = DateTime.Now, Name = "Somone" });

            // Assert
            var topDate = DateTime.Now.AddDays(1).Date;
            var expectedMovies = movies.Where(movie => movie.FeatureStartDate.GetValueOrDefault().Date == topDate.Date).ToList();

            Assert.AreEqual(expectedMovies.Count, result.Count);
            foreach (var expectedMovie in expectedMovies)
            {
                Assert.IsTrue(result.Any(movie => movie.Name == expectedMovie.Name), $"Expected movie with name {expectedMovie.Name} not found.");
            }
        }

        public void GetRecommendations_OnlyFeatureMovies_When_ViewingHistory_Empty()
        {
            Assert.Fail();
        }

        public void GetRecommendations_ExcludeViewingHistoryMovies_When_ViewingHistory_ContainsData()
        {
            Assert.Fail();
        }

        /// <summary>For the rest of the movie recommendations,
        /// we want to only include them if the user has a history of viewing that genre.</summary>
        [TestMethod]
        public void GetRecommendations_OnlyIncludesIfGenreWasVeiwed()
        {
            var movies = new List<Movie>
        {
            new() { Genre="comedy", Name="Robert", Rating=MPAARating.G, FeatureStartDate = DateTime.Now.AddDays(-5), ReleaseDate=DateTime.Now.AddDays(1) },
            new() { Genre="comedy", Name="AAA", Rating=MPAARating.G, FeatureStartDate = DateTime.Now.AddDays(-10), ReleaseDate=DateTime.Now },
            new() {Genre = "documentation", Name = "Inter", Rating = MPAARating.G, FeatureStartDate = DateTime.Now.AddDays(2), ReleaseDate=DateTime.Now.AddDays(1)},
        new() {Genre = "documentation", Name = "xMan", Rating = MPAARating.G, FeatureStartDate = DateTime.Now.AddDays(-100), ReleaseDate=DateTime.Now.AddDays(1)}

            };

            var movieGoer = new Moviegoer
            {
                BirthDate = DateTime.Now,
                Name = "Somone",
                ViewingHistory = new List<Movie> {
                    new() {Genre="comedy", Name="Welcome", Rating=MPAARating.PG, ReleaseDate=DateTime.Now}
                }
            };

            mockMovieRepository.Setup(repo => repo.GetActive()).Returns(movies);

            var recommendationManager = new RecommendationManager(mockLogger.Object, mockMovieRepository.Object);

            // Act
            var result = recommendationManager.GetRecommendations(movieGoer);

            Assert.AreEqual(result.Count, 3);
            var expectedMovies = movies.Where(x => x.Name != "xMan").ToList();

            foreach (var expectedMovie in expectedMovies)
            {
                Assert.IsTrue(result.Any(movie => movie.Name == expectedMovie.Name), $"Expected movie with name {expectedMovie.Name} not found.");
            }
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