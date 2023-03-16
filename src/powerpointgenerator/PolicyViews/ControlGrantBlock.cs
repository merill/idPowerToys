namespace IdPowerToys.PowerPointGenerator.PolicyViews;

public class ControlGrantBlock : PolicyView
{
    public bool IsGrant { get; set; }

    public bool ApprovedApplication { get; set; }
    public bool TermsOfUse { get; set; }
    public bool CustomAuthenticationFactor { get; set; }
    public bool CompliantApplication { get; set; }
    public bool CompliantDevice { get; set; }
    public bool DomainJoinedDevice { get; set; }
    public bool Mfa { get; set; }
    public bool PasswordChange { get; set; }
    public bool AuthenticationStrength { get; set; }
    public bool IsGrantRequireAll { get; set; }
    public bool IsGrantRequireOne { get; set; }        
    public int GrantControlsCount { get; set; }
    public string CustomAuthenticationFactorName { get; set; }
    public string TermsOfUseName { get; set; }
    public string AuthenticationStrengthName { get; set; }

    public ControlGrantBlock(ConditionalAccessPolicy policy, GraphData graphData) : base(policy, graphData)
    {
        IsGrant = true; //Default to grant and only change to Block if it is explicitly set.

        if (Policy.GrantControls != null)
        {
            IsGrant = !(from p in Policy.GrantControls.BuiltInControls where p == ConditionalAccessGrantControl.Block select p).Any();

            IncludeExclude = GetIncludes();
        }
    }

    private string GetIncludes()
    {
        var sb = new StringBuilder();
        var grantControls = Policy.GrantControls;
        IsGrantRequireAll = grantControls.Operator == "AND";
        IsGrantRequireOne = grantControls.Operator == "OR";
        GrantControlsCount = 0;

        if (grantControls.BuiltInControls.Any())
        {
            foreach (var control in grantControls.BuiltInControls)
            {
                switch (control)
                {
                    case ConditionalAccessGrantControl.ApprovedApplication:
                        Name += "-Approved App";
                        ApprovedApplication = true;
                        GrantControlsCount++;
                        break;
                    case ConditionalAccessGrantControl.Block: break; //Block is already shown in header
                    case ConditionalAccessGrantControl.CompliantApplication:
                        Name += "-Compliant App";
                        CompliantApplication = true;
                        GrantControlsCount++;
                        break;
                    case ConditionalAccessGrantControl.CompliantDevice:
                        Name += "-Compliant Device";
                        CompliantDevice = true;
                        GrantControlsCount++; 
                        break;
                    case ConditionalAccessGrantControl.DomainJoinedDevice:
                        Name += "-HAADJ";
                        DomainJoinedDevice = true;
                        GrantControlsCount++; 
                        break;
                    case ConditionalAccessGrantControl.Mfa:
                        Name += "-MFA";
                        Mfa = true;
                        GrantControlsCount++; 
                        break;
                    case ConditionalAccessGrantControl.PasswordChange:
                        Name += "-Password Change";
                        PasswordChange = true;
                        GrantControlsCount++; 
                        break;
                }
            }
        }
        if (grantControls.CustomAuthenticationFactors.Any())
        {
            Name += "-3PMFA";
            CustomAuthenticationFactor = true;
            bool isFirst = true;
            foreach (var caf in grantControls.CustomAuthenticationFactors)
            {
                if (!isFirst) { CustomAuthenticationFactorName += ", "; isFirst = false; }
                CustomAuthenticationFactorName += caf;
                GrantControlsCount++;
            }
        }
        if (grantControls.TermsOfUse.Any())
        {
            Name += "-ToU";
            TermsOfUse = true;
            bool isFirst = true;
            foreach (var tou in grantControls.TermsOfUse)
            {
                if (!isFirst) { TermsOfUseName += ", "; isFirst = false; }
                TermsOfUseName += Helper.GetObjectName(GraphData.ObjectCache, tou, "Terms of use");
                GrantControlsCount++;
            }
        }

        var grantControlsJson = Helper.GetConditionsGrantsJson(Policy.GrantControls);
        
        if (grantControlsJson != null && grantControlsJson.authenticationStrength != null)
        {
            Name += "-MFA Strength";
            AuthenticationStrength = true;
            AuthenticationStrengthName = $"Auth strength:{grantControlsJson.authenticationStrength.displayName}";
        }

        return sb.ToString();
    }
}
