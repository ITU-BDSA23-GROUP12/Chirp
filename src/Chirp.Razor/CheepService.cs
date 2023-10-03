using Microsoft.Data.Sqlite;
﻿using System.Data;

public record CheepViewModel(string Author, string Message, string Timestamp);

public interface ICheepService
{
    public List<CheepViewModel> GetCheeps();
    public List<CheepViewModel> GetCheepsFromAuthor(string author);
}

public class CheepService : ICheepService
{

    public List<CheepViewModel> GetCheeps()
    {
        return LoadLocalSqlite();
    }

    private List<CheepViewModel> LoadLocalSqlite(string? author = null)
    {
        var sqlDBFilePath = SeedingDBfileDir();
        bool database_is_ready = File.Exists(sqlDBFilePath); // determines if the database needs to be initialised with the schema, or if it ready to read
        var sqlQuery = 
        @"SELECT u.username AS username , m.text AS message, m.pub_date AS date FROM 
        message m JOIN user u ON u.user_id = m.author_id
        ORDER BY m.pub_date desc";
        List<CheepViewModel> cheeps = new List<CheepViewModel>();
        using (var connection = new SqliteConnection($"Data Source={sqlDBFilePath}"))
        {
            connection.Open();

            if (!database_is_ready)
            {
                InitialiseDb(connection);
            }
            SqliteCommand command = CreateQueryCommand(connection, author);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                cheeps.Add(new CheepViewModel(
                    reader.GetString(reader.GetOrdinal("username")) , 
                    reader.GetString(reader.GetOrdinal("message")) , 
                    UnixTimeStampToDateTimeString(reader.GetInt32(reader.GetOrdinal("date")))
                    ));
            }
        }
        return cheeps;
    }
    
    private void InitialiseDb(SqliteConnection connection)
    {
        SqliteCommand schema_creation_command = connection.CreateCommand();
        schema_creation_command.CommandText = 
        @"drop table if exists user;
        create table user (
          user_id integer primary key autoincrement,
          username string not null,
          email string not null,
          pw_hash string not null
        );
        drop table if exists message;
        create table message (
          message_id integer primary key autoincrement,
          author_id integer not null,
          text string not null,
          pub_date integer
        );";
        schema_creation_command.ExecuteNonQuery();
    }

    SqliteCommand CreateQueryCommand(SqliteConnection connection , string? author)
    {
        SqliteCommand queryCommand = connection.CreateCommand();
        if (author == null)
        {
            string sqlQuery = 
            @"SELECT u.username AS username , m.text AS message, m.pub_date AS date FROM 
            message m JOIN user u ON u.user_id = m.author_id
            ORDER BY m.pub_date desc";
            queryCommand.CommandText = sqlQuery;
        }
        else
        {
            string sqlQuery =
            @"SELECT u.username AS username , m.text AS message, m.pub_date AS date 
            FROM message m JOIN user u ON u.user_id = m.author_id
            WHERE u.username = @author 
            ORDER BY m.pub_date desc";
            queryCommand.CommandText = sqlQuery;
            queryCommand.Parameters.AddWithValue("@author", author);
        }
        return queryCommand;
    }

    public List<CheepViewModel> GetCheepsFromAuthor(string author)
    {
        // filter by the provided author name
        return LoadLocalSqlite(author);
    }

    private static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }

    private static string SeedingDBfileDir (){ // Retreeves the the given file from the EnvironmentVariable, else creates a path to the Tmp folder
        string chirpDBpath;                     // https://learn.microsoft.com/en-us/dotnet/api/system.environment.setenvironmentvariable?view=net-7.0
        if(Environment.GetEnvironmentVariable("CHIRPDBPATH") == null){
            chirpDBpath = Path.GetTempPath() + "chirp.db";
        } else {
            chirpDBpath = Environment.GetEnvironmentVariable("CHIRPDBPATH");
        }
        return chirpDBpath;
    }

}
