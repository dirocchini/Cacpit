using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CACPit
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();

        static void Main(string[] args)
        {
            try
            {
                //Console.Write("CCCCCCCCCCCCCCCCCC   AAAAAAAAAAAAAAAAAAA   CCCCCCCCCCCCCCCCCC");
                //Console.Write("CCCCCCCCCCCCCCCCCC   AAAAAAAAAAAAAAAAAAA   CCCCCCCCCCCCCCCCCC");
                //Console.Write("CCCCCCC              AAAAA          AAAA   CCCCCCC");
                //Console.Write("CCCCCCC              AAAAA          AAAA   CCCCCCC");
                //Console.Write("CCCCCCC              AAAAAaaaaaaaaaaAAAA   CCCCCCC");
                //Console.Write("CCCCCCC              AAAAA          AAAA   CCCCCCC");
                //Console.Write("CCCCCCCCCCCCCCCCCC   AAAAA          AAAA   CCCCCCCCCCCCCCCCCC");
                //Console.Write("CCCCCCCCCCCCCCCCCC   AAAAA          AAAA   CCCCCCCCCCCCCCCCCC        PIT");

                Console.WriteLine("CACpit v0");
                Console.WriteLine("");

                Console.Write(" INFORME SEU CPF: ");
                var document = Console.ReadLine();

                Console.Write(" INFORME O PROTOCOLO: ");
                var protocol = Console.ReadLine();


                string txt_cpf_cnpj = "350.839.478-62";
                string txt_protocolo = "00595892020";
                string url = "http://protocolosfpc.2rm.eb.mil.br/consulta_processo.php";

                Console.WriteLine("");
                Console.WriteLine("  -- consultando");
                Console.WriteLine("");


                using (var client = new WebClient())
                {
                    var values = new NameValueCollection();
                    values["txt_cpf_cnpj"] = document;
                    values["txt_protocolo"] = protocol;

                    var response = client.UploadValues(url, values);

                    var responseString = Encoding.Default.GetString(response);



                    if (responseString.ToLower().Contains("processo não localizado"))
                    {
                        Console.WriteLine("  << PROCESSO NÃO LOCALIZADO >>");
                        return;
                    }

                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(responseString);
                    var status = doc.DocumentNode.SelectSingleNode("(//div[contains(@class,'card')]//center//h3)[1]").InnerText;
                    var requester = doc.DocumentNode.SelectSingleNode("(//table[contains(@class,'table')]//tbody//tr[2]//td[2])").InnerText;
                    var requesterDoc = doc.DocumentNode.SelectSingleNode("(//table[contains(@class,'table')]//tbody//tr[2]//td[1])").InnerText;
                    var date = doc.DocumentNode.SelectSingleNode("(//table[contains(@class,'table')]//tbody//tr[1]//td[2])").InnerText;


                    Console.WriteLine("  REQUERENTE:   " + requester);
                    Console.WriteLine("  DOCUMENTO:    " + requesterDoc);
                    Console.WriteLine("  DATA DO PROT: " + date);
                    Console.WriteLine("  STATUS:       " + status);

                }



            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ixi azedou ... aconteceu algum problema não esperado, envie esta tela para seu desenvolvedor favorito!");
                Console.WriteLine(ex);
            }
            finally
            {
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine("PRESSIONE QUALQUER TECLA PARA FINALIZAR");

                Console.ReadKey();
            }

            //var values = new Dictionary<string, string>
            //{
            //    { "txt_cpf_cnpj", "350.839.478-62" },
            //    { "txt_protocolo", "00595892020" }
            //};

            //var content = new FormUrlEncodedContent(values);

            //var response = client.PostAsync("http://protocolosfpc.2rm.eb.mil.br", content).Result;

            //var responseString = response.Content.ReadAsStringAsync().Result;

        }


    }
}
