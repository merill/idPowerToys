using IdPowerToys.PowerPointGenerator.Graph;

namespace IdPowerToys.PowerPointGenerator.PolicyViews;

public class ConditionDeviceFilters : PolicyView
{
    public ConditionDeviceFilters(ConditionalAccessPolicy policy, GraphData graphData) : base(policy, graphData)
    {
        if (Policy.Conditions == null || Policy.Conditions.Devices == null || Policy.Conditions.Devices.DeviceFilter == null) { return; }

        IncludeExclude = GetIncludes();
    }

    private string GetIncludes()
    {
        var sb = new StringBuilder();
            var mode = Policy.Conditions.Devices.DeviceFilter.Mode == FilterMode.Include ? "Include when" : "Exclude when";
            sb.AppendLine(mode);
            sb.AppendLine(Policy.Conditions.Devices.DeviceFilter.Rule);

        return sb.ToString();
    }
}
