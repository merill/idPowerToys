using IdPowerToys.PowerPointGenerator.Graph;
using IdPowerToys.PowerPointGenerator.Infrastructure;

namespace IdPowerToys.PowerPointGenerator.PolicyViews;

public enum AppAccessType
{
    AppsNone,
    AppsAll,
    AppsSelected,
    UserActionsRegSecInfo,
    UserActionsRegDevice,
    AuthenticationContext,
    Unknown
}
public class AssignedCloudAppAction : PolicyView
{
    public AppAccessType AccessType { get; set; }
    public bool IsSelectedAppO365Only { get; set; }
    public bool IsSelectedMicrosoftAdminPortalsOnly { get; set; }

    private bool _isIncludeAppFilter = false, _isExcludeAppFilter = false;
    private string? _appFilterRule;

    public AssignedCloudAppAction(ConditionalAccessPolicy policy, GraphData graphData) : base(policy, graphData)
    {
        
        ConditionalAccessFilter appFilter = null;
        
        if (policy.Conditions.Applications.ApplicationFilter != null)
        {
            appFilter = policy.Conditions.Applications.ApplicationFilter;
            _appFilterRule = appFilter.Rule;
            _isIncludeAppFilter = appFilter.Mode == FilterMode.Include;
            _isExcludeAppFilter = appFilter.Mode == FilterMode.Exclude;
        }

        AccessType = GetAccessType();
        IsSelectedAppO365Only = false; //Default to false. We show special icon for O365.
        IsSelectedMicrosoftAdminPortalsOnly = false;

        switch (AccessType)
        {
            case AppAccessType.AppsNone:
                Name = "Azure Active Directory"; //Just show Azure AD icon Only happens with Workload Identities and a block policy
                IncludeExclude = string.Empty;
                break;
            case AppAccessType.AppsAll:
                Name = "All cloud apps";
                IncludeExclude = GetCloudAppIncludeExclude(Policy.Conditions);
                break;
            case AppAccessType.AppsSelected:
                Name = "Selected cloud apps";
                IncludeExclude = GetCloudAppIncludeExclude(Policy.Conditions);
                break;
            case AppAccessType.UserActionsRegDevice:
                Name = "Register or join devices ";
                break;
            case AppAccessType.UserActionsRegSecInfo:
                Name = "Register security information";
                break;
            case AppAccessType.AuthenticationContext:
                Name = "Authentication context";
                IncludeExclude = GetAuthContext(Policy.Conditions);
                break;
            case AppAccessType.Unknown:
                IncludeExclude = "Unknown";
                break;
        }
    }

    private AppAccessType GetAccessType()
    {
        AppAccessType accessType = AppAccessType.Unknown;
        var apps = Policy.Conditions.Applications;
        if (apps.IncludeApplications.Any() || _isIncludeAppFilter)
        {
            if (apps.IncludeApplications.Contains("None")) { accessType = AppAccessType.AppsNone; }
            else if (apps.IncludeApplications.Contains("All")) { accessType = AppAccessType.AppsAll; }
            else { accessType = AppAccessType.AppsSelected; }
        }
        else if (apps.IncludeUserActions.Any() || _isIncludeAppFilter)
        {
            if (apps.IncludeUserActions.Contains("urn:user:registersecurityinfo")) { accessType = AppAccessType.UserActionsRegSecInfo; }
            else if (apps.IncludeUserActions.Contains("urn:user:registerdevice")) { accessType = AppAccessType.UserActionsRegDevice; }
        }
        else if (apps.IncludeAuthenticationContextClassReferences.Any()) { accessType = AppAccessType.AuthenticationContext; }
        return accessType;
    }

    private string GetCloudAppIncludeExclude(ConditionalAccessConditionSet conditions)
    {
        var apps = conditions.Applications;
        var sb = new StringBuilder();
        
        if (apps.IncludeApplications.Any() || _isIncludeAppFilter)
        {
            var appCount = apps.IncludeApplications.Count();

            sb.AppendLine("✅ Include:");
            if (_isIncludeAppFilter)
            {
                AppendFilterRule(sb);
            }
            foreach (var val in apps.IncludeApplications)
            {
                var appId = val;
                if (appId == "Office365")
                {
                    appId = "Office 365"; //Format it to include space
                    if (appCount == 1 &&  !_isIncludeAppFilter) //If there is only one app included and it is O365 set to true to show O365 icon on page
                    {
                        IsSelectedAppO365Only = true;
                        Name = appId;
                    }
                    else {
                        AppendObjectName(sb, val);
                    }
                }

                else if (appId == "MicrosoftAdminPortals")
                {
                    appId = "Microsoft Admin Portals"; //Format it to include space
                    if (appCount == 1 &&  !_isIncludeAppFilter) //If there is only one app included and it is O365 set to true to show O365 icon on page
                    {
                        IsSelectedMicrosoftAdminPortalsOnly = true;
                        Name = appId;
                    }
                    else {
                        AppendObjectName(sb, val);
                    }
                }

                else
                {
                    AppendObjectName(sb, val);
                }
            }
        }
        if (apps.ExcludeApplications.Any() || _isExcludeAppFilter)
        {
            sb.AppendLine();
            sb.AppendLine("🚫 Exclude:");

            if (_isExcludeAppFilter)
            {
                AppendFilterRule(sb);
            }

            foreach (var val in apps.ExcludeApplications)
            {
                AppendObjectName(sb, val);
            }
        }
        return sb.ToString();
    }

    private void AppendFilterRule(StringBuilder sb)
    {
        sb.AppendLine($" Filter");
        sb.AppendLine($"    {_appFilterRule}");
    }

    private string GetAuthContext(ConditionalAccessConditionSet conditions)
    {
        var sb = new StringBuilder();
        if (conditions.Applications.IncludeAuthenticationContextClassReferences.Any())
        {
            foreach (var val in conditions.Applications.IncludeAuthenticationContextClassReferences)
            {
                Helper.AppendObjectName(sb, GraphData.AuthenticationContexts, val);
            }
        }
        return sb.ToString();
    }
}
