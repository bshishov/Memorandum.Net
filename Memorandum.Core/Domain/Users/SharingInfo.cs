namespace Memorandum.Core.Domain.Users
{
    public class SharingInfo
    {
        public string ItemRelativePath { get; set; }
        public string Target { get; set; }
        public SharingType Type { get; set; }
    }
}