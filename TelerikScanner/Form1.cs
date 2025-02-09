using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

                    if (content.Contains("Telerik.Web.UI.WebResource.axd"))
                    {
                        richTextBox2.AppendText($"[WARNING] Telerik WebResource found in {url}" + Environment.NewLine);

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

                    if (content.Contains("Telerik.Web.UI.WebResource.axd"))
                    {
                        richTextBox1.AppendText($"[WARNING] Telerik WebResource found in {url}" + Environment.NewLine);

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
    }
}
