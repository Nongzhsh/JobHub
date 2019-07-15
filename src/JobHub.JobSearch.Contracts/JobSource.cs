using System.ComponentModel;

namespace JobHub.JobSearch.Contracts
{
    /// <summary>
    /// 招聘职位提供源
    /// </summary>
    public enum JobSource
    {
        //[Description("智联招聘")]
        //智联招聘 = 0,

        [Description("前程无忧")]
        前程无忧 = 0,

        //[Description("猎聘招聘")]
        //猎聘招聘 = 1,

        //[Description("Boss招聘")]
        //Boss招聘 = 30,

        //[Description("拉钩招聘")]
        //拉钩招聘 = 4
    }
}
