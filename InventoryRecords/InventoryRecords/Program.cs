using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace InventoryRecordSystem
{
    // Marker interface so the logger can work with any inventory type
    public interface IInventoryEntity
    {
        int Id { get; }
    }

    // Immutable inventory item, records are perfect for this
    public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

    // Generic logger, works with any type that implements IInventoryEntity
    public class InventoryLogger<T> where T : IInventoryEntity
    {
        private readonly List<T> _log = new();
        private readonly string _filePath;

        public InventoryLogger(string filePath)
        {
            _filePath = filePath;
        }

        public void Add(T item)
        {
            _log.Add(item);
        }

        public List<T> GetAll()
        {
            return _log;
        }

        public void SaveToFile()
        {
            try
            {
                var json = JsonSerializer.Serialize(_log, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_filePath, json);
                Console.WriteLine($"Saved {_log.Count} items to {_filePath}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Could not save file: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Permission denied while saving: {ex.Message}");
            }
        }

        public void LoadFromFile()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    Console.WriteLine($"No saved file found at {_filePath}. Starting fresh.");
                    return;
                }

                var json = File.ReadAllText(_filePath);
                var items = JsonSerializer.Deserialize<List<T>>(json);

                _log.Clear();
                if (items != null) _log.AddRange(items);

                Console.WriteLine($"Loaded {_log.Count} items from {_filePath}");
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Saved file is corrupted: {ex.Message}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Could not read file: {ex.Message}");
            }
        }
    }

    public class InventoryApp
    {
        private readonly InventoryLogger<InventoryItem> _logger;

        public InventoryApp(string filePath)
        {
            _logger = new InventoryLogger<InventoryItem>(filePath);
        }

        public void SeedSampleData()
        {
            _logger.Add(new InventoryItem(1, "Laptop", 10, DateTime.Now));
            _logger.Add(new InventoryItem(2, "Mouse", 50, DateTime.Now));
            _logger.Add(new InventoryItem(3, "Keyboard", 35, DateTime.Now));
            _logger.Add(new InventoryItem(4, "Monitor", 15, DateTime.Now));
        }

        public void SaveData()
        {
            _logger.SaveToFile();
        }

        public void LoadData()
        {
            _logger.LoadFromFile();
        }

        public void PrintAllItems()
        {
            foreach (var item in _logger.GetAll())
            {
                Console.WriteLine($"ID: {item.Id} | Name: {item.Name} | Quantity: {item.Quantity} | Added: {item.DateAdded:d}");
            }
        }
    }

    public class Program
    {
        public static void Main()
        {
            // First session: seed data and save it to disk
            var app = new InventoryApp("inventory.json");
            app.SeedSampleData();
            app.SaveData();

            Console.WriteLine();

            // Simulate a new session by creating a fresh app instance
            var newSession = new InventoryApp("inventory.json");
            newSession.LoadData();
            newSession.PrintAllItems();
        }
    }
}