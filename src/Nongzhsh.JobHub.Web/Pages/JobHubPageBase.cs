using System;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Nongzhsh.JobHub.Localization.JobHub;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Nongzhsh.JobHub.Pages
{
    public abstract class JobHubPageBase : AbpPage
    {
        [RazorInject]
        public IHtmlLocalizer<JobHubResource> L { get; set; }

        public LocalizedHtmlString ConvertDatetimeToTimeAgo(string dtStr)
        {
            if(!DateTime.TryParse(dtStr, out var dt))
            {
                return new LocalizedHtmlString("dt", dtStr);
            }
            var timeDiff = DateTime.Now - dt;

            var diffInDays = (int)timeDiff.TotalDays;

            if(diffInDays >= 365)
            {
                return L["YearsAgo", diffInDays / 365];
            }
            if(diffInDays >= 30)
            {
                return L["MonthsAgo", diffInDays / 30];
            }
            if(diffInDays >= 7)
            {
                return L["WeeksAgo", diffInDays / 7];
            }
            if(diffInDays >= 1)
            {
                return L["DaysAgo", diffInDays];
            }

            var diffInSeconds = (int)timeDiff.TotalSeconds;

            if(diffInSeconds >= 3600)
            {
                return L["HoursAgo", diffInSeconds / 3600];
            }
            if(diffInSeconds >= 60)
            {
                return L["MinutesAgo", diffInSeconds / 60];
            }
            if(diffInSeconds >= 1)
            {
                return L["SecondsAgo", diffInSeconds];
            }

            return L["Now"];
        }
    }
}
