# Catatan Desain: Sales Order System

Dokumen ini berisi rangkuman keputusan teknis, alur pengerjaan, dan catatan refleksi saya selama membangun aplikasi Sales Order System ini.

---

## 1. Kenapa Service-nya Dipisah?

Di projek ini, saya sengaja memisah aplikasi menjadi tiga bagian: `CustomerService`, `SalesOrderService`, dan `FrontEnd`.

Alasan utamanya adalah untuk menerapkan **Separation of Concerns**:

- **Memisahkan Master Data & Transaksi:** Data Customer (master) dan Sales Order (transaksi) punya siklus hidup yang berbeda. Kalau data customer butuh pengembangan lebih lanjut di kemudian hari, service order tidak akan terganggu.
- **Backend yang Fleksibel:** Dengan membuat backend berupa REST API murni, aplikasi ini jadi _decoupled_. Kalau ke depannya mau bikin frontend baru pakai React, Vue, atau aplikasi Mobile, backend-nya tinggal dipakai tanpa perlu rombak ulang.

---

## 2. Pengerjaan Sendiri vs Dibantu AI

Dalam proses pembuatannya, saya membagi pengerjaan menjadi dua ranah:

- **Dikerjakan Sendiri:**
  - Merancang skema tabel dan relasi database (`SALES_SO`, `SALES_SO_LITEM`, `COM_CUSTOMER`).
  - Menyusun alur logika dari Frontend ke Controller sampai pemanggilan API.
  - Menangani logika JavaScript untuk form dinamis (fitur tambah/hapus baris item belanja secara _real-time_).
  - Melakukan pengujian alur aplikasi (_end-to-end testing_) dan _debugging_.

- **Dibantu oleh AI:**
  - Menggenerate boilerplate code awal (seperti struktur DTO dan kelas Model).
  - Membantu menyusun draf query Stored Procedure untuk pagination dan window function.
  - Memberikan acuan sintaks pembuatan script pengujian API menggunakan cURL dan PowerShell.
  - Merapikan snippet JavaScript SweetAlert2 agar lebih mudah dibaca.

---

## 3. Keputusan Teknis Penting

- **Pagination di Sisi Database (Stored Procedure):** Pencarian dan pembagian halaman (_paging_) sengaja ditaruh langsung di SQL Server menggunakan `OFFSET-FETCH`. Cara ini jauh lebih efisien dan hemat memori daripada narik seluruh data ke aplikasi baru di filter.
- **Penggunaan `ON DELETE CASCADE`:** Untuk relasi antara Order Header dan Order Items, saya pasang _cascade delete_ di database. Jadi pas transaksi order dihapus, semua item di dalamnya otomatis ikut terhapus tanpa bikin query hapus berulang kali.

---

## 4. Bagian yang Paling Menantang

Hal paling menantang di projek ini adalah mengelola Form Dinamis Detail Item di Frontend.

Pas pengguna menambah atau menghapus baris item belanja di layar, penataan urutan index array di JavaScript harus benar-benar presisi (items[0], items[1], dst). Kalau indexnya loncat atau berantakan setelah ada baris yang dihapus, ASP.NET Core Model Binding di server bakal bingung dan nerima data list item sebagai null. Solusinya, saya bikin fungsi utilitas kecil di JavaScript untuk mereset ulang penamaan name pada input setiap kali ada perubahan baris.
