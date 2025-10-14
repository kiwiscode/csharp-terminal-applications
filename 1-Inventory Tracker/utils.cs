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

}