using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using System.Web;

namespace webApi.Models
{
    public class User
    {
        public int UserID { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 3)]
        required public string Username { get; set; }
        [Required]
        [EmailAddress]
        required public string Email { get; set; }

        [Required]
        [PasswordPropertyText]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "Contraseña de al menos 8 carácteres")]
        required public String Password { get; set; }

        [NotMapped]
        public String? Role { get; set; }

        // Relación con roles
        public ICollection<Role> Roles { get; set; } = new List<Role>();

        public void Sanitize()
        {
            Username = SanitizeInput(Username);
            Email = SanitizeInput(Email);
        }

        private string SanitizeInput(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // Remover completamente cualquier HTML/JavaScript
            input = Regex.Replace(input, @"<[^>]*(>|$)", string.Empty); // Tags HTML
            input = Regex.Replace(input, @"on\w+=", string.Empty, RegexOptions.IgnoreCase); // Eventos
            input = Regex.Replace(input, @"javascript:", string.Empty, RegexOptions.IgnoreCase); // JS
            input = Regex.Replace(input, @"vbscript:", string.Empty, RegexOptions.IgnoreCase); // VBS

            // 4. Prevención específica para SQL Injection
            input = PreventSqlInjection(input);

            // HTML encode para caracteres especiales
            return HttpUtility.HtmlEncode(input).Trim();
        }

        private string PreventSqlInjection(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // Lista de patrones peligrosos de SQL (más completa)
            var sqlPatterns = new[]
            {
        @"\bDROP\s+TABLE\b",
        @"\bDELETE\s+FROM\b",
        @"\bINSERT\s+INTO\b",
        @"\bUPDATE\b",
        @"\bSET\b",
        @"\bUNION\s+SELECT\b",
        @"\bEXEC(\s+|\()",
        @"\bEXECUTE(\s+|\()",
        @"\bXP_CMDSHELL\b",
        @"\bWAITFOR\s+DELAY\b",
        @"\bSLEEP\s*\(",
        @"\bBENCHMARK\s*\(",
        @"\bOR\s*['""]?1['""]?=['""]?1['""]?",
        @"\bAND\s*['""]?1['""]?=['""]?1['""]?",
        @"\bSELECT\b.*\bFROM\b",
        @"\bDECLARE\b",
        @"\bCAST\b",
        @"\bCONVERT\b",
        @"\bEXEC\b",
        @"\bEXECUTE\b",
        @"\bTRUNCATE\b",
        @"\bALTER\b",
        @"\bCREATE\b",
        @"\bDROP\b",
        @"\bSHUTDOWN\b"
    };

            foreach (var pattern in sqlPatterns)
            {
                input = Regex.Replace(input, pattern, string.Empty,
                    RegexOptions.IgnoreCase);
            }

            // Remover caracteres peligrosos individuales ANTES del HTML encode
            input = input.Replace("'", "");    // Remover comillas simples
            input = input.Replace("\"", "");   // Remover comillas dobles
            input = input.Replace(";", "");    // Remover punto y coma
            input = input.Replace("--", "");   // Remover comentarios SQL
            input = input.Replace("/*", "");   // Remover comentarios de bloque
            input = input.Replace("*/", "");   // Remover comentarios de bloque
            input = input.Replace("=", "");    // Remover operadores de igualdad

            return input;
        }
    }
}