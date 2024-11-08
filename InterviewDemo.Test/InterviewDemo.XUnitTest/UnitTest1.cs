using Moq;

namespace InterviewDemo.XUnitTest;

public class UnitTest1
{
    // setup the data factory == repo

    // mock objects: models, mgr is SUT

    private readonly RecommendationManager _recommendationManager;
    private readonly IMovieRepository _movieRepo;

    //[]
    public UnitTest1()
    {
        // sut
        // logger & repo

        // setup data factories

        //this._recommendationManager = new Mock<RecommendationManager>(); //.Setup();
        //this._movieRepo = // moq
        //this._movieRepo = new Mock<IMovieRepository>().As < DataFactory.RetrieveMoviesCatalog() > ();
        
        _recommendationManager = new RecommendationManager(null, null);
    }


    //
    [Fact]
    // should get recommendations when user isnull should return empty list
    public void GetRecommendations_ReturnsEmptyListIfUserIsNull()
    {
        //Assert.Fail(); // Red /green /refactor

        // arrange
        Moviegoer user;

        // act
        user = null;
        var result = _recommendationManager.GetRecommendations(user);

        // assert
        //Assert.IsNotNull(user);
        Assert.Equal(0, result.Count);

        // refactor: improve the code to be 

    }

    /// <summary>Every user, whether they have a viewing history or not,
    /// will always be recommended the most recent featured movie.</summary>
    [Fact]
    public void GetRecommendations_AlwaysReturnsTheLatestFeature()
    {
        Assert.Fail();
    }

    /// <summary>For the rest of the movie recommendations,
    /// we want to only include them if the user has a history of viewing that genre.</summary>
    [Fact]
    public void GetRecommendations_OnlyIncludesIfGenreWasVeiwed()
    {
        Assert.Fail();
    }

    /// <summary>No movie should be recommended more than once.</summary>
    [Fact]
    public void GetRecommendations_NeverReturnsDuplicates()
    {
        Assert.Fail();
    }

    /// <summary>MPAA ratings are an enumerator whose index doubles as the minimum age a movigoer should be to be recommended that movie.
    /// We must assert that, give all of our other logic, that we also never return a movie for an appropriate age
    /// </summary>
    [Fact]
    public void GetRecommendations_NeverRecommendsInapproriateAgeRatings()
    {
        Assert.Fail();
    }

    /// <summary>errors should produce logs. Each step we perform should produce info level logs</summary>
    [Fact]
    public void GetRecommendations_LogsErrorAndInfo()
    {
        Assert.Fail();
    }
}
