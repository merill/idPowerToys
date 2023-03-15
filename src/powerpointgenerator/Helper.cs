using CADocGen.PowerPointGenerator.PolicyViews;
using Microsoft.Graph;
using System.Collections.Specialized;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CADocGen.PowerPointGenerator;

public static class Helper
{

    public static string? GetObjectName(StringDictionary? cache, string id)
    {
        return GetObjectName(cache, id, null);
    }

    public static string? GetObjectName(StringDictionary? cache, string id, string? prefix)
    {
        string name;

        if (cache != null && cache.ContainsKey(id) && !string.IsNullOrEmpty(cache[id]))
        {
            name = cache[id];
        }
        else
        {
            if(Guid.TryParse(id, out _))
            {
                var shortId = GetShortId(id);
                name = prefix == null ? shortId : $"{prefix} ({shortId})";
            }
            else
            {
                name = id;
            }
        }
        

        return name;
    }

    public static void AppendObjectName(StringBuilder sb, StringDictionary? cache, string id)
    {
        sb.AppendLine($" - {GetObjectName(cache, id)}");
    }

    public static void AppendObjectNames(StringBuilder sb, StringDictionary? cache, IEnumerable<string> ids)
    {
        foreach (var id in ids)
        {
            AppendObjectName(sb, cache, id);
        }
    }

    public static string GetShortId(string id)
    {
        if (string.IsNullOrEmpty(id) || id.Length < 10) return id; //No need to shorten if it is already short

        var shortId = $"{id.Substring(0, 4)}...{id.Substring(id.Length - 4, 4)}";
        return shortId;
    }

    public static Users GetConditionsUsersJson(ConditionalAccessConditionSet conditions)
    {
        var conditionsJson = JsonSerializer.Serialize(conditions.Users, new JsonSerializerOptions { WriteIndented = true });
        var users = JsonSerializer.Deserialize<Users>(conditionsJson);
        return users;
    }

    public static GrantControls GetConditionsGrantsJson(ConditionalAccessGrantControls controls)
    {
        var conditionsJson = JsonSerializer.Serialize(controls, new JsonSerializerOptions { WriteIndented = true });
        var grantControls = JsonSerializer.Deserialize<GrantControls>(conditionsJson);
        return grantControls;
    }

    public static Applications GetConditionsApplicationsJson(ConditionalAccessApplications conditions)
    {
        var conditionsJson = JsonSerializer.Serialize(conditions, new JsonSerializerOptions { WriteIndented = true });
        var applications = JsonSerializer.Deserialize<Applications>(conditionsJson);
        return applications;
    }

    public static SessionControls GetSessionControlsJson(ConditionalAccessSessionControls caSessionControls)
    {
        var sessionControlsJson = JsonSerializer.Serialize(caSessionControls, new JsonSerializerOptions { WriteIndented = true });
        var sessionControls = JsonSerializer.Deserialize<SessionControls>(sessionControlsJson);
        return sessionControls;
    }
}

//TODO: Remove these once these are supported in the Graph C# SDK
public record ExcludeGuestsOrExternalUsers(
    [property: JsonPropertyName("guestOrExternalUserTypes")] string guestOrExternalUserTypes,
    [property: JsonPropertyName("externalTenants")] ExternalTenants externalTenants
);

public record ExternalTenants(
    [property: JsonPropertyName("@odata.type")] string odatatype,
    [property: JsonPropertyName("membershipKind")] string membershipKind,
    [property: JsonPropertyName("members")] IReadOnlyList<string> members
);

public record IncludeGuestsOrExternalUsers(
    [property: JsonPropertyName("guestOrExternalUserTypes")] string guestOrExternalUserTypes,
    [property: JsonPropertyName("externalTenants")] ExternalTenants externalTenants);

public record Users(
    [property: JsonPropertyName("includeGuestsOrExternalUsers")] IncludeGuestsOrExternalUsers includeGuestsOrExternalUsers,
    [property: JsonPropertyName("excludeGuestsOrExternalUsers")] ExcludeGuestsOrExternalUsers excludeGuestsOrExternalUsers
);

public record AuthenticationStrength(
    [property: JsonPropertyName("id")] string id,
    [property: JsonPropertyName("createdDateTime")] DateTime? createdDateTime,
    [property: JsonPropertyName("modifiedDateTime")] DateTime? modifiedDateTime,
    [property: JsonPropertyName("displayName")] string displayName,
    [property: JsonPropertyName("description")] string description,
    [property: JsonPropertyName("policyType")] string policyType,
    [property: JsonPropertyName("requirementsSatisfied")] string requirementsSatisfied,
    [property: JsonPropertyName("allowedCombinations")] IReadOnlyList<string> allowedCombinations
);

public record GrantControls(
    [property: JsonPropertyName("authenticationStrength@odata.context")] string authenticationStrengthodatacontext,
    [property: JsonPropertyName("authenticationStrength")] AuthenticationStrength authenticationStrength
);


public record ApplicationFilter(
    [property: JsonPropertyName("mode")] string mode,
    [property: JsonPropertyName("rule")] string rule
);

public record Applications(
    [property: JsonPropertyName("applicationFilter")] ApplicationFilter applicationFilter
);

// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
public record SessionControls(
    [property: JsonPropertyName("secureSignInSession")] SecureSignInSession secureSignInSession
);

public record SecureSignInSession(
    [property: JsonPropertyName("isEnabled")] bool isEnabled
);

