namespace RetriX.Shared.Test
{
    public abstract class TestBase<T> where T : class
    {
        protected abstract T InstanceTarget();

        protected readonly T Target;

        public TestBase()
        {
            Target = InstanceTarget();
        }
    }
}
