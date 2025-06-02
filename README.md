# Treasure Hunt Solver

## Mô tả Bài toán
Bài toán tìm đường đi tối ưu để thu thập kho báu trên ma trận nxm với các ràng buộc về chìa khóa.

### Yêu cầu
- Ma trận nxm chứa các rương kho báu được đánh số từ 1 đến p
- Bắt đầu tại vị trí (1,1) với chìa khóa số 0
- Rương x chứa chìa khóa để mở rương x+1
- Rương p chứa kho báu cuối cùng
- Tìm đường đi với tổng nhiên liệu tối thiểu

### Công thức tính khoảng cách
```
distance = √((x1-x2)² + (y1-y2)²)
```

## Thuật toán

### 1. Optimal Algorithm (Thuật toán Tối ưu)
- **Sử dụng**: Brute Force cho p ≤ 8, Improved Greedy cho p > 8
- **Đặc điểm**: Đảm bảo kết quả tối ưu cho các test case nhỏ
- **Kết quả**: Chính xác 100% cho tất cả test cases

### 2. Greedy Algorithm (Thuật toán Tham lam)
- **Chiến lược**: Tại mỗi bước, chọn rương gần nhất có số thứ tự tiếp theo
- **Ưu điểm**: Nhanh, đơn giản
- **Nhược điểm**: Không đảm bảo tối ưu

## Kết quả So sánh

| Test Case | Expected | Greedy   | Optimal  | Status |
|-----------|----------|----------|----------|--------|
| 1         | 5.65685  | 6.06450  | 5.65685  | ✅ Optimal |
| 2         | 5.00000  | 5.60555  | 5.00000  | ✅ Optimal |
| 3         | 11.0000  | 11.0000  | 11.0000  | ✅ Both OK |

## Cấu trúc Project

```
TreasureHuntApp/
├── Backend/                 # C# ASP.NET Core API
│   ├── Controllers/         # API Controllers
│   ├── Services/           # Business Logic
│   │   ├── TreasureHuntService.cs           # Greedy Algorithm
│   │   └── OptimalTreasureHuntService.cs    # Optimal Algorithm
│   ├── Models/             # Data Models
│   ├── Data/               # Entity Framework
│   └── Tests/              # Unit Tests
├── Frontend/               # React TypeScript App
│   ├── src/
│   │   ├── components/     # UI Components
│   │   ├── services/       # API Services
│   │   └── types/          # TypeScript Types
│   └── public/
└── start-all.bat          # Script khởi động
```

## Cài đặt và Chạy

### Prerequisites
- .NET 8.0 SDK
- Node.js 18+ và npm
- Visual Studio Code hoặc IDE tương tự

### Khởi động nhanh
```bash
# Chạy cả Backend và Frontend
./start-all.bat
```

### Khởi động thủ công

#### Backend (Port 7000)
```bash
cd Backend
dotnet restore
dotnet run
```

#### Frontend (Port 3000)  
```bash
cd Frontend
npm install
npm start
```

## Sử dụng

### 1. Web Interface
- Mở trình duyệt tại: http://localhost:3000
- Nhập ma trận và tham số
- Chọn thuật toán: Optimal (khuyến nghị), Greedy, hoặc Compare Both
- Xem kết quả và đường đi

### 2. API Endpoints

#### Solve với Optimal Algorithm
```
POST /api/treasurehunt/solve
```

#### Solve với Greedy Algorithm  
```
POST /api/treasurehunt/solve/greedy
```

#### So sánh cả hai thuật toán
```
POST /api/treasurehunt/compare
```

#### Chạy test cases
```
GET /api/treasurehunt/test
```

#### Lịch sử và quản lý
```
GET /api/treasurehunt/history
POST /api/treasurehunt/replay/{id}
DELETE /api/treasurehunt/history/{id}
```

### 3. Console Tests
```bash
cd Backend
dotnet run test      # Chạy Greedy tests
dotnet run compare   # So sánh cả hai thuật toán
```

## Tính năng

### Backend Features
- ✅ Optimal Algorithm với kết quả chính xác
- ✅ Greedy Algorithm để so sánh
- ✅ RESTful API với Swagger documentation
- ✅ Entity Framework với In-Memory Database
- ✅ CORS support cho React frontend
- ✅ Lưu lịch sử và replay
- ✅ Unit tests và comparison tools

### Frontend Features  
- ✅ Material-UI responsive interface
- ✅ Matrix input với validation
- ✅ Chọn thuật toán (Optimal/Greedy/Compare)
- ✅ Load examples và run tests
- ✅ Hiển thị kết quả và đường đi
- ✅ So sánh hiệu suất thuật toán
- ✅ History management với DataGrid
- ✅ TypeScript type safety

## API Testing

### Test với Postman/cURL
```bash
# Test với Example 1
curl -X POST https://localhost:7000/api/treasurehunt/solve \
  -H "Content-Type: application/json" \
  -d '{
    "n": 3,
    "m": 3, 
    "p": 3,
    "matrix": [
      [3, 2, 2],
      [2, 2, 2], 
      [2, 2, 1]
    ]
  }'
```

### Response Format
```json
{
  "success": true,
  "minimumFuel": 5.65685,
  "path": [
    {"row": 1, "col": 1, "chestNumber": 0},
    {"row": 3, "col": 3, "chestNumber": 1},
    {"row": 2, "col": 2, "chestNumber": 2},
    {"row": 1, "col": 1, "chestNumber": 3}
  ],
  "errorMessage": null
}
```

## Khắc phục sự cố

### Lỗi "Failed to solve treasure hunt"

#### 1. Kiểm tra Backend
```bash
# Cách 1: Sử dụng script debug
cd TreasureHuntApp
.\debug-backend.bat

# Cách 2: Khởi động thủ công
cd TreasureHuntApp/Backend
dotnet run
```

#### 2. Test kết nối
- Mở file: `TreasureHuntApp/test-connection.html` trong trình duyệt
- Hoặc truy cập Swagger: `http://localhost:5000/swagger`

#### 3. Dấu hiệu Backend chạy thành công
```
=== Treasure Hunt API Server ===
Environment: Development
Available URLs:
  - http://localhost:5000/api
  - https://localhost:7001/api (if HTTPS enabled)
  - Swagger: http://localhost:5000/swagger
=====================================
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
```

#### 4. Frontend Auto-Detection
Frontend sẽ tự động thử các URL:
- https://localhost:7000/api  
- http://localhost:5000/api
- https://localhost:5000/api
- http://localhost:7000/api

#### 5. Status Indicators
- ✅ **Connected**: Backend đang chạy, hiển thị URL
- ❌ **Disconnected**: Backend không chạy, nhấn "Retry"  
- ⏳ **Checking**: Đang kiểm tra kết nối

#### 6. Lỗi CORS
Nếu gặp lỗi CORS, backend đã được cấu hình để accept:
- http://localhost:3000
- https://localhost:3000
- http://localhost:3001  
- https://localhost:3001

#### 7. Khởi động lại hoàn toàn
```bash
# Trong TreasureHuntApp directory
.\start-all.bat
```

## Phát triển thêm

### Possible Improvements
- Implement true Dynamic Programming với bitmask cho all cases
- Add visualization cho đường đi trên ma trận
- Support export/import scenarios
- Add performance metrics và timing
- Implement parallel processing cho large matrices

### Known Limitations
- Optimal algorithm dùng brute force chỉ hiệu quả với p ≤ 8
- Frontend chưa có matrix visualization
- Chưa có real-time collaboration features

## License
MIT License - see LICENSE file for details

## Contact
Treasure Hunt Solver - Phiên bản cải tiến với Optimal Algorithm 