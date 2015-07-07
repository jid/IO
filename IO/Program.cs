using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.AccessControl;

namespace IO
{
    class Program
    {
        static void Main(string[] args)
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
            ListDirectories(directoryInfoTree, searchPattern, depth, startLevel);

            // Moving directory
            Console.WriteLine("");
            Console.WriteLine(String.Format("Moving directory {0} to {1}...", @"C:\Temp\ProgrammingInCSharp\DirectoryToBeMoved", @"C:\Temp\ProgrammingInCSharp\DirectoryAfterMoving"));
            DeleteDirIfExist(@"C:\Temp\ProgrammingInCSharp\DirectoryAfterMoving", false);
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
            DeleteDirIfExist(dirPath, true);
            Directory.CreateDirectory(dirPath);

            if (!File.Exists(filePath))
            {
                Console.WriteLine("");
                Console.WriteLine(String.Format("Creating file {0} (using static File class)...", filePath));
                var fileTxt = File.CreateText(filePath);
                fileTxt.Close();
            }

            if(File.Exists(filePath))
            {
                Console.WriteLine("");
                Console.WriteLine(String.Format("Deleting file {0} (using static File class)...", filePath));
                File.Delete(filePath);
            }

            FileInfo fileInfo = new FileInfo(filePath);
            if(fileInfo.Exists)
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

            // Do not close console window
            Console.ReadKey();
        }

        private static void ListDirectories(DirectoryInfo directoryInfo, string searchPattern, int maxLevel, int currentLevel)
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
                    ListDirectories(subDirectory, searchPattern, maxLevel, currentLevel + 1);
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

        private static void DeleteDirIfExist(string path, bool recursive)
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
    }
}
