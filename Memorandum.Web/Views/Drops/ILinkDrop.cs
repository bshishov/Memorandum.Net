namespace Memorandum.Web.Views.Drops
{
    internal interface ILinkDrop
    {
        int? Id { get; }
        string Comment { get; }
        string StartNode { get; }
        string StartNodeProvider { get; }
        string EndNode { get; }
        string EndNodeProvider { get; }
        NodeDrop Node { get; }
    }
}