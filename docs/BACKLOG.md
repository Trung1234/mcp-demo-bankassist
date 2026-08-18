# BACKLOG — Danh sách công việc

Làm theo thứ tự. Không nhảy cóc. Đánh dấu `[x]` khi xong và đạt Definition of Done trong `CLAUDE.md`.

Ký hiệu ước lượng: **S** khoảng nửa ngày · **M** một ngày · **L** hai ngày trở lên.

Mã task giữ nguyên số cũ; các số khuyết là task đã gỡ ở đợt rút gọn kiến trúc.

---

## Đợt 0 — Nền móng (chưa đụng MCP)

Mục đích: quen domain và cấu trúc trước khi học giao thức mới.

### [ ] T-01 · Khởi tạo solution — S
Tạo `BankAssist.Mcp` (ASP.NET Core Minimal API) và `BankAssist.Mcp.Tests` (xUnit). Thêm `/health` trả `200 OK`. Thiết lập `.gitignore` loại trừ `appsettings.Development.json`.

**Xong khi:** `dotnet run` chạy được, `curl localhost:5026/health` trả OK, `dotnet test` xanh với một test giả.

### [ ] T-02 · Model và dữ liệu mẫu — M
Định nghĩa record `Customer`, `Opportunity`, `Interaction`, `Campaign`, `Product` theo SPEC mục 3.1. Viết script sinh dữ liệu giả đủ số lượng yêu cầu.

**Xong khi:** năm file JSON tồn tại, đủ số bản ghi. Không có tên người thật.

### [ ] T-03 · JsonCrmRepository — M
Cài `ICrmRepository` đọc JSON, nạp một lần lúc khởi động vào bộ nhớ.

**Test:** R-01 đến R-12.

---

## Đợt 2 — MCP

### [x] T-10 · Dựng MCP server — M
Thêm `ModelContextProtocol.AspNetCore`, cấu hình `AddMcpServer().WithHttpTransport().WithToolsFromAssembly()`, map `/mcp`. Làm một tool `ping` tạm để kiểm tra kết nối.

**Xong khi:** MCP Inspector kết nối được và thấy tool `ping`.

### [x] T-12 · Tool khách hàng — M
`get_customer` và `search_customers`, đi đúng chuỗi repo → DTO → trả về.

**Test:** T-05, US-01, US-02.

### [ ] T-13 · Tool cơ hội và tương tác — M
`get_opportunities`, `get_interactions`.

**Test:** US-03, US-04.

### [ ] T-14 · Tool chiến dịch và sản phẩm — S
`get_campaigns`, `search_products`.

### [ ] T-15 · Gỡ tool ping — S
Xoá tool tạm. Xác nhận `tools/list` trả đúng 6 tool.

**Test:** T-01, T-02, T-03.

---

## Đợt 3 — Hoàn thiện

### [ ] T-17 · Kiểm thử tích hợp — M
Viết I-01, I-02, I-04.

### [ ] T-18 · Đo hiệu năng — S
Đo P-01 và P-05. Ghi kết quả vào `docs/PERF.md`.

### [ ] T-19 · Hoàn thiện tài liệu — S
Cập nhật README với ảnh chụp màn hình MCP Inspector. Ghi lại các quyết định đã thay đổi so với SPEC ban đầu.

---

## Cần quyết định

Ghi vào đây khi gặp điểm đặc tả chưa rõ. Không tự quyết một mình.

| # | Vấn đề | Ai quyết | Trạng thái |
|---|---|---|---|
| — | (trống) | | |

---

## Hoãn lại — không làm ở MVP

Ghi ra để khỏi bị cám dỗ:

- Tool ghi hoặc sửa dữ liệu
- OAuth 2.1 cho MCP endpoint
- Phân trang cursor
- Giao diện quản trị
- Nhiều ngôn ngữ
