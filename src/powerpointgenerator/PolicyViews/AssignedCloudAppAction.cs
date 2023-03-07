using Microsoft.Graph;
using System.Text;

namespace CADocGen.PowerPointGenerator.PolicyViews;

public enum AccessType
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
    public AccessType AccessType { get; set; }
    public bool IsSelectedAppO365Only { get; set; }
    private bool _isIncludeAppFilter = false, _isExcludeAppFilter = false;
    private string _appFilterRule;

    public AssignedCloudAppAction(ConditionalAccessPolicy policy, GraphData graphData) : base(policy, graphData)
    {
        var appsJson = Helper.GetConditionsApplicationsJson(Policy.Conditions.Applications); //TODO Remove this once SDK supports appfilter.
        ApplicationFilter appFilter = null;
        string appFilterMode = string.Empty;
        

        if (appsJson.applicationFilter != null)
        {
            appFilter = appsJson.applicationFilter;
            appFilterMode = appFilter.mode;
            _appFilterRule = appFilter.rule;
            _isIncludeAppFilter = appFilterMode == "include";
            _isExcludeAppFilter = appFilterMode == "exclude";
        }

        AccessType = GetAccessType();
        IsSelectedAppO365Only = false; //Default to false. We show special icon for O365.

        switch (AccessType)
        {
            case AccessType.AppsNone:
                Name = "Azure Active Directory"; //Just show Azure AD icon Only happens with Workload Identities and a block policy
                IncludeExclude = string.Empty;
                break;
            case AccessType.AppsAll:
                Name = "All cloud apps";
                IncludeExclude = GetCloudAppIncludeExclude(Policy.Conditions);
                break;
            case AccessType.AppsSelected:
                Name = "Selected cloud apps";
                IncludeExclude = GetCloudAppIncludeExclude(Policy.Conditions);
                break;
            case AccessType.UserActionsRegDevice:
                Name = "Register or join devices ";
                break;
            case AccessType.UserActionsRegSecInfo:
                Name = "Register security information";
                break;
            case AccessType.AuthenticationContext:
                Name = "Authentication context";
                IncludeExclude = GetAuthContext(Policy.Conditions);
                break;
            case AccessType.Unknown:
                IncludeExclude = "Unknown";
                break;
        }
    }

    private AccessType GetAccessType()
    {
        AccessType accessType = AccessType.Unknown;
        var apps = Policy.Conditions.Applications;
        if (apps.IncludeApplications.Any() || _isIncludeAppFilter)
        {
            if (apps.IncludeApplications.Contains("None")) { accessType = AccessType.AppsNone; }
            else if (apps.IncludeApplications.Contains("All")) { accessType = AccessType.AppsAll; }
            else { accessType = AccessType.AppsSelected; }
        }
        else if (apps.IncludeUserActions.Any() || _isIncludeAppFilter)
        {
            if (apps.IncludeUserActions.Contains("urn:user:registersecurityinfo")) { accessType = AccessType.UserActionsRegSecInfo; }
            else if (apps.IncludeUserActions.Contains("urn:user:registerdevice")) { accessType = AccessType.UserActionsRegDevice; }
        }
        else if (apps.IncludeAuthenticationContextClassReferences.Any()) { accessType = AccessType.AuthenticationContext; }
        return accessType;
    }

    private string GetCloudAppIncludeExclude(ConditionalAccessConditionSet conditions)
    {
        var apps = conditions.Applications;
        var sb = new StringBuilder();

  
        
        if (apps.IncludeApplications.Any() || _isIncludeAppFilter)
        {
            var appCount = apps.IncludeApplications.Count();

            sb.AppendLine("Include:");
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
            sb.AppendLine("Exclude:");

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
