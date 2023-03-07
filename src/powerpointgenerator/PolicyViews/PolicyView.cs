using Microsoft.Graph;
using System.Text;

namespace CADocGen.PowerPointGenerator.PolicyViews;

public class PolicyView
{
    public ConditionalAccessPolicy Policy { get; protected set; }
    public GraphData GraphData { get; protected set; }
    public string? Name { get; protected set; }
    public string? IncludeExclude { get; protected set; }

    public PolicyView(ConditionalAccessPolicy policy, GraphData graphData)
    {
        Policy = policy;
        GraphData = graphData;
    }

    public bool HasData
    {
        get { return !string.IsNullOrEmpty(IncludeExclude); }
    }

    public virtual void AppendName(StringBuilder sb, string name)
    {
        sb.AppendLine($" - {name}");
    }

    public void AppendObjectName(StringBuilder sb, string id)
    {
        Helper.AppendObjectName(sb, GraphData.ObjectCache, id);
    }

    public void AppendObjectNames(StringBuilder sb, IEnumerable<string> ids)
    {
        Helper.AppendObjectNames(sb, GraphData.ObjectCache, ids);
    }
}
