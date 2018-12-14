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
      Task.Run(() => this.GetDataAsync()).Wait();
    }

    public async Task GetDataAsync() {
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
        }
        #endregion
      }
      catch (Exception) {
        MessageBox.Show("Verifique se foi configurado corretamente a baseURL no App.config", "Erro",
               MessageBoxButtons.OK);
        return;
      }
    }

    private void button1_Click(object sender, EventArgs e) {
      
      OpenFileDialog openFileDialog1 = new OpenFileDialog();
      DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
      if (result == DialogResult.OK) // Test result.
      {
        string file = openFileDialog1.FileName;
        try {
          #region Processamento...
          string baseUrl = System.Configuration.ConfigurationManager.AppSettings["baseURL"];
          string text = File.ReadAllText(file);
          
          System.IO.FileInfo fInfo = new System.IO.FileInfo(openFileDialog1.FileName);

          string strFileName = fInfo.Name;
          string strFilePath = fInfo.DirectoryName;          

          List<HttpResponseMessage> Response = new List<HttpResponseMessage>();
          string[] contents = text.Split(';');
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
                Response.Add(client.PostAsJsonAsync("api/Clients", Cliente).Result);
              }
            }
            else if (data.Length == 3) {
              Product Produto = new Product {
                ID = int.Parse(data[0]),
                Name = data[2]
              };
              using (var client = new HttpClient()) {
                client.BaseAddress = new Uri(baseUrl);
                Response.Add(client.PostAsync("api/Products", Produto, new JsonMediaTypeFormatter()).Result);
              }

              ProductClient ProductClients = new ProductClient() {
                ProductID = int.Parse(data[0]),
                ClientID = int.Parse(data[1])
              };
              using (var client = new HttpClient()) {
                client.BaseAddress = new Uri(baseUrl);
                Response.Add(client.PostAsync("api/ProductClients", ProductClients, new JsonMediaTypeFormatter()).Result);
              }
            }
          }
          #endregion
        }
        catch (IOException) {
          MessageBox.Show("Verifique se o arquivo importado está no formato correto", "Erro",
                 MessageBoxButtons.OK);
          return;
        }
        finally {
          Application.Restart();
          Environment.Exit(0);
        }
      }
    }   
  }
}