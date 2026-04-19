using Microsoft.Data.Sqlite;

class DatabaseManager
{
    private string _connectionString;

    public DatabaseManager(string dbPath)
    {
        _connectionString = $"Data Source={dbPath}";
    }

    public void InitializeDatabase(string operatorsCsvPath, string tariffsCsvPath)
    {
        CreateTables();

        if (GetAllOperators().Count == 0 && File.Exists(operatorsCsvPath))
        {
            ImportOperatorsFromCsv(operatorsCsvPath);
            Console.WriteLine($"[OK] Загружены операторы из {operatorsCsvPath}");
        }

        if (GetAllTariffs().Count == 0 && File.Exists(tariffsCsvPath))
        {
            ImportTariffsFromCsv(tariffsCsvPath);
            Console.WriteLine($"[OK] Загружены тарифы из {tariffsCsvPath}");
        }
    }

    private void CreateTables()
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS operators (
                operator_id INTEGER PRIMARY KEY AUTOINCREMENT,
                operator_name TEXT NOT NULL
            );
            CREATE TABLE IF NOT EXISTS tariffs (
                tariff_id INTEGER PRIMARY KEY AUTOINCREMENT,
                operator_id INTEGER NOT NULL,
                tariff_name TEXT NOT NULL,
                price_month INTEGER NOT NULL,
                FOREIGN KEY (operator_id) REFERENCES operators(operator_id)
            );";
        cmd.ExecuteNonQuery();
    }

    private void ImportOperatorsFromCsv(string path)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        string[] lines = File.ReadAllLines(path);
        for (int i = 1; i < lines.Length; i++)
        {
            string[] parts = lines[i].Split(';');
            if (parts.Length < 2) continue;
            var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO operators (operator_id, operator_name) VALUES (@id, @name)";
            cmd.Parameters.AddWithValue("@id", int.Parse(parts[0]));
            cmd.Parameters.AddWithValue("@name", parts[1]);
            cmd.ExecuteNonQuery();
        }
    }

    private void ImportTariffsFromCsv(string path)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        string[] lines = File.ReadAllLines(path);
        for (int i = 1; i < lines.Length; i++)
        {
            string[] parts = lines[i].Split(';');
            if (parts.Length < 4) continue;
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO tariffs (tariff_id, operator_id, tariff_name, price_month)
                                VALUES (@id, @opId, @name, @price)";
            cmd.Parameters.AddWithValue("@id", int.Parse(parts[0]));
            cmd.Parameters.AddWithValue("@opId", int.Parse(parts[1]));
            cmd.Parameters.AddWithValue("@name", parts[2]);
            cmd.Parameters.AddWithValue("@price", int.Parse(parts[3]));
            cmd.ExecuteNonQuery();
        }
    }

    public List<Operator> GetAllOperators()
    {
        var result = new List<Operator>();
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT operator_id, operator_name FROM operators ORDER BY operator_id";
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new Operator(reader.GetInt32(0), reader.GetString(1)));
        }
        return result;
    }

    public List<Tariff> GetAllTariffs()
    {
        var result = new List<Tariff>();
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT tariff_id, operator_id, tariff_name, price_month FROM tariffs ORDER BY tariff_id";
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new Tariff(reader.GetInt32(0), reader.GetInt32(1), reader.GetString(2), reader.GetInt32(3)));
        }
        return result;
    }

    public Tariff GetTariffById(int id)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT tariff_id, operator_id, tariff_name, price_month FROM tariffs WHERE tariff_id = @id";
        cmd.Parameters.AddWithValue("@id", id);
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new Tariff(reader.GetInt32(0), reader.GetInt32(1), reader.GetString(2), reader.GetInt32(3));
        }
        return null;
    }

    public void AddTariff(Tariff tariff)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = @"INSERT INTO tariffs (operator_id, tariff_name, price_month)
                            VALUES (@opId, @name, @price)";
        cmd.Parameters.AddWithValue("@opId", tariff.OperatorId);
        cmd.Parameters.AddWithValue("@name", tariff.Name);
        cmd.Parameters.AddWithValue("@price", tariff.PriceMonth);
        cmd.ExecuteNonQuery();
    }

    public void UpdateTariff(Tariff tariff)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = @"UPDATE tariffs
                            SET operator_id = @opId, tariff_name = @name, price_month = @price
                            WHERE tariff_id = @id";
        cmd.Parameters.AddWithValue("@id", tariff.Id);
        cmd.Parameters.AddWithValue("@opId", tariff.OperatorId);
        cmd.Parameters.AddWithValue("@name", tariff.Name);
        cmd.Parameters.AddWithValue("@price", tariff.PriceMonth);
        cmd.ExecuteNonQuery();
    }

    public void DeleteTariff(int id)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM tariffs WHERE tariff_id = @id";
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }

    public (string[] columns, List<string[]> rows) ExecuteQuery(string sql)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        using var reader = cmd.ExecuteReader();

        string[] columns = new string[reader.FieldCount];
        for (int i = 0; i < reader.FieldCount; i++)
            columns[i] = reader.GetName(i);

        var rows = new List<string[]>();
        while (reader.Read())
        {
            string[] row = new string[reader.FieldCount];
            for (int i = 0; i < reader.FieldCount; i++)
                row[i] = reader.GetValue(i)?.ToString() ?? "";
            rows.Add(row);
        }
        return (columns, rows);
    }
}