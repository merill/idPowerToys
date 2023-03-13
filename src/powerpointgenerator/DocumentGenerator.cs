using G = Microsoft.Graph;
using Syncfusion.Presentation;
using System.Text.Json;
using CADocGen.PowerPointGenerator.PolicyViews;
using IdPowerToys.PowerPointGenerator;
using System.Text;

namespace CADocGen.PowerPointGenerator;

public class DocumentGenerator
{
    private GraphData _graphData;

    public void GeneratePowerPoint(GraphData graphData, string templateFilePath, Stream outputStream, ConfigOptions configOptions)
    {
        _graphData = graphData;
        var policies = _graphData.Policies;

        IPresentation pptxDoc = Presentation.Open(templateFilePath);

        SetTitleSlideInfo(pptxDoc.Slides[0]);
        var templateSlide = pptxDoc.Slides[1];

        var enabledPolicies = from p in policies where p.State == G.ConditionalAccessPolicyState.Enabled select p;
        var disabledPolicies = from p in policies where p.State == G.ConditionalAccessPolicyState.Disabled select p;
        var reportOnlyPolicies = from p in policies where p.State == G.ConditionalAccessPolicyState.EnabledForReportingButNotEnforced select p;

        AddSlides(pptxDoc, policies, "Enabled Policies", G.ConditionalAccessPolicyState.Enabled);
        AddSlides(pptxDoc, policies, "Report-only Policies", G.ConditionalAccessPolicyState.EnabledForReportingButNotEnforced);
        AddSlides(pptxDoc, policies, "Disabled Policies", G.ConditionalAccessPolicyState.Disabled);

        pptxDoc.Slides.Remove(templateSlide);
        pptxDoc.Save(outputStream);

        pptxDoc.Close();
    }

    private void AddSlides(IPresentation pptxDoc, IEnumerable<G.ConditionalAccessPolicy> policies, string sectionTitle, G.ConditionalAccessPolicyState? policyState)
    {
        var filteredPolicies = from p in policies where p.State == policyState select p;
        var templateSlide = pptxDoc.Slides[1];

        if (filteredPolicies.Count() > 0)
        {
            var section = pptxDoc.Sections.Add();

            section.Name = sectionTitle;

            int index = 1;
            foreach (var policy in filteredPolicies)
            {
                var slide = templateSlide.Clone();

                SetPolicySlideInfo(slide, policy, index++);

                section.Slides.Add(slide);
            }
        }
    }

    private void SetPolicySlideInfo(ISlide slide, G.ConditionalAccessPolicy policy, int index)
    {
        var assignedUserWorkload = new AssignedUserWorkload(policy, _graphData);
        var assignedCloudAppAction = new AssignedCloudAppAction(policy, _graphData);

        var conditionClientAppTypes = new ConditionClientAppTypes(policy, _graphData);
        var conditionDeviceFilters = new ConditionDeviceFilters(policy, _graphData);
        var conditionLocations = new ConditionLocations(policy, _graphData);
        var conditionPlatforms = new ConditionPlatforms(policy, _graphData);
        var conditionRisks = new ConditionRisks(policy, _graphData);

        var grantControls = new ControlGrantBlock(policy, _graphData);
        var sessionControls = new ControlSession(policy, _graphData);

        var ppt = new PowerPointHelper(slide);

        var policyName = policy.DisplayName;
        if (_graphData.ConfigOptions.IsMaskPolicy == true)
        {
            policyName = GetPolicyName(policy, index, assignedUserWorkload, assignedCloudAppAction,
                conditionClientAppTypes, conditionDeviceFilters, conditionLocations,
                conditionPlatforms, conditionRisks, grantControls, sessionControls);
        }
        
        ppt.SetText(Shape.PolicyName, policyName);
        ppt.SetLink(Shape.PolicyName, GetPolicyPortalLink(policy));
        ppt.Show(policy.State == G.ConditionalAccessPolicyState.Enabled, Shape.StateEnabled);
        ppt.Show(policy.State == G.ConditionalAccessPolicyState.Disabled, Shape.StateDisabled);
        ppt.Show(policy.State == G.ConditionalAccessPolicyState.EnabledForReportingButNotEnforced, Shape.StateReportOnly);

        ppt.SetText(Shape.UserWorkload, assignedUserWorkload.Name);
        ppt.SetText(Shape.UserWorkloadIncExc, assignedUserWorkload.IncludeExclude);
        ppt.Show(assignedUserWorkload.IsWorkload, Shape.IconWorkloadIdentity);
        ppt.Show(!assignedUserWorkload.IsWorkload, Shape.IconGroupIdentity);
        ppt.Show(assignedUserWorkload.HasIncludeRoles, Shape.IconAssignedToRole);
        ppt.Show(assignedUserWorkload.HasIncludeExternalUser || assignedUserWorkload.HasIncludeExternalTenant, Shape.IconAssignedToGuest);

        ppt.SetText(Shape.CloudAppAction, assignedCloudAppAction.Name);
        ppt.SetText(Shape.CloudAppActionIncExc, assignedCloudAppAction.IncludeExclude);
        ppt.Show(assignedCloudAppAction.HasData && !assignedCloudAppAction.IsSelectedAppO365Only, Shape.CloudAppActionIncExc);
        ppt.Show(assignedCloudAppAction.AccessType == AccessType.AppsAll,
            Shape.IconAccessAllCloudApps);
        ppt.Show(assignedCloudAppAction.AccessType == AccessType.AppsSelected && !assignedCloudAppAction.IsSelectedAppO365Only,
            Shape.IconAccessSelectedCloudApps);
        ppt.Show(assignedCloudAppAction.IsSelectedAppO365Only,
            Shape.IconAccessOffice365, Shape.PicAccessOffice365);
        ppt.Show(assignedCloudAppAction.AccessType == AccessType.UserActionsRegSecInfo,
            Shape.IconAccessMySecurityInfo, Shape.PicAccessSecurityInfo);
        ppt.Show(assignedCloudAppAction.AccessType == AccessType.UserActionsRegDevice,
            Shape.IconAccessRegisterOrJoinDevice, Shape.PicAccessRegisterDevice);
        ppt.Show(assignedCloudAppAction.AccessType == AccessType.AuthenticationContext,
            Shape.IconAccessAuthenticationContext);
        ppt.Show(assignedCloudAppAction.AccessType == AccessType.AppsNone,
            Shape.IconAccessAzureAD);


        if (conditionRisks.HasData) ppt.SetText(Shape.Risks, conditionRisks.IncludeExclude);
        ppt.Show(!conditionRisks.HasData, Shape.ShadeRisk);

        if (conditionPlatforms.HasData) ppt.SetText(Shape.Platforms, conditionPlatforms.IncludeExclude);
        ppt.Show(!conditionPlatforms.HasData, Shape.ShadeDevicePlatforms);

        if (conditionClientAppTypes.HasData) ppt.SetText(Shape.ClientAppTypes, conditionClientAppTypes.IncludeExclude);
        ppt.Show(!conditionClientAppTypes.HasData, Shape.ShadeClientApps);

        if (conditionLocations.HasData) ppt.SetText(Shape.Locations, conditionLocations.IncludeExclude);
        ppt.Show(!conditionLocations.HasData, Shape.ShadeLocations);

        if (conditionDeviceFilters.HasData) ppt.SetText(Shape.DeviceFilters, conditionDeviceFilters.IncludeExclude);
        ppt.Show(!conditionDeviceFilters.HasData, Shape.ShadeFilterForDevices);


        ppt.SetText(Shape.IconGrantCustomAuthLabel, grantControls.CustomAuthenticationFactorName);
        ppt.SetText(Shape.IconGrantTermsOfUseLabel, grantControls.TermsOfUseName);
        ppt.SetText(Shape.IconGrantAuthenticationStrengthLabel, grantControls.AuthenticationStrengthName);
        ppt.Show(grantControls.IsGrant, Shape.IconGrantAccess, Shape.GrantLabelGrantAccess);
        ppt.Show(!grantControls.IsGrant, Shape.IconBlockAccess, Shape.GrantLabelBlockAccess);
        ppt.SetText(Shape.GrantRequireLabel,
            grantControls.GrantControlsCount > 1 && grantControls.IsGrantRequireOne ? "Require ONE" :
            grantControls.GrantControlsCount > 1 && grantControls.IsGrantRequireAll ? "Require ALL" : "");
        ppt.Show(grantControls.GrantControlsCount > 1, Shape.GrantRequireLabel);

        ppt.Show(!grantControls.ApprovedApplication, Shape.ShadeGrantApprovedClientApp);
        ppt.Show(!grantControls.TermsOfUse, Shape.ShadeGrantTermsOfUse);
        ppt.Show(!grantControls.CustomAuthenticationFactor, Shape.ShadeGrantCustomAuthFactor);
        ppt.Show(!grantControls.CompliantApplication, Shape.ShadeGrantApprovedClientApp);
        ppt.Show(!grantControls.CompliantDevice, Shape.ShadeGrantCompliantDevice);
        ppt.Show(!grantControls.AuthenticationStrength, Shape.ShadeGrantAuthStrength);
        ppt.Show(!grantControls.DomainJoinedDevice, Shape.ShadeGrantHybridAzureADJoined);
        ppt.Show(!grantControls.Mfa, Shape.ShadeGrantMultifactorAuth);
        ppt.Show(!grantControls.PasswordChange, Shape.ShadeGrantChangePassword);


        ppt.Show(!sessionControls.UseAppEnforcedRestrictions, Shape.ShadeSessionAppEnforced);
        ppt.Show(!sessionControls.UseConditionalAccessAppControl, Shape.ShadeSessionCas);
        ppt.SetText(Shape.SessionCasType, sessionControls.CloudAppSecurityType);
        ppt.Show(!sessionControls.SignInFrequency, Shape.ShadeSessionSif);
        ppt.SetText(Shape.SessionSifInterval, sessionControls.SignInFrequencyIntervalLabel);
        ppt.Show(!sessionControls.PersistentBrowserSession, Shape.ShadeSessionPersistentBrowser);
        ppt.SetText(Shape.SessionPersistenBrowserMode, sessionControls.PersistentBrowserSessionModeLabel);
        ppt.Show(!sessionControls.ContinousAccessEvaluation, Shape.ShadeSessionCae);
        if (sessionControls.ContinousAccessEvaluation)
        {
            ppt.SetText(Shape.SessionCaeMode, sessionControls.ContinousAccessEvaluationModeLabel);
            ppt.Show(policy.SessionControls.ContinuousAccessEvaluation.Mode == G.ContinuousAccessEvaluationMode.Disabled, Shape.IconSessionCaeDisable);
        }
        ppt.Show(!sessionControls.DisableResilienceDefaults, Shape.ShadeSessionDisableResilience);
        ppt.Show(!sessionControls.SecureSignInSession, Shape.ShadeSessionSecureSignIn);

        var json = JsonSerializer.Serialize(policy, new JsonSerializerOptions { WriteIndented = true });
        var notes = slide.AddNotesSlide();
        notes.NotesTextBody.AddParagraph(policyName);
        notes.NotesTextBody.AddParagraph("Portal link: " + GetPolicyPortalLink(policy));
        notes.NotesTextBody.AddParagraph(json);
    }

    private string GetPolicyName(G.ConditionalAccessPolicy policy, int index, AssignedUserWorkload assignedUserWorkload, AssignedCloudAppAction assignedCloudAppAction, ConditionClientAppTypes conditionClientAppTypes, ConditionDeviceFilters conditionDeviceFilters, ConditionLocations conditionLocations, ConditionPlatforms conditionPlatforms, ConditionRisks conditionRisks, ControlGrantBlock grantControls, ControlSession sessionControls)
    {
        var sb = new StringBuilder("CA");
        sb.Append(index.ToString("000"));
        var grantBlock = grantControls.IsGrant ? "Grant" : "Block";
        sb.Append($"-{assignedUserWorkload.Name}-{assignedCloudAppAction.Name}{grantControls.Name}-{grantBlock}");
        return sb.ToString();
    }

    private void SetTitleSlideInfo(ISlide slide)
    {
        var ppt = new PowerPointHelper(slide);
        if (_graphData.Organization != null && _graphData.Organization.Count > 0)
        {
            var org = _graphData.Organization.FirstOrDefault();
            ppt.SetText(Shape.TenantId, $"{org.Id}");
            ppt.SetText(Shape.TenantName, $"{org.DisplayName}");
            ppt.SetText(Shape.GeneratedBy, $"{_graphData.Me.DisplayName} ({_graphData.Me.UserPrincipalName})");
        }

        ppt.SetText(Shape.GenerationDate, DateTime.Now.ToString("dd MMM yyyy"));
    }

    private string GetPolicyPortalLink(G.ConditionalAccessPolicy policy)
    {
        return $"https://entra.microsoft.com/#view/Microsoft_AAD_ConditionalAccess/PolicyBlade/policyId/{policy.Id}";
    }
}
