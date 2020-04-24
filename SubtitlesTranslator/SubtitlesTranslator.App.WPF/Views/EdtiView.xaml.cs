using SubtitlesTranslator.App.WPF.ApplicationServices;
using SubtitlesTranslator.App.WPF.ViewModels;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for EdtiView.xaml
    /// </summary>
    public partial class EdtiView : UserControl
    {
        public EditViewModel ViewModel { get; set; }

        public EdtiView()
        {
            InitializeComponent();

            ViewModel = new EditViewModel(new GoogleTranslatorService())
            {
                NotifyCurrentLanguageChanged = OnCurrentLanguageChanged
            };

            DataContext = ViewModel;
        }

        private void OnCurrentLanguageChanged(string currentLanguage)
        {
            switch (currentLanguage)
            {
                case "ESP":
                    BtEN.Background = new SolidColorBrush(Colors.White);
                    BtESP.Background = new SolidColorBrush(Colors.Aquamarine);
                    break;

                case "EN":
                    BtEN.Background = new SolidColorBrush(Colors.Aquamarine);
                    BtESP.Background = new SolidColorBrush(Colors.White);
                    break;

                default:
                    break;
            }
        }
    }
}
