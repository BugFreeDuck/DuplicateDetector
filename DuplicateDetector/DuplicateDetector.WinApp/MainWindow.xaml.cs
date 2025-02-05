using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DuplicateDetector.WinApp.DuplicateDetection;
using Microsoft.Win32;

namespace DuplicateDetector.WinApp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly DuplicateDetectionService _duplicateDetectionService;

    public MainWindow(DuplicateDetectionService duplicateDetectionService)
    {
        _duplicateDetectionService = duplicateDetectionService;
        InitializeComponent();
        Loaded += (_, _) => FirstRun();
    }

    private void FirstRun()
    {
        const string path = @"C:\Users\Gabrielius\Documents\temp";
        var duplicates = _duplicateDetectionService.GetDuplicateFiles(path);
        DuplicateItemsListView.ItemsSource = duplicates;
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
            FolderInput.Text = selectedFolder;

            var duplicates = _duplicateDetectionService.GetDuplicateFiles(selectedFolder);
            DuplicateItemsListView.ItemsSource = duplicates;
        }
    }

    private void DuplicateItemOnCLick(object sender, MouseButtonEventArgs _)
    {
        var item = sender as ListViewItem;
        var duplicateItem = item?.Content as DuplicateEntry;
        Debug.WriteLine(duplicateItem);
    }
}
