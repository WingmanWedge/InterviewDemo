using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.Core.Arguments;

namespace InterviewDemo.Test
{
    [TestClass]
    public class RecommendationManagerTests
    {
        IMovieRepository _movieRepository;
        ILogger _logger;
        Movie _featuredFilm = new Movie { Genre = "Comedy", Name = "Whatever", Rating = MPAARating.R, ReleaseDate = DateTime.Now, FeatureStartDate = DateTime.Now };
        Movie _comedyMovie = new Movie { Genre = "Comedy", Name = "Friends", Rating = MPAARating.G, ReleaseDate = DateTime.Parse("09/01/1986"), FeatureStartDate = DateTime.Parse("09/01/1986") };
        Movie _viewedMovie = new Movie { Genre = "Action", Name = "Default Man", Rating = MPAARating.G, ReleaseDate = DateTime.Parse("09/01/2056"), FeatureStartDate = DateTime.Parse("09/01/2015") };
        RecommendationManager _manager;
        List<Movie> _fullList = [new Movie { Genre="Action", Name ="Terminator", Rating=MPAARating.R, ReleaseDate=DateTime.Parse("09/01/1956"), FeatureStartDate=DateTime.Parse("09/01/1956") },
                                 new Movie { Genre="Action", Name ="Default Man", Rating=MPAARating.G, ReleaseDate=DateTime.Parse("09/01/2056"), FeatureStartDate=DateTime.Parse("09/01/2015") },
                                 new Movie { Genre="Romnance", Name ="Oops", Rating=MPAARating.R, ReleaseDate=DateTime.Parse("09/01/2014") },
                                 new Movie { Genre="Superhero", Name ="Gold Man", Rating=MPAARating.PG13, ReleaseDate=DateTime.Parse("09/01/2016") },
                                 new Movie { Genre="Action", Name ="Terminator 2", Rating=MPAARating.R, ReleaseDate=DateTime.Parse("09/01/2015") }];
        Moviegoer _actionUser = new Moviegoer { BirthDate = DateTime.Parse("8/20/2001"), Name = "Sam", ViewingHistory = new List<Movie>() };
        Moviegoer _comedyUser = new Moviegoer { BirthDate = DateTime.Parse("8/20/2001"), Name = "Tam", ViewingHistory = new List<Movie>() };
        [TestInitialize]
        public void Init()
        {
            _actionUser.ViewingHistory?.Add(_viewedMovie);
            _comedyUser.ViewingHistory?.Add(_comedyMovie);
            _fullList.Add(_featuredFilm);
            _movieRepository = Substitute.For<IMovieRepository>();
            _movieRepository.GetActive().Returns(_fullList);
            _logger = Substitute.For<ILogger>();
            _manager = new RecommendationManager(_logger, _movieRepository);
        }

        /// <summary>The user parameter can be null.
        /// When this is the case we want to return an empty list.</summary>
        [TestMethod]
        public void GetRecommendations_ReturnsEmptyListIfUserIsNUll()
        {
            List<Movie> movieResults = _manager.GetRecommendations(null);
            Assert.IsNotNull(movieResults);
            Assert.AreEqual(movieResults.Count, 0);
        }

        /// <summary>Every user, whether they have a viewing history or not,
        /// will always be recommended the most recent featured movie.</summary>
        [TestMethod]
        public void GetRecommendations_AlwaysReturnsTheLatestFeature()
        {
            List<Movie> movieResults = _manager.GetRecommendations(new Moviegoer { BirthDate = DateTime.Now, Name = "John Doe" });
            Assert.IsNotNull(movieResults);
            Assert.AreEqual(movieResults[0],_featuredFilm);
        }

        /// <summary>For the rest of the movie recommendations,
        /// we want to only include them if the user has a history of viewing that genre.</summary>
        [TestMethod]
        public void GetRecommendations_OnlyIncludesIfGenreWasVeiwed()
        {
            List<Movie> movieResults = _manager.GetRecommendations(_actionUser);
            Assert.IsNotNull(movieResults);
            Movie? isViewed = movieResults.FirstOrDefault(movie => movie.Name == _viewedMovie.Name);
            Assert.IsNotNull(isViewed);
        }

        /// <summary>No movie should be recommended more than once.</summary>
        [TestMethod]
        public void GetRecommendations_NeverReturnsDuplicates()
        {
            List<Movie> movieResults = _manager.GetRecommendations(_comedyUser);
            Assert.IsNotNull(movieResults);
            Assert.AreEqual(movieResults.Count, movieResults.Distinct().ToList().Count);
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