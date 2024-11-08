using System;
namespace InterviewDemo.XUnitTest
{
	public static class DataFactory
	{
        public static List<Movie> RetrieveMoviesCatalog()
        {
            var movies = new List<Movie> {
                new Movie {
                    Name= "Jaws",
                    Genre = "Drama",
                    ReleaseDate= new DateTime(1972,1,1),
                    Rating = MPAARating.PG13,
                    //FeatureStartDate = 
                },
                new Movie {
                    Name= "Jaws 2",
                    Genre = "Drama",
                    ReleaseDate= new DateTime(1974,1,1),
                    Rating = MPAARating.PG13,
                    //FeatureStartDate = 
                },
                new Movie {
                    Name= "Squid Games",
                    Genre = "Terror",
                    ReleaseDate= new DateTime(2021,1,1),
                    Rating = MPAARating.R,
                    FeatureStartDate = new DateTime(2021,1,11),
                },
                new Movie {
                    Name= "The Lion King",
                    Genre = "Animation",
                    ReleaseDate= new DateTime(1990,1,1),
                    Rating = MPAARating.G,
                    //FeatureStartDate = new DateTime(2021,1,11),
                },
                new Movie {
                    Name= "Wish",
                    Genre = "Animation",
                    ReleaseDate= new DateTime(2023,12,1),
                    Rating = MPAARating.G,
                    //FeatureStartDate = new DateTime(2021,1,11),
                },
            };
            return movies;
        }


        public static List<User> RetrieveMovieGoers()// TODO: refactor this later retrieve
        {
            List<User> result= new List<User> {
                new User
                {
                    Id = 1,
                    Name= "Alice",
                    BirthDate = new DateTime(1990,1,1),
                    ViewingHistory = new List<Movie>()
                },
                new User
                {
                    Id = 2,
                    Name= "Bob",
                    BirthDate = new DateTime(2015,1,1),
                    ViewingHistory = new List<Movie>()
                },
                new User
                {
                    Id = 3,
                    Name= "Charly",
                    BirthDate = new DateTime(2018,1,1)
                }
            };

            return result;
        }
    }
}

