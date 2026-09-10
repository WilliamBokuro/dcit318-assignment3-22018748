using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace InventoryLoggingSystem
{
    // b. Marker interface for logging
    public interface IInventoryEntity
    {
        int Id { get; }
    }

    // a. Immutable record implementing interface using positional syntax
    public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

    // c. Generic Inventory Logger
    public class InventoryLogger<T> where T : IInventoryEntity
    {
        private List<T> _log = new List<T>();
        private string _filePath;

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
                string json = JsonSerializer.Serialize(_log, new JsonSerializerOptions { WriteIndented = true });
                using (StreamWriter writer = new StreamWriter(_filePath))
                {
                    writer.Write(json);
                }
                Console.WriteLine($"Data successfully persisted to '{_filePath}'.");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"File write error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Serialization error: {ex.Message}");
            }
        }

        public void LoadFromFile()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    Console.WriteLine("File not found. Initializing empty log.");
                    _log = new List<T>();
                    return;
                }

                using (StreamReader reader = new StreamReader(_filePath))
                {
                    string json = reader.ReadToEnd();
                    var items = JsonSerializer.Deserialize<List<T>>(json);
                    _log = items ?? new List<T>();
                }
                Console.WriteLine($"Data successfully loaded from '{_filePath}'.");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"File read error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Deserialization error: {ex.Message}");
            }
        }
    }

    // f. Integration Layer
    public class InventoryApp
    {
        private InventoryLogger<InventoryItem> _logger = new InventoryLogger<InventoryItem>("inventory_log.json");

        public void SeedSampleData()
        {
            _logger.Add(new InventoryItem(1, "Mechanical Keyboard", 15, DateTime.Now.AddDays(-10)));
            _logger.Add(new InventoryItem(2, "Wireless Mouse", 30, DateTime.Now.AddDays(-5)));
            _logger.Add(new InventoryItem(3, "4K Monitor", 8, DateTime.Now));
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
            Console.WriteLine("=== Current Loaded Inventory Items ===");
            foreach (var item in _logger.GetAll())
            {
                Console.WriteLine($"ID: {item.Id} | Name: {item.Name} | Qty: {item.Quantity} | Added: {item.DateAdded:yyyy-MM-dd}");
            }
        }

        // g. Main Application Flow
        public static void Main(string[] args)
        {
            // Session 1: Seed & Save
            Console.WriteLine("--- SESSION 1: Seeding and Saving Data ---");
            InventoryApp appSession1 = new InventoryApp();
            appSession1.SeedSampleData();
            appSession1.SaveData();

            // Clear memory / Simulate Session 2
            Console.WriteLine("\n--- SESSION 2: Simulating New Session & Reading from Disk ---");
            InventoryApp appSession2 = new InventoryApp();
            appSession2.LoadData();
            appSession2.PrintAllItems();
        }
    }
}