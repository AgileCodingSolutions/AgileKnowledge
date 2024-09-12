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
        public string Type { get; set; }

        protected FileStorage()
        {
        }

        public FileStorage(string name, string path,string type, long size, bool isCompression)
        {
            Name = name;
            Path = path;
            Type = type;
            Size = size;
            FullName = "";
            IsCompression = isCompression;
        }

        public void SetFullName(string fullName)
        {
            FullName = fullName;
        }
    }
}
