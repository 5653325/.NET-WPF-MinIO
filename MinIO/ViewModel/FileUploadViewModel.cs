using System.ComponentModel;

namespace MinIO.ViewModel
{
    public class FileUploadViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _filename { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName
        {
            get => _filename;
            set
            {
                _filename = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FileName"));
            }
        }

        private long _fileSize { get; set; }
        /// <summary>
        /// 文件大小
        /// </summary>
        public long FileSize
        {
            get => _fileSize;
            set
            {
                _fileSize = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FileSize"));
            }
        }

        private long _partNumber { get; set; }
        /// <summary>
        /// 当前上传块
        /// </summary>
        public long PartNumber
        {
            get => _partNumber;
            set
            {
                _partNumber = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PartNumber"));
            }
        }

        private long _totalParts { get; set; }
        /// <summary>
        /// 文件全部块数
        /// </summary>
        public long TotalParts
        {
            get => _totalParts;
            set
            {
                _totalParts = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TotalParts"));
            }
        }

        private string _uploadProcess { get; set; }
        /// <summary>
        /// 上传进度展示
        /// </summary>
        public string UploadProcess
        {
            get => _uploadProcess;
            set
            {
                _uploadProcess = value;
                PropertyChanged?.Invoke(this,new PropertyChangedEventArgs("UploadProcess"));
            }
        }

    }
}