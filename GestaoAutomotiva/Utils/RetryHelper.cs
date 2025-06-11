using Microsoft.EntityFrameworkCore;

namespace GestaoAutomotiva.Utils
{
    public static class RetryHelper
    {
        public static async Task<bool> TentarSalvarAsync(Func<Task> acao, int tentativas = 3, int delayMs = 200) {
            for (int i = 0; i < tentativas; i++)
            {
                try
                {
                    await acao();
                    return true;
                }
                catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("database is locked") == true)
                {
                    await Task.Delay(delayMs);
                }
            }
            return false;
        }
    }
}
