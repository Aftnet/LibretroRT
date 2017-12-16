using Acr.UserDialogs;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Plugin.FileSystem.Abstractions;
using RetriX.Shared.Services;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RetriX.Shared.ViewModels
{
    public class FileImporterVM : ViewModelBase
    {
        public const string SerachLinkFormat = "https://www.google.com/search?q={0}";

        public const string FileHashMismatchTitleKey = nameof(FileHashMismatchTitleKey);
        public const string FileHashMismatchMessageKey = nameof(FileHashMismatchMessageKey);

        private readonly IFileSystem FileSystem;
        private readonly IUserDialogs DialogsService;
        private readonly ILocalizationService LocalizationService;
        private readonly IPlatformService PlatformService;
        private readonly ICryptographyService CryptographyService;

        private readonly IDirectoryInfo targetFolder;
        public IDirectoryInfo TargetFolder { get { return targetFolder; } }

        private readonly string targetFileName;
        public string TargetFileName { get { return targetFileName; } }

        private readonly string targetDescription;
        public string TargetDescription { get { return targetDescription; } }

        private readonly string targetMD5;
        public string TargetMD5 { get { return targetMD5; } }

        public string SearchLink => string.Format(SerachLinkFormat, TargetMD5);

        private bool fileAvailable = false;
        public bool FileAvailable
        {
            get { return fileAvailable; }
            private set { if(Set(ref fileAvailable, value)) { ImportCommand.RaiseCanExecuteChanged(); } }
        }

        public RelayCommand ImportCommand { get; private set; }
        public RelayCommand CopyMD5ToClipboardCommand { get; private set; }

        public FileImporterVM(IFileSystem fileSystem, IUserDialogs dialogsService, ILocalizationService localizationService, IPlatformService platformService, ICryptographyService cryptographyService, IDirectoryInfo folder, string fileName, string description, string MD5)
        {
            FileSystem = fileSystem;
            DialogsService = dialogsService;
            LocalizationService = localizationService;
            PlatformService = platformService;
            CryptographyService = cryptographyService;

            targetFolder = folder;
            targetFileName = fileName;
            targetDescription = description;
            targetMD5 = MD5;

            ImportCommand = new RelayCommand(ImportHandler, () => !FileAvailable);
            CopyMD5ToClipboardCommand = new RelayCommand(() => PlatformService.CopyToClipboard(TargetMD5));

            GetTargetFileAsync().ContinueWith(d => PlatformService.RunOnUIThreadAsync(() => FileAvailable = d.Result != null));
        }

        public async Task<IFileInfo> GetTargetFileAsync()
        {
            var result = await TargetFolder.GetFileAsync(TargetFileName);
            return result;
        }

        private async void ImportHandler()
        {
            var fileExt = Path.GetExtension(TargetFileName);
            var sourceFile = await FileSystem.PickFileAsync(new string[] { fileExt });
            if (sourceFile == null)
            {
                return;
            }

            var md5 = await CryptographyService.ComputeMD5Async(sourceFile);
            if (md5.ToLowerInvariant() != TargetMD5.ToLowerInvariant())
            {
                var title = LocalizationService.GetLocalizedString(FileHashMismatchTitleKey);
                var message = LocalizationService.GetLocalizedString(FileHashMismatchMessageKey);
                await DialogsService.AlertAsync(message, title);
                return;
            }

            using (var inStream = await sourceFile.OpenAsync(FileAccess.Read))
            {
                var targetFile = await TargetFolder.CreateFileAsync(targetFileName);
                using (var outStream = await targetFile.OpenAsync(FileAccess.ReadWrite))
                {
                    await inStream.CopyToAsync(outStream);
                    await outStream.FlushAsync();
                }

                FileAvailable = true;
            }            
        }
    }
}
