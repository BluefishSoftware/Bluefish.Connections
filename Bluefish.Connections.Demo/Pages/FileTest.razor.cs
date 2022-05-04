using Bluefish.Connections.Demo.Extensions;
using Bluefish.Connections.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace Bluefish.Connections.Demo.Pages
{
    public partial class FileTest
    {
        private Model _model = new();

        [Inject]
        public IJSRuntime JSRuntime { get; set; } = null!;

        public class Model
        {
            public string ConnectionType { get; set; } = string.Empty;

            public string ConnectionSettings { get; set; } = string.Empty;

            public string ErrorMessage { get; set; } = string.Empty;

            public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

            public string Path { get; set; } = string.Empty;

            public string SuccessMessage { get; set; } = string.Empty;
        }

        private async Task OnDownloadFile()
        {
            try
            {
                _model.ErrorMessage = string.Empty;
                _model.SuccessMessage = string.Empty;
                // create connection
                var connection = _model.ConnectionType.InstantiateConnection<IFileConnection>(_model.ConnectionSettings);
                if (connection == null)
                {
                    throw new Exception($"Failed to instatiate type: {_model.ConnectionType}");
                }
                using var stream = await connection.GetFileAsync(_model.Path, default);
                using var streamRef = new DotNetStreamReference(stream: stream);
                var fileName = Path.GetFileName(_model.Path);
                await JSRuntime.InvokeVoidAsync("downloadFileFromStream", fileName, streamRef);
                _model.SuccessMessage = $"File '{fileName}' successfully downloaded";
            }
            catch (Exception ex)
            {
                _model.ErrorMessage = ex.Message;
            }
        }

        private async Task OnUploadFile(InputFileChangeEventArgs e)
        {
            try
            {
                _model.Path = Path.GetFileName(e.File.Name);
                _model.ErrorMessage = string.Empty;
                // create connection
                var connection = _model.ConnectionType.InstantiateConnection<IFileConnection>(_model.ConnectionSettings);
                if (connection == null)
                {
                    throw new Exception($"Failed to instatiate type: {_model.ConnectionType}");
                }
                using var stream = e.File.OpenReadStream();
                var success = await connection.PutFileAsync(_model.Path, stream, default);
                _model.SuccessMessage = $"File '{Path.GetFileName(_model.Path)}' successfully uploaded";
            }
            catch(Exception ex)
            {
                _model.ErrorMessage = ex.Message;
            }
        }


    }
}
