# Tổng quan đề tài 
  - Tên Thành Viên
  - Đề tài và yêu cầu
  - Các biểu mẫu và quy định
  - Kế hoạch tiến độ
  - ...

# Tên thành viên: 
  - Nguyễn Ngô Thành Tài (Nhóm trưởng)
  - Trịnh Nguyễn Vy Na
  - Lê Công Thuận                

# Project 1 - Tên đề tài: Quản lý sổ tiết kiệm

![image](https://github.com/user-attachments/assets/37887ddc-5af4-45bd-ab85-0093475a5bc2)

Danh sách các yêu cầu đã đề xuất 
  - Mở sổ tiết kiệm
  - Lập phiếu gửi tiền
  - Lập phiếu rút tiền
  - Tra cứu sổ
  - Lập báo cáo tháng
  - Thay đổi quy định

Danh sách các yêu cầu đang đề xuất thêm:
  * Tự động tất toán
  * Công cụ tính lãi suất (Pending)
  * Công cụ tính giá trị thật đồng tiền sau X năm (Pending)
  * Qui đổi tiền tệ 
  * ...

# Các biểu mẫu và quy định được đề xuất
  - QĐ1: Có 3 loại tiết kiệm (không kỳ hạn, 3 tháng, 6 tháng). Số tiền gởi (ban đầu) tối thiểu là 1.000.000đ
    ![image](https://github.com/user-attachments/assets/4d7b1484-1a8f-47a4-a3f9-1c443852ecdf)

  - QĐ2: Chỉ nhận gởi thêm tiền khi đến kỳ hạn tính lãi suất của các loại tiết kiệm tương ứng. Số tiền gởi thêm tối
thiểu là 100.000đ
    ![image](https://github.com/user-attachments/assets/af0d1018-8488-4ac7-b752-5d1bb3be3c71)

  - QĐ3: Lãi suất là 0.15% đối với loại không kỳ hạn, 0.5% với kỳ hạn 3 tháng và 0.55% với kỳ hạn 6 tháng.
    Tiền lãi = số dư * lãi suất * kỳ hạn (số tháng của loại tiết kiệm tương ứng).
    Loại tiết kiệm có kỳ hạn chỉ được rút khi quá kỳ hạn và phải rút hết toàn bộ, khi này tiền lãi được tính với mức lãi suất của loại không kỳ hạn.
    Loại tiết kiệm không kỳ hạn được rút khi gửi trên 15 ngày và có thể rút số tiền <= số dư hiện có. Sổ sau khi rút hết tiền sẽ tự động đóng.

    ![image](https://github.com/user-attachments/assets/ff7b39e9-c1c5-4f77-a435-7b969e4c7803)

- Biểu mẫu quản lý sổ tiết kiệm
  
  ![image](https://github.com/user-attachments/assets/31122d1d-85aa-4053-a466-8a587af8e267)

- Biểu mẫu quản lý hoạt động ngày và hoạt động tháng
  ![image](https://github.com/user-attachments/assets/3538a9f1-a2a9-4d79-8295-5bb39ea093b3)

- QĐ6: Người dùng có thể thay đổi các qui định như sau
    - QĐ1: Thay đổi số lượng các loại kỳ hạn, tiền gởi tối thiểu.
    - QĐ3: Thay đổi thời gian gởi tối thiểu và lãi suất các loại kỳ hạn.

# Kế hoạch tiến độ
## Giai đoạn 1: Phân tích yêu cầu (2 tuần) - 19/9 đến 2/10
- **Tuần 1 (19/9 - 25/9):**  
  - **Thành Tài:** Thu thập yêu cầu từ các bên liên quan, phân tích yêu cầu và lập tài liệu đặc tả.
- **Tuần 2 (26/9 - 2/10):**  
  - **Thành Tài:** Xác nhận yêu cầu và hoàn tất tài liệu đặc tả, xây dựng sơ đồ luồng công việc và quy trình nghiệp vụ.

## Giai đoạn 2: Thiết kế giao diện (2 tuần) - 3/10 đến 16/10
- **Tuần 3 (3/10 - 9/10):**  
  - **Thành Tài:** Thiết kế wireframe và prototype cho các trang chính của ứng dụng.
- **Tuần 4 (10/10 - 16/10):**  
  - **Thành Tài:** Hoàn thiện thiết kế giao diện chi tiết và thống nhất với các thành viên trong nhóm về giao diện cuối cùng.

## Giai đoạn 3: Phát triển tính năng (6 tuần) - 17/10 đến 27/11
- **Tuần 5 (17/10 - 23/10):**  
  - **Vy Na & Công Thuận:** Thiết lập môi trường phát triển, tạo cơ sở dữ liệu và cài đặt các cấu hình ban đầu.
- **Tuần 6 (24/10 - 30/10):**  
  - **Vy Na & Công Thuận:** Phát triển các chức năng cơ bản như đăng ký, đăng nhập, và quản lý người dùng.
- **Tuần 7 (31/10 - 6/11):**  
  - **Vy Na & Công Thuận:** Xây dựng chức năng tạo và quản lý sổ tiết kiệm.
- **Tuần 8 (7/11 - 13/11):**  
  - **Vy Na & Công Thuận:** Hoàn thiện chức năng nạp, rút tiền và tính lãi.
- **Tuần 9 (14/11 - 20/11):**  
  - **Vy Na & Công Thuận:** Tích hợp giao diện người dùng với các chức năng đã phát triển.
- **Tuần 10 (21/11 - 27/11):**  
  - **Vy Na & Công Thuận:** Kiểm tra và xử lý lỗi, tối ưu hóa hiệu suất.

## Giai đoạn 4: Kiểm thử (2 tuần) - 28/11 đến 11/12
- **Tuần 11 (28/11 - 4/12):**  
  - **Thành Tài:** Kiểm thử chức năng, kiểm tra giao diện và trải nghiệm người dùng.
- **Tuần 12 (5/12 - 11/12):**  
  - **Thành Tài:** Kiểm thử tích hợp, đảm bảo tất cả các chức năng hoạt động ổn định và không có lỗi lớn.

## Giai đoạn 5: Triển khai (1 tuần) - 12/12 đến 18/12
- **Tuần 13 (12/12 - 18/12):**  
  - **Thành Tài, Vy Na & Công Thuận:** Chuẩn bị và triển khai website lên môi trường sản xuất, thực hiện kiểm tra sau triển khai.

## Dự phòng và hoàn tất dự án - 19/12 đến 31/12
- **Tuần 14 - 15 (19/12 - 31/12):**  
  - Xử lý các vấn đề phát sinh, hoàn thiện tài liệu và bàn giao dự án.

---

Kế hoạch này đảm bảo các bước được thực hiện tuần tự và có thời gian dự phòng để giải quyết vấn đề nếu có.




