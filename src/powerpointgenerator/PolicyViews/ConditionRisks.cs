using IdPowerToys.PowerPointGenerator.PolicyViews;

namespace IdPowerToys.PowerPointGenerator;

public class ConditionRisks : PolicyView
{
    public ConditionRisks(ConditionalAccessPolicy policy, GraphData graphData) : base(policy, graphData)
    {
        if (policy.Conditions == null) { return; }

        IncludeExclude = GetIncludes();
    }

    private string GetIncludes()
    {
        var sb = new StringBuilder();

        AppendRisk(sb, Policy.Conditions.UserRiskLevels, "User risk risk:");
        AppendRisk(sb, Policy.Conditions.SignInRiskLevels, "Sign-in risk:");
        AppendRisk(sb, Policy.Conditions.ServicePrincipalRiskLevels, "Service principal risk:");

        return sb.ToString();
    }

    private void AppendRisk(StringBuilder sb, List<RiskLevel?> riskLevel, string title)
    {

        if (riskLevel != null && riskLevel.Any())
        {
            sb.AppendLine($"{title}");
            foreach (var risk in riskLevel)
            {
                switch (risk)
                {
                    case RiskLevel.Hidden: AppendName(sb, "Hidden"); break;
                    case RiskLevel.High: AppendName(sb, "High"); break;
                    case RiskLevel.Low: AppendName(sb, "Low"); break;
                    case RiskLevel.Medium: AppendName(sb, "Medium"); break;
                    case RiskLevel.None: AppendName(sb, "No risk"); break;
                }
            }
            sb.AppendLine();
        }
    }
}
