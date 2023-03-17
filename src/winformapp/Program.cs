using Microsoft.Extensions.Configuration;

namespace ConditionalAccessDocumenter
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.

            var key = "";
#if DEBUG
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            IConfiguration configuration = configurationBuilder.AddUserSecrets<MainForm>().Build();
            key = configuration.GetSection("Syncfusion")["LicenseKey"];
#endif
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(key);

            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }
}