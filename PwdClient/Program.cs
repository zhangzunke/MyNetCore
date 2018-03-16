using System;
using System.Net.Http;
using IdentityModel;
using IdentityModel.Client;

namespace PwdClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var diso = DiscoveryClient.GetAsync("http://localhost:5000").Result;
            if (diso.IsError)
            {
                Console.WriteLine(diso.Error);
            }
            var tokenClient = new TokenClient(diso.TokenEndpoint, "pwdClient", "secret");
            var tokenResponse = tokenClient.RequestResourceOwnerPasswordAsync("mike", "123qwe", "api").Result;
            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
            }
            else
            {
                Console.WriteLine(tokenResponse.Json);
            }
            var httpClicent = new HttpClient();
            httpClicent.SetBearerToken(tokenResponse.AccessToken);
            var response = httpClicent.GetAsync("http://localhost:5001/api/values").Result;
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.Content.ReadAsStringAsync().Result);
            }
            Console.ReadLine();
        }
    }
}
