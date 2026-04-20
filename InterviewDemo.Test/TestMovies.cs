namespace InterviewDemo.Test
{
    public static class TestMovies
    {
        // Featured movies — FeatureStartDate set, most recent first
        public static readonly Movie LatestFeature = new()
        {
            Name = "Galactic Odyssey",
            Genre = "Sci-Fi",
            ReleaseDate = new DateTime(2025, 11, 1),
            Rating = MPAARating.PG13,
            FeatureStartDate = new DateTime(2026, 3, 15)
        };

        public static readonly Movie OlderFeature = new()
        {
            Name = "Lost in the Alps",
            Genre = "Drama",
            ReleaseDate = new DateTime(2024, 6, 10),
            Rating = MPAARating.PG,
            FeatureStartDate = new DateTime(2025, 9, 1)
        };

        public static readonly Movie OldestFeature = new()
        {
            Name = "Robo Rumble",
            Genre = "Action",
            ReleaseDate = new DateTime(2023, 3, 5),
            Rating = MPAARating.R,
            FeatureStartDate = new DateTime(2024, 1, 20)
        };

        // Non-featured movies across genres and ratings
        public static readonly Movie SciFiNonFeature = new()
        {
            Name = "Warp Factor Zero",
            Genre = "Sci-Fi",
            ReleaseDate = new DateTime(2022, 8, 14),
            Rating = MPAARating.PG
        };

        public static readonly Movie DramaNonFeature = new()
        {
            Name = "Quiet Shores",
            Genre = "Drama",
            ReleaseDate = new DateTime(2021, 4, 22),
            Rating = MPAARating.G
        };

        public static readonly Movie ActionNonFeature = new()
        {
            Name = "Iron Blitz",
            Genre = "Action",
            ReleaseDate = new DateTime(2020, 12, 1),
            Rating = MPAARating.R
        };

        public static readonly Movie AdultOnlyMovie = new()
        {
            Name = "Dark Horizons",
            Genre = "Thriller",
            ReleaseDate = new DateTime(2023, 7, 4),
            Rating = MPAARating.NC17
        };

        public static readonly Movie FamilyMovie = new()
        {
            Name = "Bunny Brigade",
            Genre = "Comedy",
            ReleaseDate = new DateTime(2024, 3, 18),
            Rating = MPAARating.G
        };

        public static List<Movie> All =>
        [
            LatestFeature, OlderFeature, OldestFeature,
            SciFiNonFeature, DramaNonFeature, ActionNonFeature,
            AdultOnlyMovie, FamilyMovie
        ];

        public static List<Movie> Featured =>
        [
            LatestFeature, OlderFeature, OldestFeature
        ];
    }

    public static class TestUsers
    {
        public static Moviegoer AdultWithHistory => new()
        {
            Name = "Alice",
            BirthDate = new DateTime(1990, 5, 12),
            ViewingHistory = [TestMovies.SciFiNonFeature, TestMovies.DramaNonFeature]
        };

        public static Moviegoer TeenWithHistory => new()
        {
            Name = "Bob",
            BirthDate = new DateTime(2011, 8, 3),
            ViewingHistory = [TestMovies.FamilyMovie]
        };

        public static Moviegoer AdultNoHistory => new()
        {
            Name = "Carol",
            BirthDate = new DateTime(1985, 1, 20),
            ViewingHistory = []
        };
    }
}
