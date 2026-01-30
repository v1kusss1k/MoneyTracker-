using MoneyTracker.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace MoneyTracker.Core
{
    // менеджер для работы с файлами использует json для сериализации
    public static class FileManager
    {
        // путь к папке с данными в appdata
        private static readonly string DataFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "MoneyTracker");

        // путь к файлу транзакций
        private static readonly string TransactionsFile = Path.Combine(DataFolder, "transactions.json");

        // статический конструктор создает папку при первом обращении
        static FileManager()
        {
            if (!Directory.Exists(DataFolder))
                Directory.CreateDirectory(DataFolder);
        }

        // загрузка транзакций из файла
        public static List<Transaction> LoadTransactions()
        {
            try
            {
                if (!File.Exists(TransactionsFile))
                    return new List<Transaction>();

                var json = File.ReadAllText(TransactionsFile);
                return JsonSerializer.Deserialize<List<Transaction>>(json) ?? new List<Transaction>();
            }
            catch
            {
                return new List<Transaction>();
            }
        }

        // сохранение транзакций в файл
        public static void SaveTransactions(List<Transaction> transactions)
        {
            try
            {
                var json = JsonSerializer.Serialize(transactions, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(TransactionsFile, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка сохранения: {ex.Message}");
            }
        }
    }
}