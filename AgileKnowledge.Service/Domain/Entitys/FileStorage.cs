using AgileKnowledge.Service.Domain.BaseEntity;

namespace AgileKnowledge.Service.Domain.Enities
{
    public class FileStorage : FullAuditedEntity
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public long Size { get; set; }
        public bool IsCompression { get; set; }
        public string FullName { get; set; }

        protected FileStorage()
        {
        }

        public FileStorage(string name, string path, long size, bool isCompression)
        {
            Name = name;
            Path = path;
            Size = size;
            IsCompression = isCompression;
        }

        public void SetFullName(string fullName)
        {
            FullName = fullName;
        }
    }
}
