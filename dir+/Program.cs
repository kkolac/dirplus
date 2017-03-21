using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


// Znanja za 1 kolokvij iz OS
/*
System.Environment
System.IO.Directory
System.IO.DirectoryInfo
System.IO.File
System.IO.FileInfo
System.IO.DriveInfo
System.IO.StreamReader
System.IO.StreamWriter
// GUI
System.Windows.Forms.FolderBrowserDialog
System.Windows.Forms.OpenFileDialog
 */


namespace Files
{
    // podaci o folderu
    class FolderInfo
    {
        public string Name { get; set; }
        public int FilesCount { get; set; }
        public long FilesLength { get; set; }
    }

    class Program
    {
        // parametri -> folder patern
        static void Main(string[] args)
        {
            

            string rootFolder;
            string filter = "";

            // tekući folder
            if (args.Count() == 0)
            {
                rootFolder = Directory.GetCurrentDirectory();
            }
            else
            {
                rootFolder = args[0];
            }

            // pattern za filtriranje
            if (args.Count() > 1)
            {
                filter = args[1];
            }
            else
            {
                filter = "*";
            }

            if (Directory.Exists(rootFolder))
                ispisiFolder(rootFolder, filter);
            else
                Console.WriteLine("Greška ulaznog argumenta - pogrešno zadani folder!");

            Console.ReadLine();
        }

        // rekurzivna metoda za izračun broja datoteka i veličine foldera
        private static FolderInfo racunajFolder(DirectoryInfo di, string pattern)
        {
            long velicina = 0;   // velicina svih datoteka zajedno
            int koliko = 0;  // broj datoteka u folderu
            try
            {
                // prvo prođemo sve filove i zbojimo veličine
                FileInfo[] files = di.GetFiles(pattern);
                foreach (FileInfo file in files)
                {
                    koliko++;
                    velicina += file.Length;
                }

                // nakon toga prođemo sve podfoldere u folderu
                DirectoryInfo[] folders = di.GetDirectories();
                foreach (DirectoryInfo folder in folders)
                {
                    // rekurzivni poziv
                    FolderInfo fi = racunajFolder(folder, pattern);
                    velicina += fi.FilesLength;
                    koliko += fi.FilesCount;
                }
            }
            catch
            { }

            return new FolderInfo { Name = di.Name, FilesCount = koliko, FilesLength = velicina };
        }

        // metoda za ispis sadržaja foldera
        private static void ispisiFolder(string path, string pattern)
        {
            // targetFolder
            DirectoryInfo targetFolder = new DirectoryInfo(path);
            long velicina = 0, dirVelicina = 0;
            int koliko = 0, dirKoliko = 0;

            // naziv foldera i prazan red
            Console.WriteLine("MEV directory info plus 1.0");
            Console.WriteLine("\nDirecory " + path);
            Console.WriteLine();

            // popis datoteka u folderu
            try
            {
                FileInfo[] files = targetFolder.GetFiles();
                foreach (FileInfo file in files)
                {
                    velicina += file.Length;
                    koliko++;
                    Console.WriteLine("{0,-20}{1,18:N0} bytes {2}", file.LastWriteTime.ToString("dd.MM.yyyy hh:mm:ss"), file.Length, Path.GetFileName(file.Name));
                }
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Greška: nedozvoljen pristup...");
            }
            catch (PathTooLongException)
            {
                Console.WriteLine("Greška: predugački naziv foldera...");
            }

            // popis podfoldera u folderu
            DirectoryInfo[] folders = targetFolder.GetDirectories();
            foreach (DirectoryInfo folder in folders)
            {
                FolderInfo fi = racunajFolder(folder, pattern);
                dirVelicina += fi.FilesLength;
                dirKoliko += fi.FilesCount;
                Console.WriteLine("{0,13} files {1,18:N0} bytes <DIR> {2}", fi.FilesCount, fi.FilesLength, Path.GetFileName(fi.Name));
            }
            // slobodno mjesta na disku
            DriveInfo disc = new DriveInfo(Path.GetPathRoot(path));

            // rekapitulacija
            Console.WriteLine("{0,11} file(s) {1,18:N0} bytes", koliko, velicina);
            Console.WriteLine("{0,11} dirs(s) {1,18:N0} bytes", dirKoliko, dirVelicina);
            Console.WriteLine("   total free space {0,18:N0} bytes", disc.TotalFreeSpace);
        }
    }
}
