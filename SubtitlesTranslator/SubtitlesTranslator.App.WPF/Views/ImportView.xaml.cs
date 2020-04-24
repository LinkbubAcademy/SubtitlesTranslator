using Microsoft.Win32;
using SubtitlesTranslator.App.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SubtitlesTranslator.App.WPF.Views
{
    /// <summary>
    /// Interaction logic for ImportView.xaml
    /// </summary>
    public partial class ImportView : UserControl
    {
        public ImportViewModel ViewModel { get; set; }

        public ImportView()
        {
            InitializeComponent();

            ViewModel = new ImportViewModel();
            ViewModel.GetSubtitlesFilesAction = GetSubtitlesFiles;
            ViewModel.GetFileTextLinesAction = (path) =>
            {
                if (!string.IsNullOrEmpty(path))
                    return File.ReadAllLines(path).ToList();

                return new List<string>();
            };

            DataContext = ViewModel;
        }

        private List<string> GetSubtitlesFiles()
        {
            var output = new List<string>();

            var ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            ofd.Filter = "vtt subtitles files (*.vtt)|*.vtt";

            ofd.ShowDialog();
            output.AddRange(ofd.FileNames);

            return output;
        }
    }
}
