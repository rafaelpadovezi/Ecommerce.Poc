// See https://aka.ms/new-console-template for more information

using System.Net.Http.Json;

Console.WriteLine("Hello, World!");

using var httpClient = new HttpClient();
var random = new Random();

while (true)
{
    var result = await httpClient.PostAsJsonAsync("http://localhost:5002/order", new Order
    {
        OrderNumber = random.Next(9999).ToString().PadLeft(4, '0'),
        Items = new List<OrderItem>
        {
            new OrderItem
            {
                MaterialCode = "11",
                Quantity = 1
            }
        }
    });
    Console.WriteLine(result.StatusCode);
    await Task.Delay(100);
}

public class Order
{
    public Guid Id { get; set; }
    public DateTime CreationDate { get; set; } = DateTime.Now;
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    public string OrderNumber { get; set; }
}

public class OrderItem
{
    public Guid Id { get; set; }
    public string MaterialCode { get; set; }
    public int Quantity { get; set; }
}