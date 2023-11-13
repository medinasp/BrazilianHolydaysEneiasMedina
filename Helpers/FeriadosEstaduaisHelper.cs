using System.Reflection;

namespace BrazilianHolidaysEneiasMedina.Helpers
{
    /// <summary>
    /// Esta classe fornece métodos para importar feriados estaduais de uma planilha no formato CSV. A primera linha da
    /// planilha sempre é desprezada, pois deve conter o header da planilha.
    /// </summary>
    /// <remarks>A planilha deve conter um header com 4 colunas para DIA MÊS UF EVENTO, e as colunas devem ser separadas
    /// pelo caracter ';'.</remarks>
    public static class FeriadosEstaduaisHelper
    {
        /// <summary>
        /// Este método importa de uma planilha CSV, as informações sobre feriados estaduais.
        /// </summary>
        /// <param name="anoFeriado">Ano com 4 digitos numéricos, para o qual esta sendo importado os feriados.</param>
        /// <param name="abrangencia">String que representa a abrangência dos feriados. Default "Estadual".</param>
        /// <param name="pais">Sigla do país com 2 letras. Default "BR".</param>
        /// <returns>Lista contendo os feriados estaduais.</returns>
        public static List<FeriadoCelebrado> ImportarFeriadosEstaduais(int anoFeriado, string abrangencia = "Estadual", string pais = "BR")
        {
            List<FeriadoCelebrado> feriadosEstaduais = null;
            string[,] planilha = null;
            string feriadosEstaduaisResourceName = "BrazilianHolidaysEneiasMedina.assets.FeriadosEstaduaisBr.csv";

            try
            {
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(feriadosEstaduaisResourceName))
                using (StreamReader reader = new StreamReader(stream))
                {
                    string header = reader.ReadLine();
                    Console.WriteLine($"Cabeçalho do CSV: {header}");

                    int numeroDeLinhas = 1;

                    while (!reader.EndOfStream)
                    {
                        reader.ReadLine();
                        numeroDeLinhas++;
                    }

                    stream.Position = 0;
                    reader.DiscardBufferedData();

                    planilha = new string[numeroDeLinhas, 4];

                    int indice = 0;

                    while (!reader.EndOfStream)
                    {
                        string linha = reader.ReadLine();
                        string[] campos = linha.Split(';');

                        planilha[indice, 0] = campos[0];
                        planilha[indice, 1] = campos[1];
                        planilha[indice, 2] = campos[2];
                        planilha[indice, 3] = campos[3];

                        indice++;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao ler o recurso incorporado: {ex.Message}");
            }

            if (planilha != null && planilha.GetLength(0) > 0)
            {
                const int coluna1 = 0;
                const int coluna2 = 1;
                const int coluna3 = 2;
                const int coluna4 = 3;
                FeriadoCelebrado feriado;
                feriadosEstaduais = new List<FeriadoCelebrado>();

                if (anoFeriado < 1900)
                {
                    anoFeriado = DateTime.Now.Year;
                }

                for (int linha = 1, totalDeLinhas = planilha.GetLength(0); linha < totalDeLinhas; linha++)
                {
                    if (!string.IsNullOrEmpty(planilha[linha, coluna1]) &&
                        !string.IsNullOrEmpty(planilha[linha, coluna2]) &&
                        !string.IsNullOrEmpty(planilha[linha, coluna3]) &&
                        !string.IsNullOrEmpty(planilha[linha, coluna4]))
                    {
                        int dia = string.IsNullOrEmpty(planilha[linha, coluna1]) ? 1 : int.Parse((planilha[linha, coluna1]).Trim());
                        int mes = string.IsNullOrEmpty(planilha[linha, coluna2]) ? 1 : int.Parse((planilha[linha, coluna2]).Trim());
                        string estado = string.IsNullOrEmpty(planilha[linha, coluna3]) ? "" : (planilha[linha, coluna3]).Trim().ToUpper();
                        string evento = string.IsNullOrEmpty(planilha[linha, coluna4]) ? "" : (planilha[linha, coluna4]).Trim();

                        if (!string.IsNullOrEmpty(evento))
                        {
                            DateTime dataFeriado = new DateTime(anoFeriado, mes, dia);

                            var codigoFederal = "";
                            var codigoIbge = "";
                            var cidade = "";

                            feriado = FeriadoCelebrado.CriarFeriado(dataFeriado, evento, abrangencia, pais, estado, codigoFederal, codigoIbge, cidade, false, false);

                            feriadosEstaduais.Add(feriado);
                        }
                    }
                }

            }

            return feriadosEstaduais;
        }

        /// <summary>
        /// Este método libera a memória utilizada para a carga da planilha de feriados estaduais.
        /// </summary>
        /// <param name="feriadosEstaduais">Lista contendo os feriados estaduais.</param>
        public static void Dispose(List<FeriadoCelebrado> feriadosEstaduais)
        {
            if (feriadosEstaduais != null)
            {
                feriadosEstaduais.Clear();
                feriadosEstaduais = new List<FeriadoCelebrado>();
            }
        }
    }

}
