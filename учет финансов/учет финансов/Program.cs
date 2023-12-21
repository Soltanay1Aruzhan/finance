using System;
using System.Collections.Generic;
using System.Threading;

namespace Banko
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Client client = new Client();
            Account account = new Account();
            Registration(client, account); // Вызов метода регистрации
        }

        public static void Registration(Client client, Account account)
        {
            Console.Write("Введите имя: ");
            client.FName = Console.ReadLine();

            Console.Write("Введите фамилию: ");
            client.LName = Console.ReadLine();

            Console.Write("Введите пол: ");
            client.Gender = Console.ReadLine();

            Console.Write("Введите дату рождения в формате дд.ММ.гггг (день.месяц.год): ");
            string input = Console.ReadLine();
            DateTime dob;
            while (!DateTime.TryParseExact(input, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out dob))
            {
                Console.WriteLine("Некорректный формат даты. Попробуйте еще раз.");
                input = Console.ReadLine();
            }
            client.Date = dob;

            account.AccountNumber = Banc.CreateAccountNumber();
            account.Password = Banc.CreatePassword();

            Console.WriteLine($"Ваш номер счета: {account.AccountNumber}");
            Console.WriteLine($"Ваш пароль: {account.Password}");

            Console.WriteLine("Положите сумму на счет: ");
            account.Balance = Convert.ToDouble(Console.ReadLine());

            Authorization(client, account); // После регистрации авторизуем клиента
        }

        public static void Authorization(Client client, Account account)
        {
            int attempts = 3;

            while (attempts > 0)
            {
                Console.Write("Введите номер счета: ");
                string accNum = Console.ReadLine();

                Console.Write("Введите пароль: ");
                string password = Console.ReadLine();

                if (account.AccountNumber == accNum && account.Password == password)
                {
                    Console.WriteLine("Приветствуем вас, " + client.FName);
                    Thread.Sleep(2000);
                    Menu(account); // После успешной авторизации показываем меню операций
                    return;
                }
                else
                {
                    attempts--;
                    Console.WriteLine($"Неверный номер счета или пароль. Осталось попыток: {attempts}");
                }
            }

            if (attempts == 0)
            {
                Console.WriteLine("Исчерпаны все попытки.");
            }
        }

        public static void Menu(Account account)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("1) Вывод баланса на экран");
                Console.WriteLine("2) Пополнение счета");
                Console.WriteLine("3) Снять деньги со счета");
                Console.WriteLine("4) Информация о транзакциях");
                Console.WriteLine("5) Выход");

                int choice;
                if (!int.TryParse(Console.ReadLine(), out choice))
                {
                    Console.WriteLine("Введите число от 1 до 5.");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        GetBalance(account);
                        break;
                    case 2:
                        MakeReplenishment(account);
                        break;
                    case 3:
                        MakeWithdrawal(account);
                        break;
                    case 4:
                        ReferenceTransactions(account);
                        break;
                    case 5:
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Выберите действие от 1 до 5.");
                        break;
                }
            }
        }

        public static void GetBalance(Account account)
        {
            Console.WriteLine($"Ваш баланс составляет: {account.Balance}");
            Console.WriteLine("Нажмите любую клавишу, чтобы вернуться в меню.");
            Console.ReadKey(true);
        }

        public static void MakeReplenishment(Account account)
        {
            Console.Write("Пополнение на сумму: ");
            double amount = Convert.ToDouble(Console.ReadLine());
            account.Balance += amount;
            Console.WriteLine($"Ваш баланс теперь составляет: {account.Balance}");

            // Добавление транзакции пополнения
            account.AddTransaction(amount, TransactionType.Deposit);

            Console.WriteLine("Нажмите любую клавишу, чтобы вернуться в меню.");
            Console.ReadKey(true);
        }

        public static void MakeWithdrawal(Account account)
        {
            Console.Write("Введите сумму для снятия: ");
            double amount = Convert.ToDouble(Console.ReadLine());

            if (amount > account.Balance)
            {
                Console.WriteLine("Сумма для снятия превышает сумму на счете.");
                Console.WriteLine("Нажмите любую клавишу, чтобы вернуться в меню.");
                Console.ReadKey(true);
            }
            else
            {
                account.Balance -= amount;
                Console.WriteLine($"Сумма снята. Ваш баланс теперь составляет: {account.Balance}");

                // Добавление транзакции снятия
                account.AddTransaction(amount, TransactionType.Withdrawal);

                Console.WriteLine("Нажмите любую клавишу, чтобы вернуться в меню.");
                Console.ReadKey(true);
            }
        }

        public static void ReferenceTransactions(Account account)
        {
            List<Transaction> transactions = account.GetTransactions();

            if (transactions.Count == 0)
            {
                Console.WriteLine("Нет доступных транзакций.");
            }
            else
            {
                Console.WriteLine("Информация о транзакциях:");
                foreach (Transaction transaction in transactions)
                {
                    Console.WriteLine($"Дата: {transaction.Date}, Тип: {transaction.Type}, Сумма: {transaction.Amount}");
                }
            }

            Console.WriteLine("Нажмите любую клавишу, чтобы вернуться в меню.");
            Console.ReadKey(true);
        }
    }

    // Класс Client и Account не определены в предоставленном коде, но предполагается их наличие
    public class Client
    {
        public string FName { get; set; }
        public string LName { get; set; }
        public string Gender { get; set; }
        public DateTime Date { get; set; }
    }

    public class Account
    {
        public string AccountNumber { get; set; }
        public string Password { get; set; }
        public double Balance { get; set; }
        private List<Transaction> transactions; // Добавлен список для хранения транзакций

        public Account()
        {
            transactions = new List<Transaction>();
        }

        public void AddTransaction(double amount, TransactionType type)
        {
            Transaction transaction = new Transaction
            {
                Date = DateTime.Now,
                Amount = amount,
                Type = type
            };
            transactions.Add(transaction);
        }

        public List<Transaction> GetTransactions()
        {
            return transactions;
        }
    }

    public enum TransactionType
    {
        Deposit,
        Withdrawal
    }

    public class Transaction
    {
        public DateTime Date { get; set; }
        public double Amount { get; set; }
        public TransactionType Type { get; set; }
    }

    // Предполагается, что у вас есть класс Banc с методами CreateAccountNumber и CreatePassword
    public static class Banc
    {
        public static string CreateAccountNumber()
        {
            // Реализация генерации номера счета
            return "1234567890"; // Пример значения
        }

        public static string CreatePassword()
        {
            // Реализация генерации пароля
            return "password123"; // Пример значения
        }
    }
}

