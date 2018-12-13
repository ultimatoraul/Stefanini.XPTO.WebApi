namespace Stefanini.XPTO.WinForms.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ProductClient")]
    public partial class ProductClient
    {
        public int ID { get; set; }

        public int ClientID { get; set; }

        public int ProductID { get; set; }

        public virtual Client Client { get; set; }

        public virtual Product Product { get; set; }
    }
}
