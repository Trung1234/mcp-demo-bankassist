# PRD — Tài liệu Yêu cầu Sản phẩm

**Sản phẩm:** BankAssist CRM MCP Server
**Phiên bản:** 1.0 · MVP

---

## 1. Người dùng của sản phẩm này

Lưu ý quan trọng: người dùng trực tiếp của MCP server **không phải RM**, mà là **ứng dụng AI agent**. RM là người dùng cuối, ở phía sau host.

| Vai | Là ai | Cần gì từ server |
|---|---|---|
| Host application | Ứng dụng chat của ngân hàng | Tool ổn định, kết quả nhất quán |
| AI agent | Vòng lặp LLM trong host | Mô tả tool rõ ràng để chọn đúng tool |
| Kỹ sư vận hành | Đội CNTT | Health check, cấu hình đơn giản |

## 2. Danh mục tool

| Tên tool | Mô tả | Tham số | Trả về |
|---|---|---|---|
| `get_customer` | Hồ sơ một khách hàng | `customerId` | 1 CustomerDto |
| `search_customers` | Tìm khách theo tiêu chí | `segment?`, `rmId?`, `take?` | Danh sách CustomerDto |
| `get_opportunities` | Cơ hội bán hàng | `customerId?`, `maturingWithinDays?`, `stage?` | Danh sách OpportunityDto |
| `get_interactions` | Lịch sử tương tác | `customerId`, `take?` | Danh sách InteractionDto |
| `get_campaigns` | Chiến dịch đang chạy | `activeOnly?` | Danh sách CampaignDto |
| `search_products` | Tra cứu sản phẩm | `keyword`, `type?` | Danh sách ProductDto |

## 3. User story và tiêu chí chấp nhận

### US-01 — Tra cứu hồ sơ khách hàng

> Là một AI agent, tôi cần lấy hồ sơ một khách hàng theo mã, để trả lời câu hỏi của RM về khách đó.

**Tiêu chí chấp nhận**

- Cho mã khách tồn tại, tool trả về đủ trường: mã, tên, điện thoại, CCCD, số tài khoản, phân khúc, mã RM phụ trách.
- Mã khách so khớp không phân biệt hoa thường.
- Cho mã khách không tồn tại, tool trả về kết quả rỗng kèm thông báo rõ ràng, **không ném exception**.

### US-02 — Tìm khách theo tiêu chí

> Là một AI agent, tôi cần lọc danh sách khách theo phân khúc hoặc theo RM phụ trách.

**Tiêu chí chấp nhận**

- Không truyền tiêu chí nào thì trả về tối đa 20 bản ghi.
- `take` tối đa 100; vượt quá thì tự cắt về 100.
- `take` bằng 0 hoặc âm thì dùng mặc định 20.

### US-03 — Tra cứu cơ hội đáo hạn

> Là một AI agent, tôi cần biết khách nào có sổ tiết kiệm sắp đáo hạn.

**Tiêu chí chấp nhận**

- `maturingWithinDays = 7` trả về cơ hội có ngày đáo hạn từ hôm nay tới 7 ngày sau.
- Cơ hội đáo hạn đúng hôm nay được tính là nằm trong khoảng.
- Kết quả sắp xếp theo ngày đáo hạn tăng dần.

### US-04 — Xem lịch sử tương tác

> Là một AI agent, tôi cần đọc các lần tiếp xúc gần nhất với một khách hàng.

**Tiêu chí chấp nhận**

- Mặc định trả 10 bản ghi gần nhất, sắp xếp mới nhất trước.
- Mã khách không tồn tại thì trả rỗng kèm thông báo.

## 4. Yêu cầu phi chức năng

| Mã | Yêu cầu | Ngưỡng |
|---|---|---|
| NFR-1 | Thời gian phản hồi mỗi tool | p95 < 300 ms với 1.000 bản ghi JSON |
| NFR-2 | Khởi động | Server sẵn sàng trong < 3 giây |
| NFR-3 | Nền tảng | Chạy được trên macOS arm64 và Linux x64 |

## 5. Ngoài phạm vi phiên bản này

Ghi lại để nhóm không tự ý mở rộng:

- Tool ghi hoặc sửa dữ liệu
- Phân trang bằng cursor
- Streaming kết quả
- Nhiều ngôn ngữ
- Xác thực OAuth cho MCP endpoint
