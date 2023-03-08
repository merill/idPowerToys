namespace webapi.Models
{
    public class GeneratePowerPointManualRequest
    {
        public string ConditionalAccessPolicyJson { get; set; }
        public bool? IsManual { get; set; }
    }
}
