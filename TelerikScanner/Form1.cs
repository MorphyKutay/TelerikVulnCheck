using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;


namespace TelerikScanner
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private string worldlsit = "";
        private string[] urls;

        private void Form1_Load(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Select URL List";
            openFileDialog1.FileName = "Select File";

        }

        private async void submitbtn_Click(object sender, EventArgs e)
        {
            String url = site.Text;
            HttpClient client = new HttpClient();

            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    //string result = $"[{response.StatusCode}] {url}";
                    //richTextBox1.AppendText(result + Environment.NewLine);

                    string content = await response.Content.ReadAsStringAsync();

                    if (content.Contains("Telerik.Web.UI.WebResource.axd") || content.Contains("Telerik.Web.UI.RadAjaxManager"))
                    {
                        richTextBox2.AppendText($"[WARNING] Telerik WebResource found in {url}/Telerik.Web.UI.WebResource.axd?type=rau" + Environment.NewLine);

                    }
                    else
                    {
                        richTextBox2.AppendText("Telerik Not Found");
                    }
                }
                catch (Exception ex) {
                    richTextBox2.AppendText($"[ERROR] {url} - {ex.Message}" + Environment.NewLine);

                }
            }
            else
            {
                richTextBox2.AppendText(url + " => Not Ok ");
            }

        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if (urls == null || urls.Length == 0)
            {
                MessageBox.Show("Please select a file first!", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            HttpClient client = new HttpClient();

            foreach (string url in urls)
            {
                if (string.IsNullOrWhiteSpace(url))
                    continue;

                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    //string result = $"[{response.StatusCode}] {url}";
                    //richTextBox1.AppendText(result + Environment.NewLine);

                    string content = await response.Content.ReadAsStringAsync();

                    if (content.Contains("Telerik.Web.UI.WebResource.axd") || content.Contains("Telerik.Web.UI.RadAjaxManager"))
                    {
                        richTextBox1.AppendText($"[WARNING] Telerik WebResource found in {url}/Telerik.Web.UI.WebResource.axd?type=rau" + Environment.NewLine);

                    }
                }
                catch (Exception ex)
                {
                    richTextBox1.AppendText($"[ERROR] {url} - {ex.Message}" + Environment.NewLine);
                }
            }
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                worldlsit = openFileDialog1.FileName;  
            }
            try
            {
                urls = File.ReadAllLines(worldlsit);

                richTextBox1.Clear(); 
                foreach (string line in urls)
                {
                    richTextBox3.AppendText(line + Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private async void findbtn_Click(object sender, EventArgs e)
        {
            string query = dorktext.Text;
            int pageNumber = 0;  // Sayfa numarasını sıfırdan başlatıyoruz
            int resultsPerPage = 10;  // Sayfa başına 10 sonuç
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");

            richTextBox4.Clear();  // Önce içeriği temizle

            try
            {
                using (StreamWriter writer = new StreamWriter("output.txt"))
                while (true)
                {
                    // Bing URL'si oluşturuluyor, 'first' parametresi her sayfa için artacak
                    string url = $"https://www.bing.com/search?q={Uri.EscapeDataString(query)}&first={pageNumber * resultsPerPage + 1}";

                    // Sayfayı al
                    string html = await client.GetStringAsync(url);

                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(html);

                    // Sayfadaki linkleri çek
                    var nodes = doc.DocumentNode.SelectNodes("//li[@class='b_algo']//h2//a");

                    if (nodes == null || nodes.Count == 0)
                    {
                        break;  // Sayfada sonuç yoksa döngüden çık
                    }

                    foreach (var node in nodes)
                    {
                        string link = node.GetAttributeValue("href", "N/A");

                        if (!string.IsNullOrEmpty(link))
                        {
                            richTextBox4.AppendText(link + Environment.NewLine);  // Sonuçları ekrana yazdır
                            writer.WriteLine(link);
                        }
                    }

                    // Sonraki sayfaya geçmek için pageNumber'ı artır
                    pageNumber++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bir hata oluştu: " + ex.Message);
            }
        }





    }
}
