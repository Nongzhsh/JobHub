using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Pagination;

namespace Nongzhsh.JobHub.TagHelpers.Pagination
{
    [HtmlTargetElement("my-paginator")]
    public class MyPaginationTagHelper : AbpTagHelper<MyPaginationTagHelper, MyPaginationTagHelperService>
    {
        public PagerModel Model { get; set; }

        public IDictionary<string, string> RouteValues { get; set; }

        public bool? ShowInfo { get; set; }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public MyPaginationTagHelper(MyPaginationTagHelperService tagHelperService)
            : base(tagHelperService)
        {
            RouteValues = new Dictionary<string, string>();
        }
    }
}