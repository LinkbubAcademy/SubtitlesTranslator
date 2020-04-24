using SubtitlesTranslator.App.WPF.Lib.Models;
using SubtitlesTranslator.App.WPF.Lib.UI;
using SubtitlesTranslator.Lib.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace SubtitlesTranslator.App.WPF.ViewModels
{
    public class EditViewModel : ViewModelBase
    {
        #region Services
        public ITranslatorService TranslatorService { get; set; }

        #endregion

        #region Commands
        public ICommand ExportCommand { get; set; }
        public ICommand SelectEnglishSubtitlesCommand { get; set; }
        public ICommand SelectSpanishSubtitlesCommand { get; set; }

        #endregion

        #region View Actions

        public Action<string> NotifyCurrentLanguageChanged { get; set; }

        #endregion

        public string CurrentFile { get; set; }

        private Dictionary<string, ObservableCollection<SubtitleLine>> LinesByLanguage { get; set; }

        public ObservableCollection<SubtitleLine> CurrentLines
        {
            get
            {
                return _currentLines;
            }
            set
            {
                _currentLines = value;
                NotifyPropertyChange();
            }
        }
        private ObservableCollection<SubtitleLine> _currentLines = new ObservableCollection<SubtitleLine>();

        public string ImportedLanguage { get; set; }

        public string CurrentLanguage
        {
            get
            {
                return _currentLanguage;
            }
            set
            {
                _currentLanguage = value;
                NotifyPropertyChange();

                if (NotifyCurrentLanguageChanged != null)
                    NotifyCurrentLanguageChanged(_currentLanguage);

            }
        }
        private string _currentLanguage;

        public EditViewModel(ITranslatorService translatorService)
        {
            TranslatorService = translatorService;
            LinesByLanguage = new Dictionary<string, ObservableCollection<SubtitleLine>>();

            ExportCommand = new ModelCommand(x => Export());
            SelectEnglishSubtitlesCommand = new ModelCommand(x => SelectEnglishSubtitles());
            SelectSpanishSubtitlesCommand = new ModelCommand(x => SelectSpanishSubtitles());
        }

        public void SelectImportLines(List<SubtitleLine> lines, string importLanguage, string file)
        {
            CurrentFile = file;

            if (LinesByLanguage.Count > 0)
            {
                LinesByLanguage = new Dictionary<string, ObservableCollection<SubtitleLine>>();
                CurrentLines = new ObservableCollection<SubtitleLine>();
            }

            if (!LinesByLanguage.ContainsKey(importLanguage))
            {
                LinesByLanguage.Add(importLanguage, new ObservableCollection<SubtitleLine>(lines));
                TranslateToOtherLangues(lines, importLanguage);
            }
            else
            {
                // todo: handle 
            }
        }

        public void Export()
        {
            var fileName = Path.GetFileName(CurrentFile);
            var path = Path.GetDirectoryName(CurrentFile);

            var lines = new List<string>();
            lines.Add("WEBVTT");
            lines.Add(string.Empty);

            foreach (var line in CurrentLines)
            {
                lines.Add(line.LineNumber.ToString());
                lines.Add(line.Period.ToString());
                lines.Add(line.Text);
                lines.Add(string.Empty);
            }

            var newFileName = path + "\\" + CurrentLanguage + "_" + fileName;
            File.WriteAllLines(newFileName, lines.ToArray());

            MessageBox.Show("Exportación ok: " + newFileName);
        }

        private void TranslateToOtherLangues(List<SubtitleLine> lines, string importLanguage)
        {
            switch (importLanguage)
            {
                case "ESP":
                    TranslateToLangue(lines, importLanguage, "EN");
                    break;
                case "EN":
                    TranslateToLangue(lines, importLanguage, "ESP");
                    break;
                default:
                    MessageBox.Show($"Language {importLanguage} not supported");
                    break;
            }
        }

        private void TranslateToLangue(List<SubtitleLine> lines, string from, string to)
        {
            LinesByLanguage.Add(to, new ObservableCollection<SubtitleLine>());
            CurrentLines = LinesByLanguage[to];

            TranslatorService.Translate(from, to, lines, CurrentLines);

            CurrentLanguage = to;
        }

        public void SelectEnglishSubtitles()
        {
            SelectSubtitles("EN");
        }
        public void SelectSpanishSubtitles()
        {
            SelectSubtitles("ESP");
        }

        private void SelectSubtitles(string language)
        {
            if (language != ImportedLanguage)
            {
                CurrentLines = LinesByLanguage[language];
                CurrentLanguage = language;
            }
        }
    }
}
