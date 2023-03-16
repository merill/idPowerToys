using Microsoft.Kiota.Abstractions.Authentication;
using System.Collections.Specialized;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace IdPowerToys.PowerPointGenerator;

public class GraphData
{
    public ICollection<ConditionalAccessPolicy>? Policies { get; set; }
    public StringDictionary? ObjectCache { get; set; }
    public StringDictionary? AuthenticationContexts { get; set; }
    public ICollection<Organization>? Organization { get; set; }
    public User? Me { get; set; }
    public ConfigOptions ConfigOptions { get; private set; }

    public GraphData(ConfigOptions configOptions)
    {
        ConfigOptions = configOptions;
    }

    public async Task CollectData(string accessToken)
    {
        var tokenProvider = new TokenProvider();
        tokenProvider.AccessToken= accessToken;
        var accessTokenProvider = new BaseBearerTokenAuthenticationProvider(tokenProvider);

        var graphClient = new GraphServiceClient(accessTokenProvider, "https://graph.microsoft.com/beta");

        var graphHelper = new GraphHelper(graphClient, ConfigOptions);

        await CollectData(graphHelper);
    }
    public async Task CollectData(GraphHelper graph)
    {
        //TODO: Batch and call in parallel to improve perf
        Me = await graph.GetMe();
        Organization = await graph.GetOrganization();
        Policies = await graph.GetPolicies();
        ObjectCache = await graph.GetDirectoryObjectCache(Policies);
        AuthenticationContexts = await graph.GetAuthenticationContexts();
    }

    public async Task ImportPolicy()
    {
        if (ConfigOptions.ConditionalAccessPolicyJson == null) return;

        JsonNode rootNode = JsonNode.Parse(ConfigOptions.ConditionalAccessPolicyJson)!;
        JsonNode valueNode = rootNode!["value"]!;
        var policyJson = valueNode.ToString();
        Policies = JsonSerializer.Deserialize<List<ConditionalAccessPolicy>>(policyJson);

        var graph = new GraphHelper(ConfigOptions);
        ObjectCache = await graph.GetDirectoryObjectCache(Policies);
    }
}


public class TokenProvider : IAccessTokenProvider
{
    public string? AccessToken { get; set; }
    
    public Task<string> GetAuthorizationTokenAsync(Uri uri, Dictionary<string, object>? additionalAuthenticationContext = default,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(AccessToken ?? string.Empty);
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Supressing since this is part of interface.
    public AllowedHostsValidator AllowedHostsValidator { get; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. 
}