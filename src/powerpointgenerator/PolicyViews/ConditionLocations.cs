using Microsoft.Graph;
using System.Text;

namespace CADocGen.PowerPointGenerator.PolicyViews;

public class ConditionLocations : PolicyView
{
    public ConditionLocations(ConditionalAccessPolicy policy, GraphData graphData) : base(policy, graphData)
    {
        if (Policy.Conditions == null || Policy.Conditions.Locations == null) { return; }

        IncludeExclude = GetIncludes();
    }

    private string GetIncludes()
    {
        var sb = new StringBuilder();

        AppendLocations(sb, Policy.Conditions.Locations.IncludeLocations, "✅ Include");
        AppendLocations(sb, Policy.Conditions.Locations.ExcludeLocations, "🚫 Exclude");

        return sb.ToString();
    }

    private void AppendLocations(StringBuilder sb, IEnumerable<string> locations, string title)
    {
        if (locations != null && locations.Any())
        {
            sb.AppendLine($"{title}");
            AppendObjectNames(sb, locations);
            sb.AppendLine();
        }
    }
}
