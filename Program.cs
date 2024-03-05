using System.Globalization;
using Microsoft.Data.Sqlite;
// This app is used to reinforce my SQL knowledge

string connectionString = @"Data Source=HabitTracker.db";
List<Habits> habitsData = new();
List<Records> recordData = new();
string[] habitInfo = new string[2];

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
    habitInfo = ChooseHabit();
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
            GetAllRecords(habitInfo);
            break;
        case "2":
            Insert(habitInfo);
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
                habitsData.Add(new Habits
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
        if (habitsData.Count > 0)
        {
            Console.WriteLine("------------");

            foreach (var habit in habitsData)
            {
                Console.WriteLine($"{habit.Id} - {habit.Habit} - {habit.Measurement}");
            }
            if (readLine == "")
                Console.ReadLine();
        }
    }
}

void GetAllRecords(string[] habitInfo)
{
    using (var connection = new SqliteConnection(connectionString))
    {
        connection.Open();

        var tableCmd = connection.CreateCommand();
        tableCmd.CommandText = "SELECT * FROM Records";
        habitsData.Clear();

        SqliteDataReader reader = tableCmd.ExecuteReader();

        if (reader.HasRows)
        {
            while (reader.Read())
            {
                recordData.Add(new Records
                {
                    Id = reader.GetInt32(0),
                    Habit = reader.GetString(1),
                    Date = DateTime.ParseExact(reader.GetString(2), "dd-MM-yy", new CultureInfo("en-US")),
                    Quantity = reader.GetInt32(3)
                });
            }
        }
        else
            Console.WriteLine("No records found");

        connection.Close();
        if (recordData.Count > 0)
        {
            Console.WriteLine("--------------");

            foreach (var record in recordData)
            {
                foreach (var habit in habitsData)
                {
                    if (habitInfo[0] == record.Habit)
                        Console.WriteLine($"{record.Id} - {record.Habit} - {record.Date} - {habitInfo[1]}:{record.Quantity}");
                }

            }
            Console.WriteLine("----------------");
        }
        Console.ReadLine();
    }
}

void Insert(string[] habitInfo)
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

void Update(string[] habitInfo)
{
    GetAllRecords(habitInfo);
    if (recordData.Count > 0)
    {
        var recordId = GetNumInput("Type the ID of the record you want to update: ");
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            var checkCmd = connection.CreateCommand();
            checkCmd.CommandText = $"SELECT EXISTS(SELECT 1 FROM Records WHERE Id = {recordId})";
            int checkQuery = Convert.ToInt32(checkCmd.ExecuteScalar());

            if (checkQuery == 0)
            {
                Console.WriteLine($"Record with Id {recordId} doesn't exist.");
                Console.ReadLine();
                connection.Close();
                Update(habitInfo);
            }

            string date = GetDateInput();

            int quantity = GetNumInput("Enter the quantity");

            var tableCmd = connection.CreateCommand();

            tableCmd.CommandText = @$"UPDATE Records SET date = '{date}', quantity = '{quantity}' WHERE Id = {recordId}";

            tableCmd.ExecuteNonQuery();

            connection.Close();
        }
    }
    else
        Console.WriteLine("No records found");
}

void Delete()
{
    var recordId = GetNumInput("Type the ID of the record you want to delete");

    using(var connection = new SqliteConnection(connectionString))
    {
        connection.Open();

        var tableCmd = connection.CreateCommand();

        tableCmd.CommandText = $"DELETE FROM Habits WHERE Id = {recordId}";

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
    if (habitsData.Count > 0)
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
            Habits habitId = habitsData.ElementAt<Habits>(recordID);
            habitsData.Remove(habitId);
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
                while (reader.Read())
                {
                    for (int i = 0; i <= habitId; i++)
                        if (i == habitId)
                        {
                            habitInfo[0] = reader.GetString(i);
                            habitInfo[1] = reader.GetString(i + 1);
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
