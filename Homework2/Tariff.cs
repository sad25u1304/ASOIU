/// <summary>
/// Тариф (основная таблица, сторона «много»)
/// </summary>
class Tariff
{
    public int Id { get; set; }
    public int OperatorId { get; set; }
    public string Name { get; set; }

    private int _priceMonth;

    public int PriceMonth
    {
        get => _priceMonth;
        set
        {
            if (value < 0)
                throw new ArgumentException("Абонентская плата не может быть отрицательной");
            _priceMonth = value;
        }
    }

    public Tariff(int id, int operatorId, string name, int priceMonth)
    {
        Id = id;
        OperatorId = operatorId;
        Name = name;
        PriceMonth = priceMonth;
    }

    public Tariff() : this(0, 0, "", 0) { }

    public override string ToString() => $"[{Id}] {Name}, оператор #{OperatorId}, {PriceMonth} руб.";
}