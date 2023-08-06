using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Core.Tests
{
    public static class WebapplcationFactoryExtensions
    {
        public static void Post<T>(this HttpClient client, string Uri, T input)
        { 
            var inputJson = JsonSerializer.Serialize(input);
            var stringContent = new StringContent(inputJson, Encoding.UTF8, "application/json");
        }
    }
}