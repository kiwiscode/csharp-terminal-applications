using static Globals;
using inventory_tracker.Models;
using Spectre.Console;



static class Program
{
    static void Main()
    {


        if (!File.Exists("inventory.json"))
        {
            // Mock products
            InventoryList.AddRange(new List<Product>
            {
                new Product { Id = Guid.NewGuid(), Name = "Laptop", Quantity = 5, UnitPrice = 1200.50m },
                new Product { Id = Guid.NewGuid(), Name = "Mouse", Quantity = 10, UnitPrice = 25.99m },
                new Product { Id = Guid.NewGuid(), Name = "Keyboard", Quantity = 7, UnitPrice = 45.00m },
                new Product { Id = Guid.NewGuid(), Name = "Monitor", Quantity = 3, UnitPrice = 300.75m },
                new Product { Id = Guid.NewGuid(), Name = "USB Cable", Quantity = 15, UnitPrice = 10.50m }
            });

            // Save products
            Utils.SaveProducts(InventoryList);
        }
        do
        {
            AnsiConsole.Write("");



            // add, list, delete, update, total, save / load
            // additional (search, lowstock, import csv & export csv, history => command history, inventory backup)
            // string? action = "";


            Console.ReadLine();
        } while (!exitApp);


    }
}