using System.Collections.Specialized;
using IdPowerToys.PowerPointGenerator.Infrastructure;
using Microsoft.Graph;
using Microsoft.Graph.Beta;

namespace IdPowerToys.PowerPointGenerator.Graph;

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
        var policies = await _graph.Policies.ConditionalAccessPolicies.GetAsync();

        //var policies = await _graph.Policies.ConditionalAccessPolicies.GetAsync((requestConfiguration) =>
        //{
        //    requestConfiguration.QueryParameters.Filter = "id eq 'dd0766c1-aee7-44c4-b764-f611d66f374b'";
        //    requestConfiguration.Headers.Add("ConsistencyLevel", "eventual");
        //});

        return policies?.Value;
    }

    private string GetManualObjectName(string id, int index, string prefix)
    {
        return $"{prefix} {index} ({Helper.GetShortId(id)})";
    }

    public async Task<StringDictionary> GetDirectoryObjectCache(ICollection<ConditionalAccessPolicy>? policies)
    {
        if (policies == null) { return new StringDictionary(); }
        var directoryObjects = new StringDictionary();
        var groupIds = new List<string>();
        var roleIds = new List<string>();

        var maskUser = _configOptions.IsManual == true || _configOptions.IsMaskUser == true ? "User" : null;
        var maskGroup = _configOptions.IsManual == true || _configOptions.IsMaskGroup == true ? "Group" : null;
        var maskApplication = _configOptions.IsManual == true || _configOptions.IsMaskApplication == true ? "App" : null;
        var maskTenant = _configOptions.IsManual == true || _configOptions.IsMaskTenant == true ? "Tenant" : null;
        var maskTermsOfUse = _configOptions.IsManual == true || _configOptions.IsMaskTermsOfUse == true ? "Terms of use" : null;
        var maskNamedLocation = _configOptions.IsManual == true || _configOptions.IsMaskNamedLocation == true ? "Named location" : null;

        Dictionary<string, GraphHelperBatch> dirObjects = new Dictionary<string, GraphHelperBatch>();
        foreach (var policy in policies)
        {
            var conditions = policy.Conditions;
            if (conditions != null)
            {
                AddDirObjects(dirObjects, conditions.Users?.IncludeUsers, BatchType.User, maskUser);
                AddDirObjects(dirObjects, conditions.Users?.ExcludeUsers, BatchType.User, maskUser);
                AddDirObjects(dirObjects, conditions.Users?.IncludeGroups, BatchType.Group, maskGroup);
                AddDirObjects(dirObjects, conditions.Users?.ExcludeGroups, BatchType.Group, maskGroup);
                AddDirObjects(dirObjects, conditions.Applications?.IncludeApplications, BatchType.App, maskApplication);
                AddDirObjects(dirObjects, conditions.Applications?.ExcludeApplications, BatchType.App, maskApplication);
                AddDirObjects(dirObjects, conditions.ClientApplications?.IncludeServicePrincipals, BatchType.App, maskApplication);
                AddDirObjects(dirObjects, conditions.ClientApplications?.ExcludeServicePrincipals, BatchType.App, maskApplication);
                AddDirObjects(dirObjects, GetTenantIds(conditions), BatchType.Tenant, maskTenant);

                if (conditions.Users?.IncludeRoles != null) { roleIds.AddRange(conditions.Users.IncludeRoles); }
                if (conditions.Users?.ExcludeRoles != null) { roleIds.AddRange(conditions.Users.ExcludeRoles); }
                if (conditions.Users?.IncludeGroups != null) { groupIds.AddRange(conditions.Users.IncludeGroups); }
                if (conditions.Users?.ExcludeGroups != null) { groupIds.AddRange(conditions.Users.ExcludeGroups); }

            }
        }

        if (_configOptions.IsManual != true)
        {
            await AddBatch(directoryObjects, dirObjects);
            await AddGroupsCount(directoryObjects, groupIds);
            await AddRoles(directoryObjects, roleIds);

            await AddAgreements(directoryObjects, maskTermsOfUse);
            await AddNamedLocations(directoryObjects, maskNamedLocation);
        }
        return directoryObjects;
    }

    private async Task AddRoles(StringDictionary directoryObjects, List<string> roleIds)
    {
        int index = 1;
        if (_configOptions.IsManual == true)
        {
            foreach (var id in roleIds.Distinct())
            {
                if (Guid.TryParse(id, out _))
                {
                    var name = GetManualObjectName(id, index++, "Role");
                    directoryObjects.Add(id, name);
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
    }

    private async Task AddBatch(StringDictionary directoryObjects, Dictionary<string, GraphHelperBatch> dirObjects)
    {
        var batch = new BatchRequestContentCollection(_graph);
        int index = 100;
        var batchItems = new Dictionary<string, GraphHelperBatch>();
        foreach (var obj in dirObjects.Values)
        {
            if (obj.MaskLabel != null)
            {
                directoryObjects.Add(obj.Id, GetManualObjectName(obj.Id, index++, obj.MaskLabel));
            }
            else
            {
                string? key = null;
                switch (obj.Type)
                {
                    case BatchType.User:
                        key = await batch.AddBatchRequestStepAsync(_graph.Users[obj.Id].ToGetRequestInformation()); break;
                    case BatchType.Group:
                        key = await batch.AddBatchRequestStepAsync(_graph.Groups[obj.Id].ToGetRequestInformation()); break;
                    case BatchType.App:
                        key = await batch.AddBatchRequestStepAsync(_graph.ServicePrincipals
                                            .ToGetRequestInformation((requestConfiguration) =>
                                            {
                                                requestConfiguration.QueryParameters.Filter = $"appid eq '{obj.Id}' or id eq '{obj.Id}'";
                                                requestConfiguration.Headers.Add("ConsistencyLevel", "eventual");
                                            }));
                        break;
                    case BatchType.Tenant:
                        key = await batch.AddBatchRequestStepAsync(_graph.TenantRelationships.FindTenantInformationByTenantIdWithTenantId(obj.Id).ToGetRequestInformation()); break;
                }
                if (key != null) batchItems.Add(key, obj);
            }
        }

        var responseBatch = await _graph.Batch.PostAsync(batch);
        foreach (string key in batchItems.Keys)
        {
            var obj = batchItems[key];
            var res = await responseBatch.GetResponseByIdAsync(key);
            if (res.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                directoryObjects.Add(obj.Id, GetDeletedLabel(obj));
            }
            else
            {
                string? name = null;
                switch (obj.Type)
                {
                    case BatchType.User:
                        var resUser = await responseBatch.GetResponseByIdAsync<User>(key); name = resUser.DisplayName;
                        break;
                    case BatchType.Group:
                        var resGroup = await responseBatch.GetResponseByIdAsync<Group>(key); name = resGroup.DisplayName;
                        break;
                    case BatchType.App:
                        if (FirstPartyApps.Apps.ContainsKey(obj.Id)) //1P apps may not be found by graph, use static list instead
                        {
                            name = FirstPartyApps.Apps[obj.Id];
                        }
                        else
                        {
                            var resSP = await responseBatch.GetResponseByIdAsync<ServicePrincipalCollectionResponse>(key);

                            var app = resSP?.Value;
                            if (app != null && app.Count > 0)
                            {
                                name = app[0].DisplayName;
                            }
                            else
                            {
                                name = GetDeletedLabel(obj);
                            }
                        }
                        break;
                    case BatchType.Tenant:
                        var resTenant = await responseBatch.GetResponseByIdAsync<TenantInformation>(key); name = resTenant.DisplayName;
                        break;
                }
                name = string.IsNullOrEmpty(name) ? Helper.GetShortId(obj.Id) : name;
                directoryObjects.Add(obj.Id, name);
            }
        }
    }

    private static string GetDeletedLabel(GraphHelperBatch obj)
    {
        return $"Deleted {obj.Type.ToString().ToLower()} {Helper.GetShortId(obj.Id)}";
    }

    private void AddDirObjects(Dictionary<string, GraphHelperBatch> dirObjects, List<string>? ids, BatchType type, string? mask)
    {
        if (ids == null) return;
        foreach (var id in ids)
        {
            if (!dirObjects.ContainsKey(id))
            {
                if (Guid.TryParse(id, out _))
                {
                    dirObjects.Add(id, new GraphHelperBatch() { Id = id, Type = type, MaskLabel = mask });
                }
            }
        }
    }

    private async Task AddGroupsCount(StringDictionary directoryObjects, List<string> ids)
    {
        if (_configOptions.IsManual == true || _configOptions.IsMaskGroup == true) return;

        var batch = new BatchRequestContentCollection(_graph);
        var batchIds = new StringDictionary();
        foreach (var id in ids.Distinct())
        {
            if (Guid.TryParse(id, out _))
            {
                var key = await batch.AddBatchRequestStepAsync(_graph.Groups[id].TransitiveMembers.ToGetRequestInformation((requestConfiguration) =>
                    {
                        requestConfiguration.QueryParameters.Count = true;
                        requestConfiguration.QueryParameters.Top = 1;
                        requestConfiguration.QueryParameters.Select = new string[] { "displayName" };
                        requestConfiguration.Headers.Add("ConsistencyLevel", "eventual");
                    }));
                batchIds.Add(key, id);
            }
        }

        var responseBatch = await _graph.Batch.PostAsync(batch);
        foreach (string key in batchIds.Keys)
        {
            var id = batchIds[key];
            var res = await responseBatch.GetResponseByIdAsync(key);
            if (res.IsSuccessStatusCode)
            {
                var item = await responseBatch.GetResponseByIdAsync<DirectoryObjectCollectionResponse>(key);
                if (item.OdataCount != null)
                {
                    directoryObjects[id] = directoryObjects[id] + $" ({item.OdataCount})";
                }
            }
        }
    }

    private List<string> GetTenantIds(ConditionalAccessConditionSet conditions)
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

    private async Task AddAgreements(StringDictionary directoryObjects, string? mask)
    {
        try
        {
            var agreements = await _graph.IdentityGovernance.TermsOfUse.Agreements.GetAsync();
            int index = 1;
            if (agreements?.Value != null)
            {
                foreach (var ac in agreements.Value)
                {
                    if (ac.Id != null)
                    {
                        var name = _configOptions.IsMaskTermsOfUse == true
                            ? GetManualObjectName(ac.Id, index++, mask) : ac.DisplayName;
                        directoryObjects.Add(ac.Id, name);
                    }
                }
            }
        }
        catch { }
    }
    private async Task AddNamedLocations(StringDictionary directoryObjects, string? mask)
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
                            ? GetManualObjectName(ac.Id, index++, mask) : ac.DisplayName;
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

            if (authContextsGraph?.Value != null)
            {
                foreach (var ac in authContextsGraph.Value)
                {
                    if (ac?.Id != null)
                    {
                        authContexts.Add(ac.Id, ac.DisplayName);
                    }
                }
            }
        }
        catch { }

        return authContexts;
    }

    enum BatchType
    {
        User,
        Group,
        Role,
        App,
        Tenant,
        GroupCount
    }
    class GraphHelperBatch
    {
        public string Id { get; set; }
        public BatchType Type { get; set; }
        public string? MaskLabel { get; set; }
    }
}