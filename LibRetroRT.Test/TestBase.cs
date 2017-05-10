using System;

namespace LibRetroRT.Test
{
    public abstract class TestBase<T> : IDisposable
    {
        protected T Target;
        protected abstract T GetTarget();

        public virtual void Dispose()
        {
            (Target as IDisposable)?.Dispose();
        }

        public TestBase()
        {
            Target = GetTarget();
        }
    }
}
