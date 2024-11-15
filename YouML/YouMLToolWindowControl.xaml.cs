using Microsoft.Win32;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace YouML
{
    public partial class YouMLToolWindowControl
    {
        private string _csFileName; // 添加一个成员变量来存储文件名
        /// <summary>
        /// Initializes a new instance of the <see cref="YouMLToolWindowControl"/> class.
        /// </summary>
        public YouMLToolWindowControl()
        {
            this.InitializeComponent();


            _csFileName = Tools.FileUtils.GetSelectedFileName(); // 获取选中的文件名
            if (!string.IsNullOrEmpty(_csFileName))
            {
                DataContext = new YouMLToolWindowViewModel(_csFileName);
            }


        }

        private void SaveToPng(string fileName)
        {
            using (var fileStream = new FileStream(fileName, FileMode.Create))
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)UmlImage.Source));
                encoder.Save(fileStream);
            }
        }

        private void UmlImage_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "PNG Image|*.png",
                Title = "Save an Image File",
                RestoreDirectory = true,
                FileName = $"{Path.GetFileNameWithoutExtension(_csFileName)}_YouML_image.png" // 使用CS文件名
            };

            saveFileDialog.ShowDialog();

            if (saveFileDialog.FileName != "")
            {
                SaveToPng(saveFileDialog.FileName);
            }
        }

    }
}
