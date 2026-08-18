# SPEC — Đặc tả Kỹ thuật

**Đọc file này trước khi viết bất kỳ dòng code nào.**

---

## 1. Ngăn xếp công nghệ

| Thành phần | Lựa chọn | Ghi chú |
|---|---|---|
| Runtime | .NET 10 | macOS arm64 |
| Ngôn ngữ | C# 13, nullable enabled | |
| Web | ASP.NET Core Minimal API | |
| MCP | `ModelContextProtocol.AspNetCore` | Ghim version cụ thể sau khi restore |
| Nguồn dữ liệu | File JSON trong `data/` | Nạp một lần lúc khởi động |
| Test | xUnit + FluentAssertions | |

Không thêm package nào ngoài danh sách trên mà chưa hỏi.

## 2. Kiến trúc

```
┌──────────────── BankAssist.Mcp (một tiến trình) ───────────────┐
│                                                                 │
│  /mcp  ──> McpServer ──> Tools ──> ICrmRepository ──> data/*.json│
│  /health                                                         │
└──────────────────────────────────────────────────────────────────┘
```

Mọi tool đều đi qua đúng chuỗi: **Repository → DTO → trả về.** Tool không tự đọc file, không tự dựng kiểu trả về ngoài `Models/Dtos.cs`.

## 3. Mô hình dữ liệu

### 3.1 File JSON

Đặt trong `data/`, nạp lúc khởi động vào bộ nhớ.

`customers.json`
```json
[
  {
    "customerId": "CUS0001",
    "fullName": "Nguyễn Văn An",
    "phone": "0912345678",
    "cccd": "001199012345",
    "accountNo": "19001234567890",
    "email": "an.nguyen@example.com",
    "segment": "Priority",
    "assignedRmId": "RM007",
    "openedDate": "2019-04-12"
  }
]
```

`opportunities.json`
```json
[
  {
    "oppId": "OPP0001",
    "customerId": "CUS0001",
    "productCode": "SAV12M",
    "amount": 500000000,
    "maturityDate": "2026-08-15",
    "stage": "Open",
    "campaignId": "CMP0002"
  }
]
```

`interactions.json`
```json
[
  {
    "interactionId": "INT0001",
    "customerId": "CUS0001",
    "channel": "Call",
    "createdAt": "2026-07-28T09:12:00+07:00",
    "note": "Khách hẹn gọi lại sau 17h"
  }
]
```

`campaigns.json`
```json
[
  {
    "campaignId": "CMP0002",
    "name": "Ưu đãi tiết kiệm quý 3",
    "startDate": "2026-07-01",
    "endDate": "2026-09-30"
  }
]
```

`products.json`
```json
[
  {
    "productCode": "SAV12M",
    "name": "Tiết kiệm có kỳ hạn 12 tháng",
    "type": "Savings",
    "interestRate": 5.6,
    "description": "Kỳ hạn 12 tháng, lãi cuối kỳ, tối thiểu 10 triệu đồng"
  }
]
```

**Yêu cầu dữ liệu mẫu:** tối thiểu 200 khách, 300 cơ hội, 800 tương tác, 5 chiến dịch, 35 sản phẩm. Sinh bằng script, không dùng tên người có thật.

### 3.2 Tầng truy cập dữ liệu

```csharp
public interface ICrmRepository
{
    Task<Customer?> GetCustomerAsync(string customerId, CancellationToken ct);
    Task<IReadOnlyList<Customer>> SearchCustomersAsync(string? segment, string? rmId, int take, CancellationToken ct);
    Task<IReadOnlyList<Opportunity>> GetOpportunitiesAsync(string? customerId, int? maturingWithinDays, string? stage, CancellationToken ct);
    Task<IReadOnlyList<Interaction>> GetInteractionsAsync(string customerId, int take, CancellationToken ct);
    Task<IReadOnlyList<Campaign>> GetCampaignsAsync(bool activeOnly, CancellationToken ct);
    Task<IReadOnlyList<Product>> SearchProductsAsync(string keyword, string? type, CancellationToken ct);
}
```

MVP cài đặt `JsonCrmRepository`. Giai đoạn sau thêm `SqlCrmRepository` mà **không sửa một dòng nào trong `Tools/`** — đây là tiêu chí kiểm tra thiết kế có đúng hay không.

### 3.3 Ranh giới record và DTO

`Models/CrmRecords.cs` là record nguồn nạp từ JSON. `Models/Dtos.cs` là hợp đồng API — kiểu duy nhất rời khỏi tiến trình qua MCP.

Giữ hai file tách nhau. Đây là chỗ duy nhất quyết định trường nào đi ra ngoài, nên thêm trường mới phải sửa `Dtos.cs` chứ không "tiện tay" trả thẳng record nguồn.

## 4. Tool MCP

### 4.1 Khai báo

```csharp
[McpServerToolType]
public sealed class CustomerTools(
    ICrmRepository repo,
    ILogger<CustomerTools> logger)
{
    [McpServerTool(Name = "get_customer")]
    [Description("Lấy hồ sơ khách hàng theo mã khách hàng.")]
    public async Task<ToolResult<CustomerDto>> GetCustomerAsync(
        [Description("Mã khách hàng, ví dụ CUS0001")] string customerId,
        CancellationToken ct)
    {
        // 1. repo  2. DTO  3. return
    }
}
```

Mô tả tool viết bằng tiếng Việt, **cụ thể và ngắn**. Agent chọn tool dựa trên chuỗi này, nên nó là một phần của thiết kế chứ không phải chú thích.

### 4.2 Đăng ký

```csharp
builder.Services
    .AddMcpServer()
    .WithHttpTransport()
    .WithToolsFromAssembly();

app.MapMcp("/mcp");
```

### 4.3 Xử lý lỗi

- Không tìm thấy dữ liệu: trả danh sách rỗng kèm mô tả, **không ném exception**.
- Tham số sai: trả thông báo hướng dẫn giá trị hợp lệ.
- Lỗi hệ thống: log đầy đủ phía server, trả thông báo trung tính cho client.

Không bao giờ để stack trace lọt vào kết quả tool — nó có thể chứa dữ liệu.

## 5. Cấu hình

`appsettings.json` (có commit) chứa giá trị mặc định không nhạy cảm.

```json
{
  "Data": { "Directory": "data" }
}
```

Đường dẫn tương đối tính theo thư mục chứa binary, không theo working directory, để `dotnet run` và bản publish đều tìm thấy `data/`.

Server hiện không cần bí mật nào để khởi động. Nếu sau này phát sinh, đọc từ biến môi trường — không hardcode trong source.

## 6. Thứ tự triển khai

Bám theo `docs/BACKLOG.md`. Nguyên tắc: bốn task đầu **không đụng tới MCP**, để nhóm quen với domain trước khi học giao thức mới.
