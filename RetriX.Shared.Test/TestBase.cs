using Moq;
using Plugin.LocalNotifications.Abstractions;
using RetriX.Shared.Services;

namespace RetriX.Shared.Test
{
    public abstract class TestBase<T> where T : class
    {
        protected abstract T InstanceTarget();

        protected readonly T Target;

        protected readonly Mock<ILocalNotifications> NotificationServiceMock = new Mock<ILocalNotifications>();
        protected readonly Mock<ILocalizationService> LocalizationServiceMock = new Mock<ILocalizationService>();

        public TestBase()
        {
            Target = InstanceTarget();
        }
    }
}
