using System.Collections.Specialized;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using Azure.Core;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Serialization.Json;

namespace IdPowerToys.PowerPointGenerator.Graph;

public class GraphData
{
    public List<ConditionalAccessPolicy>? Policies { get; set; }
    public StringDictionary? ObjectCache { get; set; }
    public StringDictionary? AuthenticationContexts { get; set; }
    public ICollection<Organization>? Organization { get; set; }
    public User? Me { get; set; }
    public ConfigOptions ConfigOptions { get; private set; }
    private GraphHelper _graphHelper;

    public GraphData(ConfigOptions configOptions) //Manual generation
    {
        ConfigOptions = configOptions;
        _graphHelper = new GraphHelper(configOptions);
    }

    public GraphData(ConfigOptions configOptions, string accessToken) //Web API call
    {
        ConfigOptions = configOptions;
        var graphClient = GetGraphClientUsingAccessToken(accessToken);
        _graphHelper = new GraphHelper(graphClient, configOptions);
    }

    public GraphData(ConfigOptions configOptions, GraphHelper graphHelper) //Desktop app
    {
        ConfigOptions = configOptions;
        _graphHelper = graphHelper;
    }

    public async Task CollectData()
    {
        if (ConfigOptions.IsManual == true)
        {
            _graphHelper = new GraphHelper(ConfigOptions);
            SetPolicyFromJson(ConfigOptions.ConditionalAccessPolicyJson);
        }
        else
        {
            //TODO: Batch and call in parallel to improve perf
            Me = await _graphHelper.GetMe();
            Organization = await _graphHelper.GetOrganization();
            Policies = await _graphHelper.GetPolicies();
            AuthenticationContexts = await _graphHelper.GetAuthenticationContexts();
        }
        ObjectCache = await _graphHelper.GetDirectoryObjectCache(Policies);
    }

    private GraphServiceClient GetGraphClientUsingAccessToken(string accessToken)
    {
        var tokenProvider = new TokenProvider();
        tokenProvider.AccessToken = accessToken;
        var accessTokenProvider = new BaseBearerTokenAuthenticationProvider(tokenProvider);

        var graphClient = new GraphServiceClient(accessTokenProvider, "https://graph.microsoft.com/beta");
        return graphClient;
    }

    public void SetPolicyFromJson(string? caPolicyJson)
    {
        if (caPolicyJson == null)
        {
            throw new Exception("Conditional Access Policy Json was not provided.");
        }

        var jsonRootElement = JsonDocument.Parse(ConfigOptions.ConditionalAccessPolicyJson).RootElement;
        var collectionElement = jsonRootElement.GetProperty("value");
        var jsonParseNode = new JsonParseNode(collectionElement);
        Policies = jsonParseNode.GetCollectionOfObjectValues<ConditionalAccessPolicy>(ConditionalAccessPolicy.CreateFromDiscriminatorValue).ToList();
    }

    public string GetJsonFromPolicy(ConditionalAccessPolicy policy)
    {
        //Use standard serialization for now.
        return JsonSerializer.Serialize<ConditionalAccessPolicy>(policy, new JsonSerializerOptions { WriteIndented = true });
        
        //var seralizer = _graphHelper.GraphServiceClient.RequestAdapter.SerializationWriterFactory.GetSerializationWriter("application/json");
        //seralizer.WriteObjectValue(string.Empty, policy);
        //var serializedContent = seralizer.GetSerializedContent();

        //using (var sr = new StreamReader(serializedContent))
        //{
        //    return sr.ReadToEnd();
        //}
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