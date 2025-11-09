namespace NETForum.Pages.Shared.Components.TinyMceEditor
{
    public class TinyMceEditorModel
    {
        public required string EditorId { get; set; }
        public string Plugins { get; set; } = "lists link image code emoticons";
        public string Toolbar { get; set; } = "undo redo | bold italic underline | bullist numlist | link | emoticons | code";
        public bool Menubar { get; set; } = false;
        public bool Resize { get; set; } = true;
        public bool PasteAsText { get; set; } = true;
        public int Height { get; set; } = 400;
    }
}
