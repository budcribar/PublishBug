namespace Interfaces
{
    public class PrivacyDisclaimer
    {
        public string Level { get; set; }
        public string Source { get; set; }
        public bool Accepted { get; set; }
        public string[] Comments { get; set; }
    }
}
