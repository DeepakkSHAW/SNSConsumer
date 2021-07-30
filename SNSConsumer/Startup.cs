using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;


[assembly: FunctionsStartup(typeof(SNSConsumer.Startup))]
namespace SNSConsumer
{
    public interface IRepository
    {
        string GetData();
    }
    public class Repository : IRepository
    {
        public string GetData()
        {
            return $"DK Test - {DateTime.Now.ToString()}!";
        }
    }
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IRepository, Repository>();
            
            //// Basic HTTP client factory usage
            builder.Services.AddHttpClient();
            //var v1 = Environment.GetEnvironmentVariable("SquareUpURL");
            //var v2 = Environment.GetEnvironmentVariable("SquareUpURLToken");

            ////// Named HTTP Client
            builder.Services.AddHttpClient("SquareUp", s =>
            {
                s.BaseAddress = new Uri(Environment.GetEnvironmentVariable("SquareUpURL"));
                s.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Environment.GetEnvironmentVariable("SquareUpURLToken"));
                s.DefaultRequestHeaders.Add("Square-Version", "2021-06-16");
                s.DefaultRequestHeaders.Add(HttpMethod.Post.ToString(), "Post");

            });
        }
    }
}
