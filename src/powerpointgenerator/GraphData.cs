using IdPowerToys.PowerPointGenerator;
using Microsoft.Graph;
using Microsoft.Graph.CallRecords;
using System.Collections.Specialized;
using System.Net.Http.Headers;
using System.Text.Json.Nodes;

namespace CADocGen.PowerPointGenerator;

public class GraphData
{
    public ICollection<ConditionalAccessPolicy>? Policies { get; set; }
    public StringDictionary? ObjectCache { get; set; }
    public StringDictionary? AuthenticationContexts { get; set; }
    public ICollection<Organization>? Organization { get; set; }
    public User? Me { get; set; }


    public async Task CollectData(string accessToken, ConfigOptions configOptions)
    {
        var graphClient = new GraphServiceClient("https://graph.microsoft.com/beta",
            new DelegateAuthenticationProvider(async (requestMessage) =>
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", accessToken);
            }));

        var graphHelper = new GraphHelper(graphClient, configOptions);

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

    public async Task ImportPolicy(ConfigOptions configOptions)
    {
        JsonNode rootNode = JsonNode.Parse(configOptions.ConditionalAccessPolicyJson)!;
        JsonNode valueNode = rootNode!["value"]!;
        var policyJson = valueNode.ToString();
        Policies = new Serializer().DeserializeObject<List<ConditionalAccessPolicy>>(policyJson);

        var graph = new GraphHelper(configOptions);
        ObjectCache = await graph.GetDirectoryObjectCache(Policies);
    }
}
