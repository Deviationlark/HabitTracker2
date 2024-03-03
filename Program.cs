using System.Globalization;
using Microsoft.Data.Sqlite;
// Read();
// Insert();
// Update();
// Delete();

string connectionString = @"Data Source=HabitTracker.db";
List<Habits> tableData = new();
string[] habitInfo;

using (var connection = new SqliteConnection(connectionString))
{
    connection.Open();

    var tableCmd = connection.CreateCommand();

    tableCmd.CommandText = @"CREATE TABLE IF NOT EXISTS Habits (
        Id INTEGER PRIMARY KEY AUTOINCREMENT,
        Habit TEXT,
        Measurement TEXT
    )";

    tableCmd.ExecuteNonQuery();

    tableCmd.CommandText = @"CREATE TABLE IF NOT EXISTS Records (
        Id INTEGER PRIMARY KEY AUTOINCREMENT,
        Habit TEXT,
        Date TEXT,
        Quantity INTEGER
    )";

    tableCmd.ExecuteNonQuery();

    connection.Close();
}

ShowMainMenu();
void ShowMainMenu()
{
    Console.WriteLine("HABIT TRACKER");
    Console.WriteLine("--------------");
    Console.WriteLine("1. Create a habit");
    Console.WriteLine("2. Edit an existing habit");
    Console.WriteLine("3. Delete a habit");
    Console.WriteLine("4. Exit");
    var userInput = Console.ReadLine();

    switch (userInput)
    {
        case "1":
            CreateHabit();
            break;
        case "2":
            ShowMenu();
            break;
        case "3":
            DeleteHabit();
            break;
        case "4":
            Environment.Exit(0);
            break;
    }
}

string[] ChooseHabit()
{
    GetAllHabits();
    habitInfo = GetHabitInfo("Type the ID of the habit you want to edit: ");
    return habitInfo;
}

void ShowMenu()
{
    ChooseHabit();
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
            // GetAllRecords();
            break;
        case "2":
            Insert();
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

string GetDateInput()
{
    Console.WriteLine("Please insert the date: (dd-mm-yy).");
    Console.WriteLine("Type 0 to go back to menu.");
    string? dateInput = Console.ReadLine();

    if (dateInput == "0") ShowMenu();
    while (!DateTime.TryParseExact(dateInput, "dd-MM-yy", new CultureInfo("en-US"), DateTimeStyles.None, out _))
    {
        Console.WriteLine("Invalid Date. (Format dd-mm-yy)");
        dateInput = Console.ReadLine();
        if (dateInput == "0") ShowMenu();
    }

    return dateInput;
}

void GetAllHabits(string readLine = "")
{
    using (var connection = new SqliteConnection(connectionString))
    {
        connection.Open();
        var tableCmd = connection.CreateCommand();

        tableCmd.CommandText = "SELECT * FROM Habits";

        SqliteDataReader reader = tableCmd.ExecuteReader();

        if (reader.HasRows)
        {
            while (reader.Read())
            {
                tableData.Add(new Habits
                {
                    Id = reader.GetInt32(0),
                    Habit = reader.GetString(1),
                    Measurement = reader.GetString(2)
                });
            }
        }
        else
            Console.WriteLine("No habits found.");

        connection.Close();
        if (tableData.Count > 0)
        {
            Console.WriteLine("------------");

            foreach (var habit in tableData)
            {
                Console.WriteLine($"{habit.Id} - {habit.Habit} - {habit.Measurement}");
            }
            if (readLine == "")
                Console.ReadLine();
        }
    }
}

void Insert()
{
    string date = GetDateInput();
    int quantity = GetNumInput($"Type the amount of {habitInfo[1]} you want to insert: ");

    using (var connection = new SqliteConnection(connectionString))
    {
        connection.Open();
        var tableCmd = connection.CreateCommand();

        tableCmd.CommandText = $"INSERT INTO Records(habit, date, quantity) VALUES('{habitInfo[0]}', '{date}', '{quantity}')";

        tableCmd.ExecuteNonQuery();

        connection.Close();
    }
}

void CreateHabit()
{
    Console.WriteLine("Write the name of the habit you want to create: ");
    string? habitInput = Console.ReadLine();
    Console.WriteLine($"Write the measurement you want to track {habitInput} with: ");
    string? measurementInput = Console.ReadLine();

    using (var connection = new SqliteConnection(connectionString))
    {
        connection.Open();

        var tableCmd = connection.CreateCommand();
        tableCmd.CommandText =
        $"INSERT INTO Habits(habit, measurement) VALUES('{habitInput}', '{measurementInput}')";

        tableCmd.ExecuteNonQuery();

        connection.Close();
    }
}

void DeleteHabit()
{
    GetAllHabits("Show habits");
    if (tableData.Count > 0)
    {
        var recordID = GetNumInput("Type the id of the habit you want to delete");

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            var tableCmd = connection.CreateCommand();
            tableCmd.CommandText = $"DELETE from Habits WHERE Id = '{recordID}'";
            int rowCount = tableCmd.ExecuteNonQuery();

            if (rowCount == 0)
            {
                Console.WriteLine($"Habit with id {recordID} doesn't exist.");
                DeleteHabit();
            }

            Console.WriteLine($"Record with ID {recordID} has been deleted.");
            Habits habitId = tableData.ElementAt<Habits>(recordID);
            tableData.Remove(habitId);
            ShowMainMenu();
        }
    }
    else
        Console.WriteLine("No records found.");
}

string[] GetHabitInfo(string habit)
{
    int habitId = GetNumInput(habit);
    string[] habitInfo = new string[2];
    using (var connection = new SqliteConnection(connectionString))
    {
        connection.Open();
        var tableCmd = connection.CreateCommand();
        string sql = $"SELECT * FROM Habits";
        using (SqliteCommand command = new SqliteCommand(sql, connection))
        {
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    if (i == habitId)
                    {
                        habitInfo[0] = reader.GetString(i);
                        habitInfo[1] = reader.GetString(i+1);
                    }

                }
            }
            connection.Close();
        }
        return habitInfo;
    }
}

public class Habits
{
    public int Id { get; set; }
    public string? Habit { get; set; }
    public string? Measurement { get; set; }
}

public class Records
{
    public int Id { get; set; }
    public string? Habit { get; set; }
    public DateTime Date { get; set; }
    public int Quantity { get; set; }
}
