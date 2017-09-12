using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.IO;
using NFe_Util_2G;

namespace SvcNFdestinadas
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static string txtNfe, siglaWS = "AN", siglaUF, nomeCertificado = "CN=M R M KATO ASAKURA - EPP:69621187915, OU=AC CAIXA PJ-1 V1, OU=Caixa Economica Federal, O=ICP-Brasil, C=BR", versao = "1.01", licenca, CNPJ, ultNSU = "0";
        static int tipoAmbiente, indNFe, indEmi;
        static string msgDados,msgRetWS,msgResultado,dhResp,ultNSUConsultado = "0"; // variaveis de saida
        static int cStat, indCont; // variaveis de saida
        

        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new Service1()
            };
            ServiceBase.Run(ServicesToRun);
        }
        static bool ConectarBD()
        {
            string line;
            string hostname = "";
            string user = "";
            string password = "";
            StreamReader arq = new StreamReader(@"c:\windows\CONEXAO_BITFARMA_SQL.ini");
            while ((line = arq.ReadLine()) != null)
            {
                if (line.Contains("hostname"))
                {
                    string[] texto = line.Split('=');
                    hostname = texto[1];
                }
                else if (line.Contains("username"))
                {
                    string[] texto = line.Split('=');
                    user = texto[1];
                }
                else if (line.Contains("password"))
                {
                    string[] texto = line.Split('=');
                    password = texto[1];
                }
            }
            arq.Close();
            var connString = "Server="+hostname+";Database=bitfarma;Uid="+user+";Pwd="+password;
            var connection = new MySqlConnection(connString);
            var command = connection.CreateCommand();
            try
            {
                connection.Open();
                command.CommandText = "SELECT cgcreg,estado,certificado,filial FROM registro";
                MySqlDataReader readerRegistro = command.ExecuteReader();
                command.CommandText = "SELECT nfelicenca,nfeambiente,ultimonsu FROM parametr_";
                MySqlDataReader readerParametros = command.ExecuteReader();
                return true;
            }
            catch
            {
                //quando não conectar
                return false;
            }
        }
        static void ConsultaNF()
        {
            Util nfe = new Util();
            while (indCont == 1)
            {
                txtNfe = nfe.ConsultaNFDest(siglaWS, siglaUF, tipoAmbiente, nomeCertificado, versao, out msgDados, out msgRetWS, out cStat, out msgResultado, CNPJ, indNFe, indEmi, ultNSU, out dhResp, out indCont, out ultNSUConsultado, "", "", "", licenca);
                if (cStat == 138)
                {
                    //deu certo
                }
                else if (cStat != 137)
                {
                    //verificar o erro
                    indCont = 9;
                }
                else if (cStat == 137)
                {
                    //Nenhum documento encontrado
                }
                ultNSU = ultNSUConsultado;
            }
            
        }
    }
}
