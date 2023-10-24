using IdPowerToys.PowerPointGenerator.Graph;

namespace IdPowerToys.PowerPointGenerator.PolicyViews;

public class ConditionClientAppTypes : PolicyView
{
    public ConditionClientAppTypes(ConditionalAccessPolicy policy, GraphData graphData) : base(policy, graphData)
    {
        if (policy.Conditions == null || policy.Conditions.ClientAppTypes == null || !policy.Conditions.ClientAppTypes.Any()) { return; }
        IncludeExclude = GetIncludes();
    }

    private string GetIncludes()
    {
        var sb = new StringBuilder();

        var clientAppTypes = Policy.Conditions.ClientAppTypes;
        foreach (var clientAppType in clientAppTypes)
        {
            if (clientAppType == ConditionalAccessClientApp.All) { return string.Empty; } //CA blade shows "All" as 'Not configured' so we do the same and hide it in the doc
            if (clientAppType == ConditionalAccessClientApp.Browser) { AppendName(sb, "Browser"); }
            if (clientAppType == ConditionalAccessClientApp.MobileAppsAndDesktopClients) { AppendName(sb, "Mobile app and desktop clients"); }
            if (clientAppType == ConditionalAccessClientApp.ExchangeActiveSync || clientAppType == ConditionalAccessClientApp.EasSupported) { AppendName(sb, "Exchange ActiveSync clients"); }
            if (clientAppType == ConditionalAccessClientApp.Other) { AppendName(sb, "Other legacy clients"); }
        }

        return sb.ToString();
    }
}
