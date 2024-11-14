using Microsoft.VisualStudio.Shell;
using PlantUml.Net;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using YouML.Annotations;
using YouML.Parser;
using YouML.Renderer;

namespace YouML
{
    public class YouMLToolWindowViewModel : INotifyPropertyChanged
    {
        public YouMLToolWindowViewModel()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            try
            {
                var fileCode = Tools.FileUtils.GetFileContent();

                if (fileCode.Equals(string.Empty)) return;

                var classCode = ClassParser.Parse(fileCode);

                var plantCode = UmlRenderer.Render(classCode);

                var renderFactory = new RendererFactory();

                var plantUmlRenderer = renderFactory.CreateRenderer();
                OpenPlantUmlCodeInNotepad(plantCode);

                using (var mStream = new MemoryStream(plantUmlRenderer.Render(plantCode, OutputFormat.Png)))
                {
                    UmlOutput = BitmapFrame.Create(mStream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e);
            }
        }

        public void OpenPlantUmlCodeInNotepad(string plantCode)
        {
            string tempFilePath = Path.Combine(Path.GetTempPath(), "plantuml_code.txt");
            File.WriteAllText(tempFilePath, plantCode);

            Process.Start(new ProcessStartInfo
            {
                FileName = "notepad.exe",
                Arguments = tempFilePath,
                UseShellExecute = true,
                Verb = "open"
            });
        }

        private ImageSource _umlOutput;

        public ImageSource UmlOutput
        {
            get => _umlOutput;
            set
            {
                _umlOutput = value;
                OnPropertyChanged(nameof(UmlOutput));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
