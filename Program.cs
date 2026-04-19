using System.Text;
using Microsoft.Data.Sqlite;

namespace Homework2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            string dbPath = "operators.db";
            string operatorsCsv = @"C:\Users\andre\source\repos\Homework2\Data\operators.csv";
            string tariffsCsv = @"C:\Users\andre\source\repos\Homework2\Data\tariffs.csv";

            var db = new DatabaseManager(dbPath);
            db.InitializeDatabase(operatorsCsv, tariffsCsv);

            Console.WriteLine();

            string choice;
            do
            {
                Console.WriteLine("╔════════════════════════════════════════════╗");
                Console.WriteLine("║        УПРАВЛЕНИЕ ТАРИФАМИ ОПЕРАТОРОВ      ║");
                Console.WriteLine("╠════════════════════════════════════════════╣");
                Console.WriteLine("║  1 — Показать всех операторов              ║");
                Console.WriteLine("║  2 — Показать все тарифы                    ║");
                Console.WriteLine("║  3 — Добавить тариф                         ║");
                Console.WriteLine("║  4 — Редактировать тариф                    ║");
                Console.WriteLine("║  5 — Удалить тариф                          ║");
                Console.WriteLine("║  6 — Отчёты                                 ║");
                Console.WriteLine("║  0 — Выход                                  ║");
                Console.WriteLine("╚════════════════════════════════════════════╝");
                Console.Write("Ваш выбор: ");

                choice = Console.ReadLine()?.Trim() ?? "";
                Console.WriteLine();

                switch (choice)
                {
                    case "1": ShowOperators(db); break;
                    case "2": ShowTariffs(db); break;
                    case "3": AddTariff(db); break;
                    case "4": EditTariff(db); break;
                    case "5": DeleteTariff(db); break;
                    case "6": ReportsMenu(db); break;
                    case "0": Console.WriteLine("Программа завершена"); break;
                    default: Console.WriteLine("Неверный пункт меню."); break;
                }
                Console.WriteLine();
            } while (choice != "0");
        }

        static void ShowOperators(DatabaseManager db)
        {
            Console.WriteLine("---- Все операторы ----");
            var operators = db.GetAllOperators();
            foreach (var op in operators)
                Console.WriteLine(" " + op);
            Console.WriteLine($"Итого: {operators.Count}");
        }

        static void ShowTariffs(DatabaseManager db)
        {
            Console.WriteLine("---- Все тарифы ----");
            var tariffs = db.GetAllTariffs();
            foreach (var t in tariffs)
                Console.WriteLine(" " + t);
            Console.WriteLine($"Итого: {tariffs.Count}");
        }

        static void AddTariff(DatabaseManager db)
        {
            Console.WriteLine("---- Добавление тарифа ----");
            Console.WriteLine("Доступные операторы:");
            var operators = db.GetAllOperators();
            foreach (var op in operators)
                Console.WriteLine(" " + op);

            Console.Write("ID оператора: ");
            if (!int.TryParse(Console.ReadLine(), out int operatorId))
            {
                Console.WriteLine("Ошибка: введите целое число.");
                return;
            }

            Console.Write("Название тарифа: ");
            string name = Console.ReadLine()?.Trim() ?? "";
            if (name.Length == 0)
            {
                Console.WriteLine("Ошибка: название не может быть пустым.");
                return;
            }

            Console.Write("Абонентская плата (руб./мес.): ");
            if (!int.TryParse(Console.ReadLine(), out int price))
            {
                Console.WriteLine("Ошибка: введите целое число.");
                return;
            }

            try
            {
                var tariff = new Tariff(0, operatorId, name, price);
                db.AddTariff(tariff);
                Console.WriteLine("Тариф добавлен.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        static void EditTariff(DatabaseManager db)
        {
            Console.WriteLine("---- Редактирование тарифа ----");
            Console.Write("Введите ID тарифа: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Ошибка: введите целое число.");
                return;
            }

            var tariff = db.GetTariffById(id);
            if (tariff == null)
            {
                Console.WriteLine($"Тариф с ID={id} не найден.");
                return;
            }

            Console.WriteLine($"Текущие данные: {tariff}");
            Console.WriteLine("(Нажмите Enter, чтобы оставить значение без изменений)");

            Console.Write($"Название [{tariff.Name}]: ");
            string input = Console.ReadLine()?.Trim() ?? "";
            if (input.Length > 0) tariff.Name = input;

            Console.Write($"ID оператора [{tariff.OperatorId}]: ");
            input = Console.ReadLine()?.Trim() ?? "";
            if (input.Length > 0 && int.TryParse(input, out int newOpId))
                tariff.OperatorId = newOpId;

            Console.Write($"Цена [{tariff.PriceMonth}]: ");
            input = Console.ReadLine()?.Trim() ?? "";
            if (input.Length > 0 && int.TryParse(input, out int newPrice))
            {
                try
                {
                    tariff.PriceMonth = newPrice;
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                    return;
                }
            }

            db.UpdateTariff(tariff);
            Console.WriteLine("Данные обновлены.");
        }

        static void DeleteTariff(DatabaseManager db)
        {
            Console.WriteLine("---- Удаление тарифа ----");
            Console.Write("Введите ID тарифа: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Ошибка: введите целое число.");
                return;
            }

            var tariff = db.GetTariffById(id);
            if (tariff == null)
            {
                Console.WriteLine($"Тариф с ID={id} не найден.");
                return;
            }

            Console.Write($"Удалить «{tariff.Name}»? (да/нет): ");
            string confirm = Console.ReadLine()?.Trim().ToLower() ?? "";
            if (confirm == "да")
            {
                db.DeleteTariff(id);
                Console.WriteLine("Тариф удалён.");
            }
            else
            {
                Console.WriteLine("Удаление отменено.");
            }
        }

        static void ReportsMenu(DatabaseManager db)
        {
            string choice;
            do
            {
                Console.WriteLine("--- Отчёты ---");
                Console.WriteLine(" 1 - Тарифы с названиями операторов");
                Console.WriteLine(" 2 - Количество тарифов по операторам");
                Console.WriteLine(" 3 - Средняя цена тарифа по операторам");
                Console.WriteLine(" 0 - Назад");
                Console.Write("Ваш выбор: ");
                choice = Console.ReadLine()?.Trim() ?? "";

                switch (choice)
                {
                    case "1": Report1_TariffsWithOperators(db); break;
                    case "2": Report2_CountByOperator(db); break;
                    case "3": Report3_AvgPriceByOperator(db); break;
                    case "0": break;
                    default: Console.WriteLine("Неверный пункт."); break;
                }
                Console.WriteLine();
            } while (choice != "0");
        }

        static void Report1_TariffsWithOperators(DatabaseManager db)
        {
            new ReportBuilder(db)
                .Query(@"SELECT t.tariff_name, o.operator_name, t.price_month
                         FROM tariffs t
                         JOIN operators o ON t.operator_id = o.operator_id
                         ORDER BY t.tariff_name")
                .Title("Тарифы по операторам")
                .Header("Тариф", "Оператор", "Цена (руб.)")
                .ColumnWidths(20, 15, 12)
                .Numbered()
                .Print();
        }

        static void Report2_CountByOperator(DatabaseManager db)
        {
            new ReportBuilder(db)
                .Query(@"SELECT o.operator_name, COUNT(*) AS cnt
                         FROM tariffs t
                         JOIN operators o ON t.operator_id = o.operator_id
                         GROUP BY o.operator_name
                         ORDER BY o.operator_name")
                .Title("Количество тарифов по операторам")
                .Header("Оператор", "Кол-во тарифов")
                .ColumnWidths(20, 12)
                .Print();
        }

        static void Report3_AvgPriceByOperator(DatabaseManager db)
        {
            new ReportBuilder(db)
                .Query(@"SELECT o.operator_name, ROUND(AVG(t.price_month), 1) AS avg_price
                         FROM tariffs t
                         JOIN operators o ON t.operator_id = o.operator_id
                         GROUP BY o.operator_name
                         ORDER BY avg_price DESC")
                .Title("Средняя цена тарифа по операторам")
                .Header("Оператор", "Средняя цена (руб.)")
                .ColumnWidths(20, 20)
                .Print();
        }
    }
}