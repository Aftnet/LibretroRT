using Plugin.FileSystem.Abstractions;
using Plugin.LocalNotifications.Abstractions;
using RetriX.Shared.ExtensionMethods;
using System.Threading.Tasks;

namespace RetriX.Shared.Services
{
    public class SaveStateService : ISaveStateService
    {
        private const string SaveStatesFolderName = "SaveStates";

        public const string StateSavedToSlotMessageTitleKey = "StateSavedToSlotMessageTitleKey";
        public const string StateSavedToSlotMessageBodyKey = "StateSavedToSlotMessageBodyKey";

        private readonly IFileSystem FileSystem;
        private readonly ILocalNotifications NotificationService;
        private readonly ILocalizationService LocalizationService;

        private string GameId { get; set; }

        private bool OperationInProgress = false;
        private bool AllowOperations => !(OperationInProgress || SaveStatesFolder == null || GameId == null);

        private IDirectoryInfo SaveStatesFolder;

        public SaveStateService(IFileSystem fileSystem, ILocalNotifications notificationService, ILocalizationService localizationService)
        {
            FileSystem = fileSystem;
            NotificationService = notificationService;
            LocalizationService = localizationService;

            GetSubfolderAsync(FileSystem.LocalStorage, SaveStatesFolderName).ContinueWith(d =>
            {
                SaveStatesFolder = d.Result;
            });

            GameId = null;
        }

        public void SetGameId(string id)
        {
            GameId = null;
            if(string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id))
            {
                return;
            }

            GameId = id.MD5();
        }

        public async Task<byte[]> LoadStateAsync(uint slotId)
        {
            if (!AllowOperations)
            {
                return null;
            }

            OperationInProgress = true;

            var statesFolder = await GetGameSaveStatesFolderAsync();
            var fileName = GenerateSaveFileName(slotId);
            var file = await statesFolder.GetFileAsync(fileName);
            if (file == null)
            {
                OperationInProgress = false;
                return null;
            }

            using (var stream = await file.OpenAsync(System.IO.FileAccess.Read))
            {
                var output = new byte[stream.Length];
                await stream.ReadAsync(output, 0, output.Length);

                OperationInProgress = false;
                return output;
            }
        }

        public async Task<bool> SaveStateAsync(uint slotId, byte[] data)
        {
            if (!AllowOperations)
            {
                return false;
            }

            OperationInProgress = true;

            var statesFolder = await GetGameSaveStatesFolderAsync();
            var fileName = GenerateSaveFileName(slotId);

            var file = await statesFolder.GetFileAsync(fileName);
            if (file == null)
            {
                file = await statesFolder.CreateFileAsync(fileName);
            }

            using (var stream = await file.OpenAsync(System.IO.FileAccess.ReadWrite))
            {
                await stream.WriteAsync(data, 0, data.Length);
            }

            var messageTitle = LocalizationService.GetLocalizedString(StateSavedToSlotMessageTitleKey);
            var messageBody = LocalizationService.GetLocalizedString(StateSavedToSlotMessageBodyKey);
            messageBody = string.Format(messageBody, slotId);
            NotificationService.Show(messageTitle, messageBody);

            OperationInProgress = false;
            return true;
        }

        public async Task<bool> SlotHasDataAsync(uint slotId)
        {
            if (!AllowOperations)
            {
                return false;
            }

            OperationInProgress = true;

            var statesFolder = await GetGameSaveStatesFolderAsync();
            var fileName = GenerateSaveFileName(slotId);
            var file = await statesFolder.GetFileAsync(fileName);

            OperationInProgress = false;
            return file != null;
        }

        public async Task ClearSavesAsync()
        {
            if (!AllowOperations)
            {
                return;
            }

            OperationInProgress = true;

            var statesFolder = await GetGameSaveStatesFolderAsync();
            await statesFolder.DeleteAsync();
            await GetGameSaveStatesFolderAsync();

            OperationInProgress = false;
        }

        private string GenerateSaveFileName(uint slotId)
        {
            return $"{GameId}_S{slotId}.sav";
        }

        private Task<IDirectoryInfo> GetGameSaveStatesFolderAsync()
        {
            return GetSubfolderAsync(SaveStatesFolder, GameId);
        }

        private static async Task<IDirectoryInfo> GetSubfolderAsync(IDirectoryInfo parent, string name)
        {
            IDirectoryInfo output = await parent.GetDirectoryAsync(name);
            if (output == null)
            {
                output = await parent.CreateDirectoryAsync(name);
            }

            return output;
        }
    }
}
