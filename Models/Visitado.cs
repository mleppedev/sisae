using System.ComponentModel.DataAnnotations;

namespace sisae.Models
{
    public class Visitado
    {
        [Required]
        public string Apellido { get; set; }

        [Required]
        public string Cargo { get; set; }

        [Required]
        public string Departamento { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Key]
        public int ID_Visitado { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        [Phone]
        public string Telefono { get; set; }
    }
}