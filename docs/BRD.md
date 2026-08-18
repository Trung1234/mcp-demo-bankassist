# BRD — Tài liệu Yêu cầu Nghiệp vụ

**Dự án:** BankAssist CRM MCP Server
**Phiên bản:** 1.0 · MVP
**Trạng thái:** Đã duyệt để triển khai

---

## 1. Bối cảnh

Nhân viên quan hệ khách hàng (RM) của ngân hàng hiện phải mở nhiều màn hình CRM khác nhau để tra cứu thông tin trước mỗi cuộc gọi hoặc mỗi email chăm sóc. Ước tính mỗi ca làm việc mất 2–3 giờ cho thao tác tra cứu và soạn thảo lặp lại.

Ngân hàng muốn thử nghiệm một trợ lý AI trò chuyện bằng tiếng Việt. Để trợ lý đó truy cập được dữ liệu CRM một cách có kiểm soát, cần một lớp trung gian chuẩn hoá — đó là phạm vi của dự án này.

## 2. Vấn đề nghiệp vụ

| Mã | Vấn đề | Ảnh hưởng |
|---|---|---|
| P-1 | Thông tin khách nằm rải rác ở 4 module CRM | RM mất thời gian ghép thủ công |
| P-2 | Mỗi ứng dụng AI lại tự viết tích hợp CRM riêng | Trùng lặp công sức, khó kiểm soát |

## 3. Mục tiêu

| Mã | Mục tiêu | Cách đo |
|---|---|---|
| G-1 | Một điểm truy cập chuẩn cho mọi AI agent tới dữ liệu CRM | 6 tool hoạt động qua giao thức MCP |
| G-2 | Thay nguồn dữ liệu không phải sửa tool | Đổi JSON sang SQL chỉ sửa một class |

## 4. Phạm vi

### Trong phạm vi MVP

- MCP server cung cấp 6 tool đọc dữ liệu CRM
- Dữ liệu giả đọc từ file JSON

### Ngoài phạm vi MVP

| Hạng mục | Lý do hoãn |
|---|---|
| Kết nối database thật | Chưa có môi trường sandbox CRM |
| Tool ghi dữ liệu (tạo, sửa) | Rủi ro cao, cần quy trình duyệt riêng |
| Xác thực OAuth 2.1 | MVP chạy trong mạng nội bộ |
| Giao diện quản trị | Chưa cần cho giai đoạn thử nghiệm |
| Đa ngôn ngữ | Chỉ phục vụ RM người Việt |

## 5. Các bên liên quan

| Vai trò | Quan tâm chính |
|---|---|
| RM | Tra cứu nhanh, thông tin chính xác |
| Trưởng phòng CNTT | Dễ bảo trì, dễ thay nguồn dữ liệu |
| Đội thực tập | Tài liệu đủ rõ để tự triển khai |

## 6. Giả định

- CRM sandbox sẽ cung cấp API tương thích với cấu trúc JSON trong `data/`
- Ứng dụng host chạy trong cùng mạng nội bộ với MCP server

## 7. Rủi ro

| Rủi ro | Mức | Giảm thiểu |
|---|---|---|
| Nhóm thực tập chưa quen MCP | Trung bình | Backlog chia nhỏ, task đầu không đụng MCP |
| Cấu trúc JSON lệch với CRM thật | Trung bình | Cô lập trong `ICrmRepository` |

## 8. Tiêu chí nghiệm thu giai đoạn MVP

1. MCP Inspector kết nối được và liệt kê đủ 6 tool.
2. Toàn bộ ca kiểm thử trong TEST-SPEC đạt.
