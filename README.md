<div align="center">
<br>
  <br>
  <h1>Gasses Journey Home</h1>
<!--  <br>-->
<!--  [**View the App in TestFlight**]((https://testflight.apple.com/join/vsqXp2UU)) -->
</div>

## Table of Contents

- [Table of Contents](#table-of-contents)
- [Description](#description)
- [Branches](#branches)
  - [Main Branch](#main-branch)
  - [Dev Branch](#dev-branch)
  - [Supporting Branches](#supporting-branches)
- [Commit Messages](#commit-messages)
- [Run Locally](#run-locally)
- [How to Contribute](#how-to-contribute)

## Description

Gasses Journey Home is an online multiplayer co-op puzzle game built with Unity Netcode for GameObjects and Unity Relay, offering real-time collaboration and seamless connectivity. Players control two LPG tanks, Gaspi and Tanko, who fall off a delivery truck and get lost in the busy city streets. To return to the LPG base, they must work together to solve puzzles and overcome urban obstacles like busy intersections and construction sites. The game emphasizes teamwork, with players needing to communicate and coordinate their actions to navigate the challenges and bring the duo home safely.

## Branches

#### Main Branch

Branch `main` adalah default branch. Branch ini hanya menerima pull request dari branch `dev`. Semua perubahan di branch ini seharusnya sudah melalui proses pengujian yang lengkap dan siap untuk dirilis kepada pengguna akhir. `Hotfix` dapat dilakukan langsung di branch ini jika diperlukan untuk perbaikan kritis dengan solusi sementara.

#### Dev Branch

Sebagai developer, kita akan brancing dari branch `dev` & merging ke branch `dev`. Branch ini adalah branch pengembangan dari aplikasi. Semua fitur baru, perbaikan bug, dan perubahan lainnya diimplementasikan di sini sebelum digabungkan ke branch `main`. **Semua support branches (seperti feature, bugfix, refactor, dan dll) dibuat dari branch development sebagai source dan harus diajukan sebagai pull request ke branch** `dev`.

#### Supporting Branches

Supporting branches digunakan untuk membantu pengembangan paralel antara anggota tim & memudahkan pelacakan fitur. Tidak seperti cabang `main` dan `dev` branch, branches ini selalu memiliki waktu hidup yang terbatas, karena mereka akan dihapus pada akhirnya setelah di merge ke branch `dev` atau branch `main` untuk `hotfix`.

Nama branch harus `"type/task-name"`.

Berbagai jenis branch yang akan kita gunakan:

| Type      | Description                                                                                           | Example                       |
| --------- | ----------------------------------------------------------------------------------------------------- | ----------------------------- |
| `feature` | Untuk menambahkan, menghapus, atau memodifikasi fitur.                                                | feature/add-summary-view      |
| `bugfix`  | Untuk memperbaiki bug                                                                                 | bugfix/fix-button-alignment   |
| `hotfix`  | Untuk memperbaiki masalah kritis dengan cepat, biasanya di branch main dengan solusi sementara.       | hotfix/fix-critical-security  |
| `docs`    | Untuk menulis, memperbarui, atau memperbaiki dokumentasi.                                             | docs/update-readme            |
| `refactor`| Untuk meningkatkan struktur internal dan organisasi kode tanpa mengubah perilaku eksternalnya         | refactor/simplify-ml-module   |
| `chore`   | Untuk tugas rutin dan pemeliharaan yang tidak secara langsung mempengaruhi fungsionalitas aplikasi.   | chore/update-dependencies     |

## Commit Messages

Menggunakan metode semantic Git commit dapat sangat meningkatkan keterbacaan log Git dan membantu mengatur ruang lingkup komit individu.

Pesan commit harus `"type: short task description"`.

Type: Jenis perubahan yang dibuat (example: feature, bugfix).
Short Task Description: Deskripsi singkat dan jelas tentang perubahan yang dibuat.

| Type      | Description                                                                                           | Example                                   |
| --------- | ----------------------------------------------------------------------------------------------------- | ----------------------------------------- |
| `feature` | Untuk menambahkan, menghapus, atau memodifikasi fitur.                                                | feature: add user authentication          |
| `bugfix`  | Untuk memperbaiki bug                                                                                 | bugfix: fix button alignment              |
| `hotfix`  | Untuk memperbaiki masalah kritis dengan cepat, biasanya di branch main dengan solusi sementara.       | hotfix: fix critical security             |
| `docs`    | Untuk menulis, memperbarui, atau memperbaiki dokumentasi.                                             | docs: update readme                       |
| `refactor`| Untuk tugas rutin dan pemeliharaan yang tidak secara langsung mempengaruhi fungsionalitas aplikasi.   | refactor: simplify machine learning module|
| `chore`   | Untuk tugas rutin dan pemeliharaan yang tidak secara langsung mempengaruhi fungsionalitas aplikasi.   | chore: update dependencies                |

#### Additional Tips

- **Clear and Short**: Gunakan kalimat ringkas untuk menjelaskan apa yang telah diubah.
- **Describing Changes**: Pastikan singkat menjelaskan perubahan secara akurat.
- **Avoid Common Words**: Hindari kata-kata seperti 'fixed', 'changed', or 'updated' tanpa konteks tambahan.
- **Consistency**: Selalu gunakan format yang sama untuk setiap komitmen.
- **Relevance**: Pastikan setiap komit hanya mencakup satu unit perubahan logis. 
- **Additional Details**: Jika perlu, sertakan deskripsi yang lebih rinci dalam commit message body.

Dengan mengikuti panduan ini, dapat menjaga konsistensi dan kualitas pesan komitmen, membuatnya lebih mudah bagi anggota tim untuk memahami sejarah perubahan proyek.

## Run Locally

Disini akan menggunakan terminal. Jika kamu menggunakan yang lain seperti sourcetree, gitHub Dekstop, atau bahkan di xcode langsung. alur nya tetap sama, kamu bisa tetap mengikuti instruksi sesuai nomor.

1. Clone the repo

```bash
    git clone https://github.com/NDLP-ADA/Sentify.git
```

2. Enter the repo

```bash
    cd sentify
```
## How To Contribute

1. Ikuti instruksi dibagian [Run Locally](#run-locally).

2. Pastikan Anda mendapatkan commit terbaru dari branch `main` di repo remote

   ```bash
   # Get the latest commits from the remote main branch

   git checkout main

   git fetch origin main

   git pull --rebase origin main
   ```
   
3. Pindah ke branch `dev` dan pastikan juga Anda mendapatkan commit terbaru dari branch `dev` di repo remote sebelum membuat branch baru.

   ```bash
   # Get the latest commits from the remote dev branch

   git checkout dev

   git fetch origin dev

   git pull --rebase origin dev
   ```

4. Jika Anda ingin membuat perubahan pada aplikasi, buat cabang baru. Ikuti [this convention](#branches) untuk aturan branches

   Example:

   ```bash
   git checkout -b "type/task-name"
   ```

5. Buat perubahan pada aplikasi di branch baru.

   Jika Anda sudah yakin dengan perubahan yang Anda buat, tambahkan dan buat commit. Ikuti [this convention](#commit-messages) untuk aturan commit message.

   ```bash
   git add .

   git commit -m "type: short task description"
   ```

6. Sebelum anda mendorong perubahan ke remote repo, pastikan anda mendapatkan commit terbaru dari branch `dev`. di repo remote dan rebase ke branch anda.

   ```bash
   # Get the latest commits from the remote dev branch

   git checkout dev

   git fetch origin dev

   git pull --rebase origin dev
   ```

   ```bash
   # Rebase your branch

   git checkout "type/task-name"

   git rebase dev
   ```

7. Push perubahan baru ke remote repo untuk membuat perubahan itu tersedia di remote repo.

   ```bash
   git push origin "type/task-name"
   ```

8. Ketika sudah selesai, buat pull request baru di repo github untuk menggabungkan branch anda ke branch `dev`. Assign reviewer ke pull request anda. Reviewer dapat menyetujui perubahan yang anda buat dan menggabungkannya ke cabang `dev` atau meminta perubahan lebih lanjut kepada anda sebelum pull request digabungkan.

9. Anda dapat melakukan lebih banyak perubahan di branch anda jika perlu. Lakukan langkah 3, 4, 5, 6, 7 untuk membuat nya tersedia di remote.

10. Hati-hati, jangan menyentuh branch `main`.

> Harap pastikan Anda mengikuti instruksi langkah demi langkah dan "Pastikan Anda mendapatkan commit terbaru sebelum melakukan perubahan dan sebelum push perubahan baru"

> _Perhatikan bahwa ini hanyalah salah satu cara untuk memberikan kontribusi. Jika Anda memiliki cara atau saran yang lebih baik, Anda dapat memodifikasi README ini._
