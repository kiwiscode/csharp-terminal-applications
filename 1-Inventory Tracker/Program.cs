using inventory_tracker.Models;
using Spectre.Console;
using System.Text.RegularExpressions;
using static Globals;



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

            var choices = new Dictionary<int, string>()
{
    {1, "[bold green]Add Item[/]"},
    {2, "[bold blue]List Items[/]"},
    {3, "[bold red]Delete Item[/]"},
    {4, "[bold orange1]Update Item[/]"},
    {5, "[bold deepskyblue1]Total Value[/]"},
    {6, "[bold white]Search[/]"},
    {7, "[bold darkred]Low Stock Items[/]"},
    {8, "[bold magenta]Export to CSV[/]"},
    {0, "[bold red3]Exit[/]"}
};

            var action = AnsiConsole.Prompt(
                new SelectionPrompt<int>()
                    .Title("[bold rgb(85,88,253)]What do you want to do?[/]")
                    .PageSize(10)
                    .AddChoices(choices.Keys.ToArray())
                    .UseConverter(i => choices[i])
            );


            Console.WriteLine($"Selected action: {action}");

            switch (action)
            {
                case 1: Utils.AddItem(); break;
                case 2: Utils.ListItems(); break;
                case 3: Utils.DeleteItem(); break;
                case 4: Utils.UpdateItem(); break;
                case 5: Utils.TotalValue(); break;
                case 6: Utils.SearchItem(); break;
                case 7: Utils.LowStock(); break;
                case 8: Utils.ExportToCSV(); break;
                case 0: exitApp = true; break;
                default: Console.WriteLine("Invalid Choice"); break;
            }





            // add, list, delete, update, total, save / load
            // additional (search, lowstock, import csv & export csv, history => command history, inventory backup)


            Console.ReadLine();
        } while (!exitApp);


    }
}