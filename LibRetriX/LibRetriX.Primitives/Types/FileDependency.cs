namespace LibRetriX
{
    public class FileDependency
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string MD5 { get; private set; }

        public FileDependency(string name, string description, string md5)
        {
            Name = name;
            Description = description;
            MD5 = md5;
        }
    }
}
