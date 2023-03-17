using Microsoft.Graph.Beta.Models;
using Microsoft.Identity.Client;
using System.Diagnostics;
using IdPowerToys.PowerPointGenerator;

namespace ConditionalAccessDocumenter
{
    public partial class MainForm : Form
    {
        private static bool loggeIn = false;
        private static readonly string[] scopes = { "Directory.Read.All", "Policy.Read.All", "Agreement.Read.All", "CrossTenantInformation.ReadBasic.All" };


        public IPublicClientApplication PublicClientApp { get; private set; }
        public AuthenticationResult AuthResult { get; private set; }
        private void InitializeAuth(string clientId, string tenantId)
        {
            PublicClientApp = PublicClientApplicationBuilder.Create(clientId)
                    .WithRedirectUri("http://localhost")
                    .WithAuthority(AzureCloudInstance.AzurePublic, tenantId)
                    .Build();
        }

        public MainForm()
        {
            InitializeComponent();
        }

        private async void btnSignIn_Click(object sender, EventArgs e)
        {
            if (!loggeIn)
            {
                InitializeAuth(txtClientId.Text, txtTenantId.Text);
                AuthResult = await Login();
                lblUserProfileName.Text = AuthResult.Account.Username;

                btnSignIn.Text = "&Sign Out";
                loggeIn = true;
            }
            else
            {
                await Logout();
                btnSignIn.Text = "&Sign In";
                loggeIn = false;
            }

        }

        private async Task<AuthenticationResult> Login()
        {
            AuthenticationResult authResult = null;
            var accounts = await PublicClientApp.GetAccountsAsync();
            var firstAccount = accounts.FirstOrDefault();

            try
            {
                authResult = await PublicClientApp.AcquireTokenSilent(scopes, firstAccount)
                    .ExecuteAsync();
            }
            catch (MsalUiRequiredException ex)
            {
                // A MsalUiRequiredException happened on AcquireTokenSilent.
                // This indicates you need to call AcquireTokenInteractive to acquire a token
                Debug.WriteLine($"MsalUiRequiredException: {ex.Message}");

                try
                {
                    authResult = await PublicClientApp.AcquireTokenInteractive(scopes)
                        .WithAccount(accounts.FirstOrDefault())
                        .WithPrompt(Microsoft.Identity.Client.Prompt.SelectAccount)
                        .ExecuteAsync();
                }
                catch (MsalException msalex)
                {
                    Debug.WriteLine($"Error Acquiring Token:{System.Environment.NewLine}{msalex}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error Acquiring Token Silently:{System.Environment.NewLine}{ex}");
            }
            return authResult;
        }

        private async Task Logout()
        {

            var accounts = await PublicClientApp.GetAccountsAsync();
            if (accounts.Any())
            {
                try
                {
                    await PublicClientApp.RemoveAsync(accounts.FirstOrDefault());
                    this.lblUserProfileName.Text = "";
                }
                catch (MsalException ex)
                {
                    throw new Exception($"Error signing-out user: {ex.Message}");
                }
            }
        }

        private async void btnGenerate_Click(object sender, EventArgs e)
        {
            var configOptions = new IdPowerToys.PowerPointGenerator.ConfigOptions();
            var graphData = new IdPowerToys.PowerPointGenerator.GraphData(configOptions);

            await graphData.CollectData(AuthResult.AccessToken);

            var templateFilePath = "F:\\Code\\IdPowerToys\\src\\winformapp\\Assets\\PolicyTemplate.pptx";

            var gen = new IdPowerToys.PowerPointGenerator.DocumentGenerator();
            var stream = new FileStream(txtSaveFilePath.Text, FileMode.Create);
            gen.GeneratePowerPoint(graphData, templateFilePath, stream, configOptions);
            stream.Position = 0;

        }
    }
}