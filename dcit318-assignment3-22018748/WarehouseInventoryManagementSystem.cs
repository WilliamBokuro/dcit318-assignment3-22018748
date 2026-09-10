using System;
using System.Collections.Generic;

namespace WarehouseInventorySystem
{
    // a. Marker Interface
    public interface IInventoryItem
    {
        int Id { get; }
        string Name { get; }
        int Quantity { get; set; }
    }

    // b. ElectronicItem Product Class
    public class ElectronicItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public string Brand { get; set; }
        public int WarrantyMonths { get; set; }

        public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            Brand = brand;
            WarrantyMonths = warrantyMonths;
        }
    }

    // c. GroceryItem Product Class
    public class GroceryItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; set; }

        public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            ExpiryDate = expiryDate;
        }
    }

    // e. Custom Exception Classes
    public class DuplicateItemException : Exception
    {
        public DuplicateItemException(string message) : base(message) { }
    }

    public class ItemNotFoundException : Exception
    {
        public ItemNotFoundException(string message) : base(message) { }
    }

    public class InvalidQuantityException : Exception
    {
        public InvalidQuantityException(string message) : base(message) { }
    }

    // d. Generic Inventory Repository
    public class InventoryRepository<T> where T : IInventoryItem
    {
        private Dictionary<int, T> _items = new Dictionary<int, T>();

        public void AddItem(T item)
        {
            if (_items.ContainsKey(item.Id))
            {
                throw new DuplicateItemException($"Item with ID {item.Id} already exists in repository.");
            }
            _items.Add(item.Id, item);
        }

        public T GetItemById(int id)
        {
            if (!_items.TryGetValue(id, out T? item))
            {
                throw new ItemNotFoundException($"Item with ID {id} was not found.");
            }
            return item;
        }

        public void RemoveItem(int id)
        {
            if (!_items.ContainsKey(id))
            {
                throw new ItemNotFoundException($"Cannot remove. Item with ID {id} was not found.");
            }
            _items.Remove(id);
        }

        public List<T> GetAllItems()
        {
            return new List<T>(_items.Values);
        }

        public void UpdateQuantity(int id, int newQuantity)
        {
            if (newQuantity < 0)
            {
                throw new InvalidQuantityException($"Quantity cannot be negative ({newQuantity}).");
            }

            T item = GetItemById(id);
            item.Quantity = newQuantity;
        }
    }

    // f. WarehouseManager Class
    public class WarehouseManager
    {
        private InventoryRepository<ElectronicItem> _electronics = new InventoryRepository<ElectronicItem>();
        private InventoryRepository<GroceryItem> _groceries = new InventoryRepository<GroceryItem>();

        public InventoryRepository<ElectronicItem> Electronics => _electronics;
        public InventoryRepository<GroceryItem> Groceries => _groceries;

        public void SeedData()
        {
            _electronics.AddItem(new ElectronicItem(1, "Laptop", 10, "Dell", 24));
            _electronics.AddItem(new ElectronicItem(2, "Smartphone", 25, "Samsung", 12));

            _groceries.AddItem(new GroceryItem(101, "Rice", 50, DateTime.Now.AddMonths(6)));
            _groceries.AddItem(new GroceryItem(102, "Milk", 30, DateTime.Now.AddDays(14)));
        }

        public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
        {
            foreach (var item in repo.GetAllItems())
            {
                Console.WriteLine($"ID: {item.Id} | Name: {item.Name} | Qty: {item.Quantity}");
            }
        }

        public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
        {
            try
            {
                T item = repo.GetItemById(id);
                repo.UpdateQuantity(id, item.Quantity + quantity);
                Console.WriteLine($"Successfully updated stock for {item.Name}. New Qty: {item.Quantity}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating stock: {ex.Message}");
            }
        }

        public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
        {
            try
            {
                repo.RemoveItem(id);
                Console.WriteLine($"Item ID {id} removed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing item: {ex.Message}");
            }
        }

        public static void Main(string[] args)
        {
            WarehouseManager manager = new WarehouseManager();
            manager.SeedData();

            Console.WriteLine("=== Electronic Items ===");
            manager.PrintAllItems(manager.Electronics);
            Console.WriteLine();

            Console.WriteLine("=== Grocery Items ===");
            manager.PrintAllItems(manager.Groceries);
            Console.WriteLine();

            Console.WriteLine("=== Testing Exception Scenarios ===");

            // 1. Duplicate item
            try
            {
                Console.WriteLine("Attempting to add duplicate electronic item (ID 1)...");
                manager.Electronics.AddItem(new ElectronicItem(1, "Tablet", 5, "Apple", 12));
            }
            catch (DuplicateItemException ex)
            {
                Console.WriteLine($"[CAUGHT EXCEPTION] {ex.Message}");
            }

            // 2. Remove non-existent item
            try
            {
                Console.WriteLine("\nAttempting to remove non-existent item (ID 999)...");
                manager.RemoveItemById(manager.Electronics, 999);
            }
            catch (ItemNotFoundException ex)
            {
                Console.WriteLine($"[CAUGHT EXCEPTION] {ex.Message}");
            }

            // 3. Update with invalid quantity
            try
            {
                Console.WriteLine("\nAttempting to set negative quantity for Rice (ID 101)...");
                manager.Groceries.UpdateQuantity(101, -15);
            }
            catch (InvalidQuantityException ex)
            {
                Console.WriteLine($"[CAUGHT EXCEPTION] {ex.Message}");
            }
        }
    }
}