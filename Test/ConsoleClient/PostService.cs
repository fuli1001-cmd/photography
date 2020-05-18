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
            client.BaseAddress = new Uri("http://192.168.99.100:44310/");
            Client = client;
        }

        public async Task<List<PostViewModel>> GetPosts(string accessToken)
        {
            try
            {
                Client.SetBearerToken(accessToken);

                var response = await Client.GetAsync("/api/posts/hot?api-version=1.0");

                response.EnsureSuccessStatusCode();

                Console.WriteLine("成功");
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
