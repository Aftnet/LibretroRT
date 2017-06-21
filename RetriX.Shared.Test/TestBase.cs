using Acr.UserDialogs;
using Moq;
using PCLStorage;
using Plugin.LocalNotifications.Abstractions;
using RetriX.Shared.Services;
using System.Threading.Tasks;

namespace RetriX.Shared.Test
{
    public abstract class TestBase<T> where T : class
    {
        protected abstract T InstantiateTarget();

        protected readonly T Target;

        protected readonly Mock<IUserDialogs> DialogsServiceMock = new Mock<IUserDialogs>();
        protected readonly Mock<ILocalizationService> LocalizationServiceMock = new Mock<ILocalizationService>();
        protected readonly Mock<IPlatformService> PlatformServiceMock = new Mock<IPlatformService>();
        protected readonly Mock<ILocalNotifications> NotificationServiceMock = new Mock<ILocalNotifications>();
        protected readonly Mock<ICryptographyService> CryptographyServiceMock = new Mock<ICryptographyService>();

        public TestBase()
        {
            Target = InstantiateTarget();
        }

        protected Task<IFolder> GetTestFilesFolderAsync()
        {
            return FileSystem.Current.GetFolderFromPathAsync("TestFiles");
        }
    }
}
