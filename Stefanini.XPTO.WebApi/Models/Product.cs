using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stefanini.XPTO.WebApi.Models
{
  public class Product
  {
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int ID { get; set; }
    public string Name { get; set; }
  }
}