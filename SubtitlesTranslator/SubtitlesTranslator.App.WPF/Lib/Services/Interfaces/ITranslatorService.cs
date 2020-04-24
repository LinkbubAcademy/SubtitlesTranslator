using SubtitlesTranslator.App.WPF.Lib.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SubtitlesTranslator.Lib.Services
{
    public interface ITranslatorService
    {
        void Translate(string from, string to, IEnumerable<SubtitleLine> input, ObservableCollection<SubtitleLine> output);
    }
}
