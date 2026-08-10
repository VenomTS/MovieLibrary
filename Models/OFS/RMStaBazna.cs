using System.ComponentModel.DataAnnotations;

namespace Models.OFS;

public abstract class RMStaBazna
{
    public int Id { get; set; }
    public int RMZagId { get; set; }
    public int ArtikalId { get; set; }
    [MaxLength(30)]
    public string? UneseniBarkod { get; set; }
    //18,8
    public decimal FC { get; set; }
    //18,6
    public decimal ZavisniNivelacija { get; set; }
    //18,8
    public decimal NC { get; set; }
    //18,8
    public decimal Marza { get; set; }
    //18,6
    public decimal VPC { get; set; }
    //18,2
    public decimal MPC { get; set; }
       
    public int TarifaId { get; set; }
    //18,2
    public decimal PorezPosto { get; set; }
    //18,10
    public decimal Porez { get; set; }
    [MaxLength(1000)]
    public string? DodatniOpis { get; set; }
    public short? Status { get; set; }
    public int? Otacid { get; set; }
    public int? VezananaStaId { get; set; }
}