using Acr.UserDialogs;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PCLStorage;
using RetriX.Shared.Services;
using System.IO;

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
        }

        private async void ImportHandler()
        {
            var fileExt = Path.GetExtension(TargetFileName);
            var file = await PlatformService.SelectFileAsync(new string[] { fileExt });
            if (file == null)
            {
                return;
            }

            var md5 = CryptographyService.ComputeMD5(file);
            if (md5 != TargetMD5)
            {

            }

            
        }
    }
}
