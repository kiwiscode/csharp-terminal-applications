using System.Text.Json;
using inventory_tracker.Models;
using static Globals;
using Spectre.Console;




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

        var name = AnsiConsole.Ask<string>("[bold green]Enter product name:[/]");
        var model = AnsiConsole.Ask<string>("[bold blue]Enter product model:[/]");
        var description = AnsiConsole.Ask<string>("[bold grey]Enter product description:[/]");
        var quantity = AnsiConsole.Ask<int>("[bold yellow]Enter quantity:[/]");
        var unitPrice = AnsiConsole.Ask<decimal>("[bold cyan]Enter unit price:[/]");

        var loadedProducts = LoadProducts();
        var newProduct = new Product { Id = Guid.NewGuid(), Name = name, Model = model, Description = description, Quantity = quantity, UnitPrice = unitPrice };

        loadedProducts.Add(newProduct);

        SaveProducts(loadedProducts);

    }
    public static void ListItems()
    {
        var loadedProducts = LoadProducts();

        if (loadedProducts.Count == 0)
        {
            var emptyTable = CreateTable(new List<Product>());
            AnsiConsole.Write(emptyTable);
            return;
        }

        int pageSize = 10;
        int page = 0;

        while (true)
        {
            var pageItems = loadedProducts.Skip(page * pageSize).Take(pageSize).ToList();
            if (pageItems.Count == 0)
            {
                Console.WriteLine("No more products.");
                break;
            }

            var table = CreateTable(pageItems);
            AnsiConsole.Write(table);

            Console.WriteLine($"\nPage {page + 1} of {Math.Ceiling(loadedProducts.Count / (double)pageSize)}");
            Console.WriteLine("[Enter] Next page, [B] Previous page, [Q] Quit");

            var key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.Q) break;
            if (key == ConsoleKey.B && page > 0) page--;
            else if (key == ConsoleKey.Enter) page++;
        }
    }

    private static Table CreateTable(List<Product> products)
    {
        var table = new Table();
        table.Title = new TableTitle("[bold underline rgb(85,88,253)]Inventory[/]");
        table.Border = TableBorder.Rounded;
        table.Expand();

        table.AddColumn(new TableColumn("[bold yellow]Name[/]").Centered());
        table.AddColumn(new TableColumn("[bold blue]Model[/]").Centered());
        table.AddColumn(new TableColumn("[bold grey]Description[/]").Centered());
        table.AddColumn(new TableColumn("[bold green]Quantity[/]").Centered());
        table.AddColumn(new TableColumn("[bold cyan]Unit Price[/]").Centered());
        table.AddColumn(new TableColumn("[bold magenta]Total Value[/]").Centered());

        if (products.Count == 0)
        {
            table.ShowRowSeparators();
            table.AddRow(
                new Markup("[grey]-[/]"),
                new Markup("[grey]-[/]"),
                new Markup("[grey]-[/]"),
                new Markup("[grey]-[/]"),
                new Markup("[grey]-[/]"),
                new Markup("[grey]-[/]")
            );
        }
        else
        {
            foreach (var product in products)
            {
                table.ShowRowSeparators();
                table.AddRow(
                    product.Name ?? "",
                    product.Model ?? "",
                    product.Description ?? "",
                    product.Quantity.ToString(),
                    product.UnitPrice.ToString("C"),
                    product.TotalValue.ToString("C")
                );
            }
        }

        return table;
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