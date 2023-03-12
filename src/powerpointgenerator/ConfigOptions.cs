using System;
namespace IdPowerToys.PowerPointGenerator
{
    public class ConfigOptions
    {
        public string? ConditionalAccessPolicyJson { get; set; }
        public bool? IsManual { get; set; }
        public bool? IsMaskPolicy { get; set; }
        public bool? IsMaskGroup { get; set; }
        public bool? IsMaskUser { get; set; }
        public bool? IsMaskServicePrincipal { get; set; }
        public bool? IsMaskApplication { get; set; }
        public bool? IsMaskTenant { get; set; }
        public bool? IsMaskTermsOfUse { get; set; }
        public bool? IsMaskNamedLocation { get; set; }
    }
}