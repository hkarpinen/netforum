using Microsoft.AspNetCore.Mvc;

namespace NETForum.Pages.Shared.Components.TinyMceEditor
{
    public class TinyMceEditorViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(TinyMceEditorModel tinyMceEditorModel)
        {
            return View(tinyMceEditorModel);
        }
    }
}

