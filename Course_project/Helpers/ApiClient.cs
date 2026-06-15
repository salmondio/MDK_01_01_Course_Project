using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace Course_project_wpf.Helpers
{
    public static class ApiClient
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public static void Initialize(string baseAddress)
        {
            _httpClient.BaseAddress = new Uri(baseAddress);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public static void SetAuthToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        public static async Task<HttpResponseMessage> PostAsync<T>(string endpoint, T data)
        {
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return await _httpClient.PostAsync(endpoint, content);
        }

        public static async Task<HttpResponseMessage> GetAsync(string endpoint)
        {
            try
            {
                return await _httpClient.GetAsync(endpoint);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: Не удалось выполнить Get-запрос. " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public static async Task<HttpResponseMessage> PutAsync<T>(string endpoint, T data)
        {
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return await _httpClient.PutAsync(endpoint, content);
        }

        public static async Task<HttpResponseMessage> DeleteAsync(string endpoint)
        {
            return await _httpClient.DeleteAsync(endpoint);
        }

        // PATCH-запрос без тела
        public static async Task<HttpResponseMessage> PatchAsync(string endpoint)
        {
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), endpoint);
            return await _httpClient.SendAsync(request);
        }

        // PATCH-запрос с телом
        public static async Task<HttpResponseMessage> PatchAsync<T>(string endpoint, T data)
        {
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), endpoint)
            {
                Content = content
            };
            return await _httpClient.SendAsync(request);
        }
    }
}