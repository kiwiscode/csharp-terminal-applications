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
            Console.Clear();
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
                Console.Clear();
                Console.WriteLine("No more products.");
                break;
            }

            Console.Clear();

            var table = CreateTable(pageItems);
            AnsiConsole.Write(table);


            AnsiConsole.MarkupLine($"\nPage [yellow]{page + 1}[/] of [yellow]{Math.Ceiling(loadedProducts.Count / (double)pageSize)}[/]");
            AnsiConsole.MarkupLine("[grey]Enter : Next page | B : Previous page | Q : Quit[/]");


            var key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.Q) break;
            if (key == ConsoleKey.B && page > 0) page--;
            else if (key == ConsoleKey.Enter)
            {
                if ((page + 1) * pageSize < loadedProducts.Count)
                {
                    page++;
                }
            }
        }
    }

    private static Table CreateTable(List<Product> products, int selectedIndex = -1)
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
            for (int i = 0; i < products.Count; i++)
            {
                var product = products[i];
                table.ShowRowSeparators();

                if (i == selectedIndex)
                {
                    table.AddRow(
                        $"[black on cyan]{product.Name}[/]",
                        $"[black on cyan]{product.Model}[/]",
                        $"[black on cyan]{product.Description}[/]",
                        $"[black on cyan]{product.Quantity}[/]",
                        $"[black on cyan]{product.UnitPrice:C}[/]",
                        $"[black on cyan]{product.TotalValue:C}[/]"
                    );
                }
                else
                {
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
        }

        return table;
    }

    public static void DeleteItem()
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
        int selectedIndex = 0;

        while (true)
        {
            var pageItems = loadedProducts.Skip(page * pageSize).Take(pageSize).ToList();
            if (pageItems.Count == 0)
            {
                Console.Clear();
                AnsiConsole.MarkupLine("[red]No more products.[/]");
                break;
            }

            Console.Clear();

            var table = CreateTable(pageItems, selectedIndex);
            AnsiConsole.Write(table);

            AnsiConsole.MarkupLine($"\nPage [yellow]{page + 1}[/] of [yellow]{Math.Ceiling(loadedProducts.Count / (double)pageSize)}[/]");
            AnsiConsole.MarkupLine("[grey]↑ ↓ : Move | Enter : Delete | B : Previous page | N : Next page | Q : Quit[/]");

            var key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.Q)
                break;

            if (key == ConsoleKey.UpArrow)
                selectedIndex = (selectedIndex == 0) ? pageItems.Count - 1 : selectedIndex - 1;
            else if (key == ConsoleKey.DownArrow)
                selectedIndex = (selectedIndex + 1) % pageItems.Count;

            else if (key == ConsoleKey.B && page > 0)
            {
                page--;
                selectedIndex = 0;
            }
            else if (key == ConsoleKey.N)
            {
                if ((page + 1) * pageSize < loadedProducts.Count)
                {
                    page++;
                    selectedIndex = 0;
                }
            }

            else if (key == ConsoleKey.Enter)
            {
                var itemToDelete = pageItems[selectedIndex];
                bool confirm = AnsiConsole.Confirm($"Delete [red]{itemToDelete.Name}[/]?");
                if (confirm)
                {
                    loadedProducts.Remove(itemToDelete);
                    SaveProducts(loadedProducts);

                    AnsiConsole.MarkupLine($"[red]{itemToDelete.Name} deleted![/]");
                    System.Threading.Thread.Sleep(700);

                    if (page * pageSize >= loadedProducts.Count && page > 0)
                        page--;

                    selectedIndex = 0;
                }

                if (loadedProducts.Count == 0)
                {
                    Console.Clear();
                    AnsiConsole.MarkupLine("[red]All products deleted![/]");
                    break;
                }
            }
        }
    }

    public static void UpdateItem()
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
        int selectedIndex = 0;

        while (true)
        {
            var pageItems = loadedProducts.Skip(page * pageSize).Take(pageSize).ToList();
            if (pageItems.Count == 0)
            {
                Console.Clear();
                AnsiConsole.MarkupLine("[red]No more products.[/]");
                break;
            }

            Console.Clear();

            var table = CreateTable(pageItems, selectedIndex);
            AnsiConsole.Write(table);

            AnsiConsole.MarkupLine($"\nPage [yellow]{page + 1}[/] of [yellow]{Math.Ceiling(loadedProducts.Count / (double)pageSize)}[/]");
            AnsiConsole.MarkupLine("[grey]↑ ↓ : Move | Enter : Update | B : Previous page | N : Next page | Q : Quit[/]");

            var key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.Q)
                break;

            if (key == ConsoleKey.UpArrow)
                selectedIndex = (selectedIndex == 0) ? pageItems.Count - 1 : selectedIndex - 1;
            else if (key == ConsoleKey.DownArrow)
                selectedIndex = (selectedIndex + 1) % pageItems.Count;

            else if (key == ConsoleKey.B && page > 0)
            {
                page--;
                selectedIndex = 0;
            }
            else if (key == ConsoleKey.N)
            {
                if ((page + 1) * pageSize < loadedProducts.Count)
                {
                    page++;
                    selectedIndex = 0;
                }
            }

            else if (key == ConsoleKey.Enter)
            {
                var itemToUpdate = pageItems[selectedIndex];
                var existingItem = loadedProducts.FirstOrDefault(p => p.Id == itemToUpdate.Id);

                if (existingItem != null)
                {
                    Console.Clear();
                    AnsiConsole.MarkupLine($"[bold underline]Updating {existingItem.Name}[/]\n");

                    var newName = AnsiConsole.Ask<string>($"Name ([grey]{existingItem.Name}[/]):");
                    if (!string.IsNullOrWhiteSpace(newName)) existingItem.Name = newName;

                    var newModel = AnsiConsole.Ask<string>($"Model ([grey]{existingItem.Model}[/]):");
                    if (!string.IsNullOrWhiteSpace(newModel)) existingItem.Model = newModel;

                    var newDescription = AnsiConsole.Ask<string>($"Description ([grey]{existingItem.Description}[/]):");
                    if (!string.IsNullOrWhiteSpace(newDescription)) existingItem.Description = newDescription;

                    var newQuantityInput = AnsiConsole.Ask<string>($"Quantity ([grey]{existingItem.Quantity}[/]):");
                    if (int.TryParse(newQuantityInput, out int newQuantity)) existingItem.Quantity = newQuantity;

                    var newPriceInput = AnsiConsole.Ask<string>($"Unit Price ([grey]{existingItem.UnitPrice}[/]):");
                    if (decimal.TryParse(newPriceInput, out decimal newPrice)) existingItem.UnitPrice = newPrice;

                    SaveProducts(loadedProducts);

                    AnsiConsole.MarkupLine($"[green]{existingItem.Name} updated![/]");
                    System.Threading.Thread.Sleep(700);

                    if (page * pageSize >= loadedProducts.Count && page > 0)
                        page--;

                    selectedIndex = 0;
                }
            }
        }
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