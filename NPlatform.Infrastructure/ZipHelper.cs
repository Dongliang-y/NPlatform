using System.IO.Compression;

namespace NPlatform.Infrastructure
{
    public class ZipHelper
    {
        /// <summary>
        /// 获取压缩后的文件路径
        /// </summary>
        /// <param name="dirPath">压缩的文件路径</param>
        /// <param name="filesPath">多个文件路径</param>
        /// <returns>string</returns>
        public static string GetCompressPath(string dirPath, List<string> filesPath)
        {
            var fileName = $"{DateTime.Now.ToString("yyyyMMddHHmmss")}.zip";
            var zipPath = $"{dirPath}\\{fileName}";
            // 创建ZIP文件并打开写入流
            using (ZipArchive zipArchive = ZipFile.Open(zipPath, ZipArchiveMode.Create))
            {
                // 递归遍历源文件夹下的所有文件和子文件夹
                foreach (string filePath in filesPath)
                {
                    // 在ZIP文件中创建对应路径的条目
                    ZipArchiveEntry entry = zipArchive.CreateEntry("/");
                    // 读取原文件内容并写入到ZIP文件条目的流中
                    using (FileStream fileStream = File.OpenRead(filePath))
                    {
                        using (Stream entryStream = entry.Open())
                        {
                            fileStream.CopyTo(entryStream);
                        }
                    }
                }
            }
            return zipPath;
        }

        public static void ZipFolder(string sourceFolderPath, string zipFilePath)
        {
            // 创建ZIP文件并打开写入流
            using (ZipArchive zipArchive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
            {
                // 递归遍历源文件夹下的所有文件和子文件夹
                foreach (string filePath in Directory.EnumerateFiles(sourceFolderPath, "*", SearchOption.AllDirectories))
                {
                    // 获取文件相对于源文件夹的相对路径，这将作为ZIP文件中的条目路径
                    string relativePath = filePath.Substring(sourceFolderPath.Length + 1).Replace('\\', '/');
                    // 在ZIP文件中创建对应路径的条目
                    ZipArchiveEntry entry = zipArchive.CreateEntry(relativePath);
                    // 读取原文件内容并写入到ZIP文件条目的流中
                    using (FileStream fileStream = File.OpenRead(filePath))
                    {
                        using (Stream entryStream = entry.Open())
                        {
                            fileStream.CopyTo(entryStream);
                        }
                    }
                }
            }
        }
    }
}
