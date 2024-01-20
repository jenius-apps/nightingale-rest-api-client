using Nightingale.Core.Storage.Interfaces;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nightingale.Core.Services
{
    public class OneDriveSyncClient : ICloudBackupClient
    {
        private const string _backupFileUrl = "https://graph.microsoft.com/v1.0/me/drive/special/approot:/nightingaleBackup.json";
        private const string _backupFileContentUrl = _backupFileUrl + ":/content";
        private readonly string[] _scopes = new string[] { "user.read", "files.readwrite.appfolder" };
        private readonly HttpClient _httpClient;
        private readonly IStorageImporter _storageImporter;
        private readonly IStorageExporter _storageExporter;

        public OneDriveSyncClient(
            HttpClient httpClient,
            IStorageImporter storageImporter,
            IStorageExporter storageExporter)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _storageImporter = storageImporter ?? throw new ArgumentNullException(nameof(storageImporter));
            _storageExporter = storageExporter ?? throw new ArgumentNullException(nameof(storageExporter));
        }

        public async Task UploadBackupAsync(string accessToken, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new ArgumentNullException(nameof(accessToken));
            }

            string backupContent = await _storageExporter.ExportAsync();
            await UploadBackupContentAsync(backupContent, accessToken, ct);
        }

        public async Task RestoreBackupAsync(string accessToken, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new ArgumentNullException(nameof(accessToken));
            }

            string backupContent = await DownloadBackupContentAsync(accessToken, ct);
            await _storageImporter.ImportAsync(backupContent);
        }

        private async Task<string> DownloadBackupContentAsync(string accessToken, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new ArgumentNullException(nameof(accessToken));
            }

            using (var msg = new HttpRequestMessage(HttpMethod.Get, _backupFileContentUrl))
            {
                msg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                HttpResponseMessage httpResponse = await _httpClient.SendAsync(msg, ct);

                if (httpResponse.IsSuccessStatusCode && httpResponse.Content.Headers.ContentType.ToString().Contains("application/json"))
                {
                    string responseString = await httpResponse.Content.ReadAsStringAsync();
                    return responseString;
                }
                else
                {
                    throw new Exception($"{httpResponse.StatusCode} {httpResponse.ReasonPhrase}");
                }
            }
        }

        private async Task UploadBackupContentAsync(string content, string accessToken, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(content) || string.IsNullOrWhiteSpace(accessToken))
            {
                throw new ArgumentNullException($"{nameof(content)} {nameof(accessToken)}");
            }

            using (var msg = new HttpRequestMessage(HttpMethod.Put, _backupFileContentUrl))
            {
                msg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                msg.Content = new StringContent(content, Encoding.UTF8, "application/json");
                HttpResponseMessage httpResponse = await _httpClient.SendAsync(msg, ct);

                if (!httpResponse.IsSuccessStatusCode)
                {
                    throw new Exception($"{httpResponse.StatusCode} {httpResponse.ReasonPhrase}");
                }
            }
        }

        public string[] GetScopes()
        {
            return _scopes;
        }
    }
}
