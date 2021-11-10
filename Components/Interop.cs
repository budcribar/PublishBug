using Microsoft.JSInterop;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Components
{

    public static class Interop
    {
        public static ValueTask<bool> HasInternetConnection(IJSRuntime js)
        {
            return js.InvokeAsync<bool>("hasInternetConnection");
        }

        public async static Task SaveAs(IJSRuntime js, string filename, string data)
        {
            await js.InvokeAsync<object>(
                "saveAsFile",
                filename,
                Convert.ToBase64String(Encoding.UTF8.GetBytes(data)));
        }
    }
}
