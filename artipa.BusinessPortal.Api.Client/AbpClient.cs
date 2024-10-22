using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace artipa.BusinessPortal.Api.Client;

public class AbpClient
{
    private readonly HttpClient _httpClient;

    public AbpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Creates 3 test files in the server inbox
    /// </summary>
    public async Task CreateTestDataAsync()
    {
        await _httpClient.PostAsync($"{_httpClient.BaseAddress}abp/test", null);
    }

    /// <summary>
    /// Downloads list of files in the ABP server inbox
    /// </summary>
    /// <returns>List of file names</returns>
    public async Task<List<string>> DownloadListAsync()
    {
        var json = await _httpClient.GetStringAsync($"{_httpClient.BaseAddress}abp");

        return JsonConvert.DeserializeObject<List<string>>(json);
    }

    // <summary>
    /// Downloads list of files in the ABP server inbox
    /// </summary>
    /// <returns>List of file names</returns>
    public List<string> DownloadList()
    {
        var json = _httpClient.GetStringAsync($"{_httpClient.BaseAddress}abp").GetAwaiter().GetResult();

        return JsonConvert.DeserializeObject<List<string>>(json);
    }

    /// <summary>
    /// Download single file content from ABP server
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns>File byte array content</returns>
    public async Task<byte[]> DownloadFileAsync(string fileName)
    {
        var json = await _httpClient.GetStringAsync($"{_httpClient.BaseAddress}abp/{fileName}");

        return JsonConvert.DeserializeObject<byte[]>(json);
    }

    // <summary>
    /// Download single file content from ABP server
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns>File byte array content</returns>
    public byte[] DownloadFile(string fileName)
    {
        var json = _httpClient.GetStringAsync($"{_httpClient.BaseAddress}abp/{fileName}").GetAwaiter().GetResult();

        return JsonConvert.DeserializeObject<byte[]>(json);
    }

    /// <summary>
    /// Upload file to ABP server outbox
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="content"></param>
    public async Task UploadFileAsync(string fileName, byte[] content)
    {
        var json = JsonConvert.SerializeObject(new { Content = content, Name = fileName });
        var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
        await _httpClient.PostAsync($"{_httpClient.BaseAddress}abp", httpContent);
    }

    /// <summary>
    /// Upload file to ABP server outbox
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="content"></param>
    public void UploadFile(string fileName, byte[] content)
    {
        var json = JsonConvert.SerializeObject(new
        {
            Content = content,
            Name = fileName
        });
        var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
        _httpClient.PostAsync($"{_httpClient.BaseAddress}abp", httpContent).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Commit download of file (file will be moved into archive on the ABP server side)
    /// </summary>
    /// <param name="fileName"></param>
    public async Task CommitDownloadAsync(string fileName)
    {
        await _httpClient.DeleteAsync($"{_httpClient.BaseAddress}abp/{fileName}");
    }

    /// <summary>
    /// Commit download of file (file will be moved into archive on the ABP server side)
    /// </summary>
    /// <param name="fileName"></param>
    public void CommitDownload(string fileName)
    {
        _httpClient.DeleteAsync($"{_httpClient.BaseAddress}abp/{fileName}").GetAwaiter().GetResult();
    }

    /// <summary>
    /// Creates instance of ABPClient with API key authentication.
    /// (ABP server API supports also basic http  and client certificate authentication.)
    /// </summary>
    /// <param name="apiKey"></param>
    /// <param name="baseAddress"></param>
    /// <returns></returns>
    public static AbpClient CreateClientWithApiKey(string apiKey, string baseAddress=AbpClientDefaults.BaseAddress)
    {
        var httpClient = new HttpClient()
        {
            BaseAddress = new Uri(baseAddress)
        };

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        return new AbpClient(httpClient);
    }

    /// <summary>
    /// Creates instance of ABPClient with client certificate authentication.
    /// (ABP server API supports also basic http  and API key authentication.)
    /// </summary>
    /// <param name="cert"></param>
    /// <param name="baseAddress"></param>
    /// <param name="acceptAllServerCerts"></param>
    /// <returns></returns>
    public static AbpClient CreateClientWithClientCertificate(X509Certificate2 cert,
        string baseAddress = AbpClientDefaults.BaseAddress, bool acceptAllServerCerts = false)
    {
        var handler = new HttpClientHandler()
        {
            ClientCertificateOptions = ClientCertificateOption.Manual
        };
        handler.ClientCertificates.Add(cert);

        if(acceptAllServerCerts)
            handler.ServerCertificateCustomValidationCallback = (message, crt, chain, errors) => true;

        return new AbpClient(new HttpClient(handler)
        {
            BaseAddress = new Uri(baseAddress)
        });
    }

}
