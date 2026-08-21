# Sales Order Management System

Sistem manajemen **Sales Order** berbasis .NET dengan arsitektur **Microservices / Multi-project**, yang terdiri dari:

- **Backend API**
- **Frontend MVC**
- Stored Procedure untuk pagination dan pencarian
- Export data ke Excel
- UI interaktif menggunakan **SweetAlert2**

---

## 1. Prasyarat Sistem

Sebelum menjalankan aplikasi, pastikan perangkat telah terinstall:

- **.NET SDK** versi 8.0 / 10.0 atau yang kompatibel
- **SQL Server** — SQL Server Express / Developer Edition
- **SQL Server Management Studio (SSMS)**
- **PowerShell** versi 5.1 atau lebih baru untuk pengujian terminal/scripting

---

## 2. Setup Database

### 2.1 Membuat Database

Buka **SQL Server Management Studio (SSMS)** dan hubungkan ke server lokal.

Buat database dengan nama `SalesOrderDB`:

```sql
CREATE DATABASE SalesOrderDB;
GO

USE SalesOrderDB;
GO
```

### 2.2 Membuat Tabel

Buat tabel utama berikut:

- `COM_CUSTOMER`
- `SALES_SO`
- `SALES_SO_LITEM`

```sql
CREATE TABLE COM_CUSTOMER (
    COM_CUSTOMER_ID INT IDENTITY(1,1) PRIMARY KEY,
    CUSTOMER_NAME VARCHAR(100) NOT NULL
);

CREATE TABLE SALES_SO (
    SALES_SO_ID INT IDENTITY(1,1) PRIMARY KEY,
    SO_NO VARCHAR(50) NOT NULL,
    ORDER_DATE DATETIME NOT NULL,
    COM_CUSTOMER_ID INT FOREIGN KEY REFERENCES COM_CUSTOMER(COM_CUSTOMER_ID),
    ADDRESS VARCHAR(255)
);

CREATE TABLE SALES_SO_LITEM (
    SALES_SO_LITEM_ID INT IDENTITY(1,1) PRIMARY KEY,
    SALES_SO_ID INT FOREIGN KEY REFERENCES SALES_SO(SALES_SO_ID) ON DELETE CASCADE,
    ITEM_NAME VARCHAR(100) NOT NULL,
    QUANTITY INT NOT NULL,
    PRICE DECIMAL(18,2) NOT NULL
);

ALTER TABLE SALES_SO 
    ADD CONSTRAINT FK_SALES_SO_CUSTOMER 
    FOREIGN KEY (COM_CUSTOMER_ID) REFERENCES COM_CUSTOMER(COM_CUSTOMER_ID);

ALTER TABLE SALES_SO_LITEM 
    ADD CONSTRAINT FK_SALES_SO_LITEM_SO 
    FOREIGN KEY (SALES_SO_ID) REFERENCES SALES_SO(SALES_SO_ID) 
    ON DELETE CASCADE;
GO
```

### 2.3 Membuat Stored Procedure

Stored Procedure `sp_get_orders` digunakan untuk fitur:

- Pencarian berdasarkan nomor SO
- Pencarian berdasarkan nama customer
- Filter tanggal order
- Pagination
- Perhitungan grand total
- Mendapatkan total jumlah data

```sql
CREATE PROCEDURE sp_get_orders
    @Keyword VARCHAR(100) = NULL,
    @OrderDate DATE = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        so.SALES_SO_ID AS SalesSoId,
        so.SO_NO AS SoNo,
        so.ORDER_DATE AS OrderDate,
        so.COM_CUSTOMER_ID AS CustomerId,
        c.CUSTOMER_NAME AS CustomerName,
        so.ADDRESS AS Address,
        CAST(
            ISNULL(SUM(i.QUANTITY * i.PRICE), 0)
            AS DECIMAL(18,2)
        ) AS GrandTotal,
        COUNT(*) OVER() AS TotalCount
    FROM SALES_SO so
    INNER JOIN COM_CUSTOMER c
        ON so.COM_CUSTOMER_ID = c.COM_CUSTOMER_ID
    LEFT JOIN SALES_SO_LITEM i
        ON so.SALES_SO_ID = i.SALES_SO_ID
    WHERE
        (
            @Keyword IS NULL
            OR so.SO_NO LIKE '%' + @Keyword + '%'
            OR c.CUSTOMER_NAME LIKE '%' + @Keyword + '%'
        )
        AND (
            @OrderDate IS NULL
            OR CAST(so.ORDER_DATE AS DATE) = @OrderDate
        )
    GROUP BY
        so.SALES_SO_ID,
        so.SO_NO,
        so.ORDER_DATE,
        so.COM_CUSTOMER_ID,
        c.CUSTOMER_NAME,
        so.ADDRESS
    ORDER BY
        so.ORDER_DATE DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END;
GO
```

---

## 3. Menjalankan Service

Buka **3 jendela Terminal / PowerShell** secara terpisah pada masing-masing folder project.

| Service                   | Folder              | Perintah       | Default URL             |
| ------------------------- | ------------------- | -------------- | ----------------------- |
| Customer Service          | `CustomerService`   | `dotnet watch` | `http://localhost:5001` |
| Sales Order Service (API) | `SalesOrderService` | `dotnet watch` | `http://localhost:5002` |
| Frontend (MVC)            | `FrontEnd`          | `dotnet watch` | `http://localhost:5000` |

### 3.1 Customer Service

```powershell
cd CustomerService
dotnet watch
```

Default URL:

```text
http://localhost:5001
```

### 3.2 Sales Order Service

```powershell
cd SalesOrderService
dotnet watch
```

Default URL:

```text
http://localhost:5002
```

### 3.3 Frontend MVC

```powershell
cd FrontEnd
dotnet watch
```

Default URL:

```text
http://localhost:5000
```

> Pastikan ketiga service berjalan secara bersamaan.

---

## 4. Mengakses UI

Setelah seluruh service berjalan, buka browser dan akses:

```text
http://localhost:5000/Order/Index
```

### Fitur UI

#### Daftar Order & Pagination

Menampilkan daftar Sales Order dengan navigasi halaman secara dinamis.

#### Pencarian & Filter

Mendukung pencarian berdasarkan:

- Nomor SO
- Nama Customer
- Tanggal Order

#### Export Excel

Mengunduh seluruh daftar order dalam format `.xlsx`.

#### Tambah & Edit Order

Mengelola:

- Header Sales Order
- Customer
- Alamat
- Tanggal Order
- Daftar item
- Quantity
- Price

Item order dapat dikelola secara dinamis melalui UI.

#### Hapus Order

Menghapus Sales Order dengan konfirmasi modal menggunakan **SweetAlert2**.

---

## 5. API Endpoint

> **Catatan Autentikasi:**  
> Seluruh endpoint saat ini bersifat publik dan **belum menggunakan autentikasi JWT**. Endpoint dapat diakses langsung oleh client.

### Base URL

```text
http://localhost:5002
```

---

### A. Mendapatkan Daftar Order

**GET**

```text
http://localhost:5002/api/orders
```

#### Query Parameters

| Parameter    | Tipe   | Keterangan                         |
| ------------ | ------ | ---------------------------------- |
| `keyword`    | string | Pencarian nomor SO / nama customer |
| `orderDate`  | date   | Filter berdasarkan tanggal order   |
| `pageNumber` | int    | Nomor halaman                      |
| `pageSize`   | int    | Jumlah data per halaman            |

#### Contoh Request — PowerShell

```powershell
Invoke-RestMethod `
    -Uri "http://localhost:5002/api/orders?pageNumber=1&pageSize=10" `
    -Method Get
```

#### Contoh dengan Keyword

```powershell
Invoke-RestMethod `
    -Uri "http://localhost:5002/api/orders?keyword=SO-2026&pageNumber=1&pageSize=10" `
    -Method Get
```

#### Contoh dengan Filter Tanggal

```powershell
Invoke-RestMethod `
    -Uri "http://localhost:5002/api/orders?orderDate=2026-08-21&pageNumber=1&pageSize=10" `
    -Method Get
```

#### Contoh dengan Keyword dan Tanggal

```powershell
Invoke-RestMethod `
    -Uri "http://localhost:5002/api/orders?keyword=SO-2026&orderDate=2026-08-21&pageNumber=1&pageSize=10" `
    -Method Get
```

---

### B. Membuat Order Baru

**POST**

```text
http://localhost:5002/api/orders
```

#### Request Header

```text
Content-Type: application/json
```

#### Request Body

```json
{
  "soNo": "SO-2026-099",
  "orderDate": "2026-08-21T00:00:00",
  "customerId": 1,
  "address": "Jl. Sudirman No. 12, Jakarta",
  "items": [
    {
      "itemName": "Laptop Workstation",
      "quantity": 1,
      "price": 15000000
    },
    {
      "itemName": "Wireless Mouse",
      "quantity": 1,
      "price": 250000
    }
  ]
}
```

#### Contoh Request — PowerShell

```powershell
$body = @{
    soNo = "SO-2026-099"
    orderDate = "2026-08-21T00:00:00"
    customerId = 1
    address = "Jl. Sudirman No. 12, Jakarta"
    items = @(
        @{
            itemName = "Laptop Workstation"
            quantity = 1
            price = 15000000
        },
        @{
            itemName = "Wireless Mouse"
            quantity = 1
            price = 250000
        }
    )
} | ConvertTo-Json -Depth 5

Invoke-RestMethod `
    -Uri "http://localhost:5002/api/orders" `
    -Method Post `
    -ContentType "application/json; charset=utf-8" `
    -Body ([System.Text.Encoding]::UTF8.GetBytes($body))
```

---

### C. Mengubah Order

**PUT**

```text
http://localhost:5002/api/orders/{id}
```

Contoh:

```text
http://localhost:5002/api/orders/1
```

#### Request Header

```text
Content-Type: application/json
```

#### Request Body

```json
{
  "soNo": "SO-2026-001",
  "orderDate": "2026-08-20T00:00:00",
  "customerId": 1,
  "address": "Jl. Pemuda No. 123 (Updated)",
  "items": [
    {
      "itemName": "Monitor LG 27 Inch",
      "quantity": 2,
      "price": 3200000
    }
  ]
}
```

#### Contoh Request — cURL

```bash
curl -X PUT http://localhost:5002/api/orders/1 \
  -H "Content-Type: application/json" \
  -d '{
        "soNo": "SO-2026-001",
        "orderDate": "2026-08-20T00:00:00",
        "customerId": 1,
        "address": "Jl. Pemuda No. 123 (Updated)",
        "items": [
          {
            "itemName": "Monitor LG 27 Inch",
            "quantity": 2,
            "price": 3200000
          }
        ]
      }'
```

---

### D. Menghapus Order

**DELETE**

```text
http://localhost:5002/api/orders/{id}
```

Contoh:

```text
http://localhost:5002/api/orders/1
```

#### Contoh Request — PowerShell

```powershell
Invoke-RestMethod `
    -Uri "http://localhost:5002/api/orders/1" `
    -Method Delete
```

---

## 6. Ringkasan Endpoint

| Method   | Endpoint           | Fungsi                                         |
| -------- | ------------------ | ---------------------------------------------- |
| `GET`    | `/api/orders`      | Mendapatkan daftar order + pagination + search |
| `POST`   | `/api/orders`      | Membuat order baru                             |
| `PUT`    | `/api/orders/{id}` | Mengubah order                                 |
| `DELETE` | `/api/orders/{id}` | Menghapus order                                |

---

## 7. Urutan Menjalankan Aplikasi

Ikuti langkah berikut untuk menjalankan aplikasi dari awal:

1. Pastikan SQL Server aktif.
2. Buka SQL Server Management Studio (SSMS).
3. Buat database `SalesOrderDB`.
4. Jalankan script pembuatan tabel.
5. Jalankan Stored Procedure `sp_get_orders`.
6. Pastikan connection string pada masing-masing project sudah mengarah ke database yang benar.
7. Buka terminal pertama dan jalankan `CustomerService`.
8. Buka terminal kedua dan jalankan `SalesOrderService`.
9. Buka terminal ketiga dan jalankan `FrontEnd`.
10. Pastikan ketiga service berjalan tanpa error.
11. Buka browser.
12. Akses:

```text
http://localhost:5000/Order/Index
```

---

## 8. Teknologi

Teknologi utama yang digunakan:

- **.NET 8 / .NET 10**
- **ASP.NET Core Web API**
- **ASP.NET Core MVC**
- **SQL Server**
- **Stored Procedure**
- **PowerShell**
- **cURL**
- **SweetAlert2**
- **Excel Export**
- **Microservices / Multi-project Architecture**
- **Pagination & Search**

---

## 9. Arsitektur Project

Struktur project secara umum:

```text
SalesOrderManagementSystem/
│
├── CustomerService/
│   └── CustomerService
│
├── SalesOrderService/
│   └── SalesOrderService
│
└── FrontEnd/
    └── FrontEnd
```

### CustomerService

Service yang menangani data dan operasi terkait customer.

```text
http://localhost:5001
```

### SalesOrderService

Backend API yang menangani Sales Order, termasuk:

- Create Order
- Get Orders
- Update Order
- Delete Order
- Search
- Pagination
- Perhitungan Grand Total

```text
http://localhost:5002
```

### FrontEnd

Aplikasi ASP.NET Core MVC yang menyediakan UI untuk pengguna.

```text
http://localhost:5000
```

---

## 10. Fitur Utama

| Fitur                       | Status            |
| --------------------------- | ----------------- |
| Customer Management         | ✅                |
| Sales Order Management      | ✅                |
| Add Order                   | ✅                |
| Edit Order                  | ✅                |
| Delete Order                | ✅                |
| Search Order                | ✅                |
| Filter by Order Date        | ✅                |
| Pagination                  | ✅                |
| Stored Procedure Pagination | ✅                |
| Grand Total Calculation     | ✅                |
| Export Excel                | ✅                |
| SweetAlert2 Confirmation    | ✅                |
| JWT Authentication          | ❌ Belum tersedia |

---

## 11. Catatan

- Pastikan **SQL Server** sedang berjalan sebelum menjalankan API.
- Pastikan database bernama `SalesOrderDB`.
- Pastikan connection string sesuai dengan konfigurasi SQL Server lokal.
- Jalankan seluruh service secara bersamaan.
- Pastikan port `5000`, `5001`, dan `5002` tidak digunakan oleh aplikasi lain.
- Endpoint API saat ini belum menggunakan autentikasi JWT.
