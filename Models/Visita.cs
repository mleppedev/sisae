using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sisae.Models
{
    public class Visita
    {
        public string Comentarios { get; set; }

        [Required]
        public string Estado { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Fecha_Visita { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan Hora_Entrada { get; set; }

        [DataType(DataType.Time)]
        public TimeSpan? Hora_Salida { get; set; }

        [Key]
        public int ID_Visita { get; set; }

        [Required]
        [ForeignKey("Visitado")]
        public int ID_Visitado { get; set; }

        [Required]
        [ForeignKey("Visitante")]
        public int ID_Visitante { get; set; }

        [Required]
        public string Motivo_Visita { get; set; }

        // Propiedades de navegación: no deben ser validadas
        public virtual Visitado Visitado { get; set; }

        public virtual Visitante Visitante { get; set; }
    }
}