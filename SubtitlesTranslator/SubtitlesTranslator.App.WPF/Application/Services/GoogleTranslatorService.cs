using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Net;
using System.Timers;
using System.Web;
using System.Windows;
using System.Windows.Input;
using Newtonsoft.Json;
using SubtitlesTranslator.App.WPF.Lib;
using SubtitlesTranslator.App.WPF.Lib.Models;
using SubtitlesTranslator.Lib.Services;

namespace SubtitlesTranslator.App.WPF.ApplicationServices
{
    public class GoogleTranslatorService : ITranslatorService
    {
        private string LanguageFrom { get; set; }
        private string LanguageTo { get; set; }
        private SubtitleLine[] Input { get; set; }

        private List<SubtitleLine> Preoutput { get; set; }
        private ObservableCollection<SubtitleLine> Output { get; set; }

        private static Timer aTimer;

        private int Count { get; set; } = 0;

        public GoogleTranslatorService()
        {
        }

        public void Translate(string from, string to, IEnumerable<SubtitleLine> input, ObservableCollection<SubtitleLine> output)
        {
            LanguageFrom = from;
            LanguageTo = to;
            Input = input.ToArray();
            Output = output;

            var translatedTexts = new List<string>();
            var inputs = Input.Select(x => x.Text);
            var total = inputs.Count();

            if (total > 50)
            {
                var page = 0;

                while (translatedTexts.Count < total)
                {
                    var section = inputs.Skip(page * 50).Take(50).ToArray();
                    translatedTexts.AddRange(Translate(LanguageFrom, LanguageTo, section));
                    page++;
                }
            }
            else
            {
                translatedTexts.AddRange(Translate(LanguageFrom, LanguageTo, inputs.ToArray()));
            }


            Preoutput = new List<SubtitleLine>();

            for (var i = 0; i < Input.Length; i++)
            {
                Preoutput.Add(new SubtitleLine()
                {
                    LineNumber = Input[i].LineNumber,
                    Period = Input[i].Period,
                    Text = translatedTexts[i]
                });
            }

            // Create a timer with a two second interval.
            aTimer = new Timer(50);

            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        private List<string> Translate(string fromLanguage, string toLanguage, string[] inputs)
        {            
            var from = LanguageOptions.Types[fromLanguage];
            var to = LanguageOptions.Types[toLanguage];
           
            var key = "Pon Aquí tu Clave!";
            var url = $"https://translation.googleapis.com/language/translate/v2?key={key}&sl={from}&target={to}";

            foreach (var input in inputs)
            {
                url += $"&q={HttpUtility.UrlEncode(input)}";
            }

            var webClient = new WebClient
            {
                Encoding = System.Text.Encoding.UTF8
            };
            try
            {
                var result = webClient.DownloadString(url);
                var trs = JsonConvert.DeserializeObject<Root>(result);
                return trs.data.translations.Select(x => x.translatedText).ToList();
            }
            catch (Exception e1)
            {
                return  null;
            }
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            var line = Input[Count];

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                Output.Add(Preoutput[Count]);
            }));            

            Count++;

            if (Count == Input.Length)
            {
                aTimer.Stop();
                aTimer.Dispose();

                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    MessageBox.Show($"Traducción completada. Total de líneas:{Count}");
                    Count = 0;
                }));
            }
        }


        #region Métodos alternativos que no nos han funcionado

        private string Translate(string fromLanguage, string toLanguage, string input)
        {
            var from = LanguageOptions.Types[fromLanguage];
            var to = LanguageOptions.Types[toLanguage];
            var url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={from}&tl={to}&dt=t&q={HttpUtility.UrlEncode(input)}";
            var webClient = new WebClient
            {
                Encoding = System.Text.Encoding.UTF8
            };
            try
            {
                var result = webClient.DownloadString(url);
                result = result.Substring(4, result.IndexOf("\"", 4, StringComparison.Ordinal) - 4);
                return result;
            }
            catch (Exception e1)
            {
                return "Error";
            }
        }


        //private List<string> TranslateTextGoogleApi(IEnumerable<string> texts,
        //    string sourceLanguage,
        //    string targetLanguage)
        //{

        //    var credentialsJson = "subtitlestranslator_google_translate_api_key.json";
        //    var cred = GoogleCredential.FromFile(credentialsJson);

        //    var builder = new TranslationServiceClientBuilder();
        //    builder.ChannelCredentials = cred.ToChannelCredentials();

        //    var translationServiceClient = builder.Build();
        //    var request = new TranslateTextRequest
        //    {
        //        Contents =
        //        {
        //            // The content to translate.
        //            texts
        //        },                
        //        SourceLanguageCode = sourceLanguage,
        //        TargetLanguageCode = targetLanguage,
        //        ParentAsLocationName = new LocationName("subtitlestranslator-275212", "global"),
        //    };
        //    TranslateTextResponse response = translationServiceClient.TranslateText(request);
        //    return response.Translations.Select(x => x.TranslatedText).ToList();
        //}        

        #endregion
    }

    class Translation
    {
        public string translatedText { get; set; }
        public string detectedSourceLanguage { get; set; }
    }

    class Data
    {
        public List<Translation> translations { get; set; }
    }

    class Root
    {
        public Data data { get; set; }
    }
}
