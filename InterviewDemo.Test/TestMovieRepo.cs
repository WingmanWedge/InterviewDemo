namespace InterviewDemo.Test;

public class TestMovieRepo : IMovieRepository
{
	private readonly List<Movie> _movies = new();
	public TestMovieRepo()
	{
		_movies.Add(
			new Movie{
				Name="Alien Romulus", 
				FeatureStartDate = new DateTime(2024, 8, 15),
				Rating = MPAARating.PG,
				Genre = "Horror",
				ReleaseDate = new DateTime(2024,8,15)

			});
		_movies.Add(new Movie
		{
			Name = "FunnyRepoOne",
			FeatureStartDate = new DateTime(2024, 8, 21),
			Rating = MPAARating.PG,
			Genre = "Comedy",
			ReleaseDate = new DateTime(2024, 8, 15)
		});
		_movies.Add(new Movie
		{
			Name = "ExcitingRepoOne",
			FeatureStartDate = new DateTime(2024, 8, 21),
			Rating = MPAARating.PG,
			Genre = "Action",
			ReleaseDate = new DateTime(2024, 8, 15)
		});
		_movies.Add(new Movie
		{
			Name = "Trampy",
			FeatureStartDate = new DateTime(2024, 8, 21),
			Rating = MPAARating.PG,
			Genre = "Silent",
			ReleaseDate = new DateTime(2024, 8, 15)
		});
		_movies.Add(new Movie
		{
			Name = "Trampy",
			FeatureStartDate = new DateTime(2024, 8, 21),
			Rating = MPAARating.PG,
			Genre = "Silent",
			ReleaseDate = new DateTime(2024, 8, 15)
		});
	}
	public List<Movie> GetActive()
	{
		return _movies;
	}

	public void AddMovie(Movie movieToAdd)
	{
		_movies.Add(movieToAdd);
	}

	public void AddMany(List<Movie> moviesToAdd)
	{
		_movies.AddRange(moviesToAdd);
	}
}