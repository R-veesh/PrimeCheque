using System;
using Microsoft.Data.Sqlite;

class Program
{
    static void Main()
    {
        string dbPath = @"D:\PrimeOneWork\C#\PrimeCheque\PrimeCheque\PrimeCheque.db";
        using (var connection = new SqliteConnection($"Data Source={dbPath}"))
        {
            connection.Open();
            try
            {
                var command = connection.CreateCommand();
                command.CommandText = "ALTER TABLE PrinterCalibrations ADD COLUMN PrintLandscape INTEGER NOT NULL DEFAULT 0;";
                command.ExecuteNonQuery();
                Console.WriteLine("Column added successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}
