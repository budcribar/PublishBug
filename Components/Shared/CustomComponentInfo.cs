using ClientInterfaces;

namespace Components.Shared
{
    public class CustomComponentInfo : ICustomComponentInfo
    {
        public string html { get; set; }
        public string js { get; set; }
        public ITestContext testContext { get; set; }

        public CustomComponentInfo(string html, string js, ITestContext testContext)
        {
            this.html = html;
            this.js = js;
            this.testContext = testContext;
        }
    }
}
