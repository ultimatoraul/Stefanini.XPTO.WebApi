namespace Stefanini.XPTO.WebMvc.Models {
  using Newtonsoft.Json;
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Data.Entity.Spatial;

  [Table("ProductClient")]
  public partial class ProductClient {
    public int ID { get; set; }

    public int ClientID { get; set; }

    public int ProductID { get; set; }

    [JsonIgnore]
    public virtual Client Client { get; set; }

    [JsonIgnore]
    public virtual Product Product { get; set; }
  }
}
