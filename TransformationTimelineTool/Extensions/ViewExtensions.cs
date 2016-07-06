namespace System.Web.Mvc
{
    public static class ViewExtensions
    {
        public static IHtmlString MyValidationSummary(this HtmlHelper html, string validationMessage = "")
        {
            string msg = string.Empty;
            var state = html.ViewData.ModelState;
            if (!state.IsValid)
            {
                msg += "<div class='validation-error module-alert'>";
                foreach (var key in state.Keys)
                {
                    msg += "<ul>";
                    foreach (var err in state[key].Errors)
                    {
                        msg += "<li>" + html.Encode(err.ErrorMessage) + "</li>";
                    }
                    msg += "</ul>";
                }
                msg += "</div>";
            }
            return html.Raw(msg);
        }
    }
}