namespace RetriX.Shared.FileProviders
{
    public static class FileAccessExtensions
    {
        public static PCLStorage.FileAccess ToPCLStorageAccess(this System.IO.FileAccess value)
        {
            switch (value)
            {
                case System.IO.FileAccess.Read:
                    return PCLStorage.FileAccess.Read;
                case System.IO.FileAccess.ReadWrite:
                    return PCLStorage.FileAccess.ReadAndWrite;
                default:
                    return PCLStorage.FileAccess.ReadAndWrite;
            }
        }
    }
}
