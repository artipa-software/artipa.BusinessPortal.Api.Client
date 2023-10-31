using System.Text;
using artipa.BusinessPortal.Api.Client;
using Microsoft.Extensions.DependencyInjection;

//Create DI container
var servicesProvider = BuildDi();

//Get AbpClient instance from DI container
var client = servicesProvider.GetRequiredService<AbpClient>();

//Download list of files
foreach (var fileName in await client.DownloadListAsync())
{
    Console.WriteLine(fileName);

    //Download file content
    var content = await client.DownloadFileAsync(fileName);
    
    Console.WriteLine(Encoding.ASCII.GetString(content));
    
    //Commit download of file (file will be moved into archive on the server side)
    await client.CommitDownloadAsync(fileName);
}

//Upload file
await client.UploadFileAsync("test.txt", Encoding.ASCII.GetBytes("Hello world!"));

return;


static IServiceProvider BuildDi()
{
    var services = new ServiceCollection();

    services.AddAbpClientWithApiKey("TEST-xxxx");

    return services.BuildServiceProvider();
}