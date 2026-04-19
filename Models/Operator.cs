/// <summary>
/// Оператор связи (справочная таблица, сторона «один»)
/// </summary>
class Operator
{
    public int Id { get; set; }
    public string Name { get; set; }

    public Operator(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public Operator() : this(0, "") { }

    public override string ToString() => $"[{Id}] {Name}";
}