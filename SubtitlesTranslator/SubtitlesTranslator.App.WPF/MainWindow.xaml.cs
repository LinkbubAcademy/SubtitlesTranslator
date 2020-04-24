using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.ObjectModel;
using SubtitlesTranslator.App.WPF.Lib.Models;

namespace SubtitlesTranslator.App.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ImportView.ViewModel.SelectSubtilesForEditAction = (lines, language, file) =>
            {
                EditView.ViewModel.SelectImportLines(lines, language, file);
            };
        }
    }
}
