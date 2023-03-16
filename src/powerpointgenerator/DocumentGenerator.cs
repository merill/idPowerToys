using S=Syncfusion.Presentation;
using System.Text.Json;
using IdPowerToys.PowerPointGenerator.PolicyViews;

namespace IdPowerToys.PowerPointGenerator;

public class DocumentGenerator
{
    private GraphData _graphData;

    public void GeneratePowerPoint(GraphData graphData, string templateFilePath, Stream outputStream, ConfigOptions configOptions)
    {
        _graphData = graphData;
        var policies = _graphData.Policies;

        S.IPresentation pptxDoc = S.Presentation.Open(templateFilePath);

        SetTitleSlideInfo(pptxDoc.Slides[0]);
        var templateSlide = pptxDoc.Slides[1];

        if (configOptions.GroupSlidesByState == true)
        {            
            var enabledPolicies = from p in policies where p.State ==   ConditionalAccessPolicyState.Enabled select p;
            var disabledPolicies = from p in policies where p.State == ConditionalAccessPolicyState.Disabled select p;
            var reportOnlyPolicies = from p in policies where p.State == ConditionalAccessPolicyState.EnabledForReportingButNotEnforced select p;

            AddSlides(pptxDoc, policies, "Enabled Policies", ConditionalAccessPolicyState.Enabled);
            AddSlides(pptxDoc, policies, "Report-only Policies", ConditionalAccessPolicyState.EnabledForReportingButNotEnforced);
            AddSlides(pptxDoc, policies, "Disabled Policies", ConditionalAccessPolicyState.Disabled);
        }
        else
        {
            AddSlides(pptxDoc, policies, "Policies", null);
        }
        pptxDoc.Slides.Remove(templateSlide);
        pptxDoc.Save(outputStream);

        pptxDoc.Close();
    }

    private void AddSlides(S.IPresentation pptxDoc, ICollection<ConditionalAccessPolicy> policies, string? sectionTitle, ConditionalAccessPolicyState? policyState)
    {
        var filteredPolicies = policyState == null
            ? from p in policies orderby p.DisplayName select p
            : from p in policies where p.State == policyState orderby p.DisplayName select p;

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

    private void SetPolicySlideInfo(S.ISlide slide, ConditionalAccessPolicy policy, int index)
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
        ppt.Show(policy.State == ConditionalAccessPolicyState.Enabled, Shape.StateEnabled);
        ppt.Show(policy.State == ConditionalAccessPolicyState.Disabled, Shape.StateDisabled);
        ppt.Show(policy.State == ConditionalAccessPolicyState.EnabledForReportingButNotEnforced, Shape.StateReportOnly);
        string lastModified = GetLastModified(policy);
        ppt.SetText(Shape.LastModified, lastModified);

        ppt.SetText(Shape.UserWorkload, assignedUserWorkload.Name);
        ppt.SetTextFormatted(Shape.UserWorkloadIncExc, assignedUserWorkload.IncludeExclude);
        ppt.Show(assignedUserWorkload.IsWorkload, Shape.IconWorkloadIdentity);
        ppt.Show(!assignedUserWorkload.IsWorkload, Shape.IconGroupIdentity);
        ppt.Show(assignedUserWorkload.HasIncludeRoles, Shape.IconAssignedToRole);
        ppt.Show(assignedUserWorkload.HasIncludeExternalUser || assignedUserWorkload.HasIncludeExternalTenant, Shape.IconAssignedToGuest);

        ppt.SetText(Shape.CloudAppAction, assignedCloudAppAction.Name);
        ppt.SetTextFormatted(Shape.CloudAppActionIncExc, assignedCloudAppAction.IncludeExclude);
        ppt.Show(assignedCloudAppAction.HasData && !assignedCloudAppAction.IsSelectedAppO365Only, Shape.CloudAppActionIncExc);
        ppt.Show(assignedCloudAppAction.AccessType == AppAccessType.AppsAll,
            Shape.IconAccessAllCloudApps);
        ppt.Show(assignedCloudAppAction.AccessType == AppAccessType.AppsSelected && !assignedCloudAppAction.IsSelectedAppO365Only,
            Shape.IconAccessSelectedCloudApps);
        ppt.Show(assignedCloudAppAction.IsSelectedAppO365Only,
            Shape.IconAccessOffice365, Shape.PicAccessOffice365);
        ppt.Show(assignedCloudAppAction.AccessType == AppAccessType.UserActionsRegSecInfo,
            Shape.IconAccessMySecurityInfo, Shape.PicAccessSecurityInfo);
        ppt.Show(assignedCloudAppAction.AccessType == AppAccessType.UserActionsRegDevice,
            Shape.IconAccessRegisterOrJoinDevice, Shape.PicAccessRegisterDevice);
        ppt.Show(assignedCloudAppAction.AccessType == AppAccessType.AuthenticationContext,
            Shape.IconAccessAuthenticationContext);
        ppt.Show(assignedCloudAppAction.AccessType == AppAccessType.AppsNone,
            Shape.IconAccessAzureAD);


        if (conditionRisks.HasData) ppt.SetTextFormatted(Shape.Risks, conditionRisks.IncludeExclude);
        ppt.Show(!conditionRisks.HasData, Shape.ShadeRisk);

        if (conditionPlatforms.HasData) ppt.SetTextFormatted(Shape.Platforms, conditionPlatforms.IncludeExclude);
        ppt.Show(!conditionPlatforms.HasData, Shape.ShadeDevicePlatforms);

        if (conditionClientAppTypes.HasData) ppt.SetTextFormatted(Shape.ClientAppTypes, conditionClientAppTypes.IncludeExclude);
        ppt.Show(!conditionClientAppTypes.HasData, Shape.ShadeClientApps);

        if (conditionLocations.HasData) ppt.SetTextFormatted(Shape.Locations, conditionLocations.IncludeExclude);
        ppt.Show(!conditionLocations.HasData, Shape.ShadeLocations);

        if (conditionDeviceFilters.HasData) ppt.SetTextFormatted(Shape.DeviceFilters, conditionDeviceFilters.IncludeExclude);
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
            ppt.Show(policy.SessionControls.ContinuousAccessEvaluation.Mode == ContinuousAccessEvaluationMode.Disabled, Shape.IconSessionCaeDisable);
        }
        ppt.Show(!sessionControls.DisableResilienceDefaults, Shape.ShadeSessionDisableResilience);
        ppt.Show(!sessionControls.SecureSignInSession, Shape.ShadeSessionSecureSignIn);

        var json = JsonSerializer.Serialize(policy, new JsonSerializerOptions { WriteIndented = true });
        var notes = slide.AddNotesSlide();
        notes.NotesTextBody.AddParagraph(policyName);
        notes.NotesTextBody.AddParagraph("Portal link: " + GetPolicyPortalLink(policy));
        notes.NotesTextBody.AddParagraph(json);
    }

    private static string GetLastModified(ConditionalAccessPolicy policy)
    {
        const string dateLabel = "Last modified: ";
        const string dateFormat = "yyyy-MM-dd";
        string dateValue = policy.ModifiedDateTime.HasValue ? dateLabel + policy.ModifiedDateTime.Value.ToString(dateFormat) :
            policy.CreatedDateTime.HasValue ? dateLabel + policy.CreatedDateTime.Value.ToString(dateFormat) : string.Empty;

        return dateValue;
    }

    private string GetPolicyName(ConditionalAccessPolicy policy, int index, AssignedUserWorkload assignedUserWorkload, AssignedCloudAppAction assignedCloudAppAction, ConditionClientAppTypes conditionClientAppTypes, ConditionDeviceFilters conditionDeviceFilters, ConditionLocations conditionLocations, ConditionPlatforms conditionPlatforms, ConditionRisks conditionRisks, ControlGrantBlock grantControls, ControlSession sessionControls)
    {
        var sb = new StringBuilder("CA");
        sb.Append(index.ToString("000"));
        var grantBlock = grantControls.IsGrant ? "Grant" : "Block";
        sb.Append($"-{assignedUserWorkload.Name}-{assignedCloudAppAction.Name}{grantControls.Name}-{grantBlock}");
        return sb.ToString();
    }

    private void SetTitleSlideInfo(S.ISlide slide)
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

    private string GetPolicyPortalLink(ConditionalAccessPolicy policy)
    {
        return $"https://entra.microsoft.com/#view/Microsoft_AAD_ConditionalAccess/PolicyBlade/policyId/{policy.Id}\r\n";
    }
}
