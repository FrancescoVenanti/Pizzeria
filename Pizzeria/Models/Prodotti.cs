namespace Pizzeria.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Prodotti")]
    public partial class Prodotti
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Prodotti()
        {
            Dettagli = new HashSet<Dettagli>();
        }

        [Key]
        public int idProdotto { get; set; }

        [Required]
        [StringLength(50)]
        public string Nome { get; set; }

        public string Foto { get; set; }

        public string foto2 { get; set; }

        public string foto3 { get; set; }

        [Column(TypeName = "money")]
        public decimal Prezzo { get; set; }

        public TimeSpan Consegna { get; set; }

        [Required]
        public string Ingredienti { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Dettagli> Dettagli { get; set; }
        
    }
}
