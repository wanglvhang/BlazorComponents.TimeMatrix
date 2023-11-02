using Microsoft.JSInterop;

namespace BlazorComponents.TimeMatrix
{
    // This class provides an example of how JavaScript functionality can be wrapped
    // in a .NET class for easy consumption. The associated JavaScript module is
    // loaded on demand when first needed.
    //
    // This class can be registered as scoped DI service and then injected into Blazor
    // components for use.

    public class MatrixViewJsInterop : IAsyncDisposable
    {

        private readonly Lazy<Task<IJSObjectReference>> moduleTask;


        public MatrixViewJsInterop(IJSRuntime jsRuntime)
        {
            moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./_content/BlazorComponents.TimeMatrix/matrixViewJsInterop.js").AsTask());
        }


        //
        internal async Task<string>  DrawTimeMatrixAsync(string svgid, MatrixData matrixData)
        {
            var module = await moduleTask.Value;
            //´«µÝlist²ÎÊý
            return await module.InvokeAsync<string>("drawTimeMatrix",svgid, matrixData);
        }



        public async ValueTask DisposeAsync()
        {
            if (moduleTask.IsValueCreated)
            {
                var module = await moduleTask.Value;
                await module.DisposeAsync();
            }
        }


    }
}