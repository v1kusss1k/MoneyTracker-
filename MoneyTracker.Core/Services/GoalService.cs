// MoneyTracker.Core/Services/GoalService.cs
using MoneyTracker.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace MoneyTracker.Core.Services
{
    public class GoalService
    {
        private List<Goal> _goals;
        private readonly string _filePath;

        private List<Goal> _cachedGoals;
        private DateTime _goalsCacheTime;
        public GoalService()
        {
            _filePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "MoneyTracker", "goals.json");
            _goals = new List<Goal>();
            LoadGoals();
        }

        private void LoadGoals()
        {
            try
            {
                if (File.Exists(_filePath))
                {
                    var json = File.ReadAllText(_filePath);
                    var loadedGoals = JsonSerializer.Deserialize<List<Goal>>(json);
                    if (loadedGoals != null)
                    {
                        _goals = loadedGoals;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки целей: {ex.Message}");
            }
        }

        private void SaveGoals()
        {
            try
            {
                var dir = Path.GetDirectoryName(_filePath);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                var json = JsonSerializer.Serialize(_goals, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка сохранения целей: {ex.Message}");
            }
        }

        // Метод был GetAllGoals, исправляем на GetGoals
        public List<Goal> GetGoals()
        {
            // Кэшируем на 10 секунд
            if (_cachedGoals != null && (DateTime.Now - _goalsCacheTime).TotalSeconds < 10)
            {
                return _cachedGoals.Where(g => !g.IsArchived).ToList();
            }

            LoadGoals();
            _cachedGoals = _goals;
            _goalsCacheTime = DateTime.Now;

            return _goals.Where(g => !g.IsArchived).ToList();
        }

        public Goal? GetGoalById(Guid id)
        {
            return _goals.FirstOrDefault(g => g.Id == id);
        }

        public void AddGoal(Goal goal)
        {
            if (goal == null)
                throw new ArgumentNullException(nameof(goal));

            _goals.Add(goal);
            SaveGoals();
            // СБРАСЫВАЕМ КЭШ
            _cachedGoals = null;
        }

        public void UpdateGoal(Goal goal)
        {
            if (goal == null)
                throw new ArgumentNullException(nameof(goal));

            var existing = GetGoalById(goal.Id);
            if (existing != null)
            {
                existing.Name = goal.Name;
                existing.Description = goal.Description;
                existing.TargetAmount = goal.TargetAmount;
                existing.CurrentAmount = goal.CurrentAmount;
                existing.TargetDate = goal.TargetDate;
                existing.IsCompleted = goal.IsCompleted;
                existing.Icon = goal.Icon;
                existing.Color = goal.Color;
                existing.IsArchived = goal.IsArchived;
                existing.WasNotified = goal.WasNotified;
                SaveGoals();
                // СБРАСЫВАЕМ КЭШ
                _cachedGoals = null;
            }
        }

        public void DeleteGoal(Guid id)
        {
            _goals.RemoveAll(g => g.Id == id);
            SaveGoals();
            // СБРАСЫВАЕМ КЭШ
            _cachedGoals = null;
        }

        public void AddToGoal(Guid goalId, decimal amount)
        {
            var goal = GetGoalById(goalId);
            if (goal != null && amount > 0)
            {
                goal.AddAmount(amount);
                SaveGoals();
                // СБРАСЫВАЕМ КЭШ
                _cachedGoals = null;
            }
        }
    }
}