using System.Globalization;
using System.Text;

namespace GestaoAutomotiva.Utils
{
    public static class TextoHelper
    {
        public static string RemoverCaracteresEspeciais(string texto) {
            if (string.IsNullOrWhiteSpace(texto))
                return string.Empty;

            var normalized = texto.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in normalized)
            {
                var unicode = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicode != UnicodeCategory.NonSpacingMark && c != '"' && c != '\'' && c != '\\')
                    sb.Append(c);
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
