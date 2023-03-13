using Microsoft.Graph;
using System.Text;

namespace CADocGen.PowerPointGenerator.PolicyViews;

public class ControlSession : PolicyView
{
    public bool UseAppEnforcedRestrictions { get; set; }
    public bool UseConditionalAccessAppControl { get; set; }
    public bool SignInFrequency { get; set; }
    public bool PersistentBrowserSession { get; set; }
    public bool ContinousAccessEvaluation { get; set; }
    public bool DisableResilienceDefaults { get; set; }
    public bool SecureSignInSession { get; set; }
    public string SignInFrequencyIntervalLabel { get; set; }
    public string CloudAppSecurityType { get; set; }
    public string PersistentBrowserSessionModeLabel { get; set; }
    public string ContinousAccessEvaluationModeLabel { get; set; }

    public ControlSession(ConditionalAccessPolicy policy, GraphData graphData) : base(policy, graphData)
    {
        if (Policy.SessionControls == null) { return; }

        UpdateProps();
    }

    private void UpdateProps()
    {
        var appEnforced = Policy.SessionControls.ApplicationEnforcedRestrictions;
        UseAppEnforcedRestrictions = appEnforced != null && appEnforced.IsEnabled.HasValue && appEnforced.IsEnabled.Value;

        var cloudAppSecurity = Policy.SessionControls.CloudAppSecurity;
        UseConditionalAccessAppControl = cloudAppSecurity != null && cloudAppSecurity.IsEnabled.HasValue && cloudAppSecurity.IsEnabled.Value;
        if (UseConditionalAccessAppControl)
        {
            switch (cloudAppSecurity.CloudAppSecurityType)
            {
                case CloudAppSecuritySessionControlType.MonitorOnly: CloudAppSecurityType = "Monitor cloud apps"; break;
                case CloudAppSecuritySessionControlType.BlockDownloads: CloudAppSecurityType = "Block downloads"; break;
                case CloudAppSecuritySessionControlType.McasConfigured: CloudAppSecurityType = "Use custom policy"; break;
            }
        }

        var signInFrequency = Policy.SessionControls.SignInFrequency;
        SignInFrequency = signInFrequency != null && signInFrequency.IsEnabled.HasValue && signInFrequency.IsEnabled.Value;
        if (SignInFrequency)
        {
            switch (signInFrequency.FrequencyInterval)
            {
                case SignInFrequencyInterval.EveryTime: SignInFrequencyIntervalLabel= "Every time"; break;
                case SignInFrequencyInterval.TimeBased: 
                    var frequency = signInFrequency.Type.ToString().ToLower();
                    if(signInFrequency.Value == 1 && frequency.Length > 2) //Remove last s to show singular
                    {
                        frequency = frequency[..^1];
                    }
                    SignInFrequencyIntervalLabel = $"{signInFrequency.Value} {frequency}"
                        .Replace("Days", "Day(s)").Replace("Hours", "Hour(s)"); break;
            }
        }

        var persistentBrowser = Policy.SessionControls.PersistentBrowser;
        PersistentBrowserSession = persistentBrowser != null && persistentBrowser.IsEnabled.HasValue && persistentBrowser.IsEnabled.Value;
        if (PersistentBrowserSession)
        {
            switch (persistentBrowser.Mode)
            {
                case PersistentBrowserSessionMode.Always: PersistentBrowserSessionModeLabel = "Always persistent"; break;
                case PersistentBrowserSessionMode.Never: PersistentBrowserSessionModeLabel = "Never persistent"; break;
            }
        }

        var cae = Policy.SessionControls.ContinuousAccessEvaluation;
        ContinousAccessEvaluation = cae != null;
        if (ContinousAccessEvaluation)
        {
            switch (cae.Mode)
            {
                case ContinuousAccessEvaluationMode.Disabled: ContinousAccessEvaluationModeLabel = "Disabled"; break;
                case ContinuousAccessEvaluationMode.StrictEnforcement: ContinousAccessEvaluationModeLabel = "Strictly enforce location policies"; break;
            }
        }

        var disableResilienceDefaults = Policy.SessionControls.DisableResilienceDefaults;
        DisableResilienceDefaults = disableResilienceDefaults != null && disableResilienceDefaults.HasValue && disableResilienceDefaults.Value;

        var sessionControlsJson = Helper.GetSessionControlsJson(Policy.SessionControls);

        if (sessionControlsJson != null && sessionControlsJson.secureSignInSession != null && sessionControlsJson.secureSignInSession.isEnabled)
        {
            SecureSignInSession = true;
        }
    }
}
