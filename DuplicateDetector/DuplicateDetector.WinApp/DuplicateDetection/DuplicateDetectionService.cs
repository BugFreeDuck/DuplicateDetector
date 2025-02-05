using System.IO;
using System.Security.Cryptography;

namespace DuplicateDetector.WinApp.DuplicateDetection;

public record DuplicateEntry(string FileName, string Hash, string[] Occurrences);


public class DuplicateDetectionService
{
    public IEnumerable<DuplicateEntry> GetDuplicateFiles(string directory)
    {
        return Directory
            .GetFiles(directory, "*.*", SearchOption.AllDirectories)
            .Select(file =>
            {
                using var fs = new FileStream(file, FileMode.Open, FileAccess.Read);
                return new
                {
                    FullPath = file,
                    FileHash = BitConverter.ToString(MD5.Create().ComputeHash(fs))
                };
            })
            .GroupBy(x => x.FileHash)
            .Where(group => group.Count() > 1)
            .Select(group =>
            {
                var fileName = Path.GetFileName(group.First().FullPath);
                return new DuplicateEntry(fileName, group.Key, group.Select(x => x.FullPath).ToArray());
            });
    }
}
