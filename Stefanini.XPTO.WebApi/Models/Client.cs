using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stefanini.XPTO.WebApi.Models
{
  public class Client
  {
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int ID { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public string Gender { get; set; }
    public string Email { get; set; }
    public int Active { get; set; }
    public DateTime BirthDate { get; set; }
  }
}