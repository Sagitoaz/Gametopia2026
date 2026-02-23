# TÀI LIỆU TỔNG QUAN DỰ ÁN: CODER GO HAPPY

## 1. THÔNG TIN CHUNG
* **Tên dự án:** Coder Go Happy
* **Thể loại:** Point-and-Click Puzzle (Giải đố màn hình tĩnh).
* **Phong cách đồ họa:** 2D Cartoon / Office Style.
* **Cốt truyện:** Một lập trình viên đang tuyệt vọng vì "Dead-line" và "Bugs". Người chơi phải sử dụng các công cụ đời thực để giải quyết các nút thắt logic, giúp anh ta hoàn thành dự án và mỉm cười trở lại.

---

## 2. CƠ CHẾ GAMEPLAY (CORE LOOP)
Dựa trên cấu trúc kinh điển của dòng game "Monkey GO Happy":
1.  **Khám phá (Explore):** Di chuyển giữa các Scene (Màn hình bối cảnh) trong cùng một Level (Trái/Phải/Vào cửa).
2.  **Thu thập (Collect):** Tìm kiếm các vật phẩm ẩn giấu (Items) và 10-20 "Mini-Bugs" (thay cho khỉ con).
3.  **Tương tác (Interact):** Kéo thả vật phẩm từ túi đồ (Inventory) vào các điểm nóng (Hotspots) trên màn hình.
4.  **Giải mã (Decode):** Quan sát các manh mối (màu sắc, con số, ký hiệu) vẽ trên tường hoặc đồ vật để nhập mật mã.
5.  **Kết thúc:** Giải xong câu đố cuối cùng -> Coder mỉm cười -> Qua màn.

---

## 3. CHI TIẾT CẤU TRÚC 3 MÀN CHƠI (LEVEL DESIGN)

### LEVEL 1: THE LEGACY SYSTEM (Dựa trên Video 87)
* **Bối cảnh:** Một phòng máy chủ (Server Room) cũ kỹ, bụi bặm.
* **Cấu trúc:** 3 Scene liên kết (Hành lang máy chủ - Góc kỹ thuật - Tủ điện trung tâm).
* **Logic giải đố chính:**
    * **Sao chép:** Giữ nguyên vị trí các nút bấm và cần gạt từ video 87.
    * **Thay đổi:** Các biểu tượng cổ đại chuyển thành các ký tự lập trình như `&&`, `||`, `!`, `==`.
    * **Công cụ:** * **Đèn pin:** Dùng để soi vào gầm tủ Server tối tăm tìm mã số.
        * **Tua vít:** Mở nắp lưng của một chiếc PC đời cũ để lấy card đồ họa.
    * **Nhiệm vụ phụ:** Tìm 10 "Bug" màu xanh lá đang bò trên các dây cáp.

### LEVEL 2: THE LOGIC GATE MAZE (Dựa trên Video 232)
* **Bối cảnh:** Văn phòng làm việc hiện đại nhưng đang bị "lock" toàn bộ hệ thống.
* **Cấu trúc:** 4 Scene (Quầy lễ tân - Phòng Dev - Phòng Họp - Kho lưu trữ).
* **Logic giải đố chính:**
    * **Sao chép:** Cấu trúc các cánh cửa yêu cầu mật mã hình học.
    * **Thay đổi:** Các hình tam giác/tròn/vuông chuyển thành các loại ngoặc `()`, `[]`, `{}`.
    * **Công cụ:**
        * **Kéo:** Cắt dây cáp mạng bị rối để lấy được chìa khóa USB.
        * **Nam châm:** Hút chiếc chìa khóa vạn năng rơi dưới khe thang máy.
    * **Nhiệm vụ phụ:** Thu thập 15 "Bug" nhỏ đang trốn trong các tách cà phê và dưới bàn phím.

### LEVEL 3: THE SECURITY BREACH (Dựa trên Video 351)
* **Bối cảnh:** Trung tâm dữ liệu bảo mật cao (Data Center).
* **Cấu trúc:** Đa bối cảnh phức tạp (Cổng an ninh - Phòng điều khiển - Hầm chứa dữ liệu).
* **Logic giải đố chính:**
    * **Sao chép:** Hệ thống bảng xoay mật mã và các công tắc thứ tự.
    * **Thay đổi:** Mật mã màu sắc dựa trên logo các ngôn ngữ lập trình (Python-Vàng/Xanh, JS-Vàng, C#-Tím).
    * **Công cụ:**
        * **Búa:** Phá vỡ "Bức tường lửa" (Firewall) bằng gạch vật lý để tiến vào khu vực lõi.
        * **Thanh đòn bẩy:** Kích hoạt lại máy phát điện dự phòng khi hệ thống bị sập (Shutdown).
    * **Nhiệm vụ phụ:** Tìm 20 "Bug" nhỏ ẩn sau các bức tường và màn hình máy tính.

---

## 4. BẢNG QUY ĐỔI ASSETS (DÀNH CHO ART & DEV)

| Thành phần gốc | Quy đổi sang Coder | Trạng thái / Hành động |
| :--- | :--- | :--- |
| **Chú khỉ buồn** | **Lập trình viên (Coder)** | Buồn: Khóc ròng, gãi đầu. Vui: Nhảy múa, đeo kính đen. |
| **Khỉ con (Mini)** | **Bọ phần mềm (Mini Bugs)** | Trốn ở các góc khuất, cần click để thu thập. |
| **Thùng gỗ / Hòm** | **Thùng rác / Case máy tính** | Chứa vật phẩm bên trong, cần công cụ để mở. |
| **Giấy nháp mật mã** | **Snippet Code / Comment** | Chứa gợi ý về số hoặc thứ tự nhấn nút. |
| **Chìa khóa sắt** | **Thẻ từ (Keycard) / USB** | Dùng để mở cửa hoặc đăng nhập máy tính. |

---

## 5. CÁC CÔNG CỤ TRỌNG TÂM (BTC YÊU CẦU)
Để làm nổi bật đề tài của BTC, các công cụ sẽ được nhấn mạnh bằng hiệu ứng đặc biệt khi sử dụng:
* **Kéo:** "Refactor" - Cắt bỏ những dòng code/dây cáp thừa.
* **Búa:** "Force Close" - Phá vỡ những rào cản hệ thống cứng nhắc.
* **Đèn pin:** "Identify" - Làm lộ diện những đoạn mã ẩn (Hidden logs).
* **Nam châm:** "Data Mining" - Thu thập linh kiện kim loại từ xa.

---

## 6. GHI CHÚ PHÁT TRIỂN
* Tài liệu này mô tả thiết kế game "Coder Go Happy" dựa trên cơ chế Monkey GO Happy
* Cần chi tiết hóa thêm bảng mã hóa (Input/Output) cho các câu đố khi triển khai
* Nguồn tham khảo: Video 87, 232, 351