using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Pagination;

namespace JobHub.Web.TagHelpers.Pagination
{
    [HtmlTargetElement("my-paginator", Attributes="asp-all-route-data")]
    [HtmlTargetElement("my-paginator", Attributes="asp-route-*")]
    public class MyPaginationTagHelper : AbpTagHelper<MyPaginationTagHelper, MyPaginationTagHelperService>
    {
        public PagerModel Model { get; set; }

        [HtmlAttributeName("show-info")]
        public bool? ShowInfo { get; set; }

        [HtmlAttributeName("asp-all-route-data", DictionaryAttributePrefix="asp-route-")]
        public IDictionary<string, string> RouteValues { get; set; } = new Dictionary<string, string>();

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public MyPaginationTagHelper(MyPaginationTagHelperService tagHelperService)
            : base(tagHelperService)
        {
        }
    }
}
