using Newtonsoft.Json;
using Stefanini.XPTO.WinForms.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stefanini.XPTO.WinForms {
  public partial class Form1 : Form {
    DataTable dt = new DataTable();
    public Form1() {
      InitializeComponent();

      //ATENÇÃO!!!
      //É UMA SOLUÇÃO PERIGOSA POIS ELA BLOQUEIA A THREAD ENQUANTO EM EXECUÇÃO
      //PRECISARIA DE MAIS TEMPO PARA ESTUDAR E IMPLEMENTAR UMA MELHOR SOLUÇÂO
      Task.Run(() => this.TesteAsync()).Wait();
    }

    public async Task TesteAsync() {
      try {
        #region Processamento...
        string baseUrl = System.Configuration.ConfigurationManager.AppSettings["baseURL"];
        using (var client = new HttpClient()) {
          client.BaseAddress = new Uri(baseUrl);
          client.DefaultRequestHeaders.Clear();
          client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
          HttpResponseMessage Res = await client.GetAsync("api/ProductClients");
          List<ProductClient> ObjData = new List<ProductClient>();
          List<ProductClient> Model = new List<ProductClient>();

          if (Res.IsSuccessStatusCode) {
            var Response = Res.Content.ReadAsStringAsync().Result;
            ObjData = JsonConvert.DeserializeObject<List<ProductClient>>(Response);
          }

          foreach (ProductClient data in ObjData) {
            Res = await client.GetAsync(string.Format("api/Clients/{0}", data.ClientID));
            if (Res.IsSuccessStatusCode) {
              var Response = Res.Content.ReadAsStringAsync().Result;
              data.Client = JsonConvert.DeserializeObject<Client>(Response);
            }
            Res = await client.GetAsync(string.Format("api/Products/{0}", data.ProductID));
            if (Res.IsSuccessStatusCode) {
              var Response = Res.Content.ReadAsStringAsync().Result;
              data.Product = JsonConvert.DeserializeObject<Product>(Response);
            }
            Model.Add(data);
          }

          dt.Columns.Add("Produto", typeof(string));
          dt.Columns.Add("Proprietário", typeof(string));
          dt.Columns.Add("Sexo", typeof(string));
          dt.Columns.Add("Data de Nascimento", typeof(DateTime));
          dt.Columns.Add("Email", typeof(string));
          dt.Columns.Add("Ativo", typeof(int));

          for (int a = 0; a < Model.Count; a++) {
            dt.Rows.Add(new object[] {
            Model[a].Product.Name,
            Model[a].Client.FirstName + " " + Model[a].Client.LastName,
            Model[a].Client.Gender,
            Model[a].Client.BirthDate.ToString("dd/MM/yyyy"),
            Model[a].Client.Email,
            Model[a].Client.Active});
          }
          dataGridView1.DataSource = dt;
          #endregion
        }
      }
      catch (Exception) {
        DialogResult result = MessageBox.Show("Verifique se foi configurado corretamente a baseURL no App.config", "Erro",
               MessageBoxButtons.OK);
        return;
      }
    }

    private void button1_Click(object sender, EventArgs e) {
      string baseUrl = System.Configuration.ConfigurationManager.AppSettings["baseURL"];
      OpenFileDialog openFileDialog1 = new OpenFileDialog();
      int size = -1;
      DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
      if (result == DialogResult.OK) // Test result.
      {
        string file = openFileDialog1.FileName;
        try {
          string text = File.ReadAllText(file);
          //size = text.Length;
          
          System.IO.FileInfo fInfo = new System.IO.FileInfo(openFileDialog1.FileName);

          string strFileName = fInfo.Name;
          string strFilePath = fInfo.DirectoryName;          

          Encoding Enc = GetFileEncoding(strFilePath);
          
          string importedTxt;
          using (StreamReader reader = new StreamReader(strFilePath, Enc)) {
            importedTxt = reader.ReadToEnd();
          }

          List<HttpResponseMessage> Response = new List<HttpResponseMessage>();
          string[] contents = importedTxt.Split(';');
          foreach (string content in contents) {
            string[] data = content.Split(',');
            if (data.Length == 7) {
              Client Cliente;

              using (var client = new HttpClient()) {
                Cliente = new Client {
                  ID = int.Parse(data[0]),
                  FirstName = data[1],
                  LastName = data[2],
                  BirthDate = DateTime.ParseExact(data[3], "dd/MM/yyyy", CultureInfo.InvariantCulture),
                  Gender = data[4],
                  Email = data[5],
                  Active = data[6] == "true" ? 1 : 0
                };
                client.BaseAddress = new Uri(baseUrl);
                //Response.Add(client.PostAsJsonAsync("api/Clients", Cliente).Result);
              }
            }
            else if (data.Length == 3) {
              Product Produto = new Product {
                ID = int.Parse(data[1]),
                Name = data[2]
              };
              using (var client = new HttpClient()) {
                client.BaseAddress = new Uri(baseUrl);
                //Response.Add(client.PostAsync("api/Products", Produto, new JsonMediaTypeFormatter()).Result);
              }

              ProductClient ProductClients = new ProductClient() {
                ClientID = int.Parse(data[0]),
                ProductID = int.Parse(data[1])
              };
              using (var client = new HttpClient()) {
                client.BaseAddress = new Uri(baseUrl);
                //Response.Add(client.PostAsync("api/ProductClients", ProductClients, new JsonMediaTypeFormatter()).Result);
              }
            }
          }
        }
        catch (IOException) {
        }
      }
      //Console.WriteLine(size); // <-- Shows file size in debugging mode.
      //Console.WriteLine(result); // <-- For debugging use.
    }

    public Encoding GetFileEncoding(string srcFile) {
      Encoding enc = Encoding.Default;
      //FileStream file;
      byte[] buffer = new byte[10];
      FileStream file = new FileStream(srcFile, FileMode.Open, FileAccess.Read, FileShare.Read);
      //using (var stream = File.Open(srcFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
      //  file = stream;//do something with the stream here
      //}
      //FileStream file = new FileStream(srcFile, FileMode.Open, FileAccess.Read);
      file.Read(buffer, 0, 10);
      file.Close();

      if (buffer[0] == 0xef && buffer[1] == 0xbb && buffer[2] == 0xbf)
        enc = Encoding.UTF8;
      else if (buffer[0] == 0xfe && buffer[1] == 0xff)
        enc = Encoding.Unicode;
      else if (buffer[0] == 0 && buffer[1] == 0 && buffer[2] == 0xfe && buffer[3] == 0xff)
        enc = Encoding.UTF32;
      else if (buffer[0] == 0x2b && buffer[1] == 0x2f && buffer[2] == 0x76)
        enc = Encoding.UTF7;
      else if (buffer[0] == 0xFE && buffer[1] == 0xFF)
        // 1201 unicodeFFFE Unicode (Big-Endian)
        enc = Encoding.GetEncoding(1201);
      else if (buffer[0] == 0xFF && buffer[1] == 0xFE)
        // 1200 utf-16 Unicode
        enc = Encoding.GetEncoding(1200);
      else if (ValidateUtf8whitBOM(srcFile))
        enc = Encoding.Unicode;

      return enc;
    }

    private bool ValidateUtf8whitBOM(string FileSource) {
      bool bReturn = false;
      string TextUTF8 = "", TextANSI = "";
      StreamReader srFileWhitBOM = new StreamReader(FileSource);
      TextUTF8 = srFileWhitBOM.ReadToEnd();
      srFileWhitBOM.Close();
      srFileWhitBOM = new StreamReader(FileSource, Encoding.Default, false);

      TextANSI = srFileWhitBOM.ReadToEnd();
      srFileWhitBOM.Close();
      // if the file contains special characters is UTF8 text read ansi show signs

      if (TextANSI.Contains("Ã") || TextANSI.Contains("±"))
        bReturn = true;
      return bReturn;
    }
  }
}