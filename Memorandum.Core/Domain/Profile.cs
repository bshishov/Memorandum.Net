namespace Memorandum.Core.Domain
{
    public class Profile
    {
        public virtual int Id { get; set; }
        public virtual User User { get; set; }
        public virtual TextNode Home { get; set; }

    }
}
