using Volo.Abp.Settings;

namespace JobHub.Settings
{
    public class JobHubSettingDefinitionProvider : SettingDefinitionProvider
    {
        public override void Define(ISettingDefinitionContext context)
        {
            //Define your own settings here. Example:
            //context.Add(new SettingDefinition(JobHubSettings.MySetting1));
        }
    }
}
