using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.DependencyInjection;

namespace artipa.BusinessPortal.Api.Client;

public static class Extensions
{
    public static void AddAbpClientWithApiKey(this IServiceCollection services, string apikey,
        string baseAddress = AbpClientDefaults.BaseAddress)
    {
        services.AddHttpClient<AbpClient>(client =>
        {
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apikey);
        });
    }

    public static void AddAbpClientWithClientCert(this IServiceCollection services, X509Certificate2 cert,
        string baseAddress = AbpClientDefaults.BaseAddress, bool acceptAllServerCerts = false)
    {
        services.AddHttpClient<AbpClient>(client => { client.BaseAddress = new Uri(baseAddress); })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new HttpClientHandler();
                handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                handler.ClientCertificates.Add(cert);

                if(acceptAllServerCerts)
                    handler.ServerCertificateCustomValidationCallback = (message, crt, chain, errors) => true;

                return handler;
            });
    }


}
