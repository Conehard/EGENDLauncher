using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.ComponentModel;

namespace Launcher
{
    public partial class Form1 : Form
    {
        static string urlVersao = "http://egend.com.br/top/att/vrsn.egz";
        string downloadDestination = Path.GetTempFileName();
        string version = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        public void buscarArquivo()
        {
            WebClient webClient = new WebClient();

            label1.Text = "Buscando atualização...";

            string versaoRemota = webClient.DownloadString(urlVersao).Trim();
            string[] partesArquivo = (new Regex(@"\s+")).Split(versaoRemota);
            string urlRemota = partesArquivo[1];
            string hashRemoto = partesArquivo[2];
            version = partesArquivo[0];

            Version versaoLocal = new Version(File.ReadAllText("vrsn.egz").Trim());
            Version versaoRemotaArquivo = new Version(partesArquivo[0]);

            if (versaoRemotaArquivo > versaoLocal)
            {
                label1.Text = "Existe uma nova versão disponivel.";
                Atualizar(urlRemota, hashRemoto);
            }
            else
            {
                label1.Text = "Nenhuma atualização disponível.";
            }
        }

        public void Atualizar(string urlRemota, string hashRemoto)
        {
            label1.Text = "Iniciando atualização";

            label1.Text = "Atualizando...";

            WebClient downloadifier = new WebClient();
            downloadifier.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
            downloadifier.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
            downloadifier.DownloadFileAsync(new Uri(urlRemota), downloadDestination);

        }

        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            string diretorioExtracao = System.Environment.CurrentDirectory + @"./updated";
            string final = System.Environment.CurrentDirectory;
            if (Directory.Exists(diretorioExtracao))
            {
                Directory.Delete(diretorioExtracao, true);
            }
            using (ZipArchive archive = ZipFile.Open(downloadDestination, ZipArchiveMode.Update))
            {
                archive.ExtractToDirectory(diretorioExtracao);
            }
            CopiarArquivos(diretorioExtracao, final);
            label1.Text = "Finalizando...";
            label1.Text = "Limpando arquivos desnecessários - ";
            Directory.Delete(diretorioExtracao, true);
            File.WriteAllText("vrsn.egz", version);

            label1.Text = "Atualização finalizada.";
        }


        static string GetSHA1HashFromFile(string fileName)
        {
            FileStream file = new FileStream(fileName, FileMode.Open);
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] byteHash = sha1.ComputeHash(file);
            file.Close();

            StringBuilder hashString = new StringBuilder();
            for (int i = 0; i < byteHash.Length; i++)
                hashString.Append(byteHash[i].ToString("x2"));
            return hashString.ToString();
        }

        public void CopiarArquivos(string pastaBaixados, string pastaFinal)
        {
            if (!Directory.Exists(pastaFinal))
                Directory.CreateDirectory(pastaFinal);

            string[] arquivos = Directory.GetFiles(pastaBaixados);
            foreach (string arquivo in arquivos)
            {
                string nome = Path.GetFileName(arquivo);
                string destino = Path.Combine(pastaFinal, nome);
                File.Copy(arquivo, destino, true);
            }

            string[] pastas = Directory.GetDirectories(pastaBaixados);
            foreach (string pasta in pastas)
            {
                string nome = Path.GetFileName(pasta);
                string destino = Path.Combine(pastaFinal, nome);
                CopiarArquivos(pasta, destino);
            }
        }


        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string filename = Path.Combine(Environment.CurrentDirectory, @"system\Game.exe");
            var proc = System.Diagnostics.Process.Start(filename, "startgame");
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            buscarArquivo();
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }
    }
}
