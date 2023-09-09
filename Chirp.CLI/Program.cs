﻿using System.IO;
using System;
using CsvHelper;
using System.Globalization;
using System.ComponentModel.Design;


//All references to "GPT" in comments are references to chat.opanai.com


string path = "../src/chirp_cli_db.csv"; //Path to CSV file 



if (args[0]=="read")
{   // Read part from: https://learn.microsoft.com/en-us/dotnet/standard/io/how-to-read-text-from-a-file

    
    using (StreamReader reader = new StreamReader(path))
    using (CsvReader csv = new CsvReader(reader,CultureInfo.InvariantCulture))
    {
        IEnumerable<Cheep> records = csv.GetRecords<Cheep>();
    
    
        if (args.Length == 1) 
        {
            foreach (Cheep cheep in records)
            {
                Console.WriteLine($"{cheep.Author} @ {cheep.Timestamp} : {cheep.Message}");
            }
        }
        else if (args.Length == 2)
        {
            int cheeps_left = int.Parse(args[1]);
            foreach (Cheep cheep in records)

            {
                Console.WriteLine($"{cheep.Author} @ {cheep.Timestamp} : {cheep.Message}");
                cheeps_left -= 1;
                if (cheeps_left == 0) { break; }
            }
        }
    }
}

if (args[0]=="cheep")
{

    long unixTimestamp = (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds; //Used GPT for this
    Cheep cheep = new Cheep(Environment.UserName , args[1] , unixTimestamp);
    using (StreamWriter sw = File.AppendText(path))  
    using (CsvWriter csv = new CsvWriter(sw , CultureInfo.InvariantCulture))
    {
        csv.WriteRecord(cheep);
    }
}


public record Cheep(string Author , string Message , long Timestamp);