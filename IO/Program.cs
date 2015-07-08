using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.AccessControl;
using System.IO.Compression;
using System.Threading;
using System.Net;

namespace IO
{
    class Program
    {
        private static async void Parctice1DirFileOp()
        {
            Console.WriteLine("######################");
            Console.WriteLine("1. Listing drives info");
            Console.WriteLine("######################");
            Console.WriteLine("");

            // Gathering information about HDDs
            DriveInfo[] drivesInfo = DriveInfo.GetDrives();
            foreach (DriveInfo driveInfo in drivesInfo)
            {
                Console.WriteLine("");
                Console.WriteLine("Drive {0}", driveInfo.Name);
                Console.WriteLine("  File type: {0}", driveInfo.DriveType);
                if (driveInfo.IsReady == true)
                {
                    Console.WriteLine("  Volume label: {0}", driveInfo.VolumeLabel);
                    Console.WriteLine("  File system: {0}", driveInfo.DriveFormat);
                    Console.WriteLine(
                        "  Available space to current user:{0,15} bytes",
                        driveInfo.AvailableFreeSpace);
                    Console.WriteLine(
                        "  Total available space:          {0,15} bytes",
                        driveInfo.TotalFreeSpace);
                    Console.WriteLine(
                        "  Total size of drive:            {0,15} bytes ",
                        driveInfo.TotalSize);
                }
            }

            Console.WriteLine("");
            Console.WriteLine("#######################");
            Console.WriteLine("2. Folder manipulations");
            Console.WriteLine("#######################");
            Console.WriteLine("");

            // Folder creation (two methods)
            try
            {
                Console.WriteLine("Creating two folders...");
                var directory = Directory.CreateDirectory(@"C:\Temp\ProgrammingInCSharp\Directory");
                var directoryInfo = new DirectoryInfo(@"C:\Temp\ProgrammingInCSharp\DirectoryInfo");
                directoryInfo.Create();
            }
            catch (UnauthorizedAccessException exc)
            {
                Console.WriteLine("");
                Console.WriteLine("---ERROR-BEGIN---");
                Console.WriteLine(String.Format("UnauthorizedAccessException: {0}", exc.ToString()));
                Console.WriteLine("---ERROR-END---");
                Console.WriteLine("");
            }

            // Delete folders
            try
            {
                Console.WriteLine("Deleting non existing folder...");
                Directory.Delete(@"C:\Temp\ProgrammingInCSharp\DirectoryNonExistent");
            }
            catch (DirectoryNotFoundException exc)
            {
                Console.WriteLine("");
                Console.WriteLine("---ERROR-BEGIN---");
                Console.WriteLine(String.Format("Trying to delete invalid directory \"{0}\". DirectoryNotFoundException: {1}", @"C:\Temp\ProgrammingInCSharp\DirectoryNonExistent", exc.ToString()));
                Console.WriteLine("---ERROR-END---");
                Console.WriteLine("");
            }


            if (Directory.Exists(@"C:\Temp\ProgrammingInCSharp\Directory"))
            {
                Console.WriteLine("");
                Console.WriteLine("Deleting existing folder (using static Directory class)...");
                Directory.Delete(@"C:\Temp\ProgrammingInCSharp\Directory");
            }

            var directoryInfo2 = new DirectoryInfo(@"C:\Temp\ProgrammingInCSharp\DirectoryInfo");
            if (directoryInfo2.Exists)
            {
                Console.WriteLine(String.Format("Folder {0} data: ", @"C:\Temp\ProgrammingInCSharp\DirectoryInfo"));
                Console.WriteLine(String.Format("   Attributes: {0}", directoryInfo2.Attributes));

                Console.WriteLine("Deleting existing folder (using new DirectoryInfo object)...");
                directoryInfo2.Delete();
            }

            // Access permissions to folders
            Console.WriteLine("");
            Console.WriteLine("Creating folder with custom permissions...");

            if (Directory.Exists(@"C:\Temp\ProgrammingInCSharp\SecurityTest"))
            {
                Directory.Delete(@"C:\Temp\ProgrammingInCSharp\SecurityTest");
            }

            DirectoryInfo directoryInfo3 = new DirectoryInfo(@"C:\Temp\ProgrammingInCSharp\SecurityTest");
            directoryInfo3.Create();
            DirectorySecurity directorySecurity = directoryInfo3.GetAccessControl();
            directorySecurity.AddAccessRule(
                new FileSystemAccessRule("Backup Operators", FileSystemRights.CreateFiles, AccessControlType.Allow)
            );
            directoryInfo3.SetAccessControl(directorySecurity);

            // Directory tree listing
            var depth = 3;
            string searchPattern = "*a*"; //accepts '?' and '*'.
            string listingRoot = @"C:\Program Files";
            var startLevel = 0;

            Console.WriteLine("");
            Console.WriteLine(String.Format("Directory listing of \"{0}\": search pattern \"{1}\", depth {2}, start level {3}...", listingRoot, searchPattern, depth, startLevel));
            DirectoryInfo directoryInfoTree = new DirectoryInfo(@"C:\Program Files");
            _listDirectories(directoryInfoTree, searchPattern, depth, startLevel);

            // Moving directory
            Console.WriteLine("");
            Console.WriteLine(String.Format("Moving directory {0} to {1}...", @"C:\Temp\ProgrammingInCSharp\DirectoryToBeMoved", @"C:\Temp\ProgrammingInCSharp\DirectoryAfterMoving"));
            _deleteDirIfExist(@"C:\Temp\ProgrammingInCSharp\DirectoryAfterMoving", false);
            Directory.CreateDirectory(@"C:\Temp\ProgrammingInCSharp\DirectoryToBeMoved");
            Directory.Move(@"C:\Temp\ProgrammingInCSharp\DirectoryToBeMoved", @"C:\Temp\ProgrammingInCSharp\DirectoryAfterMoving");

            // Files listing
            Console.WriteLine("");
            Console.WriteLine(String.Format("Listing files from directory {0} (LAZY, using static Directory class)...", @"C:\"));
            foreach (var fl in Directory.EnumerateFiles(@"C:\"))
            {
                Console.WriteLine(String.Format("-- {0}", fl));
            }
            Console.WriteLine("");
            Console.WriteLine(String.Format("Listing files from directory {0} (LAZY, using new DirectoryInfo object)...", @"C:\"));
            DirectoryInfo directoryInfoListing = new DirectoryInfo(@"C:\");
            foreach (var fl in directoryInfoListing.EnumerateFiles())
            {
                Console.WriteLine(String.Format("-- {0}, size {1} b", fl.FullName, fl.Length));
            }

            Console.WriteLine("");
            Console.WriteLine("######################");
            Console.WriteLine("3. Files manipulations");
            Console.WriteLine("######################");
            Console.WriteLine("");

            // File creation/deletion (two methods)
            string dirPath = @"C:\Temp\ProgrammingInCSharp\FilesOperations";
            string fileName = Path.GetRandomFileName();
            string filePath = Path.Combine(dirPath, fileName);
            _deleteDirIfExist(dirPath, true);

            // Create directory in a new thread and await for completing operation.
            // Problem is when we navigate into existing folder in Windows Explorer and run application.
            // Folder is first removed and created once again.
            // Before forder is created (don't know why!) application tries to create files in it!
            Thread thread = new Thread(new ThreadStart(() => Directory.CreateDirectory(dirPath)));
            thread.Start();
            thread.Join();

            if (!File.Exists(filePath))
            {
                Console.WriteLine("");
                Console.WriteLine(String.Format("Creating file {0} (using static File class)...", filePath));
                var fileTxt = File.CreateText(filePath);
                fileTxt.WriteLine("{0,15}", 1234);
                fileTxt.Close();
            }

            if (File.Exists(filePath))
            {
                Console.WriteLine("");
                Console.WriteLine(String.Format("Deleting file {0} (using static File class)...", filePath));
                File.Delete(filePath);
            }

            FileInfo fileInfo = new FileInfo(filePath);
            if (fileInfo.Exists)
            {
                Console.WriteLine("");
                Console.WriteLine(String.Format("Deleting file {0} (using new FileInfo object)...", filePath));
                fileInfo.Delete();
            }

            // Additional file operations (immilar to directory operations)
            // File.Move(src, dest)
            // (new FileInfo(...)).MoveTo(dest)
            // File.Copy(src, dest)
            // (new FileInfo(...)).CopyTo(dest)

            // Additional Path methods
            //string path = @"C:\temp\subdir\file.txt";
            //Console.WriteLine(Path.GetDirectoryName(path)); // Displays C:\temp\subdir
            //Console.WriteLine(Path.GetExtension(path)); // Displays .txt
            //Console.WriteLine(Path.GetFileName(path)); // Displays file.txt
            //Console.WriteLine(Path.GetPathRoot(path)); // Displays C:\
        }

        private static void Parctice2StreamsOp()
        {
            Console.WriteLine("");
            Console.WriteLine("#####################################");
            Console.WriteLine("STREAMS (implemets decorator pattern)");
            Console.WriteLine("#####################################");
            Console.WriteLine("");

            Console.WriteLine("");
            Console.WriteLine("#######################");
            Console.WriteLine("1. File writing/reading");
            Console.WriteLine("#######################");
            Console.WriteLine("");

            string path = @"C:\Temp\ProgrammingInCSharp\FilesOperations\test.dat";
            using(FileStream fileStream = File.Create(path))
            {
                string myVal = "MyValue";
                byte[] data = Encoding.UTF8.GetBytes(myVal);
                fileStream.Write(data, 0, data.Length);
            }

            Console.WriteLine(String.Format("File has been created \"{0}\"", path));

            using(StreamReader fileStream = File.OpenText(path))
            {
                Console.WriteLine("");
                Console.WriteLine("Reading written file as whole string at once (using implicit UTF-8 StreamReader object from File.OpenText(path))");
                Console.WriteLine(fileStream.ReadToEnd()); // Should display MyValue
            }

            using (FileStream fileStream = File.OpenRead(path))
            {
                Console.WriteLine("");
                Console.WriteLine("Reading written file byte by byte (using FileStream object from OpenRead(path))");
                byte[] data = new byte[fileStream.Length];
                // fileStream.Read(data, 0, data.Length); // METHOD 1
                for (int index = 0; index < fileStream.Length; index++) // METHOD 2
                {
                    data[index] = (byte)fileStream.ReadByte();
                }
                Console.WriteLine(Encoding.UTF8.GetString(data)); // Should display MyValue
            }

            Console.WriteLine("");
            Console.WriteLine("##############################################");
            Console.WriteLine("2. Compression - decorator pattern in practice");
            Console.WriteLine("##############################################");
            Console.WriteLine("");

            string folder = @"C:\Temp\ProgrammingInCSharp\FilesOperations";
            string uncompressedFilePath = Path.Combine(folder, "uncompressed.dat");
            string compressedFilePath = Path.Combine(folder, "compressed.gz");
            byte[] dataToCompress = Enumerable.Repeat((byte)'a', 1024 * 1024).ToArray();

            // Write raw file
            using (FileStream uncompressedFileStream = File.Create(uncompressedFilePath))
            {
                uncompressedFileStream.Write(dataToCompress, 0, dataToCompress.Length);
            }

            // Write compressed file
            using (FileStream compressedFileStream = File.Create(compressedFilePath))
            {
                using (GZipStream compressionStream = new GZipStream(compressedFileStream, CompressionMode.Compress))
                {
                    compressionStream.Write(dataToCompress, 0, dataToCompress.Length);
                }
            }
            FileInfo uncompressedFile = new FileInfo(uncompressedFilePath);
            FileInfo compressedFile = new FileInfo(compressedFilePath);
            Console.WriteLine(String.Format("Rozmiar pliku przed kompresją: {0} kb", uncompressedFile.Length / 1024)); // Displays 1048576
            Console.WriteLine(String.Format("Rozmiar pliku po kompresji: {0} kb", compressedFile.Length / 1024)); // Displays 1052

            Console.WriteLine("");
            Console.WriteLine("##################");
            Console.WriteLine("3. Buffered stream");
            Console.WriteLine("##################");
            Console.WriteLine("");

            string pathBufferedStream = @"C:\Temp\ProgrammingInCSharp\FilesOperations\bufferedStream.txt";
            using(FileStream fileStream = File.Create(pathBufferedStream))
            {
                using(BufferedStream bufferedStream = new BufferedStream(fileStream))
                {
                    using(StreamWriter streamWriter = new StreamWriter(bufferedStream, Encoding.UTF8))
                    {
                        streamWriter.WriteLine("A line of text");
                        streamWriter.WriteLine("Another line of text");
                        streamWriter.WriteLine("The last line of text");
                    }
                }
            }
            Console.WriteLine(String.Format("Plik \"{0}\" został zapisany.", pathBufferedStream));
        }

        private static void Parctice2WebOp()
        {
            Console.WriteLine("");
            Console.WriteLine("##################################");
            Console.WriteLine("Network communication (basic http)");
            Console.WriteLine("##################################");
            Console.WriteLine("");

            Console.WriteLine("");
            Console.WriteLine("############################");
            Console.WriteLine("1. HTTP request and response");
            Console.WriteLine("############################");
            Console.WriteLine("");

            // Use static method to create request
            string url = "http://www.microsoft.com";
            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();

            Console.WriteLine(String.Format("Request do \"{0}\".", url));
            StreamReader responseStream = new StreamReader(response.GetResponseStream());
            
            Console.WriteLine("");
            Console.WriteLine("Odczyt pierwszej linii:");
            string responseText = responseStream.ReadLine();
            Console.WriteLine(responseText);
            response.Close();
        }

        /// <summary>
        /// Main function
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // Examples for basic directory, files and drives operations
            Parctice1DirFileOp();

            // Examples for streams handling
            Parctice2StreamsOp();

            // Examples for HTTP communication
            Parctice2WebOp();

            // Do not close console window
            Console.ReadKey();
        }

        #region helper functions
        private static void _listDirectories(DirectoryInfo directoryInfo, string searchPattern, int maxLevel, int currentLevel)
        {
            if (currentLevel >= maxLevel)
            {
                return;
            }
            string indent = new string('-', currentLevel);
            try
            {
                //DirectoryInfo[] subDirectories = directoryInfo.GetDirectories(searchPattern); // for small directory trees
                var subDirectories = directoryInfo.EnumerateDirectories(searchPattern);// for large directory trees (lazy loading)
                foreach (DirectoryInfo subDirectory in subDirectories)
                {
                    Console.WriteLine(indent + subDirectory.Name);
                    _listDirectories(subDirectory, searchPattern, maxLevel, currentLevel + 1);
                }
            }
            catch (UnauthorizedAccessException)
            {
                // You don’t have access to this folder.
                Console.WriteLine(indent + "Can’t access: " + directoryInfo.Name);
                return;
            }
            catch (DirectoryNotFoundException)
            {
                // The folder is removed while iterating
                Console.WriteLine(indent + "Can’t find: " + directoryInfo.Name);
                return;
            }
        }

        private static void _deleteDirIfExist(string path, bool recursive)
        {
            try
            {
                if (Directory.Exists(path))
                    Directory.Delete(path, recursive);
            }
            catch (Exception exc)
            {
                Console.WriteLine("");
                Console.WriteLine("---ERROR-BEGIN---");
                Console.WriteLine(String.Format("Trying to delete directory \"{0}\", recursively: {1}. {2}", path, recursive, exc.ToString()));
                Console.WriteLine("---ERROR-END---");
                Console.WriteLine("");
            }

            return;
        }
        #endregion
    }
}
