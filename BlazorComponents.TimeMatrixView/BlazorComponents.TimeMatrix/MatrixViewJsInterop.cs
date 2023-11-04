using Microsoft.JSInterop;
using System;

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


        //绘制matrix
        internal async Task DrawTimeMatrixAsync(string svgid, MatrixData matrixData)
        {
            var module = await moduleTask.Value;
            //传递list参数
            await module.InvokeVoidAsync("drawTimeMatrix",svgid, matrixData);
        }

        
        internal async Task UnselectCell(int cell_val)
        {
            var module = await moduleTask.Value;
            //传递list参数
            await module.InvokeVoidAsync("unselectCell",cell_val);
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