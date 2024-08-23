using System.Reflection.Metadata;
using System.Runtime.InteropServices;

namespace InterviewDemo.Test
{
	[TestClass]
	public class RecommendationManagerTests
	{
		private RecommendationManager sut;
		private TestMovieRepo movieRepo;
		private Moviegoer viewer = new Moviegoer
		{
			Name = "Tony",
			BirthDate = new DateTime(1969, 2, 12)
		};
		[TestInitialize]
		public void Init()
		{
			movieRepo = new TestMovieRepo();
			var logger = new TestLogger();

			sut = new RecommendationManager(logger, movieRepo);
		}

		/// <summary>The user parameter can be null.
		/// When this is the case we want to return an empty list.</summary>
		[TestMethod]
		public void GetRecommendations_ReturnsEmptyListIfUserIsNUll()
		{

			Assert.IsTrue(sut.GetRecommendations(null).Count == 0);
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

			var testMovieList = sut.GetRecommendations(viewer);


			Assert.IsTrue(testMovieList.Exists(m => m.Name == "Alien"));
		}

		/// <summary>For the rest of the movie recommendations,
		/// we want to only include them if the user has a history of viewing that genre.</summary>
		[TestMethod]
		public void GetRecommendations_OnlyIncludesIfGenreWasVeiwed()
		{
			//arrange
			var viewedMovies = new List<Movie>();
			viewedMovies.Add(new Movie
			{
				Name = "FunnyOne",
				FeatureStartDate = new DateTime(2024, 8, 21),
				Rating = MPAARating.PG,
				Genre = "Comedy",
				ReleaseDate = new DateTime(2024, 8, 15)
			});
			viewedMovies.Add(new Movie
			{
				Name = "ExcitingOne",
				FeatureStartDate = new DateTime(2024, 8, 21),
				Rating = MPAARating.PG,
				Genre = "Action",
				ReleaseDate = new DateTime(2024, 8, 15)
			});
			viewedMovies.Add(new Movie
			{
				Name = "SadOne",
				FeatureStartDate = new DateTime(2024, 8, 21),
				Rating = MPAARating.PG,
				Genre = "Romance",
				ReleaseDate = new DateTime(2024, 8, 15)
			});

			viewer.ViewingHistory = viewedMovies;

			//act
			var testMovieList = sut.GetRecommendations(viewer);

			//assert
			Assert.IsTrue(testMovieList.Any(m=>m.Genre=="Comedy") || testMovieList.Any(m => m.Genre == "Action"));
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