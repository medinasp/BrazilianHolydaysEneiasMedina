using System.Reflection;

namespace BrazilianHolidaysEneiasMedina.Helpers
{
    /// <summary>
    /// Esta classe fornece métodos para importar feriados nacionais de uma planilha no formato CSV. A primera linha da
    /// planilha sempre é desprezada, pois deve conter o header da planilha.
    /// </summary>
    /// <remarks>A planilha deve conter um header com 3 colunas para DIA MÊS EVENTO, e as colunas devem ser separadas
    /// pelo caracter ';'.</remarks>
    public static class FeriadosNacionaisHelper
    {
        /// <summary>
        /// Este método importa de uma planilha CSV, as informações sobre feriados nacionais.
        /// </summary>
        /// <param name="anoFeriado">Ano com 4 digitos numéricos, para o qual esta sendo importado os feriados.</param>
        /// <param name="abrangencia">String que representa a abrangência dos feriados. Default "Nacional".</param>
        /// <param name="pais">Sigla do país com 2 letras. Default "BR".</param>
        /// <returns>Lista contendo os feriados nacionais.</returns>
        public static List<FeriadoCelebrado> ImportarFeriadosNacionais(int anoFeriado, string abrangencia = "Nacional", string pais = "BR")
        {

            //verifica se os recursos do pacote (arquivos fontes csv neste caso) foram incluidos
            //string[] recursos = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            //foreach (string recurso in recursos)
            //{
            //    Console.WriteLine(recurso);
            //}

            List<FeriadoCelebrado> feriadosNacionais = null;
            string[,] planilha = null;
            string feriadosNacionaisResourceName = "BrazilianHolidaysEneiasMedina.assets.FeriadosNacionaisBr.csv";

            try
            {
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(feriadosNacionaisResourceName))
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

                    planilha = new string[numeroDeLinhas, 3];

                    int indice = 0;

                    while (!reader.EndOfStream)
                    {
                        string linha = reader.ReadLine();
                        string[] campos = linha.Split(';');

                        planilha[indice, 0] = campos[0];
                        planilha[indice, 1] = campos[1];
                        planilha[indice, 2] = campos[2];

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
                    FeriadoCelebrado feriado;
                    feriadosNacionais = new List<FeriadoCelebrado>();

                    if (anoFeriado < 1900)
                    {
                        anoFeriado = DateTime.Now.Year;
                    }

                    for (int linha = 1, totalDeLinhas = planilha.GetLength(0); linha < totalDeLinhas; linha++)
                    {
                        if (!string.IsNullOrEmpty(planilha[linha, coluna1]) &&
                            !string.IsNullOrEmpty(planilha[linha, coluna2]) &&
                            !string.IsNullOrEmpty(planilha[linha, coluna3]))
                        {
                            int dia = string.IsNullOrEmpty(planilha[linha, coluna1]) ? 1 : int.Parse((planilha[linha, coluna1]).Trim());
                            int mes = string.IsNullOrEmpty(planilha[linha, coluna2]) ? 1 : int.Parse((planilha[linha, coluna2]).Trim());
                            var evento = string.IsNullOrEmpty(planilha[linha, coluna3]) ? "" : (planilha[linha, coluna3]).Trim();

                            if (!string.IsNullOrEmpty(evento))
                            {
                                DateTime dataFeriado = new DateTime(anoFeriado, mes, dia);

                                var codigoFederal = "";
                                var codigoIbge = "";
                                var estado = "";
                                var nomeMunicipio = "";

                                feriado = FeriadoCelebrado.CriarFeriado(dataFeriado, evento, abrangencia, pais, estado, codigoFederal, codigoIbge, nomeMunicipio, false, false);

                                feriadosNacionais.Add(feriado);
                            }
                        }
                    }
                }
            return feriadosNacionais;
        }

        /// <summary>
        /// Este método libera a memória utilizada para a carga da planilha de feriados nacionais.
        /// </summary>
        /// <param name="feriadosNacionais">Lista contendo os feriados nacionais.</param>
        public static void Dispose(List<FeriadoCelebrado> feriadosNacionais)
            {
                if (feriadosNacionais != null)
                {
                    feriadosNacionais.Clear();
                    feriadosNacionais = new List<FeriadoCelebrado>();
                }
            }
        }
    }
