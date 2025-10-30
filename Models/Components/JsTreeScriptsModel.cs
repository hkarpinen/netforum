namespace NETForum.Models.Components
{
    public class JsTreeScriptsModel
    {
        public required string TreeId { get; set; }
        public bool AllowModification { get; set; }
        public bool EnableDragDrop { get; set; }
        public bool EnableContextMenu { get; set; }
        public List<string> Plugins { get; set; } = new()
        {
            "dnd", "contextmenu"
        };

        // Default handler values for JS events in JsTree
        public string OnSelectNode { get; set; } = "handleNodeSelect(data)";
        public string OnMoveNode { get; set; } = "handleNodeMove(data)";
        public string OnAddChild { get; set; } = "handleNodeAdd(node.id)";
        public string OnNodeEdit { get; set; } = "handleNodeEdit(node.id)";
        public string OnNodeDelete { get; set; } = "handleNodeDelete(node.id)";
    }
}
