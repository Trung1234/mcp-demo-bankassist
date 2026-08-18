namespace BankAssist.Mcp.Tests;

using BankAssist.Mcp.Data;
using BankAssist.Mcp.Models;
using BankAssist.Mcp.Tools;
using Microsoft.Extensions.Logging.Abstractions;

/// <summary>
/// TEST-SPEC mục 6 — tool khách hàng.
/// </summary>
[Trait("Category", "Tools")]
public sealed class CustomerToolsTests
{
    [Fact(DisplayName = "T-05: get_customer với mã không tồn tại - rỗng, có thông báo, không exception")]
    public async Task GetCustomer_WithUnknownId_ReturnsEmptyWithMessage()
    {
        var tools = CreateTools();

        var result = await tools.GetCustomerAsync("CUS9999", CancellationToken.None);

        result.Count.Should().Be(0);
        result.Items.Should().BeEmpty();
        result.Message.Should().NotBeNullOrWhiteSpace();
    }

    private static CustomerTools CreateTools() => new(
        new JsonCrmRepository(CreateStore(), TimeProvider.System),
        NullLogger<CustomerTools>.Instance);

    private static CrmDataStore CreateStore()
    {
        var customers = new List<Customer>
        {
            new("CUS0001", "Trần Thị Bích Ngọc", "0912345678", "001199012345", "19001234567890", "bich.ngoc@example.com", "Priority", "RM007", new DateOnly(2019, 4, 12)),
            new("CUS0002", "Lê Hoàng Vũ", "0987654321", "001199054321", "19001234567891", "vu.le@example.com", "Priority", "RM007", new DateOnly(2020, 1, 5)),
        };

        return new CrmDataStore(customers, [], [], [], []);
    }
}
