using Acr.UserDialogs;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PCLStorage;
using RetriX.Shared.Services;
using System.IO;
using System.Threading.Tasks;

namespace RetriX.Shared.ViewModels
{
    public class FileImporterVM : ViewModelBase
    {
        public const string FileHashMismatchTitleKey = nameof(FileHashMismatchTitleKey);
        public const string FileHashMismatchMessageKey = nameof(FileHashMismatchMessageKey);

        private readonly IUserDialogs DialogsService;
        private readonly ILocalizationService LocalizationService;
        private readonly IPlatformService PlatformService;
        private readonly ICryptographyService CryptographyService;

        private readonly IFolder targetFolder;
        public IFolder TargetFolder { get { return targetFolder; } }

        private readonly string targetFileName;
        public string TargetFileName { get { return targetFileName; } }

        private readonly string targetMD5;
        public string TargetMD5 { get { return targetMD5; } }

        private bool fileAvailable = false;
        public bool FileAvailable
        {
            get { return fileAvailable; }
            set { if(Set(ref fileAvailable, value)) { ImportCommand.RaiseCanExecuteChanged(); } }
        }

        public RelayCommand ImportCommand { get; private set; }

        public FileImporterVM(IUserDialogs dialogsService, ILocalizationService localizationService, IPlatformService platformService, ICryptographyService cryptographyService, IFolder folder, string fileName, string MD5)
        {
            DialogsService = dialogsService;
            LocalizationService = localizationService;
            PlatformService = platformService;
            CryptographyService = cryptographyService;

            targetFolder = folder;
            targetFileName = fileName;
            targetMD5 = MD5;

            ImportCommand = new RelayCommand(ImportHandler, () => !FileAvailable);

            GetTargetFileAsync().ContinueWith(d => FileAvailable = d.Result != null);
        }

        private async Task<IFile> GetTargetFileAsync()
        {
            var result = await TargetFolder.CheckExistsAsync(TargetFileName);
            if (result != ExistenceCheckResult.FileExists)
            {
                return null;
            }

            var output = await TargetFolder.GetFileAsync(TargetFileName);
            return output;
        }

        private async void ImportHandler()
        {
            var fileExt = Path.GetExtension(TargetFileName);
            var sourceFile = await PlatformService.SelectFileAsync(new string[] { fileExt });
            if (sourceFile == null)
            {
                return;
            }

            using (var inStream = await sourceFile.OpenAsync(PCLStorage.FileAccess.Read))
            {
                var md5 = CryptographyService.ComputeMD5(inStream);
                if (md5 != TargetMD5)
                {
                    var title = LocalizationService.GetLocalizedString(FileHashMismatchTitleKey);
                    var message = LocalizationService.GetLocalizedString(FileHashMismatchMessageKey);
                    await DialogsService.AlertAsync(message, title);
                    return;
                }

                var targetFile = await TargetFolder.CreateFileAsync(targetFileName, CreationCollisionOption.ReplaceExisting);
                using (var outStream = await targetFile.OpenAsync(PCLStorage.FileAccess.ReadAndWrite))
                {
                    await inStream.CopyToAsync(outStream);
                    await outStream.FlushAsync();
                }

                FileAvailable = true;
            }            
        }
    }
}
