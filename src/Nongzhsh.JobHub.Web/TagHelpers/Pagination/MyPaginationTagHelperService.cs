using System.Collections.Generic;
using System.Text;
using System.Text.Encodings.Web;
using Localization.Resources.AbpUi;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.Microsoft.AspNetCore.Razor.TagHelpers;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Extensions;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Pagination;

namespace Nongzhsh.JobHub.TagHelpers.Pagination
{
    public class MyPaginationTagHelperService : AbpTagHelperService<MyPaginationTagHelper>
    {
        private readonly IHtmlGenerator _generator;
        private readonly HtmlEncoder _encoder;
        private readonly IAbpTagHelperLocalizer _tagHelperLocalizer;

        public MyPaginationTagHelperService(
          IHtmlGenerator generator,
          HtmlEncoder encoder,
          IAbpTagHelperLocalizer tagHelperLocalizer)
        {
            this._generator = generator;
            this._encoder = encoder;
            this._tagHelperLocalizer = tagHelperLocalizer;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if(this.TagHelper.Model.ShownItemsCount <= 0)
                output.SuppressOutput();
            this.ProcessMainTag(context, output);
            this.SetContentAsHtml(context, output);
        }

        protected virtual void SetContentAsHtml(TagHelperContext context, TagHelperOutput output)
        {
            StringBuilder stringBuilder = new StringBuilder("");
            stringBuilder.AppendLine(this.GetOpeningTags(context, output));
            stringBuilder.AppendLine(this.GetPreviousButton(context, output));
            stringBuilder.AppendLine(this.GetPages(context, output));
            stringBuilder.AppendLine(this.GetNextButton(context, output));
            stringBuilder.AppendLine(this.GetClosingTags(context, output));
            output.Content.SetHtmlContent(stringBuilder.ToString());
        }

        protected virtual void ProcessMainTag(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.AddClass("row");
            output.Attributes.AddClass("mt-3");
        }

        protected virtual string GetPages(TagHelperContext context, TagHelperOutput output)
        {
            StringBuilder stringBuilder = new StringBuilder("");
            foreach(PageItem page in this.TagHelper.Model.Pages)
                stringBuilder.AppendLine(this.GetPage(context, output, page));
            return stringBuilder.ToString();
        }

        protected virtual string GetPage(
          TagHelperContext context,
          TagHelperOutput output,
          PageItem page)
        {
            StringBuilder stringBuilder1 = new StringBuilder("");
            stringBuilder1.AppendLine("<li class=\"page-item " + (this.TagHelper.Model.CurrentPage == page.Index ? "active" : "") + "\">");
            if(page.IsGap)
                stringBuilder1.AppendLine("<span class=\"page-link gap\">…</span>");
            else if(!page.IsGap && this.TagHelper.Model.CurrentPage == page.Index)
            {
                stringBuilder1.AppendLine("                                 <span class=\"page-link\">\r\n                                    " + (object)page.Index + "\r\n                                    <span class=\"sr-only\">(current)</span>\r\n                                </span>");
            }
            else
            {
                StringBuilder stringBuilder2 = stringBuilder1;
                TagHelperContext context1 = context;
                TagHelperOutput output1 = output;
                int index = page.Index;
                string currentPage = index.ToString();
                index = page.Index;
                string localizationKey = index.ToString();
                string str = this.RenderAnchorTagHelperLinkHtml(context1, output1, currentPage, localizationKey);
                stringBuilder2.AppendLine(str);
            }
            stringBuilder1.AppendLine("</li>");
            return stringBuilder1.ToString();
        }

        protected virtual string GetPreviousButton(TagHelperContext context, TagHelperOutput output)
        {
            string localizationKey = "PagerPrevious";
            string currentPage = this.TagHelper.Model.CurrentPage == 1 ? this.TagHelper.Model.CurrentPage.ToString() : (this.TagHelper.Model.CurrentPage - 1).ToString();
            return "<li class=\"page-item " + (this.TagHelper.Model.CurrentPage == 1 ? "disabled" : "") + "\">\r\n" + this.RenderAnchorTagHelperLinkHtml(context, output, currentPage, localizationKey) + "                </li>";
        }

        protected virtual string GetNextButton(TagHelperContext context, TagHelperOutput output)
        {
            string localizationKey = "PagerNext";
            string currentPage = (this.TagHelper.Model.CurrentPage + 1).ToString();
            return "<li class=\"page-item " + (this.TagHelper.Model.CurrentPage >= this.TagHelper.Model.TotalPageCount ? "disabled" : "") + "\">\r\n" + this.RenderAnchorTagHelperLinkHtml(context, output, currentPage, localizationKey) + "                </li>";
        }

        protected virtual string RenderAnchorTagHelperLinkHtml(
          TagHelperContext context,
          TagHelperOutput output,
          string currentPage,
          string localizationKey)
        {
            IStringLocalizer localizer = this._tagHelperLocalizer.GetLocalizer(typeof(AbpUiResource));
            TagHelperAttributeList attributeList;
            TagHelperOutput output1 = this.GetAnchorTagHelper(currentPage, out attributeList).ProcessAndGetOutput(attributeList, context, "a", TagMode.StartTagAndEndTag, false);
            output1.Content.SetHtmlContent((string)localizer[localizationKey]);
            return output1.Render(this._encoder);
        }

        private AnchorTagHelper GetAnchorTagHelper(
          string currentPage,
          out TagHelperAttributeList attributeList)
        {
            AnchorTagHelper anchorTagHelper = new AnchorTagHelper(this._generator)
            {
                Page = this.TagHelper.Model.PageUrl,
                ViewContext = this.TagHelper.ViewContext
            };

            foreach(var item in TagHelper.RouteValues)
            {
                anchorTagHelper.RouteValues.AddIfNotContains(item);
            }

            anchorTagHelper.RouteValues.Add(nameof(currentPage), currentPage);
            anchorTagHelper.RouteValues.Add("sort", this.TagHelper.Model.Sort);
            attributeList = new TagHelperAttributeList()
      {
        new TagHelperAttribute("tabindex", (object) "-1"),
        new TagHelperAttribute("class", (object) "page-link")
      };
            return anchorTagHelper;
        }

        protected virtual string GetOpeningTags(TagHelperContext context, TagHelperOutput output)
        {
            IStringLocalizer localizer = this._tagHelperLocalizer.GetLocalizer(typeof(AbpUiResource));
            bool? showInfo = this.TagHelper.ShowInfo;
            string str;
            if((showInfo.HasValue ? (showInfo.GetValueOrDefault() ? 1 : 0) : 0) == 0)
                str = "";
            else
                str = "    <div class=\"col-sm-12 col-md-5\"> " + (string)localizer["PagerInfo", new object[3]
                {
          (object) this.TagHelper.Model.ShowingFrom,
          (object) this.TagHelper.Model.ShowingTo,
          (object) this.TagHelper.Model.TotalItemsCount
                }] + "</div>\r\n";
            return str + "    <div class=\"col-sm-12 col-md-7\">\r\n        <nav aria-label=\"Page navigation\">\r\n            <ul class=\"pagination justify-content-center\">";
        }

        protected virtual string GetClosingTags(TagHelperContext context, TagHelperOutput output)
        {
            return "            </ul>\r\n         </ nav>\r\n    </div>\r\n";
        }
    }
}
