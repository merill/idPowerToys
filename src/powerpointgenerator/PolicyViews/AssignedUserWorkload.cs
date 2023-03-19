using IdPowerToys.PowerPointGenerator.Graph;
using IdPowerToys.PowerPointGenerator.Infrastructure;

namespace IdPowerToys.PowerPointGenerator.PolicyViews;

public class AssignedUserWorkload : PolicyView
{
    public bool IsWorkload { get; set; }
    public bool HasIncludeUsers { get; set; }
    public bool HasIncludeGroups { get; set; }
    public bool HasIncludeExternalUser { get; set; }
    public bool HasIncludeExternalTenant { get; set; }
    public bool HasIncludeRoles { get; set; }

    public bool HasExcludeUsers { get; set; }
    public bool HasExcludeGroups { get; set; }
    public bool HasExcludeExternalUser { get; set; }
    public bool HasExcludeExternalTenant { get; set; }
    public bool HasExcludeRoles { get; set; }

    string? _incGuestsOrExternalUsers;
    string? _incExternalTenantMembershipKind;
    IReadOnlyList<string> _incExternalTenantMembers;
    string? _excGuestsOrExternalUsers;
    string? _excExternalTenantMembershipKind;
    IReadOnlyList<string> _excExternalTenantMembers;

    public AssignedUserWorkload(ConditionalAccessPolicy policy, GraphData graphData) : base(policy, graphData)
    {
        UpdateProps();

        if (IsWorkload)
        {
            Name = "Workload identity";
            IncludeExclude = GetWorkloadIncludeExclude();
        }
        else
        {
            Name = "Users";
            IncludeExclude = GetUserIncludeExclude();
        }

    }

    private void UpdateProps()
    {
        IsWorkload = Policy.Conditions.ClientApplications != null && Policy.Conditions.ClientApplications.IncludeServicePrincipals.Count() > 0;

        var users = Policy.Conditions.Users;

        //Include users
        HasIncludeUsers = users.IncludeUsers.Count() > 0;
        HasIncludeGroups = users.IncludeGroups.Count() > 0;
        HasIncludeRoles = users.IncludeRoles.Count() > 0;
        HasIncludeExternalUser = users.IncludeGuestsOrExternalUsers != null;

        //Exclude users
        HasExcludeUsers = users.ExcludeUsers.Count() > 0;
        HasExcludeGroups = users.ExcludeGroups.Count() > 0;
        HasExcludeRoles = users.ExcludeRoles.Count() > 0;
        HasExcludeExternalTenant = users.ExcludeGuestsOrExternalUsers != null;
    }



    private string GetWorkloadIncludeExclude()
    {
        var apps = Policy.Conditions.ClientApplications;
        var sb = new StringBuilder();
        if (apps.IncludeServicePrincipals.Any())
        {
            sb.AppendLine("✅ Include:");
            if (apps.IncludeServicePrincipals.First() == "ServicePrincipalsInMyTenant")
            {
                AppendName(sb, "All owned service principals");
            }
            else
            {
                AppendObjectNames(sb, apps.IncludeServicePrincipals);
            }
            
        }
        if (apps.ExcludeServicePrincipals.Any())
        {
            sb.AppendLine("🚫 Exclude:");
            AppendObjectNames(sb, apps.ExcludeServicePrincipals);
        }
        return sb.ToString();
    }

    private string GetUserIncludeExclude()
    {
        var users = Policy.Conditions.Users;


        var sb = new StringBuilder();

        if (HasIncludeUsers || HasIncludeGroups || HasIncludeRoles || HasIncludeExternalUser || HasIncludeExternalTenant)
        {
            sb.AppendLine("✅ Include:");
            if (HasIncludeExternalUser || HasIncludeExternalTenant)
            {
                sb.AppendLine(" Guest or external users");
                AppendExternalUserTypes(sb, users.IncludeGuestsOrExternalUsers.GuestOrExternalUserTypes);
                AppendExternalTenants(sb, users.IncludeGuestsOrExternalUsers.ExternalTenants);
            }
            if (HasIncludeRoles)
            {
                sb.AppendLine(" Directory roles");
                AppendObjectNames(sb, users.IncludeRoles);
            }
            if (HasIncludeGroups) {
                sb.AppendLine(" Groups");
                AppendObjectNames(sb, users.IncludeGroups);
                
            }
            if (HasIncludeUsers) {
                sb.AppendLine(" Users");
                AppendObjectNames(sb, users.IncludeUsers); 
            }
            sb.AppendLine();
        }

        if (HasExcludeUsers || HasExcludeGroups || HasExcludeRoles || HasExcludeExternalUser || HasExcludeExternalTenant)
        {
            sb.AppendLine("🚫 Exclude:");
            if (HasExcludeExternalUser || HasExcludeExternalTenant)
            {
                sb.AppendLine(" Guest or external users");
                AppendExternalUserTypes(sb, users.ExcludeGuestsOrExternalUsers.GuestOrExternalUserTypes);
                AppendExternalTenants(sb, users.ExcludeGuestsOrExternalUsers.ExternalTenants);
            }
            if (HasExcludeRoles)
            {
                sb.AppendLine(" Directory roles");
                AppendObjectNames(sb, users.ExcludeRoles);
            }
            if (HasExcludeGroups)
            {
                sb.AppendLine(" Groups");
                AppendObjectNames(sb, users.ExcludeGroups);

            }
            if (HasExcludeUsers)
            {
                sb.AppendLine(" Users");
                AppendObjectNames(sb, users.ExcludeUsers);
            }

        }
        return sb.ToString();
    }

    /// <summary>
    /// Add a few more spaces to indent correctly
    /// </summary>
    /// <param name="sb"></param>
    /// <param name="name"></param>
    public override void AppendName(StringBuilder sb, string name)
    {
        sb.AppendLine($"  - {name}"); 
    }
    private void AppendExternalUserTypes(StringBuilder sb, ConditionalAccessGuestOrExternalUserTypes? guestsOrExternalUserTypes)
    {
        var type = guestsOrExternalUserTypes.Value;
        //TODO :need to show this once c# sdk has value
        //foreach (var type in guestOrExternalUserTypes.Split(','))
        //{
        //    switch (type)
        //    {
        //        case "internalGuest":
        //            sb.AppendLine("  - Local guest users");
        //            break;
        //        case "b2bCollaborationGuest":
        //            sb.AppendLine("  - B2B collaboration guest users");
        //            break;
        //        case "b2bCollaborationMember":
        //            sb.AppendLine("  - B2B collaboration member users");
        //            break;
        //        case "b2bDirectConnectUser":
        //            sb.AppendLine("  - B2B direct connect users");
        //            break;
        //        case "otherExternalUser":
        //            sb.AppendLine("  - Other external users");
        //            break;
        //        case "serviceProvider":
        //            sb.AppendLine("  - Service provider users");
        //            break;
        //        default:
        //            sb.AppendLine(type);
        //            break;
        //    }
        //}
    }

    private void AppendExternalTenants(StringBuilder sb, ConditionalAccessExternalTenants externalTenants)
    {

        if (externalTenants != null)
        {
            switch (externalTenants.MembershipKind)
            {
                case ConditionalAccessExternalTenantsMembershipKind.All:
                    sb.AppendLine("  All external Azure AD organizations");
                    break;

                case ConditionalAccessExternalTenantsMembershipKind.Enumerated:
                    var enumeratedExternalTenants = (ConditionalAccessEnumeratedExternalTenants)externalTenants;
                    if (enumeratedExternalTenants != null && enumeratedExternalTenants.Members != null) {
                        sb.AppendLine("  Selected external Azure AD organizations");
                        foreach (var tenantId in enumeratedExternalTenants.Members)
                        {
                            string tenantName = Helper.GetObjectName(GraphData.ObjectCache, tenantId);
                            if (string.IsNullOrEmpty(tenantId)) { tenantName = tenantId; }
                            sb.AppendLine($"    - {tenantName}");
                        }
                    }
                    break;
            }
        }
    }
}
