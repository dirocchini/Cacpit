using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace CACPitFrame
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("CACpit v0.2.0");

                var action = "i";

                List<Protocol> protocols = new List<Protocol>();

                Console.WriteLine("");
                Console.Write(" DESEJA UTILIZAR O ARQUIVO DE PROCESSOS? aperte enter para sim ou digite n para não: ");
                var useFile = Console.ReadLine().Trim();

                if (string.IsNullOrEmpty(useFile))
                {
                    Console.WriteLine("");
                    Console.WriteLine("  -- consultando");

                    ReadAndProcessAll();
                    return;
                }

                while (action.ToLower() == "i")
                {
                    Console.WriteLine("");
                    Console.Write(" INFORME O CPF: ");
                    var document = Console.ReadLine();

                    Console.Write(" INFORME O PROTOCOLO: ");
                    var protocol = Console.ReadLine();

                    Console.Write(" PARA INFORMAR OUTRO PROTOCOLO, DIGITE i, CASO CONTRÁRIO APERTE ENTER PARA CONSULTAR: ");
                    action = Console.ReadLine();

                    protocols.Add(new Protocol() { Document = document, ProtocolNbr = protocol });
                }


                Console.WriteLine("");
                Console.WriteLine("  -- consultando");


                using (var client = new WebClient())
                {
                    foreach (Protocol protocol in protocols)
                    {
                        Console.WriteLine("");
                        GetProcessInfo(client, protocol.Document, protocol.ProtocolNbr);
                    }
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
        }

        private static void ReadAndProcessAll()
        {
            var file = Path.Combine(Directory.GetCurrentDirectory(), "processos.txt");
            if (File.Exists(file))
            {
                var lines = File.ReadAllLines(file).ToList();

                if (lines != null && lines.Count > 1)
                {
                    using (var client = new WebClient())
                    {
                        for (int i = 1; i < lines.Count; i++)
                        {
                            Console.WriteLine("");
                            GetProcessInfo(client, lines[i].Split(';')[0], lines[i].Split(';')[1]);
                        }
                    }
                }
                else
                    Console.Write(" O ARQUIVO NÃO CONTÉM PROCESSOS PARA SEREM CONSULTADOS");

            }
            else
                Console.Write($" O ARQUIVO {file} NÃO FOI ENCONTRADO");

        }

        private static void GetProcessInfo(WebClient client, string document, string protocolNbr)
        {
            string url = "http://protocolosfpc.2rm.eb.mil.br/consulta_processo.php";

            var values = new NameValueCollection();
            values["txt_cpf_cnpj"] = document;
            values["txt_protocolo"] = protocolNbr;

            var response = client.UploadValues(url, values);

            var responseString = Encoding.Default.GetString(response);



            if (responseString.ToLower().Contains("processo não localizado") || responseString.ToLower().Contains("<strong><span class='oi oi-warning'></span> processo "))
            {
                Console.WriteLine("  PROTOCOLO:    " + protocolNbr);
                Console.WriteLine("  STATUS:       " + "NÃO LOCALIZADO");

                return;
            }

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(responseString);
            var status = doc.DocumentNode.SelectSingleNode("(//div[contains(@class,'card')]//center//h3)[1]").InnerText;
            var requester = doc.DocumentNode.SelectSingleNode("(//table[contains(@class,'table')]//tbody//tr[2]//td[2])").InnerText;
            var requesterDoc = doc.DocumentNode.SelectSingleNode("(//table[contains(@class,'table')]//tbody//tr[2]//td[1])").InnerText;
            var date = doc.DocumentNode.SelectSingleNode("(//table[contains(@class,'table')]//tbody//tr[1]//td[2])").InnerText;


            Console.WriteLine("  PROTOCOLO:    " + protocolNbr);
            Console.WriteLine("  REQUERENTE:   " + requester);
            Console.WriteLine("  DOCUMENTO:    " + requesterDoc);
            Console.WriteLine("  DATA DO PROT: " + date);
            Console.WriteLine("  STATUS:       " + status);
        }
    }
}