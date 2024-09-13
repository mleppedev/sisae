using System;
using System.ComponentModel.DataAnnotations;

namespace sisae.Models
{
    public class Visitante
    {
        [Required]
        public string Apellido { get; set; }

        [Required]
        public string Direccion { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime FechaNacimiento { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime FechaVencimientoCarnet { get; set; }

        public byte[] FotoBiometrica { get; set; }

        [Key]
        public int ID_Visitante { get; set; }

        [Required]
        public string Nacionalidad { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        [MaxLength(12)]  // Tamaño máximo adecuado para un RUT, que incluye puntos y guion
        [RegularExpression(@"\d{1,2}\.\d{3}\.\d{3}-[\dkK]", ErrorMessage = "El RUT no es válido.")]
        public string RUT { get; set; }

        [Required]
        [Phone]
        public string Telefono { get; set; }
    }
}