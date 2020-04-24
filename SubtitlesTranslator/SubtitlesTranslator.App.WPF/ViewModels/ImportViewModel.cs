using SubtitlesTranslator.App.WPF.Lib;
using SubtitlesTranslator.App.WPF.Lib.Models;
using SubtitlesTranslator.App.WPF.Lib.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace SubtitlesTranslator.App.WPF.ViewModels
{
    public class ImportViewModel : ViewModelBase
    {
        public Func<List<string>> GetSubtitlesFilesAction { get; set; }
        public Func<string, List<string>>  GetFileTextLinesAction { get; set; }
        public Action<List<SubtitleLine>, string, string> SelectSubtilesForEditAction { get; set; }

        public List<string> FilesNames
        {
            get
            {
                return _filesName;
            }
            set
            {
                _filesName = value;
                NotifyPropertyChange();
            }
        }
        private List<string> _filesName = new List<string>();

        public IEnumerable<string> LanguageTypes
        {
            get
            {
                return LanguageOptions.Types.Keys;
            }
        }

        public string SelectedLanguage
        {
            get
            {
                return _selectedLanguage;
            }
            set
            {
                _selectedLanguage = value;
                NotifyPropertyChange();
            }
        }
        private string _selectedLanguage;
        public string SelectedFile
        {
            get
            {
                return _selectedFile;
            }
            set
            {
                if (_selectedFile != value)
                {
                    _selectedFile = value;
                    SelectFile(value);
                }

                NotifyPropertyChange();
            }
        }
        private string _selectedFile;

        public ImportViewModel()
        {
            SelectedLanguage = "ESP";

            ImportCommand = new ModelCommand(x => Import());
        }

        public void Import()
        {
            if (GetSubtitlesFilesAction != null)
            {
                FilesNames = GetSubtitlesFilesAction().OrderBy(x => x).ToList();
            }
        }

        public void SelectFile(string file)
        {
            if (GetFileTextLinesAction != null)
            {
                var textLines = GetFileTextLinesAction(file);

                try
                {
                    var subtitlesLines = SubtitleLine.GetFromTextLines(textLines);

                    if (SelectSubtilesForEditAction != null)
                        SelectSubtilesForEditAction(subtitlesLines, SelectedLanguage, file);
                }
                catch (ArgumentException e1)
                {
                    MessageBox.Show(e1.Message);
                }
            }
        }

        public ICommand ImportCommand { get; set; }

    }
}
