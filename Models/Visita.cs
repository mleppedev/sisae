using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sisae.Models
{
    public class Visita
    {
        [Required(ErrorMessage = "Debe ingresar el comentario de la visita.")]
        [Display(Name = "Comentarios")]
        public string Comentarios { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un estado.")]
        [Display(Name = "Estado de la Visita")]
        public string Estado { get; set; } = "Activa"; // Valor predeterminado

        [Required(ErrorMessage = "Debe ingresar la fecha de la visita.")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de la Visita")]
        public DateTime Fecha_Visita { get; set; }

        [Required(ErrorMessage = "Debe ingresar la hora de entrada.")]
        [DataType(DataType.Time)]
        [Display(Name = "Hora de Entrada")]
        public TimeSpan Hora_Entrada { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Hora de Salida")]
        public TimeSpan? Hora_Salida { get; set; }

        [Key]
        public int ID_Visita { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un visitado.")]
        [ForeignKey("Visitado")]
        [Display(Name = "Visitado")]
        public int ID_Visitado { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un visitante.")]
        [ForeignKey("Visitante")]
        [Display(Name = "Visitante")]
        public int ID_Visitante { get; set; }

        [Required(ErrorMessage = "Debe ingresar el motivo de la visita.")]
        [Display(Name = "Motivo de la Visita")]
        public string Motivo_Visita { get; set; }

        // Propiedades de navegación: no deben ser validadas
        public virtual Visitado Visitado { get; set; }

        public virtual Visitante Visitante { get; set; }
    }
}
