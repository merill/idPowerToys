using Microsoft.Graph;
using System.Text;

namespace CADocGen.PowerPointGenerator.PolicyViews;

public class ConditionPlatforms : PolicyView
{
    public ConditionPlatforms(ConditionalAccessPolicy policy, GraphData graphData) : base(policy, graphData)
    {
        if (Policy.Conditions == null || Policy.Conditions.Platforms == null) { return; }

        IncludeExclude = GetIncludes();
    }

    private string GetIncludes()
    {
        var sb = new StringBuilder();

        var platforms = Policy.Conditions.Platforms;
        if (platforms != null)
        {
            if (platforms.IncludePlatforms != null && platforms.IncludePlatforms.Any())
            {
                sb.AppendLine("Include");
                AppendPlatforms(sb, platforms.IncludePlatforms);
                sb.AppendLine();
            }
            if (platforms.ExcludePlatforms != null && platforms.ExcludePlatforms.Any())
            {
                sb.AppendLine("Exclude");
                AppendPlatforms(sb, platforms.ExcludePlatforms);
                sb.AppendLine();
            }
        }

        return sb.ToString();
    }

    private void AppendPlatforms(StringBuilder sb, IEnumerable<ConditionalAccessDevicePlatform> platforms)
    {
        foreach (var platform in platforms)
        {
            if (platform == ConditionalAccessDevicePlatform.All) { sb.AppendLine(" - All"); }
            if (platform == ConditionalAccessDevicePlatform.Android) { sb.AppendLine(" - Android"); }
            if (platform == ConditionalAccessDevicePlatform.IOS) { sb.AppendLine(" - iOS"); }
            if (platform == ConditionalAccessDevicePlatform.Linux) { sb.AppendLine(" - Linux"); }
            if (platform == ConditionalAccessDevicePlatform.MacOS) { sb.AppendLine(" - macOS"); }
            if (platform == ConditionalAccessDevicePlatform.Windows) { sb.AppendLine(" - Windows"); }
            if (platform == ConditionalAccessDevicePlatform.WindowsPhone) { sb.AppendLine(" - Windows Phone"); }
        }
    }
}
