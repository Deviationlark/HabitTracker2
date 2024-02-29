using Microsoft.Data.Sqlite;
ShowMainMenu();
// GetNumInput();
// Read();
// Insert();
// Update();
// Delete();
// CreateHabit();
// DeleteHabit();

string connectionString = @"Data Source=Habit-Tracker.db";

using (var connection = new SqliteConnection(connectionString))
{
    connection.Open();

    var tableCmd = connection.CreateCommand();

    tableCmd.CommandText = @"CREATE TABLE IF NOT EXISTS Habits (
        Id INTEGER PRIMARY KEY INCREMENT
        Habit Text
    )";

    tableCmd.ExecuteNonQuery();

    tableCmd.CommandText = @"CREATE TABLE IF NOT EXIST Records (
        Id INTEGER PRIMARY KEY INCREMENT
        Date TEXT,

    )";

    tableCmd.ExecuteNonQuery();

    connection.Close();
}

void ShowMainMenu()
{
    Console.WriteLine("HABIT TRACKER");
    Console.WriteLine("--------------");
    Console.WriteLine("1.Create a habit");
    Console.WriteLine("2. Edit an existing habit");
    Console.WriteLine("3. Delete a habit");
    Console.WriteLine("4. Exit");
    var userInput = Console.ReadLine();

    switch (userInput)
    {
        case "1":
            // CreateHabit();
            break;
        case "2":
            ShowMenu();
            break;
        case "3":
            // DeleteHabit();
            break;
        case "4":
            Environment.Exit(0);
            break;
    }
}

void ShowMenu()
{
    Console.WriteLine("HABIT TRACKER");
    Console.WriteLine("--------------");
    Console.WriteLine("1. Show all records");
    Console.WriteLine("2. Insert a record");
    Console.WriteLine("3. Update a record");
    Console.WriteLine("4. Delete a record");
    Console.WriteLine("5. Exit");
    var userInput = Console.ReadLine();

    switch (userInput)
    {
        case "1":
            // Read();
            break;
        case "2":
            // Insert();
            break;
        case "3":
            // Update();
            break;
        case "4":
            // Delete();
            break;
        case "5":
            Environment.Exit(0);
            break;
    }
}

int GetNumInput(string message)
{
    bool numParse = false;
    int numInput = 0;
    while (!numParse)
    {
        Console.WriteLine(message);
        var userInput = Console.ReadLine();
        numParse = int.TryParse(userInput, out numInput);
    }
    return numInput;

}
