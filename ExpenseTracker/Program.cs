using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// In-memory database
List<Expense> expenses = new List<Expense>();

// Home Page (View All)
app.MapGet("/", async context =>
{
    string html = "<h1>Expense Tracker</h1>" +
                  "<a href='/add'>Add Expense</a><br><br>";

    html += "<table border='1'><tr><th>ID</th><th>Title</th><th>Amount</th><th>Action</th></tr>";

    foreach (var e in expenses)
    {
        html += $"<tr>" +
                $"<td>{e.Id}</td>" +
                $"<td>{e.Title}</td>" +
                $"<td>{e.Amount}</td>" +
                $"<td>" +
                $"<a href='/edit/{e.Id}'>Edit</a> | " +
                $"<a href='/delete/{e.Id}'>Delete</a>" +
                $"</td></tr>";
    }

    html += "</table>";

    await context.Response.WriteAsync(html);
});

// Add Page
app.MapGet("/add", async context =>
{
    string html = "<h2>Add Expense</h2>" +
                  "<form method='post'>" +
                  "Title: <input name='title'/><br>" +
                  "Amount: <input name='amount'/><br>" +
                  "<button type='submit'>Add</button>" +
                  "</form>";

    await context.Response.WriteAsync(html);
});

// Add POST
app.MapPost("/add", async context =>
{
    var form = await context.Request.ReadFormAsync();

    expenses.Add(new Expense
    {
        Id = expenses.Count + 1,
        Title = form["title"],
        Amount = double.Parse(form["amount"])
    });

    context.Response.Redirect("/");
});

// Edit Page
app.MapGet("/edit/{id:int}", async (int id, HttpContext context) =>
{
    var exp = expenses.FirstOrDefault(e => e.Id == id);

    string html = "<h2>Edit Expense</h2>" +
                  "<form method='post'>" +
                  $"Title: <input name='title' value='{exp.Title}'/><br>" +
                  $"Amount: <input name='amount' value='{exp.Amount}'/><br>" +
                  "<button type='submit'>Update</button>" +
                  "</form>";

    await context.Response.WriteAsync(html);
});

// Edit POST
app.MapPost("/edit/{id:int}", async (int id, HttpContext context) =>
{
    var exp = expenses.FirstOrDefault(e => e.Id == id);
    var form = await context.Request.ReadFormAsync();

    exp.Title = form["title"];
    exp.Amount = double.Parse(form["amount"]);

    context.Response.Redirect("/");
});

// Delete
app.MapGet("/delete/{id:int}", (int id) =>
{
    var exp = expenses.FirstOrDefault(e => e.Id == id);
    if (exp != null)
        expenses.Remove(exp);

    return Results.Redirect("/");
});

app.Run();

// Model
class Expense
{
    public int Id { get; set; }
    public string Title { get; set; }
    public double Amount { get; set; }
}
