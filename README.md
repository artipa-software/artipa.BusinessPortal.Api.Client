# artipa.BusinessPortal.Api.Cleint
Is a NET Standard 2.0 component you can use to integrate with ABP (artipa.BusinessPortal) EDI service. 

You can integrate this component into your application (ERP system) sou you can diractly download and upload EDI interchanges from/to ABP server API (https://abpapi.artipa.cz) without need to work with external EDI client app and filesystem.

Component is targeted to NET Standard 2.0 so you can reference it from .NET Framework and NET Core as well.

For full feature examples please look into /Examples folder of project source code

For testing purpose use test account with APIKey=TEST-xxxx.
For production use you will need real account with real APIKey. In this case please contact us on support@artipa.com 


```c#
var apiKey = "TEST-xxxx";
```

If you use DI, add ABPClient into DI Contaniner

```c#
services.AddAbpClientWithApiKey(apiKey);
```

And then inject it into your classes or retrive it from container like this:

```c#
var client = servicesProvider.GetRequiredService<AbpClient>();
```

If you dont use DI, you can create ABPClient instace the following way:

```c#
//Create AbpClient instance with API key
var client = AbpClient.CreateClientWithApiKey("apiKey");
```

For testing purpose you can ask ABP server to put some test files into test inbox:

```c#
//Create some test files on the server
await client.CreateTestDataAsync();
```

Folowing code downloads all incomming files from ABP inbox and confirms the server succesfull download:

```c#
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
```

Last example shows how you can send EDI file into ABP server outbox

```c#
//Upload file
await client.UploadFileAsync("test.txt", Encoding.ASCII.GetBytes("Hello world!"));
```

support@artipa.com