using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Cadlix_backend.DataAccess.Context;
using Cadlix_backend.Domain.Entities.Categories;
using Cadlix_backend.Domain.Entities.History;
using Cadlix_backend.Domain.Entities.Leaderboard;
using Cadlix_backend.Domain.Entities.ListsOfUserFilms;
using Cadlix_backend.Domain.Entities.Movie;
using Cadlix_backend.Domain.Entities.Subscription;
using Cadlix_backend.Domain.Entities.User;
using Cadlix_backend.Domain.Enum;
using Microsoft.EntityFrameworkCore;

var connectionString = args.Length > 0
    ? args[0]
    : "Server=localhost;Database=Cadlix;User Id=sa;Password=MySecret@Password;TrustServerCertificate=True;";

Cadlix_backend.DataAccess.DbSession.ConnectionString = connectionString;

using var context = new AppDbContext();

Console.WriteLine("Connected to database. Checking existing data...");

if (await context.Users.CountAsync() > 1)
{
    Console.WriteLine("Seed data already exists (more than 1 user). Skipping.");
    return;
}

Console.WriteLine("Seeding data...");

// ──────────────────────────────────────────────
// Helper: SHA256 hash (same as existing admin)
// ──────────────────────────────────────────────
string HashPassword(string plain)
{
    var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(plain));
    return Convert.ToBase64String(bytes);
}

// ──────────────────────────────────────────────
// 1. USERS (20 users, IDs 2-21)
// ──────────────────────────────────────────────
var passwordHash = HashPassword("password123");
var userNames = new[]
{
    "MovieFan42", "StreamQueen", "FlickPicker", "CinephileJoe", "SeriesBinge",
    "PopcornLover", "FilmBuff99", "NightOwlViewer", "ReelDealer", "DramaKing",
    "ActionJunkie", "ComedyGold", "SciFiGeek", "HorrorFanatic", "DocuWatcher",
    "AnimeLover", "ThrillerSeeker", "ClassicFilmNerd", "IndieFilmFan", "MarathonWatcher"
};

var users = new List<UserData>();
for (int i = 0; i < userNames.Length; i++)
{
    var joined = DateTime.UtcNow.AddDays(-Random.Shared.Next(30, 730));
    var daysOnSite = (int)(DateTime.UtcNow - joined).TotalDays;
    var titlesWatched = Random.Shared.Next(5, 200);
    var reviewCount = Random.Shared.Next(0, 30);
    var hoursWatched = Random.Shared.Next(10, 500);

    users.Add(new UserData
    {
        Name = userNames[i],
        ExternalId = Guid.NewGuid().ToString("N")[..12],
        Avatar = $"https://api.dicebear.com/8.x/avataaars/svg?seed={userNames[i]}",
        Group = "User",
        Plan = ((SubscriptionPlan)Random.Shared.Next(0, 3)).ToString(),
        Status = "Active",
        JoinedAt = joined,
        TitlesWatched = titlesWatched,
        ReviewCount = reviewCount,
        Comments = Random.Shared.Next(0, 100),
        LikesGiven = Random.Shared.Next(0, 300),
        LikesReceived = Random.Shared.Next(0, 200),
        HoursWatched = hoursWatched,
        AddedToList = Random.Shared.Next(0, 50),
        DaysOnSite = daysOnSite,
        Password = passwordHash,
        Email = $"{userNames[i].ToLowerInvariant()}@example.com",
        Level = URole.User
    });
}

context.Users.AddRange(users);
await context.SaveChangesAsync();
Console.WriteLine($"  ✓ {users.Count} users seeded");

// ──────────────────────────────────────────────
// 2. MOVIES (50 movies)
// ──────────────────────────────────────────────
var movieData = new[]
{
    new { Title = "The Last Horizon", Type = "Movie", Year = 2024, Director = (string?)"James Cameron", Cast = "Leonardo DiCaprio, Emma Stone, Idris Elba", Duration = "2h 28m", DurationSeconds = 8880, Score = 85, Description = "A crew of astronauts embarks on humanity's first interstellar voyage to a distant earth-like planet." },
    new { Title = "Midnight in Paris", Type = "Movie", Year = 2023, Director = "Sofia Coppola", Cast = "Timothée Chalamet, Saoirse Ronan", Duration = "1h 52m", DurationSeconds = 6720, Score = 78, Description = "A struggling writer discovers a magical portal that transports him to 1920s Paris each night." },
    new { Title = "Steel Thunder", Type = "Movie", Year = 2025, Director = "Christopher Nolan", Cast = "Tom Hardy, Cillian Murphy, Zendaya", Duration = "2h 15m", DurationSeconds = 8100, Score = 91, Description = "An elite military unit must stop a rogue AI from launching nuclear missiles." },
    new { Title = "Laugh Factory", Type = "Movie", Year = 2024, Director = "Judd Apatow", Cast = "Kevin Hart, Tiffany Haddish, Seth Rogen", Duration = "1h 45m", DurationSeconds = 6300, Score = 72, Description = "A behind-the-scenes look at the chaotic world of stand-up comedy." },
    new { Title = "Crimson Rivers", Type = "Movie", Year = 2023, Director = "Denis Villeneuve", Cast = "Jake Gyllenhaal, Amy Adams", Duration = "2h 10m", DurationSeconds = 7800, Score = 88, Description = "A detective investigates a series of ritualistic murders in a remote mountain village." },
    new { Title = "Starlight Express", Type = "Movie", Year = 2025, Director = "Taika Waititi", Cast = "Chris Pratt, Scarlett Johansson", Duration = "1h 58m", DurationSeconds = 7080, Score = 75, Description = "In a galaxy far away, a ragtag crew must deliver a mysterious cargo across enemy lines." },
    new { Title = "The Haunting of Blackwood Manor", Type = "Movie", Year = 2024, Director = "Mike Flanagan", Cast = "Florence Pugh, Oliver Jackson-Cohen", Duration = "1h 55m", DurationSeconds = 6900, Score = 81, Description = "A family moves into a Victorian manor only to discover its dark supernatural secrets." },
    new { Title = "Love in Bloom", Type = "Movie", Year = 2024, Director = "Greta Gerwig", Cast = "Emma Watson, Ryan Gosling", Duration = "1h 48m", DurationSeconds = 6480, Score = 79, Description = "Two florists competing for the same contract unexpectedly fall in love." },
    new { Title = "Neon Knights", Type = "Movie", Year = 2025, Director = "Ridley Scott", Cast = "Ryan Reynolds, Ana de Armas", Duration = "2h 05m", DurationSeconds = 7500, Score = 83, Description = "In a cyberpunk dystopia, a hacker uncovers a conspiracy that threatens the entire city." },
    new { Title = "Wild Frontier", Type = "Movie", Year = 2023, Director = "Quentin Tarantino", Cast = "Brad Pitt, Margot Robbie", Duration = "2h 45m", DurationSeconds = 9900, Score = 87, Description = "A gunslinger seeks revenge across the American West in this epic adventure." },
    new { Title = "The Silent Patient", Type = "Movie", Year = 2024, Director = "David Fincher", Cast = "Rosamund Pike, Ben Affleck", Duration = "1h 50m", DurationSeconds = 6600, Score = 84, Description = "A psychotherapist becomes obsessed with a famous painter who refuses to speak after murdering her husband." },
    new { Title = "Ocean's Eight", Type = "Movie", Year = 2025, Director = "Steven Soderbergh", Cast = "Cate Blanchett, Sandra Bullock, Anne Hathaway", Duration = "2h 00m", DurationSeconds = 7200, Score = 76, Description = "Eight skilled women plan the most daring heist in casino history." },
    new { Title = "Shadow Protocol", Type = "Movie", Year = 2024, Director = "Kathryn Bigelow", Cast = "John Boyega, Lupita Nyong'o", Duration = "2h 20m", DurationSeconds = 8400, Score = 80, Description = "A covert operative must expose a government conspiracy before it's too late." },
    new { Title = "Ember Falls", Type = "Movie", Year = 2023, Director = "Guillermo del Toro", Cast = "Sally Hawkins, Doug Jones", Duration = "1h 56m", DurationSeconds = 6960, Score = 89, Description = "In a world where fire is extinct, a young girl discovers the last ember of flame." },
    new { Title = "The Perfect Heist", Type = "Movie", Year = 2025, Director = "Michael Mann", Cast = "Denzel Washington, Jason Statham", Duration = "2h 10m", DurationSeconds = 7800, Score = 77, Description = "A master thief plans one last job to retire from a life of crime." },
    new { Title = "Celestial Gardens", Type = "Movie", Year = 2024, Director = "Hayao Miyazaki", Cast = "voiced by Masaki Suda, Hana Sugisaki", Duration = "2h 04m", DurationSeconds = 7440, Score = 93, Description = "A beautifully animated tale of a young botanist who discovers floating sky gardens." },
    new { Title = "True Detective: Season 5", Type = "Series", Year = 2025, Director = "Issa López", Cast = "Jodie Foster, Kali Reis", Duration = "55m", DurationSeconds = 3300, Score = 86, Description = "The latest season of the acclaimed crime anthology series." },
    new { Title = "Stranger Worlds", Type = "Series", Year = 2024, Director = "The Duffer Brothers", Cast = "Millie Bobby Brown, Finn Wolfhard", Duration = "50m", DurationSeconds = 3000, Score = 82, Description = "Kids in a small town uncover a secret government experiment that opens a gateway to another dimension." },
    new { Title = "The Crown: New Era", Type = "Series", Year = 2025, Director = "Peter Morgan", Cast = "Imelda Staunton, Jonathan Pryce", Duration = "58m", DurationSeconds = 3480, Score = 88, Description = "The royal family navigates the challenges of the modern world." },
    new { Title = "Cyber Edge", Type = "Series", Year = 2024, Director = "Vince Gilligan", Cast = "Aaron Paul, Krysten Ritter", Duration = "48m", DurationSeconds = 2880, Score = 85, Description = "A former hacker is pulled back into the world of cybercrime." },
    new { Title = "Ancient Empires", Type = "Documentary", Year = 2024, Director = (string?)null, Cast = "Narrated by Morgan Freeman", Duration = "45m", DurationSeconds = 2700, Score = 90, Description = "Documentary series exploring the rise and fall of ancient civilizations." },
    new { Title = "Planet Earth III", Type = "Documentary", Year = 2024, Director = (string?)null, Cast = "Narrated by David Attenborough", Duration = "50m", DurationSeconds = 3000, Score = 95, Description = "The latest installment of the breathtaking nature documentary series." },
    new { Title = "Quantum Realm", Type = "Movie", Year = 2025, Director = "Peyton Reed", Cast = "Paul Rudd, Evangeline Lilly", Duration = "2h 12m", DurationSeconds = 7920, Score = 74, Description = "An explorer ventures into the mysterious quantum realm beneath our reality." },
    new { Title = "The Baker's Dozen", Type = "Movie", Year = 2023, Director = "Jon Favreau", Cast = "John Leguizamo, Sofia Vergara", Duration = "1h 38m", DurationSeconds = 5880, Score = 71, Description = "A charming baker must win the world's toughest pastry competition to save his shop." },
    new { Title = "Eclipse", Type = "Movie", Year = 2025, Director = "Alfonso Cuarón", Cast = "Cate Blanchett, Oscar Isaac", Duration = "2h 18m", DurationSeconds = 8280, Score = 87, Description = "During a total solar eclipse, a grieving widow discovers an extraordinary cosmic secret." },
    new { Title = "Thunder Road", Type = "Movie", Year = 2024, Director = "George Miller", Cast = "Tom Hardy, Charlize Theron", Duration = "2h 30m", DurationSeconds = 9000, Score = 89, Description = "The latest chapter in the post-apocalyptic saga of survival on the open road." },
    new { Title = "The Night Gardener", Type = "Movie", Year = 2023, Director = "Ari Aster", Cast = "Toni Collette, Alex Wolff", Duration = "2h 02m", DurationSeconds = 7320, Score = 78, Description = "A psychological horror about a gardener whose creations have a sinister life of their own." },
    new { Title = "Sonic Storm", Type = "Movie", Year = 2025, Director = "Justin Lin", Cast = "Michelle Yeoh, Simu Liu", Duration = "1h 50m", DurationSeconds = 6600, Score = 76, Description = "A physicist creates a device that can manipulate sound waves into devastating energy." },
    new { Title = "The Art of Letting Go", Type = "Movie", Year = 2024, Director = "Noah Baumbach", Cast = "Adam Driver, Greta Gerwig", Duration = "1h 42m", DurationSeconds = 6120, Score = 80, Description = "A couple navigates the difficult journey of separation while remaining friends." },
    new { Title = "Dragons of the East", Type = "Movie", Year = 2024, Director = "Zhang Yimou", Cast = "Jet Li, Gong Li", Duration = "2h 15m", DurationSeconds = 8100, Score = 83, Description = "An epic martial arts fantasy set in ancient China featuring mythical dragons." },
    new { Title = "Below Zero", Type = "Movie", Year = 2025, Director = "J.A. Bayona", Cast = "Tom Hiddleston, Alicia Vikander", Duration = "2h 08m", DurationSeconds = 7680, Score = 81, Description = "A research team stranded in Antarctica discovers an ancient organism beneath the ice." },
    new { Title = "The Puppeteer", Type = "Movie", Year = 2023, Director = "Park Chan-wook", Cast = "Song Kang-ho, Lee Ji-eun", Duration = "2h 00m", DurationSeconds = 7200, Score = 86, Description = "A master manipulator pulls the strings of a city's criminal underworld." },
    new { Title = "Wanderlust", Type = "Movie", Year = 2024, Director = "Wes Anderson", Cast = "Bill Murray, Tilda Swinton", Duration = "1h 44m", DurationSeconds = 6240, Score = 84, Description = "A whimsical journey through five countries as a family searches for their lost cat." },
    new { Title = "The Iron Trial", Type = "Movie", Year = 2025, Director = "Peter Jackson", Cast = "Christian Bale, Cate Blanchett", Duration = "2h 40m", DurationSeconds = 9600, Score = 92, Description = "A young apprentice must pass five deadly trials to become a master magician." },
    new { Title = "Fractured", Type = "Movie", Year = 2024, Director = "M. Night Shyamalan", Cast = "James McAvoy, Anya Taylor-Joy", Duration = "1h 48m", DurationSeconds = 6480, Score = 75, Description = "A family's vacation turns terrifying when reality begins to fracture around them." },
    new { Title = "Bossa Nova", Type = "Movie", Year = 2023, Director = "Fernando Meirelles", Cast = "Wagner Moura, Sônia Braga", Duration = "1h 55m", DurationSeconds = 6900, Score = 77, Description = "A romantic comedy set in the vibrant streets of Rio de Janeiro during carnival." },
    new { Title = "The Whispering Pines", Type = "Series", Year = 2024, Director = "Sam Esmail", Cast = "Julia Roberts, Mahershala Ali", Duration = "48m", DurationSeconds = 2880, Score = 83, Description = "A mystery series set in a small town where residents start disappearing without a trace." },
    new { Title = "Velocity", Type = "Movie", Year = 2025, Director = "Joseph Kosinski", Cast = "Tom Cruise, Jennifer Connelly", Duration = "2h 15m", DurationSeconds = 8100, Score = 88, Description = "A test pilot pushes the boundaries of speed in a revolutionary aircraft." },
    new { Title = "Sakura Nights", Type = "Movie", Year = 2024, Director = "Hirokazu Kore-eda", Cast = "Masami Nagasawa, Arata Iura", Duration = "1h 50m", DurationSeconds = 6600, Score = 82, Description = "Three generations of a family reunite under the cherry blossoms to fulfill a grandmother's wish." },
    new { Title = "Rogue AI", Type = "Movie", Year = 2025, Director = "Alex Garland", Cast = "Jessie Buckley, John David Washington", Duration = "1h 58m", DurationSeconds = 7080, Score = 85, Description = "An artificial intelligence escapes containment and begins rewriting its own code." },
    new { Title = "The Comfort Zone", Type = "Movie", Year = 2023, Director = "Mike White", Cast = "Jennifer Lawrence, Steve Buscemi", Duration = "1h 40m", DurationSeconds = 6000, Score = 73, Description = "A satire about a wellness retreat that takes its philosophy a bit too far." },
    new { Title = "Ghost Signal", Type = "Movie", Year = 2024, Director = "Denis Villeneuve", Cast = "Timothée Chalamet, Rebecca Ferguson", Duration = "2h 35m", DurationSeconds = 9300, Score = 90, Description = "A mysterious signal from deep space challenges everything we know about the universe." },
    new { Title = "Paws & Effect", Type = "Movie", Year = 2024, Director = "Chris Columbus", Cast = "voiced by Ryan Reynolds, Awkwafina", Duration = "1h 32m", DurationSeconds = 5520, Score = 70, Description = "An animated comedy about a dog who gains the ability to talk and becomes a social media star." },
    new { Title = "The Diplomat's Wife", Type = "Movie", Year = 2025, Director = "Steve McQueen", Cast = "Viola Davis, Daniel Kaluuya", Duration = "2h 05m", DurationSeconds = 7500, Score = 89, Description = "A diplomat's wife becomes an unlikely spy during a international crisis." },
    new { Title = "Code Black", Type = "Series", Year = 2024, Director = "Steven Soderbergh", Cast = "Clive Owen, Claire Foy", Duration = "50m", DurationSeconds = 3000, Score = 81, Description = "A medical drama set in the busiest trauma center in the United States." },
    new { Title = "The Gilded Cage", Type = "Movie", Year = 2023, Director = "Yorgos Lanthimos", Cast = "Emma Stone, Willem Dafoe", Duration = "1h 53m", DurationSeconds = 6780, Score = 84, Description = "An absurdist dark comedy about a wealthy family trapped in their own mansion." },
    new { Title = "Arctic Descent", Type = "Movie", Year = 2025, Director = "Ron Howard", Cast = "Chris Hemsworth, Michael Caine", Duration = "2h 12m", DurationSeconds = 7920, Score = 82, Description = "A submarine crew discovers an unexplored world beneath the Arctic ice cap." },
    new { Title = "Echoes of War", Type = "Documentary", Year = 2024, Director = (string?)null, Cast = "Narrated by Bryan Cranston", Duration = "55m", DurationSeconds = 3300, Score = 91, Description = "A powerful documentary series examining the human cost of conflict throughout history." },
    new { Title = "Soul Kitchen", Type = "Movie", Year = 2024, Director = "Fatih Akin", Cast = "Adam Sandler, Penélope Cruz", Duration = "1h 46m", DurationSeconds = 6360, Score = 75, Description = "A heartwarming comedy about a mismatched team of chefs opening a fusion restaurant." },
    new { Title = "The Infinite Loop", Type = "Movie", Year = 2025, Director = "Christopher Nolan", Cast = "Hugh Jackman, Elizabeth Debicki", Duration = "2h 22m", DurationSeconds = 8520, Score = 93, Description = "A physicist discovers time is a loop and must break the cycle to save humanity." },
};

var categories = await context.Categories.ToListAsync();
var categoryMap = categories.ToDictionary(c => c.Name!, c => c.Id);

var movies = new List<MovieData>();
var movieGenreLinks = new List<(int MovieIndex, int CategoryId)>();

// Movie-type -> genre mapping
var movieGenres = new Dictionary<int, string[]>
{
    { 0, new[] { "Action", "Sci-Fi" } },
    { 1, new[] { "Romance", "Fantasy" } },
    { 2, new[] { "Action", "Thriller" } },
    { 3, new[] { "Comedy" } },
    { 4, new[] { "Thriller", "Crime", "Mystery" } },
    { 5, new[] { "Sci-Fi", "Adventure" } },
    { 6, new[] { "Horror", "Thriller" } },
    { 7, new[] { "Romance", "Comedy" } },
    { 8, new[] { "Sci-Fi", "Thriller", "Action" } },
    { 9, new[] { "Adventure", "Action" } },
    { 10, new[] { "Thriller", "Mystery" } },
    { 11, new[] { "Comedy", "Crime" } },
    { 12, new[] { "Action", "Thriller" } },
    { 13, new[] { "Fantasy", "Adventure" } },
    { 14, new[] { "Crime", "Thriller" } },
    { 15, new[] { "Animation", "Fantasy", "Adventure" } },
    { 16, new[] { "Crime", "Drama" } },
    { 17, new[] { "Sci-Fi", "Horror" } },
    { 18, new[] { "Drama" } },
    { 19, new[] { "Sci-Fi", "Thriller" } },
    { 20, new[] { "Documentary" } },
    { 21, new[] { "Documentary" } },
    { 22, new[] { "Sci-Fi", "Adventure" } },
    { 23, new[] { "Comedy" } },
    { 24, new[] { "Drama", "Fantasy" } },
    { 25, new[] { "Action", "Adventure" } },
    { 26, new[] { "Horror", "Mystery" } },
    { 27, new[] { "Action", "Sci-Fi" } },
    { 28, new[] { "Drama", "Romance" } },
    { 29, new[] { "Fantasy", "Action", "Adventure" } },
    { 30, new[] { "Sci-Fi", "Horror" } },
    { 31, new[] { "Crime", "Thriller" } },
    { 32, new[] { "Comedy", "Adventure" } },
    { 33, new[] { "Fantasy", "Adventure" } },
    { 34, new[] { "Horror", "Thriller" } },
    { 35, new[] { "Comedy", "Romance" } },
    { 36, new[] { "Mystery", "Thriller" } },
    { 37, new[] { "Action", "Sci-Fi" } },
    { 38, new[] { "Drama" } },
    { 39, new[] { "Sci-Fi", "Thriller" } },
    { 40, new[] { "Comedy" } },
    { 41, new[] { "Sci-Fi", "Drama" } },
    { 42, new[] { "Animation", "Comedy", "Family" } },
    { 43, new[] { "Drama", "Thriller" } },
    { 44, new[] { "Drama" } },
    { 45, new[] { "Comedy", "Drama" } },
    { 46, new[] { "Action", "Adventure" } },
    { 47, new[] { "Documentary" } },
    { 48, new[] { "Comedy" } },
    { 49, new[] { "Sci-Fi", "Thriller" } },
};

for (int i = 0; i < movieData.Length; i++)
{
    var m = movieData[i];
    var movie = new MovieData
    {
        Title = m.Title,
        ExternalId = Guid.NewGuid().ToString("N")[..12],
        Type = m.Type,
        Category = m.Type,
        Year = m.Year,
        Country = new List<string> { "United States", "United Kingdom" },
        Director = m.Director,
        Cast = m.Cast?.Split(", ").ToList(),
        Duration = m.Duration,
        Score = m.Score,
        Rank = i + 1,
        TrendPercentage = $"+{Random.Shared.Next(5, 95)}%",
        Views = $"{Random.Shared.Next(10, 500)}K",
        Series = m.Type == "Movie" ? null : m.Title,
        Episode = m.Type == "Series" ? $"S01E01 - Pilot" : null,
        DurationSeconds = m.DurationSeconds,
        Description = m.Description,
        Poster = $"https://picsum.photos/seed/movie{i + 1}/300/450",
        Thumbnail = $"https://picsum.photos/seed/thumb{i + 1}/200/300",
        Backdrop = $"https://picsum.photos/seed/backdrop{i + 1}/1280/720",
        VideoSource = $"https://download.blender.org/peach/bigbuckbunny_movies/BigBuckBunny_320x180.mp4",
        VideoSources = new Dictionary<string, string>
        {

            {"Auto":"https://aura.host.cinemap.cc/bb83ea5db83373deb7b7266fbf6b7204:2026061101/movies/8d7e495bcb6fa2c6ba1b80bfaae87f1dcaa4caab/720.mp4",
            "1080p":"https://magic.host.cinemap.cc/47f99925070ea51c175720fea97d1bee:2026061101/movies/8d7e495bcb6fa2c6ba1b80bfaae87f1dcaa4caab/1080.mp4",
            "720p":"https://aura.host.cinemap.cc/bb83ea5db83373deb7b7266fbf6b7204:2026061101/movies/8d7e495bcb6fa2c6ba1b80bfaae87f1dcaa4caab/720.mp4",
            "480p":"https://loki.host.cinemap.cc/7b11b9a363774ee62d372eb4537d7b80:2026061101/movies/8d7e495bcb6fa2c6ba1b80bfaae87f1dcaa4caab/480.mp4",
            "360p":"https://marten.host.cinemap.cc/b04bd714680b1a7ccb025c6376df6619:2026061101/movies/8d7e495bcb6fa2c6ba1b80bfaae87f1dcaa4caab/360.mp4"}

            ["Auto"] = "https://aura.host.cinemap.cc/bb83ea5db83373deb7b7266fbf6b7204:2026061101/movies/8d7e495bcb6fa2c6ba1b80bfaae87f1dcaa4caab/720.mp4",
            ["1080p"] = "https://magic.host.cinemap.cc/47f99925070ea51c175720fea97d1bee:2026061101/movies/8d7e495bcb6fa2c6ba1b80bfaae87f1dcaa4caab/1080.mp4",
            ["720p"] = "https://aura.host.cinemap.cc/bb83ea5db83373deb7b7266fbf6b7204:2026061101/movies/8d7e495bcb6fa2c6ba1b80bfaae87f1dcaa4caab/720.mp4",
            ["480p"] = "https://loki.host.cinemap.cc/7b11b9a363774ee62d372eb4537d7b80:2026061101/movies/8d7e495bcb6fa2c6ba1b80bfaae87f1dcaa4caab/480.mp4",
            ["360p"] = "https://marten.host.cinemap.cc/b04bd714680b1a7ccb025c6376df6619:2026061101/movies/8d7e495bcb6fa2c6ba1b80bfaae87f1dcaa4caab/360.mp4",
        },
        IsPrivate = false
    };
    movies.Add(movie);

    // assign genres
    if (movieGenres.TryGetValue(i, out var genreNames))
    {
        foreach (var gn in genreNames)
        {
            if (categoryMap.TryGetValue(gn, out var catId))
                movieGenreLinks.Add((i, catId));
        }
    }
}

context.Movies.AddRange(movies);
await context.SaveChangesAsync();
Console.WriteLine($"  ✓ {movies.Count} movies seeded");

// ──────────────────────────────────────────────
// 2b. Additional episodes for series
// ──────────────────────────────────────────────
var episodeTitles = new Dictionary<string, string[]>
{
    ["True Detective: Season 5"] = new[] { "Night Country", "The Long Dark", "Signal", "Buried Secrets", "Resolution" },
    ["Stranger Worlds"] = new[] { "The Vanishing of Will Smith", "The Upside Down", "Holly, Jolly", "The Gate", "The Dive" },
    ["The Crown: New Era"] = new[] { "The New Reign", "A Royal Scandal", "Family Matters", "The Commonwealth", "Legacy" },
    ["Cyber Edge"] = new[] { "Ghost in the Wire", "Firewall", "Zero Day", "Blackout", "The Final Protocol" },
    ["The Whispering Pines"] = new[] { "The Disappearance", "Dark Roots", "Echoes", "The Hollow", "Revelation" },
    ["Code Black"] = new[] { "Code Black", "Triage", "The Bleeding Edge", "Resurrection", "Last Shift" },
};

var seriesMovies = movies.Where(m => m.Type == "Series" && m.Series != null).ToList();
var extraEpisodes = new List<MovieData>();

foreach (var seriesMovie in seriesMovies)
{
    if (!episodeTitles.TryGetValue(seriesMovie.Series!, out var titles)) continue;

    var epIndex = 1;
    foreach (var epTitle in titles)
    {
        epIndex++;
        var ep = new MovieData
        {
            Title = $"{seriesMovie.Series} - {epTitle}",
            ExternalId = Guid.NewGuid().ToString("N")[..12],
            Type = "Series",
            Category = "Series",
            Year = seriesMovie.Year,
            Country = seriesMovie.Country?.ToList(),
            Director = seriesMovie.Director,
            Cast = seriesMovie.Cast?.ToList(),
            Duration = seriesMovie.Duration,
            Score = seriesMovie.Score + Random.Shared.Next(-5, 6),
            Rank = seriesMovie.Rank,
            TrendPercentage = seriesMovie.TrendPercentage,
            Views = seriesMovie.Views,
            Series = seriesMovie.Series,
            Episode = $"S01E{epIndex:D2} - {epTitle}",
            DurationSeconds = seriesMovie.DurationSeconds + Random.Shared.Next(-300, 300),
            Description = epTitle,
            Poster = seriesMovie.Poster,
            Thumbnail = seriesMovie.Thumbnail,
            Backdrop = seriesMovie.Backdrop,
            VideoSource = seriesMovie.VideoSource,
            VideoSources = seriesMovie.VideoSources != null
                ? new Dictionary<string, string>(seriesMovie.VideoSources)
                : null,
            IsPrivate = false,
        };
        extraEpisodes.Add(ep);
    }
}

context.Movies.AddRange(extraEpisodes);
await context.SaveChangesAsync();
Console.WriteLine($"  ✓ {extraEpisodes.Count} extra episodes seeded");

// movie-genre join table
foreach (var (movieIdx, catId) in movieGenreLinks)
{
    var movie = movies[movieIdx];
    var cat = categories.First(c => c.Id == catId);
    movie.Genres ??= new List<CategoryData>();
    movie.Genres.Add(cat);
}
// same genres for extra episodes
foreach (var ep in extraEpisodes)
{
    var parentSeries = seriesMovies.FirstOrDefault(s => s.Series == ep.Series);
    if (parentSeries?.Genres != null)
    {
        ep.Genres ??= new List<CategoryData>();
        foreach (var g in parentSeries.Genres)
            ep.Genres.Add(g);
    }
}
await context.SaveChangesAsync();
Console.WriteLine($"  ✓ {movieGenreLinks.Count} movie-genre links seeded");

// ──────────────────────────────────────────────
// 3. HISTORY (watch history)
// ──────────────────────────────────────────────
var watchStatuses = new[] { "Watched", "In Progress", "Plan to Watch", "Paused" };
var allUsers = await context.Users.ToListAsync();
var allMovies = await context.Movies.Include(m => m.Genres).ToListAsync();

var historyEntries = new List<HistoryData>();
for (int i = 0; i < 150; i++)
{
    var user = allUsers[Random.Shared.Next(allUsers.Count)];
    var movie = allMovies[Random.Shared.Next(allMovies.Count)];
    var status = watchStatuses[Random.Shared.Next(watchStatuses.Length)];
    var progress = status == "Watched" ? 100 : Random.Shared.Next(10, 99);
    double? userRating = status == "Watched" ? Math.Round(Random.Shared.NextDouble() * 5 + 5, 1) : null;

    historyEntries.Add(new HistoryData
    {
        UserId = user.Id,
        MovieId = movie.Id,
        ExternalMovieId = movie.ExternalId,
        MovieTitle = movie.Title,
        Category = movie.Category,
        Series = movie.Series,
        Episode = movie.Episode,
        WatchedAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(0, 365)).AddHours(-Random.Shared.Next(0, 24)),
        WatchStatus = status,
        ProgressPercentage = progress,
        Progress = progress < 100 ? $"{progress}%" : null,
        UserRating = userRating
    });
}

context.Histories.AddRange(historyEntries);
await context.SaveChangesAsync();
Console.WriteLine($"  ✓ {historyEntries.Count} history entries seeded");

// ──────────────────────────────────────────────
// 4. LISTS (user film lists)
// ──────────────────────────────────────────────
var filmStatuses = new[] { "Watched", "Watching", "Want to Watch", "Abandoned" };
var listTypes = new[] { "Watchlist", "Favorites", "Completed", "Custom" };
var listEntries = new List<ListsData>();

for (int i = 0; i < 80; i++)
{
    var user = allUsers[Random.Shared.Next(allUsers.Count)];
    var movie = allMovies[Random.Shared.Next(allMovies.Count)];
    var genreNames = movie.Genres?.Select(g => g.Name).ToList();

    listEntries.Add(new ListsData
    {
        UserId = user.Id,
        FilmId = movie.Id,
        ExternalFilmId = movie.ExternalId,
        FilmTitle = movie.Title,
        Type = listTypes[Random.Shared.Next(listTypes.Length)],
        Category = movie.Category,
        Genre = genreNames?.Count > 0 ? string.Join(", ", genreNames) : null,
        Episode = movie.Episode,
        Poster = movie.Poster,
        AddedAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(0, 180)),
        FilmStatus = filmStatuses[Random.Shared.Next(filmStatuses.Length)],
        FilmRating = Math.Round(Random.Shared.NextDouble() * 10, 1),
        FilmScore = Math.Round(Random.Shared.NextDouble() * 100, 0)
    });
}

context.Lists.AddRange(listEntries);
await context.SaveChangesAsync();
Console.WriteLine($"  ✓ {listEntries.Count} list entries seeded");

// ──────────────────────────────────────────────
// 5. SUBSCRIPTIONS (one per user)
// ──────────────────────────────────────────────
var plans = new[]
{
    new { Name = "Free", PlanId = "FREE", Price = 0m, Duration = 0, Features = "Basic streaming, SD quality, Ads", MaxDevices = 1, HasAds = true, Offline = false, Quality = "SD" },
    new { Name = "Standard", PlanId = "STD", Price = 9.99m, Duration = 1, Features = "HD streaming, No ads, 2 devices, 10 downloads", MaxDevices = 2, HasAds = false, Offline = true, Quality = "HD" },
    new { Name = "Premium", PlanId = "PRM", Price = 14.99m, Duration = 1, Features = "4K streaming, No ads, 4 devices, Unlimited downloads, HDR", MaxDevices = 4, HasAds = false, Offline = true, Quality = "4K" }
};

var subscriptions = new List<SubscriptionData>();
foreach (var user in allUsers)
{
    if (user.Id == 1) continue; // admin already has a subscription?
    var plan = plans[Random.Shared.Next(plans.Length)];
    subscriptions.Add(new SubscriptionData
    {
        UserId = user.Id,
        Name = plan.Name,
        PlanId = plan.PlanId,
        Subtitle = plan.Price == 0 ? "Free" : $"${plan.Price}/month",
        PriceLabel = plan.Price == 0 ? "Free" : $"${plan.Price}/mo",
        PriceValue = plan.Price == 0 ? "$0" : $"${plan.Price}",
        Cta = plan.Price == 0 ? "Current Plan" : "Upgrade",
        Price = plan.Price,
        DurationInMonths = plan.Duration,
        Features = plan.Features,
        MaxDevices = plan.MaxDevices,
        HasAds = plan.HasAds,
        SupportsOfflineDownload = plan.Offline,
        VideoQuality = plan.Quality,
        CreatedAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(30, 365)),
        IsCanceled = plan.Price == 0 ? false : Random.Shared.NextDouble() < 0.1 // 10% chance of cancellation for paid plans
    });
}

context.Subscriptions.AddRange(subscriptions);
await context.SaveChangesAsync();
Console.WriteLine($"  ✓ {subscriptions.Count} subscriptions seeded");

// ──────────────────────────────────────────────
// 6. LEADERBOARD (top users)
// ──────────────────────────────────────────────
var leaderboardEntries = new List<LeaderboardData>();
var topUsers = allUsers.OrderByDescending(u => u.HoursWatched).Take(20).ToList();
for (int i = 0; i < topUsers.Count; i++)
{
    var u = topUsers[i];
    var prevRank = i + 1 + Random.Shared.Next(-2, 3);
    if (prevRank < 1) prevRank = 1;
    if (prevRank > topUsers.Count) prevRank = topUsers.Count;

    leaderboardEntries.Add(new LeaderboardData
    {
        Username = u.Name,
        ExternalUserId = u.ExternalId,
        Avatar = u.Avatar,
        Country = "US",
        PreviousRank = prevRank,
        HoursWatched = u.HoursWatched,
        MoviesWatched = u.TitlesWatched,
        EpisodesWatched = Random.Shared.Next(0, 50),
        AverageRating = Math.Round(Random.Shared.NextDouble() * 5 + 5, 1),
        ReviewsWritten = u.ReviewCount,
        LikesReceived = u.LikesReceived,
        Score = u.HoursWatched * 10 + u.TitlesWatched * 5 + u.ReviewCount * 20
    });
}

context.Leaderboards.AddRange(leaderboardEntries);
await context.SaveChangesAsync();
Console.WriteLine($"  ✓ {leaderboardEntries.Count} leaderboard entries seeded");

Console.WriteLine("\n✅ All seed data generated successfully!");
