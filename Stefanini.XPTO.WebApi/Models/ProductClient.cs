using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Stefanini.XPTO.WebApi.Models
{
  public class ProductClient
  {
    public int ID { get; set; }
    public int ClientID { get; set; }
    public int ProductID { get; set; }

    [JsonIgnore]
    public virtual Client Client { get; set; }
    [JsonIgnore]
    public virtual Product Product { get; set; }
  }
}