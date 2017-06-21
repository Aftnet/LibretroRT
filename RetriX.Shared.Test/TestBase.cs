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

        protected readonly Mock<ILocalNotifications> NotificationServiceMock = new Mock<ILocalNotifications>();
        protected readonly Mock<ILocalizationService> LocalizationServiceMock = new Mock<ILocalizationService>();

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
