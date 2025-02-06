using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using DuplicateDetector.WinApp.DuplicateDetection;
using Microsoft.Win32;

namespace DuplicateDetector.WinApp;

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
        var duplicates = _duplicateDetectionService.GetDuplicateFiles(path).ToList();
        DuplicateItemsListView.ItemsSource = new ObservableCollection<DuplicateEntry>(duplicates);
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

            var duplicates = _duplicateDetectionService.GetDuplicateFiles(selectedFolder).ToList();
            DuplicateItemsListView.ItemsSource = new ObservableCollection<DuplicateEntry>(duplicates);
        }
    }

    private void DuplicateItemOnCLick(object sender, MouseButtonEventArgs _)
    {
        var item = sender as ListViewItem;
        var duplicateItem = item?.Content as DuplicateEntry;

        if (duplicateItem is null)
        {
            return;
        }

        var firstFile = duplicateItem.Occurrences.First();
        var lastFile = duplicateItem.Occurrences.Last();
        RenderImage(ImageA, firstFile);
        RenderImage(ImageB, lastFile);

        OccurrencesListView.ItemsSource = duplicateItem.Occurrences;
    }

    private void OccurrenceOnSelection(object sender, SelectionChangedEventArgs e)
    {
        KeepSelectedBtn.IsEnabled = !string.IsNullOrWhiteSpace(OccurrencesListView.SelectedItem?.ToString());
    }

    private void KeepSelected_OnCLick(object sender, RoutedEventArgs e)
    {
        if (
            OccurrencesListView.ItemsSource is not string[] occurrences ||
            OccurrencesListView.SelectedItem is not string keepItem
        )
        {
            return;
        }

        ImageA.Source = null;
        ImageB.Source = null;

        foreach (var occurence in occurrences.Except([keepItem]))
        {
            File.Delete(occurence);
        }

        var selectedDuplicate = (DuplicateEntry)DuplicateItemsListView.SelectedItem;
        var items = (IList<DuplicateEntry>)DuplicateItemsListView.ItemsSource;

        items.Remove(selectedDuplicate);

        OccurrencesListView.ItemsSource = null;
    }

    private static void RenderImage(Image imageControl, string filename)
    {
        var bitmap = LoadImage(filename);
        imageControl.Source = bitmap;
    }

    private static BitmapImage LoadImage(string filename)
    {
        var image = new BitmapImage();
        using var stream = File.OpenRead(filename);
        image.BeginInit();
        image.CacheOption = BitmapCacheOption.OnLoad;
        image.StreamSource = stream;
        image.EndInit();

        return image;
    }
}
