using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== DEMO REAL: MANUAL THREAD vs TASK (THREADPOOL) ===\n");

        int jumlahTugas = 10;

        // ------------------------------------------------------------------
        // METODE 1: MANUAL THREAD (Membuat 10 Thread OS baru dari nol)
        // ------------------------------------------------------------------
        Console.WriteLine("--- 1. MENJALANKAN DENGAN MANUAL THREAD (new Thread) ---");
        Stopwatch sw1 = Stopwatch.StartNew();

        Thread[] threads = new Thread[jumlahTugas];
        for (int i = 0; i < jumlahTugas; i++)
        {
            int idTugas = i + 1;
            // Memaksa OS mengalokasikan memori & thread fisik baru
            threads[i] = new Thread(() => ProsesTugasManual(idTugas));
            threads[i].Start();
        }

        // Tunggu semua thread manual selesai
        foreach (var t in threads)
        {
            t.Join();
        }

        sw1.Stop();
        Console.WriteLine($"--> Total Waktu Manual Thread: {sw1.ElapsedMilliseconds} ms\n");


        // ------------------------------------------------------------------
        // METODE 2: TASK / TPL (Meminjam Thread yang sudah ada di ThreadPool)
        // ------------------------------------------------------------------
        Console.WriteLine("--- 2. MENJALANKAN DENGAN TASK (Task.Run / ThreadPool) ---");
        Stopwatch sw2 = Stopwatch.StartNew();

        Task[] tasks = new Task[jumlahTugas];
        for (int i = 0; i < jumlahTugas; i++)
        {
            int idTugas = i + 1;
            // Menyerahkan tugas ke antrean ThreadPool
            tasks[i] = Task.Run(() => ProsesTugasTask(idTugas));
        }

        // Menggunakan 'await' agar thread utama tidak macet/freeze saat menunggu
        await Task.WhenAll(tasks);

        sw2.Stop();
        Console.WriteLine($"--> Total Waktu Task (ThreadPool): {sw2.ElapsedMilliseconds} ms\n");

        Console.WriteLine("=== PROSES SELESAI ===");
    }

    static void ProsesTugasManual(int id)
    {
        Console.WriteLine($"[Manual Thread] Tugas {id} dikerjakan di Thread ID: {Environment.CurrentManagedThreadId}");
        Thread.Sleep(200); // Simulasi kerjaan butuh waktu 0.2 detik
    }

    static void ProsesTugasTask(int id)
    {
        Console.WriteLine($"[ThreadPool Task] Tugas {id} dikerjakan di Thread ID: {Environment.CurrentManagedThreadId}");
        Thread.Sleep(200); // Simulasi kerjaan butuh waktu 0.2 detik
    }
}