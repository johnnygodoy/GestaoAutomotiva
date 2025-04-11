using System;

namespace GestaoAutomotiva.Utils
{
    public class LicencaService
    {
        private readonly IConfiguration _config;

        public LicencaService(IConfiguration config) {
            _config = config;
        }

        public bool EstaEmModoTeste => _config.GetValue<bool>("Licenca:ModoTeste");

        public bool LicencaExpirada
        {
            get
            {
                var dataExpiracao = _config.GetValue<DateTime>("Licenca:DataExpiracao");
                return DateTime.Now.Date > dataExpiracao;
            }
        }

        public string MensagemLicenca() {
            if (!EstaEmModoTeste) return string.Empty;

            return LicencaExpirada
                ? "⚠️ Licença de teste expirada! Entre em contato para ativação."
                : $"🧪 Modo Teste ativo até {_config.GetValue<DateTime>("Licenca:DataExpiracao"):dd/MM/yyyy}.";
        }


    }
}
