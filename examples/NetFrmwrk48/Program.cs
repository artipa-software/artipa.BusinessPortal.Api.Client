using System;
using System.Text;
using System.Threading.Tasks;
using artipa.BusinessPortal.Api.Client;

namespace NetFrmwrk48
{
    internal abstract class Program
    {
        public static async Task Main(string[] args)
        {
            //Create AbpClient instance with API key
            var client = AbpClient.CreateClientWithApiKey("TEST-xxxx");

            //Create some test files on the server
            await client.CreateTestDataAsync();
            
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
        }
    }
}