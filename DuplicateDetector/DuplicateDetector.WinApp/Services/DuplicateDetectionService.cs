namespace DuplicateDetector.WinApp.Services;

public class DuplicateDetectionService
{
    public IEnumerable<string> GetDuplicateFiles(string directory)
    {
        return
        [
            "1.txt",
            "2.txt",
            "3.txt",
            "4.txt",
        ];
    }
}
