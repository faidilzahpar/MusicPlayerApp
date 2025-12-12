# 🎵 MusicPlayerApp (WPF .NET 8 + ManagedBass)

MusicPlayerApp adalah aplikasi pemutar musik desktop berbasis **WPF (.NET 8)**  
dengan dukungan audio menggunakan **ManagedBass**, pembacaan metadata via **TagLib#**,  
serta fitur sinkronisasi lagu dari folder lokal.

Aplikasi ini dibuat sebagai proyek mata kuliah **Pemrograman Berbasis Objek (PBO)**.

---

## 📥 1. Clone Repository ke Local

Pastikan kamu sudah menginstall **Git**.

```sh
git clone https://github.com/USERNAME/MusicPlayerApp.git
cd MusicPlayerApp
```

---
## 📦 2. Dependencies (NuGet)

Project ini menggunakan beberapa library:

- ManagedBass  
- TagLibSharp  
- SQLite-net-pcl  
- WindowsAPICodePack-Shell  

---
## 🎧 3. Wajib Download bass.dll (Native Audio Engine)

ManagedBass membutuhkan file native bernama bass.dll agar pemutaran audio dapat berjalan.

Silakan download dari link resmi berikut:

👉 **BASS 2.4 for Windows**
https://www.un4seen.com/download.php?bass24

Setelah selesai download, ekstrak lalu ambil file:
```
bass24\x64\bass.dll
```
---
## 📂 4. Tempatkan bass.dll di folder output aplikasi

Copy file bass.dll ke:
```
MusicPlayerApp/bin/Debug/net8.0-windows/
```
atau jika menggunakan mode release:
```
MusicPlayerApp/bin/Release/net8.0-windows/
```
⚠️ **Tanpa file ini aplikasi akan crash saat memutar lagu.**

---
## 🗄 5. Melihat Database SQLite

Aplikasi ini menggunakan SQLite untuk menyimpan data lagu.  
Untuk melihat isi database (`musicplayer.db`), Anda dapat menggunakan salah satu aplikasi berikut:

### ✔ SQLiteStudio (Gratis & Mudah)
Download di:
https://sqlitestudio.pl/

Cara membuka database:
1. Buka SQLiteStudio  
2. Klik **Database → Add a database**  
3. Cari file:

```
%LOCALAPPDATA%/MusicPlayerApp/musicplayer.db
```

atau lokasi manual:

```
C:\Users\USERNAME\AppData\Local\MusicPlayerApp\musicplayer.db
```

4. Klik **OK**, database akan tampil.

---

### ✔ DB Browser for SQLite (Alternatif)
Unduh di:
https://sqlitebrowser.org/

Cara membuka:
1. Open Database  
2. Pilih file `musicplayer.db`

---

### 📍 Lokasi Database

Secara default, database tersimpan di:

```
C:\Users\<USERNAME>\AppData\Local\MusicPlayerApp\musicplayer.db
```

Jika Anda menjalankan aplikasi dalam mode Debug dan ingin memastikan lokasinya, aplikasi akan menampilkan MessageBox lokasi database saat startup.
