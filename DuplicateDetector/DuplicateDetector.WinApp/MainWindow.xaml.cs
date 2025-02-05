using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DuplicateDetector.WinApp.Services;
using Microsoft.Win32;

namespace DuplicateDetector.WinApp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private string? _folderToCheck;

    private readonly DuplicateDetectionService _duplicateDetectionService;

    public MainWindow(DuplicateDetectionService duplicateDetectionService)
    {
        _duplicateDetectionService = duplicateDetectionService;
        InitializeComponent();
    }

    private void FolderInput_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs _)
    {
        var dialog = new OpenFolderDialog
        {
            Title = "Select Folder",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        };

        if (dialog.ShowDialog() is true)
        {
            var selectedFolder = dialog.FolderName;
            _folderToCheck = selectedFolder;
            FolderInput.Text = selectedFolder;


            var duplicates = _duplicateDetectionService.GetDuplicateFiles(selectedFolder);
            PopulateDuplicates(duplicates);
        }
    }

    private void PopulateDuplicates(IEnumerable<string> duplicates)
    {
        foreach (var duplicate in duplicates)
        {
            var item = new ListViewItem
            {
                Content = duplicate
            };

            item.PreviewMouseDown += DuplicateItemOnCLick;
            DuplicateItemsListView.Items.Add(item);
        }
    }

    private static void DuplicateItemOnCLick(object sender, MouseButtonEventArgs _)
    {
        var item = sender as ListViewItem;
        Debug.WriteLine(item!.Content);
    }
}
