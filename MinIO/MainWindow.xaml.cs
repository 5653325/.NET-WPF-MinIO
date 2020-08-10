using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
using Minio;
using Minio.Exceptions;
using MinIO.Helper;
using MinIO.ViewModel;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace MinIO
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private static string _endpoint = "192.168.127.131:9000";
        private static string _accessKey = "minioadmin";
        private static string _secretKey = "minioadmin";
        private static MinioClient _minioClient;

        public MainWindow()
        {
            InitializeComponent();
        }
        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            _minioClient = new MinioClient(_endpoint, _accessKey, _secretKey);
            Messenger.Default.Register<int>(this, "process", obj =>
             {
                 try
                 {
                     Debug.WriteLine($"当前块编号：{obj}");
                     if (obj == 0)
                     {
                         ViewModelLocator.Instance.FileUploadViewModel.UploadProcess = "0.00%";
                         return;
                     }
                     ViewModelLocator.Instance.FileUploadViewModel.PartNumber = obj;
                     ViewModelLocator.Instance.FileUploadViewModel.UploadProcess =
                         $"{(float)ViewModelLocator.Instance.FileUploadViewModel.PartNumber / ViewModelLocator.Instance.FileUploadViewModel.TotalParts:P2}";
                 }
                 catch (Exception exception)
                 {
                     App.NewNLog.Error($"计算上传进度时出错：{exception}");
                 }
             });
        }
        private void ButtonUpload_Click(object sender, RoutedEventArgs e)
        {
            var open = new OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
            };
            if (open.ShowDialog(this) == false)
            {
                return;
            }

            ViewModelLocator.Instance.FileUploadViewModel.FileName = open.SafeFileName;
            try
            {
                Dispatcher?.InvokeAsync(async () =>
                {
                    await Run(_minioClient, "test", open.FileName, ViewModelLocator.Instance.FileUploadViewModel.FileName);
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static async Task Run(MinioClient minio, string userBucketName, string uploadFilePath, string saveFileName)
        {
            var bucketName = userBucketName;
            var location = "us-east-1";
            var objectName = saveFileName;
            var filePath = uploadFilePath;
            var contentType = ContentTypeHelper.GetContentType(saveFileName.Substring(saveFileName.LastIndexOf('.') + 1));
            var file = new FileInfo(uploadFilePath);
            var isUpdateOnce = true;

            try
            {
                var found = await minio.BucketExistsAsync(bucketName);
                if (!found)
                {
                    await minio.MakeBucketAsync(bucketName, location);
                }

                ViewModelLocator.Instance.FileUploadViewModel.FileSize = file.Length;
                ViewModelLocator.Instance.FileUploadViewModel.TotalParts = file.Length / App.MinimumPartSize + 1;
                if (ViewModelLocator.Instance.FileUploadViewModel.FileSize > App.MinimumPartSize && ViewModelLocator.Instance.FileUploadViewModel.FileSize >= 0)
                {
                    _minioClient.SetTraceOn(new LogHelper());
                    isUpdateOnce = false;
                }
                await minio.PutObjectAsync(bucketName, objectName, filePath, contentType);
                if (isUpdateOnce)
                {
                    Messenger.Default.Send(1, "process");
                }

                MessageBox.Show($"{objectName}上传成功。");
                Debug.WriteLine("Successfully uploaded " + objectName);
            }
            catch (MinioException e)
            {
                App.NewNLog.Error($"File Upload Error: {e}");
                Debug.WriteLine($"File Upload Error: {e.Message}");
                MessageBox.Show($"{objectName}上传失败：{e.Message}。");
            }
            finally
            {
                if (!isUpdateOnce)
                {
                    _minioClient.SetTraceOff();
                }
            }
        }
    }
}
