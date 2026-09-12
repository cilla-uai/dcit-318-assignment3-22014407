using System;
using System.Collections.Generic;

namespace WarehouseInventorySystem
{
    // Marker interface for all inventory items
    public interface IInventoryItem
    {
        int Id { get; }
        string Name { get; }
        int Quantity { get; set; }
    }

    // Electronic product type
    public class ElectronicItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public string Brand { get; }
        public int WarrantyMonths { get; }

        public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            Brand = brand;
            WarrantyMonths = warrantyMonths;
        }
    }

    // Grocery product type
    public class GroceryItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; }

        public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            ExpiryDate = expiryDate;
        }
    }

    // Custom exceptions
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

    // Generic inventory repository constrained to inventory items
    public class InventoryRepository<T> where T : IInventoryItem
    {
        private readonly Dictionary<int, T> _items = new();

        public void AddItem(T item)
        {
            if (_items.ContainsKey(item.Id))
            {
                throw new DuplicateItemException($"An item with ID {item.Id} already exists in the inventory.");
            }
            _items[item.Id] = item;
        }

        public T GetItemById(int id)
        {
            if (!_items.TryGetValue(id, out var item))
            {
                throw new ItemNotFoundException($"No item found with ID {id}.");
            }
            return item;
        }

        public void RemoveItem(int id)
        {
            if (!_items.ContainsKey(id))
            {
                throw new ItemNotFoundException($"Cannot remove: no item found with ID {id}.");
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
                throw new InvalidQuantityException($"Quantity cannot be negative. Received: {newQuantity}.");
            }
            var item = GetItemById(id);
            item.Quantity = newQuantity;
        }
    }

    // Manager layer
    public class WareHouseManager
    {
        private readonly InventoryRepository<ElectronicItem> _electronics = new();
        private readonly InventoryRepository<GroceryItem> _groceries = new();

        public InventoryRepository<ElectronicItem> Electronics => _electronics;
        public InventoryRepository<GroceryItem> Groceries => _groceries;

        public void SeedData()
        {
            _electronics.AddItem(new ElectronicItem(1, "Laptop", 10, "Dell", 24));
            _electronics.AddItem(new ElectronicItem(2, "Smartphone", 25, "Samsung", 12));
            _electronics.AddItem(new ElectronicItem(3, "Television", 8, "LG", 36));

            _groceries.AddItem(new GroceryItem(1, "Rice", 100, DateTime.Now.AddMonths(6)));
            _groceries.AddItem(new GroceryItem(2, "Milk", 50, DateTime.Now.AddDays(7)));
            _groceries.AddItem(new GroceryItem(3, "Bread", 30, DateTime.Now.AddDays(3)));
        }

        public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
        {
            foreach (var item in repo.GetAllItems())
            {
                Console.WriteLine($"ID: {item.Id} | Name: {item.Name} | Quantity: {item.Quantity}");
            }
            Console.WriteLine();
        }

        public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
        {
            try
            {
                var item = repo.GetItemById(id);
                repo.UpdateQuantity(id, item.Quantity + quantity);
                Console.WriteLine($"Stock increased. {item.Name} (ID: {id}) now has {item.Quantity} units.");
            }
            catch (ItemNotFoundException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (InvalidQuantityException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
        {
            try
            {
                repo.RemoveItem(id);
                Console.WriteLine($"Item with ID {id} removed successfully.");
            }
            catch (ItemNotFoundException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    // Entry point
    public class Program
    {
        public static void Main()
        {
            var manager = new WareHouseManager();
            manager.SeedData();

            Console.WriteLine("=== GROCERY ITEMS ===");
            manager.PrintAllItems(manager.Groceries);

            Console.WriteLine("=== ELECTRONIC ITEMS ===");
            manager.PrintAllItems(manager.Electronics);

            Console.WriteLine("=== ERROR HANDLING TESTS ===");

            // Test 1: add a duplicate item
            try
            {
                manager.Electronics.AddItem(new ElectronicItem(1, "Tablet", 5, "Apple", 12));
            }
            catch (DuplicateItemException ex)
            {
                Console.WriteLine($"Caught: {ex.Message}");
            }

            // Test 2: remove a non-existent item
            try
            {
                manager.Groceries.RemoveItem(99);
            }
            catch (ItemNotFoundException ex)
            {
                Console.WriteLine($"Caught: {ex.Message}");
            }

            // Test 3: update with an invalid quantity
            try
            {
                manager.Electronics.UpdateQuantity(2, -5);
            }
            catch (InvalidQuantityException ex)
            {
                Console.WriteLine($"Caught: {ex.Message}");
            }
        }
    }
}