using ConsoleClient.Models;
using IdentityModel.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleClient
{
    public class PostService
    {
        public HttpClient Client { get; }

        public PostService(HttpClient client)
        {
            client.BaseAddress = new Uri("https://localhost:5001/");
            Client = client;
        }

        public async Task<List<PostViewModel>> GetPosts(string accessToken)
        {
            try
            {
                Client.SetBearerToken(accessToken);

                var response = await Client.GetAsync("/api/posts/followed?api-version=1.0");

                response.EnsureSuccessStatusCode();

                return null;
                //return JsonConvert.DeserializeObject<List<PostViewModel>>(await response.Content.ReadAsStringAsync());
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
