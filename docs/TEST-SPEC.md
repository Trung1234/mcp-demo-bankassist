# TEST-SPEC — Đặc tả Kiểm thử

Viết test trước, chạy cho fail, rồi mới viết code.

Đặt `[Trait("Category", "...")]` để lọc: `Repository`, `Tools`, `Integration`.

---

## 1. ICrmRepository

| Mã | Tình huống | Kỳ vọng |
|---|---|---|
| R-01 | Lấy khách theo mã tồn tại | Trả đúng bản ghi |
| R-02 | Lấy khách theo mã không tồn tại | Trả `null`, không ném exception |
| R-03 | Mã khách chữ thường `cus0001` | Vẫn tìm thấy — so sánh không phân biệt hoa thường |
| R-04 | Tìm khách không truyền tiêu chí | Trả tối đa 20 bản ghi |
| R-05 | `take = 500` | Bị cắt về 100 |
| R-06 | `take = 0` hoặc âm | Dùng mặc định 20 |
| R-07 | Cơ hội đáo hạn trong 7 ngày | Chỉ trả cơ hội trong khoảng, sắp xếp tăng dần |
| R-08 | Cơ hội đáo hạn hôm nay | Được tính là nằm trong khoảng |
| R-09 | Tương tác của khách | Mới nhất trước, mặc định 10 bản |
| R-10 | Chiến dịch `activeOnly = true` | Chỉ trả chiến dịch còn hiệu lực theo ngày hiện tại |
| R-11 | Tìm sản phẩm bằng từ khoá không dấu `tiet kiem` | Vẫn khớp `Tiết kiệm` |
| R-12 | File JSON hỏng cú pháp | Server báo lỗi rõ ràng lúc khởi động, không chạy tiếp |

## 2. Tool MCP

| Mã | Tình huống | Kỳ vọng |
|---|---|---|
| T-01 | `tools/list` | Trả đúng 6 tool, tên đúng `snake_case` |
| T-02 | Mỗi tool | Có mô tả tiếng Việt không rỗng |
| T-03 | Mỗi tham số | Có `[Description]` |
| T-05 | `get_customer` với mã không tồn tại | Kết quả rỗng, thông báo rõ, không exception |
| T-09 | Tool ném exception nội bộ | Client nhận thông báo trung tính, không có stack trace |

Mã ca giữ nguyên số cũ để không phải sửa chéo tài liệu; các số khuyết là ca đã gỡ ở đợt rút gọn kiến trúc.

## 3. Kiểm thử tích hợp

| Mã | Kịch bản |
|---|---|
| I-01 | Khởi động server, MCP Inspector kết nối, liệt kê 6 tool |
| I-02 | Gọi `get_customer` qua Inspector, xác nhận trả đúng bản ghi |
| I-04 | Chuỗi ba lượt: `get_customer` → `get_opportunities` → `get_interactions`, kiểm tra dữ liệu khớp nhau theo `customerId` |

## 4. Ngưỡng hiệu năng

| Mã | Phép đo | Ngưỡng |
|---|---|---|
| P-01 | `get_customer` với 1.000 bản ghi trong bộ nhớ | p95 < 300 ms |
| P-05 | Thời gian khởi động | < 3 giây |
