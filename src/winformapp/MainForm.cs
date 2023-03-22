using Microsoft.Identity.Client;
using System.Diagnostics;
using IdPowerToys.PowerPointGenerator;
using System.Reflection;
using ConditionalAccessDocumenter.Properties;
using IdPowerToys.PowerPointGenerator.Graph;

namespace ConditionalAccessDocumenter
{
    public partial class MainForm : Form
    {
        private static bool loggedIn = false;
        private static readonly string[] scopes = { "Directory.Read.All", "Policy.Read.All", "Agreement.Read.All", "CrossTenantInformation.ReadBasic.All" };

        private const string Default_TenantId = "10407d69-1ba5-4bec-8ebe-9af2f0b9e06a";
        private const string Default_ClientId = "520aa3af-bd78-4631-8f87-d48d356940ed";

        public IPublicClientApplication PublicClientApp { get; private set; }
        private void InitializeAuth(string clientId, string tenantId)
        {

            if (tenantId == Default_TenantId) //Multi tenant app use /common endpoint
            {
                PublicClientApp = PublicClientApplicationBuilder.Create(clientId)
                        .WithRedirectUri("http://localhost")
                        .Build();
            }
            else
            {
                PublicClientApp = PublicClientApplicationBuilder.Create(clientId)
                        .WithRedirectUri("http://localhost")
                        .WithAuthority(AzureCloudInstance.AzurePublic, tenantId)
                        .Build();
            }
        }

        public MainForm()
        {
            InitializeComponent();
            progressBar.Visible = false;
            lblStatus.Visible = false;
            LoadSettings();
        }

        private async void btnSignIn_Click(object sender, EventArgs e)
        {
            try
            {
                if (!loggedIn)
                {
                    await DoLogin();
                }
                else
                {
                    await Logout();
                    btnSignIn.Text = "&Sign In";
                    loggedIn = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured. " + ex.Message, "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
            }
        }

        private async Task DoLogin()
        {
            InitializeAuth(txtClientId.Text, txtTenantId.Text);
            var authResult = await GetToken();
            lblUserProfileName.Text = authResult.Account.Username;

            btnSignIn.Text = "&Sign Out";
            loggedIn = true;
        }

        private async Task<string> GetAccessToken()
        {
            var token = await GetToken();
            return token.AccessToken;
        }

        private async Task<AuthenticationResult> GetToken()
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
                        .WithPrompt(Prompt.SelectAccount)
                        .ExecuteAsync();
                }
                catch (MsalException msalex)
                {
                    Debug.WriteLine($"Error Acquiring Token:{Environment.NewLine}{msalex}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error Acquiring Token Silently:{Environment.NewLine}{ex}");
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
            try
            {
                bool isManual = !string.IsNullOrEmpty(txtManualCaPolicy.Text);
                string token = string.Empty;
                var configOptions = new ConfigOptions();

                if (isManual)
                {
                    configOptions.IsManual = true;
                    configOptions.ConditionalAccessPolicyJson = txtManualCaPolicy.Text;
                }
                else if (!loggedIn)
                {
                    await DoLogin();
                    token = await GetAccessToken();
                }

                SaveSettings();
                progressBar.Visible = true;
                btnGenerate.Enabled = false;
                lblStatus.Visible = false;
                Application.DoEvents();

                var graphData = new GraphData(configOptions);

                if (configOptions.IsManual == true)
                {
                    await graphData.ImportPolicy();
                }
                else
                {
                    await graphData.CollectData(token);
                }

                Stream templateStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ConditionalAccessDocumenter.Assets.PolicyTemplate.pptx");


                var gen = new DocumentGenerator();
                var saveFilePath = Path.Combine(txtSaveFolderPath.Text, $"Conditional Access {DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")}.pptx");
                using (var stream = new FileStream(saveFilePath, FileMode.Create))
                {
                    gen.GeneratePowerPoint(graphData, templateStream, stream, configOptions);
                    stream.Position = 0;
                }
                progressBar.Visible = false;
                btnGenerate.Enabled = true;
                lblStatus.Visible = true;
                Application.DoEvents();

                OpenWithDefaultProgram(saveFilePath);
            }
            catch (Exception ex)
            {
                progressBar.Visible = false;
                btnGenerate.Enabled = true;
                Application.DoEvents();

                MessageBox.Show("An error occured. " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveSettings()
        {
            try
            {
                var settings = Settings.Default;
                settings.ClientId = txtClientId.Text;
                settings.TenantId = txtTenantId.Text;
                settings.SaveFolderPath = txtSaveFolderPath.Text;
                Settings.Default.Save();
            }
            catch { }
        }

        private void LoadSettings()
        {
            var settings = Settings.Default;
            if (string.IsNullOrEmpty(settings.ClientId) || string.IsNullOrEmpty(settings.TenantId))
            {
                SetDefaultSettings();
            }

            if (string.IsNullOrEmpty(settings.SaveFolderPath))
            {
                settings.SaveFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
            txtClientId.Text = settings.ClientId;
            txtTenantId.Text = settings.TenantId;
            txtSaveFolderPath.Text = settings.SaveFolderPath;
        }

        private void SetDefaultSettings()
        {
            var settings = Settings.Default;
            settings.ClientId = Default_ClientId;
            settings.TenantId = Default_TenantId;
            txtClientId.Text = settings.ClientId;
            txtTenantId.Text = settings.TenantId;
        }

        public static void OpenWithDefaultProgram(string path)
        {
            using Process fileopener = new Process();

            fileopener.StartInfo.FileName = "explorer";
            fileopener.StartInfo.Arguments = "\"" + path + "\"";
            fileopener.Start();
        }

        private void btnResetSettings_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you wish to reset to the default tenant settings?", "Change default settings", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                SetDefaultSettings();
            }
        }
    }
}