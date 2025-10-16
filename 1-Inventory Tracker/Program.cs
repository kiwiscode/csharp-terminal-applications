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
            var mockProducts = new List<Product>();
            var random = new Random();

            string[] productNames = { "Laptop", "Mouse", "Keyboard", "Monitor", "USB Cable", "Headphones", "Webcam", "Printer", "Speaker", "External HDD" };
            string[] models = { "Model A", "Model B", "Model C", "Model D", "Model E" };
            string[] descriptions = { "High performance", "Ergonomic", "Gaming", "Wireless", "Portable", "Professional", "Budget", "Premium" };

            for (int i = 0; i < 100; i++) // 100 product
            {
                var name = productNames[random.Next(productNames.Length)];
                var model = models[random.Next(models.Length)] + " " + (i + 1);
                var description = descriptions[random.Next(descriptions.Length)] + " " + name.ToLower();
                var quantity = random.Next(1, 20);
                var unitPrice = Math.Round((decimal)(random.NextDouble() * 500 + 10), 2);

                mockProducts.Add(new Product
                {
                    Id = Guid.NewGuid(),
                    Name = name,
                    Model = model,
                    Description = description,
                    Quantity = quantity,
                    UnitPrice = unitPrice
                });
            }

            InventoryList.AddRange(mockProducts);

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


        } while (!exitApp);


    }
}