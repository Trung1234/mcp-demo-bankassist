using System.ComponentModel;
using BankAssist.Mcp.Data;
using BankAssist.Mcp.Models;
using ModelContextProtocol.Server;

namespace BankAssist.Mcp.Tools;

/// <summary>
/// Tool tra cứu khách hàng: repo → DTO → trả về.
/// </summary>
[McpServerToolType]
public sealed class CustomerTools(
    ICrmRepository repository,
    ILogger<CustomerTools> logger)
{
    [McpServerTool(Name = "get_customer")]
    [Description("Lấy hồ sơ khách hàng theo mã khách hàng.")]
    public async Task<ToolResult<CustomerDto>> GetCustomerAsync(
        [Description("Mã khách hàng, ví dụ CUS0001")] string customerId,
        CancellationToken ct)
    {
        const string ToolName = "get_customer";

        if (string.IsNullOrWhiteSpace(customerId))
        {
            return ToolResult<CustomerDto>.Empty("Thiếu mã khách hàng. Truyền mã dạng CUS0001.");
        }

        try
        {
            var customer = await repository.GetCustomerAsync(customerId, ct);

            return customer is null
                ? ToolResult<CustomerDto>.Empty($"Không tìm thấy khách hàng có mã '{customerId}'.")
                : ToolResult<CustomerDto>.Ok([ToDto(customer)]);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            // Chi tiết ở lại phía server: stack trace có thể chứa dữ liệu (SPEC mục 6.3).
            logger.LogError(ex, "Lỗi khi chạy tool {ToolName}.", ToolName);
            return ToolResult<CustomerDto>.Empty("Không thể xử lý yêu cầu này. Vui lòng thử lại.");
        }
    }

    [McpServerTool(Name = "search_customers")]
    [Description("Tìm khách hàng theo phân khúc hoặc theo mã RM phụ trách.")]
    public async Task<ToolResult<CustomerDto>> SearchCustomersAsync(
        // Ba tham số đều phải có giá trị mặc định, nếu không JSON Schema sinh ra sẽ liệt chúng
        // vào `required` và agent buộc phải truyền đủ — trái với mô tả "bỏ trống để không lọc".
        [Description("Phân khúc khách hàng, ví dụ Priority. Bỏ trống để không lọc theo phân khúc")] string? segment = null,
        [Description("Mã RM phụ trách, ví dụ RM007. Bỏ trống để không lọc theo RM")] string? rmId = null,
        [Description("Số bản ghi tối đa, mặc định 20, tối đa 100")] int take = 20,
        CancellationToken ct = default)
    {
        const string ToolName = "search_customers";

        try
        {
            var customers = await repository.SearchCustomersAsync(segment, rmId, take, ct);
            var items = customers.Select(ToDto).ToList();

            return items.Count == 0
                ? ToolResult<CustomerDto>.Empty("Không có khách hàng nào khớp tiêu chí. Thử bỏ bớt bộ lọc.")
                : ToolResult<CustomerDto>.Ok(items);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Lỗi khi chạy tool {ToolName}.", ToolName);
            return ToolResult<CustomerDto>.Empty("Không thể xử lý yêu cầu này. Vui lòng thử lại.");
        }
    }

    private static CustomerDto ToDto(Customer customer) => new(
        customer.CustomerId,
        customer.FullName,
        customer.Phone,
        customer.Cccd,
        customer.AccountNo,
        customer.Email,
        customer.Segment,
        customer.AssignedRmId,
        customer.OpenedDate.ToString("yyyy-MM-dd"));
}
