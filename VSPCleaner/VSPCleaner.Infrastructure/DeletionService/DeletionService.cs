namespace VSPCleaner.Infrastructure.DeletionService
{
    using System;
    using System.IO;

    using Directory = Delimon.Win32.IO.Directory;

    public class DeletionService
    {
        public static void DeleteFoldersRecursively(string directory)
        {
            string[] directories;
            try
            {
                directories = Directory.GetDirectories(directory);
            }
            catch (UnauthorizedAccessException)
            {
                return;
            }

            foreach (var currentDirectory in directories)
            {
                var folder = currentDirectory.Substring(
                    currentDirectory.LastIndexOf(@"\", StringComparison.Ordinal) + 1);
                if (folder.Equals("bin", StringComparison.OrdinalIgnoreCase)
                    || folder.Equals("obj", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        DeleteFolder(currentDirectory);
                    }
                    catch (IOException)
                    {
                    }
                    catch (UnauthorizedAccessException)
                    {
                    }
                }
                else
                {
                    DeleteFoldersRecursively(currentDirectory);
                }
            }
        }

        public static void DeleteFolder(string directory)
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, true);
            }
        }
    }
}