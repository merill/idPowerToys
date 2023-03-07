using Microsoft.Graph;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace CADocGen.PowerPointGenerator;

public class GraphHelper
{
    private bool _isManualGeneration;
    GraphServiceClient? _graph;
    public GraphServiceClient GraphServiceClient { get { return _graph; } }

    /// <summary>
    /// Perform a manual generation without making Graph API calls
    /// </summary>
    public GraphHelper()
    {
        _isManualGeneration = true;
    }

    public GraphHelper(GraphServiceClient graphServiceClient)
    {
        _graph = graphServiceClient;
    }

    public async Task<User> GetMe()
    {
        var me = await _graph.Me
            .Request()
            .GetAsync();
        return me;
    }

    public async Task<ICollection<Organization>> GetOrganization()
    {
        var org = await _graph.Organization
            .Request()
            .GetAsync();
        return org;
    }

    public async Task<string?> GetTenantName(string tenantId)
    {
        try {
            var tenantInfo = await _graph.HttpProvider.SendAsync(new HttpRequestMessage(HttpMethod.Get, $"https://graph.microsoft.com/beta/tenantRelationships/findTenantInformationByTenantId(tenantId='{tenantId}')"));
            var info = await tenantInfo.Content.ReadFromJsonAsync<TenantInformation>();
            return info.displayName;
        }
        catch(Exception ex)
        {
            return null;
        }
    }

    public async Task<ICollection<ConditionalAccessPolicy>> GetPolicies()
    {
        var policies = await _graph.Identity.ConditionalAccess.Policies
            .Request()//.Filter("id eq '80f881c0-ab7c-426e-955d-9d48717d7659'")//.Top(10)
            .GetAsync();
        return policies;
    }

    private async Task<string?> GetUserName(string id)
    {
        try
        {
            var user = await _graph.Users[id]
                .Request()
                .GetAsync();
            return user.DisplayName;
        }
        catch (ServiceException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
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
            var user = await _graph.Groups[id]
                .Request()
                .GetAsync();
            return user.DisplayName;
        }
        catch (ServiceException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return Helper.GetShortId(id);
        }
    }

    private async Task<string?> GetServicePrincipalName(string id)
    {
        try
        {
            var sp = await _graph.ServicePrincipals[id]
                .Request()
                .GetAsync();
            return sp.DisplayName;
        }
        catch (ServiceException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return Helper.GetShortId(id);
        }
    }

    private async Task<string?> GetApplicationName(string id)
    {
        try
        {
            var sp = await _graph.Applications
                .Request().Filter($"appid eq '{id}'")
                .GetAsync();
            var app = sp.CurrentPage.FirstOrDefault();
            if (app == null)
            {
                return Helper.GetShortId(id);
            }
            else
            {
                return app.DisplayName;
            }
        }
        catch (ServiceException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
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
                var name = _isManualGeneration ? GetManualObjectName(id, index++, "User") : await GetUserName(id);
                directoryObjects.Add(id, name); //TODO use batch
            }
        }
        index = 1;
        foreach (var id in groupIds.Distinct())
        {
            if (Guid.TryParse(id, out _))
            {
                var name = _isManualGeneration ? GetManualObjectName(id, index++, "Group") : await GetGroupName(id);
                directoryObjects.Add(id, name); //TODO use batch
            }
        }

        if (_isManualGeneration)
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
                var name = _isManualGeneration ? GetManualObjectName(id, index++, "Service Principal") : await GetServicePrincipalName(id);
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
                    name = _isManualGeneration ? GetManualObjectName(id, index++, "App") : await GetApplicationName(id);
                }
                directoryObjects.Add(id, name); //TODO use batch
            }
        }

        index = 1;
        foreach (var id in tenantIds.Distinct())
        {
            if (Guid.TryParse(id, out _))
            {
                var name = _isManualGeneration ? GetManualObjectName(id, index++, "Tenant") : await GetTenantName(id);
                directoryObjects.Add(id, name); //TODO use batch
            }
        }

        if (!_isManualGeneration)
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
        var authContextsGraph = await _graph.IdentityGovernance.TermsOfUse.Agreements
                    .Request()
                    .GetAsync();
        foreach (var ac in authContextsGraph)
        {
            directoryObjects.Add(ac.Id, ac.DisplayName);
        }
    }
    private async Task AddNamedLocations(StringDictionary directoryObjects)
    {
        var authContextsGraph = await _graph.Identity.ConditionalAccess.NamedLocations
                    .Request()
                    .GetAsync();
        foreach (var ac in authContextsGraph)
        {
            directoryObjects.Add(ac.Id, ac.DisplayName);
        }
    }

    internal async Task<StringDictionary> GetAuthenticationContexts()
    {
        var authContextsGraph = await _graph.Identity.ConditionalAccess.AuthenticationContextClassReferences
                    .Request()
                    .GetAsync();
        var authContexts = new StringDictionary();
        foreach (var ac in authContextsGraph)
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
