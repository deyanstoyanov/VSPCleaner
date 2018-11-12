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
                DirectoryInfo directoryInfo = new DirectoryInfo(directory);
                SetAttributeNormal(directoryInfo);

                Directory.Delete(directory, true);
            }
        }

        private static void SetAttributeNormal(DirectoryInfo directoryInfo)
        {
            foreach (var directory in directoryInfo.GetDirectories())
            {
                SetAttributeNormal(directory);
            }

            foreach (var file in directoryInfo.GetFiles())
            {
                file.Attributes = FileAttributes.Normal;
            }
        }
    }
}