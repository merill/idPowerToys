using Microsoft.Graph;

namespace IdPowerToys.PowerPointGenerator;

public static class DirectoryRoles
{
    //We use a static class and variable so we only need to load this once per application instance (to avoid having to call Graph for each page).
    //TODO Add a timer cache to refresh this every 24 hours or so.
    private static Dictionary<string, string>? _directoryRolesList;

    public static async Task<Dictionary<string, string>?> GetRoles(GraphServiceClient graph)
    {
        try
        {
            if (_directoryRolesList == null)
            {
                var directoryRoles = await graph.DirectoryRoleTemplates
                    .GetAsync();

                var directoryRolesList = new Dictionary<string, string>();

                foreach (var role in directoryRoles.Value)
                {
                    directoryRolesList.Add(role.Id, role.DisplayName);
                }
                _directoryRolesList = directoryRolesList;
            }
            return _directoryRolesList;
        }
        catch (ServiceException ex) when (ex.IsMatch(GraphErrorCode.ItemNotFound.ToString()))
        {
            return null; //role not found (most probably deleted user in ca policy)
        }
    }
}
