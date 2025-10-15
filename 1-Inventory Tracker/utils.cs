using System.Text.Json;
using inventory_tracker.Models;


public static class Utils
{

    public static void SaveProducts(List<Product> productList)
    {
        string json = JsonSerializer.Serialize(productList, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("inventory.json", json);
    }

    public static List<Product> LoadProducts()
    {
        if (!File.Exists("inventory.json")) return new List<Product>();

        string json = File.ReadAllText("inventory.json");
        return JsonSerializer.Deserialize<List<Product>>(json) ?? new List<Product>();
    }


    public static void AddItem()
    {
        Console.WriteLine("Add Item");
    }
    public static void ListItems()
    {
        Console.WriteLine("List Items");
    }
    public static void DeleteItem()
    {
        Console.WriteLine("Delete Item");
    }
    public static void UpdateItem()
    {
        Console.WriteLine("Update Item");
    }
    public static void TotalValue()
    {
        Console.WriteLine("Total Value");
    }
    public static void SearchItem()
    {
        Console.WriteLine("Search Item");
    }
    public static void LowStock()
    {
        Console.WriteLine("Low Stock");
    }
    public static void ExportToCSV()
    {
        Console.WriteLine("Export To CSV");
    }

}