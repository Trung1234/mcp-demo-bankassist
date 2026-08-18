# BankAssist CRM MCP Server

MCP server viết bằng .NET, cung cấp tool tra cứu CRM ngân hàng cho AI agent.

**Giai đoạn MVP:** dữ liệu đọc từ file JSON, chưa có database.

---

## Yêu cầu môi trường

- macOS (Apple Silicon), .NET 10 SDK arm64
- Không cần database, không cần Docker cho MVP

```bash
dotnet --version     # kỳ vọng 10.x
```

## Chạy

```bash
git clone <repo> && cd mcp-demo
dotnet restore
dotnet run --project src/BankAssist.Mcp
```

Server lắng nghe `http://localhost:5026`. Không cần cấu hình gì thêm trước lần chạy đầu.

| Đường dẫn | Mục đích | Trạng thái |
|---|---|---|
| `/mcp` | Endpoint MCP (Streamable HTTP) | ✅ |
| `/health` | Kiểm tra sống | ✅ |

## Kiểm tra bằng MCP Inspector

```bash
npx @modelcontextprotocol/inspector
# Transport: Streamable HTTP
# URL: http://localhost:5026/mcp
```

Bấm **List Tools**. Đích cuối là 6 tool; hiện có `get_customer`, `search_customers` và `ping` tạm (T-15 sẽ gỡ `ping`).

---

## Cách kiểm thử

Hai tầng, đi từ nhanh nhất tới sát thực tế nhất. Tầng 1 chạy trong vài chục mili giây nên dùng khi đang code; tầng 2 dùng khi muốn tin vào kết quả.

### Tầng 1 — Test tự động

```bash
dotnet test                              # toàn bộ
dotnet test --filter Category=Tools      # chỉ ca tầng tool
dotnet test --filter Category=Repository # chỉ tầng truy cập dữ liệu
```

| Ca | Kiểm điều gì | Ở đâu |
|---|---|---|
| T-05 | Mã không tồn tại → rỗng kèm thông báo, không ném exception | `CustomerToolsTests.cs` |
| R-01…R-12 | Repository: tra cứu, lọc, sắp xếp, cắt số lượng, JSON hỏng | `UnitTest1.cs` |

### Tầng 2 — Gọi tay qua HTTP

Chỉ cần **một** request, không phải bắt tay trước:

```bash
curl -s -X POST http://localhost:5026/mcp \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json, text/event-stream' \
  -d '{"jsonrpc":"2.0","id":1,"method":"tools/call",
       "params":{"name":"get_customer","arguments":{"customerId":"CUS0001"}}}'
```

Kết quả kỳ vọng:

```json
{"items":[{"customerId":"CUS0001","fullName":"Hoàng Văn Linh","phone":"00576804025",
"cccd":null,"accountNo":"65258086319300","email":"user1@example.com",
"segment":"Standard","assignedRmId":"RM002","openedDate":"2022-06-06"}],"count":1}
```

Trong Postman: **POST** `http://localhost:5026/mcp`, hai header trên, body y hệt. Response là `text/event-stream` nên nội dung nằm ở dòng bắt đầu bằng `data:`.

Vài ca đáng thử tay:

| Gửi gì | Kỳ vọng |
|---|---|
| `arguments: {"customerId":"CUS0001"}` | Đủ trường của khách CUS0001 |
| `arguments: {"customerId":"cus0001"}` | Vẫn tìm thấy — mã khách so sánh không phân biệt hoa thường |
| `arguments: {"customerId":"CUS9999"}` | `count: 0` kèm thông báo, không stack trace |
| `arguments: {"customerId":""}` | Thông báo hướng dẫn giá trị hợp lệ |

---

## Cấu trúc thư mục

```
src/BankAssist.Mcp/
  Program.cs
  Configuration/    # Options (đường dẫn data/)
  Tools/            # các class [McpServerToolType]
  Data/             # ICrmRepository + JsonCrmRepository
  Models/           # record nguồn (CrmRecords) và DTO trả qua MCP (Dtos)
data/               # JSON nguồn cho MVP, copy sang output lúc build
tests/BankAssist.Mcp.Tests/
docs/               # BRD, PRD, SPEC, TEST-SPEC, BACKLOG
```

`Models/CrmRecords.cs` là dữ liệu nguồn, `Models/Dtos.cs` là hợp đồng API. Giữ hai file tách nhau vì `Dtos.cs` là chỗ duy nhất quyết định trường nào rời khỏi tiến trình — nhìn `using` là thấy ngay chỗ nào nhầm.

## Tài liệu

| File | Nội dung |
|---|---|
| [docs/BRD.md](docs/BRD.md) | Bối cảnh nghiệp vụ, vấn đề, phạm vi |
| [docs/PRD.md](docs/PRD.md) | Yêu cầu sản phẩm, user story, tiêu chí chấp nhận |
| [docs/SPEC.md](docs/SPEC.md) | Đặc tả kỹ thuật — đọc file này trước khi code |
| [docs/TEST-SPEC.md](docs/TEST-SPEC.md) | Ca kiểm thử |
| [docs/BACKLOG.md](docs/BACKLOG.md) | Danh sách công việc theo thứ tự thực hiện |


---

## Điều tuyệt đối không làm

**Không đưa dữ liệu khách hàng thật vào `data/`.** Toàn bộ file JSON là dữ liệu giả sinh bằng script.

## Giấy phép

Nội bộ — không phát hành công khai.
