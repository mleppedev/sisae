using sisae.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class AccesoProhibido
{
    [DataType(DataType.Date)]
    public DateTime? Fecha_Expiracion { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime Fecha_Prohibicion { get; set; }

    [Key]
    public int ID_Acceso_Prohibido { get; set; }

    [Required]
    [ForeignKey("Visitante")]
    public int ID_Visitante { get; set; }

    [Required]
    public string Motivo { get; set; }

    public Visitante Visitante { get; set; }
}