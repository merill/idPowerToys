using Microsoft.Graph;
using System.Collections.Specialized;
using System.Text.Json.Serialization;

namespace IdPowerToys.PowerPointGenerator;

public class GraphHelper
{
    private ConfigOptions _configOptions;
    GraphServiceClient? _graph;
    public GraphServiceClient GraphServiceClient { get { return _graph; } }

    /// <summary>
    /// Perform a manual generation without making Graph API calls
    /// </summary>
    public GraphHelper(ConfigOptions configOptions)
    {
        _configOptions = configOptions;
    }

    public GraphHelper(GraphServiceClient graphServiceClient, ConfigOptions configOptions) : this(configOptions)
    {
        _graph = graphServiceClient;
    }

    public async Task<User> GetMe()
    {
        var me = await _graph.Me.GetAsync();
        return me;
    }

    public async Task<ICollection<Organization>> GetOrganization()
    {
        var org = await _graph.Organization.GetAsync();
        return org.Value;
    }

    public async Task<string?> GetTenantName(string tenantId)
    {
        try {
            var tenantInfo = await _graph.TenantRelationships.FindTenantInformationByTenantIdWithTenantId(tenantId).GetAsync();
            return tenantInfo.DisplayName;
        }
        catch(Exception ex)
        {
            return null;
        }
    }

    public async Task<List<ConditionalAccessPolicy>> GetPolicies()
    {
        var policies = await _graph.Identity.ConditionalAccess.Policies
            //.Filter("id eq '80f881c0-ab7c-426e-955d-9d48717d7659'")//.Top(10)
            .GetAsync();
        return policies.Value;
    }

    private async Task<string?> GetUserName(string id)
    {
        try
        {
            var user = await _graph.Users[id].GetAsync();
            return user.DisplayName;
        }
        catch (ServiceException ex) when (ex.ResponseStatusCode == 404)
        {
            return Helper.GetShortId(id);
        }
    }



    private string GetManualObjectName(string id, int index, string prefix)
    {
        return $"{prefix} {index} ({Helper.GetShortId(id)})";
    }
    private async Task<string?> GetGroupName(string id)
    {
        try
        {
            var user = await _graph.Groups[id].GetAsync();
            return user.DisplayName;
        }
        catch (ServiceException ex) when (ex.ResponseStatusCode == 404)
        {
            return Helper.GetShortId(id);
        }
    }

    private async Task<string?> GetServicePrincipalName(string id)
    {
        try
        {
            var sp = await _graph.ServicePrincipals[id].GetAsync();
            return sp.DisplayName;
        }
        catch (ServiceException ex) when (ex.ResponseStatusCode == 404)
        {
            return Helper.GetShortId(id);
        }
    }

    private async Task<string?> GetApplicationName(string id)
    {
        try
        {
            var sp = await _graph.Applications
                .GetAsync((requestConfiguration) =>
                {
                    requestConfiguration.QueryParameters.Filter = "appid eq '";
                    requestConfiguration.Headers.Add("ConsistencyLevel", "eventual");
                });
            var app = sp.Value;
            if (app != null || app.Count == 0)
            {
                return Helper.GetShortId(id);
            }
            else
            {
                return app[0].DisplayName;
            }
        }
        catch (ServiceException ex) when (ex.ResponseStatusCode == 404)
        {
            return Helper.GetShortId(id);
        }
    }

    public async Task<StringDictionary> GetDirectoryObjectCache(ICollection<ConditionalAccessPolicy> policies)
    {
        var directoryObjects = new StringDictionary();
        var userIds = new List<string>();
        var groupIds = new List<string>();
        var roleIds = new List<string>();
        var servicePrincipalIds = new List<string>();
        var applicationIds = new List<string>();
        var tenantIds = new List<string>();

        foreach (var policy in policies)
        {
            var users = policy.Conditions.Users;
            userIds.AddRange(users.IncludeUsers);
            userIds.AddRange(users.ExcludeUsers);
            groupIds.AddRange(users.IncludeGroups);
            groupIds.AddRange(users.ExcludeGroups);
            roleIds.AddRange(users.IncludeRoles);
            roleIds.AddRange(users.ExcludeRoles);
            applicationIds.AddRange(policy.Conditions.Applications.IncludeApplications);
            applicationIds.AddRange(policy.Conditions.Applications.ExcludeApplications);
            var apps = policy.Conditions.ClientApplications;
            if (apps != null)
            {
                servicePrincipalIds.AddRange(apps.IncludeServicePrincipals);
                servicePrincipalIds.AddRange(apps.ExcludeServicePrincipals);
            }
            tenantIds.AddRange(GetTenantIds(policy.Conditions));
        }

        int index = 1;
        foreach (var id in userIds.Distinct())
        {
            if (Guid.TryParse(id, out _))
            {
                var name = _configOptions.IsManual == true || _configOptions.IsMaskUser == true ? GetManualObjectName(id, index++, "User") : await GetUserName(id);
                directoryObjects.Add(id, name); //TODO use batch
            }
        }
        index = 1;
        foreach (var id in groupIds.Distinct())
        {
            if (Guid.TryParse(id, out _))
            {
                var name = _configOptions.IsManual == true || _configOptions.IsMaskGroup == true ? GetManualObjectName(id, index++, "Group") : await GetGroupName(id);
                directoryObjects.Add(id, name); //TODO use batch
            }
        }

        if (_configOptions.IsManual == true)
        {
            foreach (var id in roleIds.Distinct())
            {
                if (Guid.TryParse(id, out _))
                {
                    var name = GetManualObjectName(id, index++, "Role");
                    directoryObjects.Add(id, name); //TODO use batch
                }
            }
        }
        else
        {
            var directoryRoles = await DirectoryRoles.GetRoles(_graph);
            if (directoryRoles != null)
            {
                foreach (var item in directoryRoles)
                {
                    directoryObjects.Add(item.Key, item.Value);
                }
            }
        }

        index = 1;
        foreach (var id in servicePrincipalIds.Distinct())
        {
            if (Guid.TryParse(id, out _))
            {
                var name = _configOptions.IsManual == true || _configOptions.IsMaskServicePrincipal == true
                    ? GetManualObjectName(id, index++, "Service Principal") : await GetServicePrincipalName(id);
                directoryObjects.Add(id, name); //TODO use batch
            }
        }

        foreach (var id in applicationIds.Distinct())
        {
            if (Guid.TryParse(id, out _))
            {
                string? name;
                if (FirstPartyApps.Apps.ContainsKey(id)) //1P apps may not be found by graph, use static list instead
                {
                    name = FirstPartyApps.Apps[id];
                }
                else
                {
                    name = _configOptions.IsManual == true || _configOptions.IsMaskApplication == true
                        ? GetManualObjectName(id, index++, "App") : await GetApplicationName(id);
                }
                directoryObjects.Add(id, name); //TODO use batch
            }
        }

        index = 1;
        foreach (var id in tenantIds.Distinct())
        {
            if (Guid.TryParse(id, out _))
            {
                var name = _configOptions.IsManual == true || _configOptions.IsMaskTenant == true
                    ? GetManualObjectName(id, index++, "Tenant") : await GetTenantName(id);
                directoryObjects.Add(id, name); //TODO use batch
            }
        }

        if (_configOptions.IsManual != true)
        {
            await AddAgreements(directoryObjects);
            await AddNamedLocations(directoryObjects);
        }
        return directoryObjects;
    }

    private IEnumerable<string> GetTenantIds(ConditionalAccessConditionSet conditions)
    {
        var tenantIds = new List<string>();
        var usersJson = Helper.GetConditionsUsersJson(conditions);
        if (usersJson.includeGuestsOrExternalUsers != null)
        {
            var externalTenants = usersJson.includeGuestsOrExternalUsers.externalTenants;
            AppendTenantIds(tenantIds, externalTenants);
        }

        if (usersJson.excludeGuestsOrExternalUsers != null)
        {
            var externalTenants = usersJson.excludeGuestsOrExternalUsers.externalTenants;
            AppendTenantIds(tenantIds, externalTenants);
        }
        return tenantIds;
    }

    private void AppendTenantIds(List<string> tenantIds, ExternalTenants externalTenants)
    {
        if (externalTenants != null && !string.IsNullOrEmpty(externalTenants.membershipKind))
        {
            if(externalTenants.members != null)
            {
                tenantIds.AddRange(externalTenants.members);
            }
        }
    }

    private async Task AddAgreements(StringDictionary directoryObjects)
    {
        var agreements = await _graph.IdentityGovernance.TermsOfUse.Agreements
                    .GetAsync();
        int index = 1;
        foreach (var ac in agreements.Value)
        {
            var name = _configOptions.IsMaskTermsOfUse == true
                ? GetManualObjectName(ac.Id, index++, "Terms of use") : ac.DisplayName;
            directoryObjects.Add(ac.Id, name);
        }
    }
    private async Task AddNamedLocations(StringDictionary directoryObjects)
    {
        var namedLocations = await _graph.Identity.ConditionalAccess.NamedLocations
                    .GetAsync();

        int index = 1;
        foreach (var ac in namedLocations.Value)
        {
            var name = _configOptions.IsMaskNamedLocation == true
                ? GetManualObjectName(ac.Id, index++, "Terms of use") : ac.DisplayName;
            directoryObjects.Add(ac.Id, ac.DisplayName);
        }
    }

    internal async Task<StringDictionary> GetAuthenticationContexts()
    {
        var authContextsGraph = await _graph.Identity.ConditionalAccess.AuthenticationContextClassReferences
                    .GetAsync();
        var authContexts = new StringDictionary();
        foreach (var ac in authContextsGraph.Value)
        {
            authContexts.Add(ac.Id, ac.DisplayName);
        }
        return authContexts;
    }

    public record TenantInformation(
        [property: JsonPropertyName("@odata.context")] string odatacontext,
        [property: JsonPropertyName("tenantId")] string tenantId,
        [property: JsonPropertyName("federationBrandName")] object federationBrandName,
        [property: JsonPropertyName("displayName")] string displayName,
        [property: JsonPropertyName("defaultDomainName")] string defaultDomainName
    );


}
