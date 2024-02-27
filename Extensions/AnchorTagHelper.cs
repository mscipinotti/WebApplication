using Microsoft.AspNetCore.Razor.TagHelpers;

namespace WebAPP.Extensions
{
    [HtmlTargetElement("Extensions")]
    public class AnchorTagHelper : TagHelper
    {
        public string Model { get; set; } = string.Empty;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "a";
            
            output.Attributes.SetAttribute("model", $"{Model}");
        }
    }
}
