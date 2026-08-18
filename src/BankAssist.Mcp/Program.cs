using BankAssist.Mcp.Configuration;
using BankAssist.Mcp.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<DataOptions>(builder.Configuration.GetSection(DataOptions.SectionName));

var dataOptions = builder.Configuration.GetSection(DataOptions.SectionName).Get<DataOptions>() ?? new();

// Đường dẫn tương đối tính theo thư mục chứa binary, không theo working directory,
// để `dotnet run --project src/BankAssist.Mcp` và bản publish đều tìm thấy data/.
var dataDirectory = Path.IsPathRooted(dataOptions.Directory)
    ? dataOptions.Directory
    : Path.Combine(AppContext.BaseDirectory, dataOptions.Directory);

try
{
    var store = CrmDataStore.LoadFrom(dataDirectory);
    builder.Services.AddSingleton(store);
}
catch (InvalidOperationException ex)
{
    Console.Error.WriteLine($"Lỗi nạp dữ liệu: {ex.Message}");
    Environment.Exit(1);
}

builder.Services.AddScoped<ICrmRepository, JsonCrmRepository>();
builder.Services.AddSingleton<TimeProvider>(TimeProvider.System);

builder.Services
    .AddMcpServer()
    .WithHttpTransport()
    .WithToolsFromAssembly();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapMcp("/mcp");

await app.RunAsync();

public partial class Program { }

