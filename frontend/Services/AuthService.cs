using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace frontend.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly Blazored.LocalStorage.ILocalStorageService _localStorage;
        public AuthService(HttpClient httpClient, Blazored.LocalStorage.ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            var response = await _httpClient.PostAsJsonAsync("User/login", new { Username = username, Password = password });
            if (!response.IsSuccessStatusCode)
                return false;

            var json = await response.Content.ReadFromJsonAsync<LoginResult>();
            if (json != null && !string.IsNullOrEmpty(json.token))
            {
                // Guardar el token en localStorage
                await _localStorage.SetItemAsync("jwt_token", json.token);

                // Decodificar el JWT para extraer los roles
                var roles = json.roles;
                Console.WriteLine(roles);
                if (roles != null)
                {
                    await _localStorage.SetItemAsync("user_roles", roles);
                }

                return true;
            }
            return false;
        }


        private class LoginResult
        {
            public string? token { get; set; }

            public List<String>? roles { get; set; }
        }
    }
}
