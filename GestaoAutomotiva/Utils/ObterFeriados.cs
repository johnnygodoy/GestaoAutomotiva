namespace GestaoAutomotiva.Utils
{
    public static class DataUtil
    {
        public static List<DateTime> ObterFeriados(int ano) {
            var feriadosFixos = new List<DateTime>
            {
                new DateTime(ano, 1, 1),    // Confraternização Universal
                new DateTime(ano, 4, 21),   // Tiradentes
                new DateTime(ano, 5, 1),    // Dia do Trabalho
                new DateTime(ano, 7, 9),    // Revolução Constitucionalista (SP)
                new DateTime(ano, 9, 7),    // Independência
                new DateTime(ano, 10, 12),  // Nossa Senhora Aparecida
                new DateTime(ano, 11, 2),   // Finados
                new DateTime(ano, 11, 15),  // Proclamação da República
                new DateTime(ano, 12, 25),  // Natal
            };

            // Feriados móveis com base na Páscoa
            DateTime pascoa = CalcularPascoa(ano);
            var feriadosMoveis = new List<DateTime>
            {
                pascoa.AddDays(-48), // Segunda-feira de Carnaval
                pascoa.AddDays(-47), // Terça-feira de Carnaval
                pascoa.AddDays(-46), // Quarta-feira de Cinzas
                pascoa.AddDays(60),  // Corpus Christi
            };

            // Feriados municipais e pontos facultativos aplicáveis a qualquer ano
            var feriadosMunicipais = new List<DateTime>
            {
                new DateTime(ano, 11, 20), // Consciência Negra
                new DateTime(ano, 11, 30), // Aniversário de Franco da Rocha
                new DateTime(ano, 12, 8),  // Padroeira
                new DateTime(ano, 10, 28), // Servidor Público (facultativo nacional)
            };

            return feriadosFixos
                .Concat(feriadosMoveis)
                .Concat(feriadosMunicipais)
                .ToList();
        }

        public static DateTime CalcularPascoa(int ano) {
            int a = ano % 19;
            int b = ano / 100;
            int c = ano % 100;
            int d = b / 4;
            int e = b % 4;
            int f = (b + 8) / 25;
            int g = (b - f + 1) / 3;
            int h = (19 * a + b - d - g + 15) % 30;
            int i = c / 4;
            int k = c % 4;
            int l = (32 + 2 * e + 2 * i - h - k) % 7;
            int m = (a + 11 * h + 22 * l) / 451;
            int mes = (h + l - 7 * m + 114) / 31;
            int dia = ((h + l - 7 * m + 114) % 31) + 1;

            return new DateTime(ano, mes, dia);
        }
    }
}
