#nullable disable

using MoneyTracker.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;

namespace MoneyTracker.Core
{
    public static class FileManager
    {
        private static readonly string DataFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "MoneyTracker");

        private static readonly string TransactionsFile = Path.Combine(DataFolder, "transactions.json");
        private static readonly string SettingsFile = Path.Combine(DataFolder, "settings.json");

        private static Timer _autoSaveTimer;
        private static List<Transaction> _transactionsToSave;
        private static bool _autoSaveEnabled = true;

        static FileManager()
        {
            if (!Directory.Exists(DataFolder))
                Directory.CreateDirectory(DataFolder);

            LoadAutoSaveSettings();
            StartAutoSaveTimer();
        }

        private static void LoadAutoSaveSettings()
        {
            try
            {
                if (File.Exists(SettingsFile))
                {
                    var json = File.ReadAllText(SettingsFile);
                    var settings = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
                    if (settings != null && settings.ContainsKey("AutoSave"))
                    {
                        _autoSaveEnabled = settings["AutoSave"].GetBoolean();
                    }
                }
            }
            catch { }
        }

        private static void StartAutoSaveTimer()
        {
            _autoSaveTimer = new Timer(AutoSaveCallback, null, 30000, 30000);
        }

        private static void AutoSaveCallback(object state)
        {
            if (_autoSaveEnabled && _transactionsToSave != null && _transactionsToSave.Count > 0)
            {
                SaveTransactionsInternal(_transactionsToSave);
                _transactionsToSave = null;
            }
        }

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

        public static void SaveTransactions(List<Transaction> transactions)
        {
            if (_autoSaveEnabled)
            {
                _transactionsToSave = transactions;
            }
            else
            {
                SaveTransactionsInternal(transactions);
            }
        }

        private static void SaveTransactionsInternal(List<Transaction> transactions)
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

        public static void SetAutoSaveEnabled(bool enabled)
        {
            _autoSaveEnabled = enabled;

            if (!enabled && _transactionsToSave != null)
            {
                SaveTransactionsInternal(_transactionsToSave);
                _transactionsToSave = null;
            }
        }

        public static void SaveJsonFile<T>(string fileName, T data)
        {
            try
            {
                var filePath = Path.Combine(DataFolder, fileName);
                var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка сохранения {fileName}: {ex.Message}");
            }
        }

        public static T LoadJsonFile<T>(string fileName) where T : new()
        {
            try
            {
                var filePath = Path.Combine(DataFolder, fileName);
                if (File.Exists(filePath))
                {
                    var json = File.ReadAllText(filePath);
                    return JsonSerializer.Deserialize<T>(json) ?? new T();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки {fileName}: {ex.Message}");
            }

            return new T();
        }
    }
}