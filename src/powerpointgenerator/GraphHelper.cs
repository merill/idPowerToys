using Azure.Core;
using Microsoft.Graph.Beta.Models.ODataErrors;
using System.Collections.Specialized;
using System.Text.Json.Serialization;

namespace IdPowerToys.PowerPointGenerator;

public class GraphHelper
{
    private ConfigOptions _configOptions;
    GraphServiceClient _graph;
    public GraphServiceClient GraphServiceClient { get { return _graph; } }

    /// <summary>
    /// Perform a manual generation without making Graph API calls
    /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public GraphHelper(ConfigOptions configOptions)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        _configOptions = configOptions;
    }

    public GraphHelper(GraphServiceClient graphServiceClient, ConfigOptions configOptions) : this(configOptions)
    {
        _graph = graphServiceClient;
    }

    public async Task<User?> GetMe()
    {
        try
        {
            var me = await _graph.Me.GetAsync();
            return me;
        }
        catch { return null; }
    }


    public async Task<List<Organization>?> GetOrganization()
    {
        try
        {
            var org = await _graph.Organization.GetAsync();
            return org?.Value;
        }
        catch { return null; }
    }

    public async Task<string?> GetTenantName(string tenantId)
    {
        try 
        {
            var tenantInfo = await _graph.TenantRelationships.FindTenantInformationByTenantIdWithTenantId(tenantId).GetAsync();
            return tenantInfo?.DisplayName;
        }
        catch { return null; }
    }

    public async Task<List<ConditionalAccessPolicy>?> GetPolicies()
    {
        try
        {
            var policies = await _graph.Identity.ConditionalAccess.Policies.GetAsync();

            //var policies = await _graph.Policies.ConditionalAccessPolicies.GetAsync((requestConfiguration) =>
            //{
            //    requestConfiguration.QueryParameters.Filter = "id eq 'dd0766c1-aee7-44c4-b764-f611d66f374b'";
            //    requestConfiguration.Headers.Add("ConsistencyLevel", "eventual");
            //});

            return policies?.Value;
        }
        catch { return null; }
    }

    private async Task<string?> GetUserName(string id)
    {
        try
        {
            var user = await _graph.Users[id].GetAsync();
            return user?.DisplayName;
        }
        catch (ODataError err) when (err.ResponseStatusCode == 404)
        {
            return $"Deleted user {Helper.GetShortId(id)}";
        }
        catch
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
            return user?.DisplayName;
        }
        catch (ODataError err) when (err.ResponseStatusCode == 404)
        {
            return $"Deleted group {Helper.GetShortId(id)}";
        }
        catch 
        {
            return Helper.GetShortId(id);
        }
    }

    private async Task<long?> GetGroupCount(string id)
    {
        try
        {
            var count = await _graph.Groups[id].TransitiveMembers.GetAsync((requestConfiguration) =>
            {
                requestConfiguration.QueryParameters.Count = true;
                requestConfiguration.QueryParameters.Top = 1;
                requestConfiguration.Headers.Add("ConsistencyLevel", "eventual");
            });
            return count?.OdataCount;
        }
        catch
        { 
            return null;
        }
    }

    private async Task<string?> GetAppName(string id)
    {
        try
        {
            var sp = await _graph.ServicePrincipals
                .GetAsync((requestConfiguration) =>
                {
                    requestConfiguration.QueryParameters.Filter = $"appid eq '{id}' or id eq '{id}'";
                    requestConfiguration.Headers.Add("ConsistencyLevel", "eventual");
                });
            var app = sp?.Value;
            if (app != null && app.Count > 0)
            {
                return app[0].DisplayName;
            }
            else
            {
                return $"Deleted app {Helper.GetShortId(id)}";
            }
        }
        catch
        {
            return Helper.GetShortId(id);
        }
    }

    public async Task<StringDictionary> GetDirectoryObjectCache(ICollection<ConditionalAccessPolicy>? policies)
    {
        if(policies == null) { return new StringDictionary(); }
        var directoryObjects = new StringDictionary();
        var userIds = new List<string>();
        var groupIds = new List<string>();
        var roleIds = new List<string>();
        var servicePrincipalIds = new List<string>();
        var applicationIds = new List<string>();
        var tenantIds = new List<string>();

        foreach (var policy in policies)
        {
            var conditions = policy.Conditions;
            if (conditions != null)
            {
                var users = conditions.Users;
                if (users != null)
                {
                    if (users.IncludeUsers != null) { userIds.AddRange(users.IncludeUsers); }
                    if (users.ExcludeUsers != null) { userIds.AddRange(users.ExcludeUsers); }
                    if (users.IncludeGroups != null) { groupIds.AddRange(users.IncludeGroups); }
                    if (users.ExcludeGroups != null) { groupIds.AddRange(users.ExcludeGroups); }
                    if (users.IncludeRoles != null) { roleIds.AddRange(users.IncludeRoles); }
                    if (users.ExcludeRoles != null) { roleIds.AddRange(users.ExcludeRoles); }
                }
                var apps = conditions.Applications;
                if (apps != null)
                {
                    if (apps.IncludeApplications != null) { applicationIds.AddRange(apps.IncludeApplications); }
                    if (apps.ExcludeApplications != null) { applicationIds.AddRange(apps.ExcludeApplications); }
                }
                var clientApps = conditions.ClientApplications;
                if(clientApps != null)
                {
                    if (clientApps.IncludeServicePrincipals != null) { servicePrincipalIds.AddRange(clientApps.IncludeServicePrincipals); }
                    if (clientApps.ExcludeServicePrincipals != null) { servicePrincipalIds.AddRange(clientApps.ExcludeServicePrincipals); }
                }

                tenantIds.AddRange(GetTenantIds(conditions));
            }
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
                string? name;
                if(_configOptions.IsManual == true || _configOptions.IsMaskGroup == true)
                {
                    name = GetManualObjectName(id, index++, "Group");
                }
                else
                {
                    name = await GetGroupName(id);
                    var count = await GetGroupCount(id);
                    if(count.HasValue) { name = $"{name} ({count})"; }
                }
                     
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
        var appsp = servicePrincipalIds.Concat(applicationIds).Distinct();
        foreach(var id in appsp)
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
                        ? GetManualObjectName(id, index++, "App") : await GetAppName(id);
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
        var users = conditions.Users;
        if (users == null) return tenantIds;

        AppendTenantIds(tenantIds, users.IncludeGuestsOrExternalUsers);
        AppendTenantIds(tenantIds, users.ExcludeGuestsOrExternalUsers);

        return tenantIds;
    }

    private void AppendTenantIds(List<string> tenantIds, ConditionalAccessGuestsOrExternalUsers? guestsOrExternalUsers)
    {

        if (guestsOrExternalUsers != null && guestsOrExternalUsers.ExternalTenants != null)
        {
            switch (guestsOrExternalUsers.ExternalTenants.MembershipKind)
            {
                case ConditionalAccessExternalTenantsMembershipKind.All:
                    break;

                case ConditionalAccessExternalTenantsMembershipKind.Enumerated:
                    var externalTenants = (ConditionalAccessEnumeratedExternalTenants)guestsOrExternalUsers.ExternalTenants;
                    if (externalTenants != null && externalTenants.Members != null) { tenantIds.AddRange(externalTenants.Members); }
                    break;
            }
        }
    }

    private async Task AddAgreements(StringDictionary directoryObjects)
    {
        try
        {
            var agreements = await _graph.IdentityGovernance.TermsOfUse.Agreements.GetAsync();
            int index = 1;
            if (agreements?.Value != null)
            {
                foreach (var ac in agreements.Value)
                {
                    if(ac.Id != null)
                    {
                        var name = _configOptions.IsMaskTermsOfUse == true
                            ? GetManualObjectName(ac.Id, index++, "Terms of use") : ac.DisplayName;
                        directoryObjects.Add(ac.Id, name);
                    }
                }
            }
        }
        catch { }
    }
    private async Task AddNamedLocations(StringDictionary directoryObjects)
    {
        try
        {
            var namedLocations = await _graph.Identity.ConditionalAccess.NamedLocations.GetAsync();

            int index = 1;
            if (namedLocations?.Value != null)
            {
                foreach (var ac in namedLocations.Value)
                {
                    if (ac?.Id != null)
                    {
                        var name = _configOptions.IsMaskNamedLocation == true
                            ? GetManualObjectName(ac.Id, index++, "Terms of use") : ac.DisplayName;
                        directoryObjects.Add(ac.Id, ac.DisplayName);
                    }
                }
            }
        }
        catch { }
    }

    internal async Task<StringDictionary> GetAuthenticationContexts()
    {
        var authContexts = new StringDictionary();
        try
        {
            var authContextsGraph = await _graph.Identity.ConditionalAccess.AuthenticationContextClassReferences.GetAsync();
            
            if(authContextsGraph?.Value != null)
            {
                foreach (var ac in authContextsGraph.Value)
                {
                    if(ac?.Id != null)
                    {
                        authContexts.Add(ac.Id, ac.DisplayName);
                    }
                }
            }
        }
        catch { }

        return authContexts;
    }
}