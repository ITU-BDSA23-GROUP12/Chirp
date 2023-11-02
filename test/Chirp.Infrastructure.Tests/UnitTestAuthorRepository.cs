namespace Chirp.Infrastructure.Tests;

public class UnitTestAuthorRepository
{

    [Theory]
    [InlineData("Helge")]
    [InlineData("Rasmus")]
    public async void TestAuthorRepositoryGetAuthorByName(string name)
    {

        //Arrange
        var connection = new SqliteConnection("DataSource=:memory:"); //Configuring connenction using in-memory connectionString
        connection.Open(); // Open the connection. (So EF Core doesnt close it automatically)

        var options = new DbContextOptionsBuilder<ChirpDBContext>()
            .UseSqlite(connection)
            .Options; //Create an instance of DBConnectionOptions, and configure it to use SQLite connection.

        using var context = new ChirpDBContext(options); //Creates a context, and passes in the options.

        await context.Database.EnsureCreatedAsync();
        DbInitializer.SeedDatabase(context); //Seed the database.
        var repository = new AuthorRepository(context);

        // Act
        AuthorDto author = await repository.GetAuthorByName(name);

        // Assert
        Assert.Equal(author.Name, name);
    }

    [Theory]
    [InlineData("ropf@itu.dk")]
    [InlineData("rnie@itu.dk")]
    public async void TestAuthorRepositoryGetAuthorByEmail(string email)
    {

        //Arrange
        var connection = new SqliteConnection("DataSource=:memory:"); //Configuring connenction using in-memory connectionString
        connection.Open(); // Open the connection. (So EF Core doesnt close it automatically)

        var options = new DbContextOptionsBuilder<ChirpDBContext>()
            .UseSqlite(connection)
            .Options; //Create an instance of DBConnectionOptions, and configure it to use SQLite connection.

        using var context = new ChirpDBContext(options); //Creates a context, and passes in the options.

        await context.Database.EnsureCreatedAsync();
        DbInitializer.SeedDatabase(context); //Seed the database.
        var repository = new AuthorRepository(context);

        // Act
        AuthorDto author = await repository.GetAuthorByEmail(email);

        // Assert
        Assert.Equal(author.Email, email);
    }

    [Theory]
    [InlineData("Helge", "ropf@itu.dk")]
    [InlineData("Rasmus", "rnie@itu.dk")]
    public async void TestAuthorRepositoryCreateAuthor(string name, string email)
    {

        //Arrange
        var connection = new SqliteConnection("DataSource=:memory:"); //Configuring connenction using in-memory connectionString
        connection.Open(); // Open the connection. (So EF Core doesnt close it automatically)

        var options = new DbContextOptionsBuilder<ChirpDBContext>()
            .UseSqlite(connection)
            .Options; //Create an instance of DBConnectionOptions, and configure it to use SQLite connection.

        using var context = new ChirpDBContext(options); //Creates a context, and passes in the options.

        await context.Database.EnsureCreatedAsync();
        DbInitializer.SeedDatabase(context); //Seed the database.
        var repository = new AuthorRepository(context);

        // Act
        repository.CreateAuthor(name, email);
        AuthorDto newly_created_author = await repository.GetAuthorByEmail(email);

        // Assert
        Assert.Equal(newly_created_author.Email, email);

    }
}