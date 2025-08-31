using System;

namespace frontend.Services
{
    public class AuthStateService
    {
    public event Action? OnChange;
    public bool IsAuthenticated { get; private set; }
    public string? Username { get; private set; }

        public void SetAuthenticated(string username)
        {
            IsAuthenticated = true;
            Username = username;
            NotifyStateChanged();
        }

        public void Logout()
        {
            Console.WriteLine("LOGOUT");
            IsAuthenticated = false;
            Username = string.Empty;
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
