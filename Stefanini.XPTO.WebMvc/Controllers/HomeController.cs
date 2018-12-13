using Newtonsoft.Json;
using Stefanini.XPTO.WebMvc.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Stefanini.XPTO.WebMvc.Controllers {
  public class HomeController : Controller {
    public async Task<ActionResult> Index() {
      string baseUrl = ConfigurationManager.AppSettings["baseURL"];

      List<ProductClient> ObjData = new List<ProductClient>();
      List<ProductClient> Model = new List<ProductClient>();
      #region Processamento...
      using (var client = new HttpClient()) {
        client.BaseAddress = new Uri(baseUrl);
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        HttpResponseMessage Res = await client.GetAsync("api/ProductClients");

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

        return View(Model);
      }
      #endregion
    }

    [HttpPost]
    public ActionResult ImportarTxt(HttpPostedFileBase postedFile) {
      string filePath = string.Empty;
      if (postedFile != null) {
        try {
          #region Processamento...
          string path = Server.MapPath("~/Uploads/");
          if (!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
          }
          filePath = path + Path.GetFileName(postedFile.FileName);
          string extension = Path.GetExtension(postedFile.FileName);
          postedFile.SaveAs(filePath);

          //Read the contents of CSV file.
          //string csvData = System.IO.File.ReadAllText(filePath);
          Encoding Enc = GetFileEncoding(filePath);


          string baseUrl = ConfigurationManager.AppSettings["baseURL"];
          string importedTxt;
          using (StreamReader reader = new StreamReader(postedFile.InputStream, Enc)) {
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
                Response.Add(client.PostAsJsonAsync("api/Clients", Cliente).Result);
              }
            }
            else if (data.Length == 3) {
              Product Produto = new Product {
                ID = int.Parse(data[1]),
                Name = data[2]
              };
              using (var client = new HttpClient()) {
                client.BaseAddress = new Uri(baseUrl);
                Response.Add(client.PostAsync("api/Products", Produto, new JsonMediaTypeFormatter()).Result);
              }

              ProductClient ProductClients = new ProductClient() {
                ClientID = int.Parse(data[0]),
                ProductID = int.Parse(data[1])
              };
              using (var client = new HttpClient()) {
                client.BaseAddress = new Uri(baseUrl);
                Response.Add(client.PostAsync("api/ProductClients", ProductClients, new JsonMediaTypeFormatter()).Result);
              }
            }
          }       
          #endregion
        }
        catch (Exception ex) {
          //CRIAR TRATATIVA
          return View(ex);
        }
        finally {
          if (System.IO.File.Exists(filePath)) {
            System.IO.File.Delete(filePath);
          }
        }
      }

      return RedirectToAction("Index", "Home");
    }

    public Encoding GetFileEncoding(string srcFile) {
      Encoding enc = Encoding.Default;

      byte[] buffer = new byte[10];
      FileStream file = new FileStream(srcFile, FileMode.Open);
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