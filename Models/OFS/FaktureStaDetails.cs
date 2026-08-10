using System.ComponentModel.DataAnnotations;

namespace Models.OFS;

public class FaktureStaDetails : FaktStaDTO
    {

        public FaktureStaDetails() { }

        public FaktureStaDetails(FaktStaDTO original) : base(original)
        {
            this.Id = original.Id;
            this.RMZagId = original.RMZagId;
            this.ArtikalId = original.ArtikalId;
            this.Izlaz = original.Izlaz;
            this.UneseniBarkod = original.UneseniBarkod;
            this.FC = original.FC;
            this.NC = original.NC;
            this.Marza = original.Marza;
            this.VPC = original.VPC; //CIJENABEZ
            this.TarifaId = original.TarifaId;
            this.Porez = original.Porez;
            this.PorezPosto = original.PorezPosto;
            this.MPC = original.MPC;    //CIJENASA
            this.PopustPosto = original.PopustPosto;
            this.PopustVrijednost = original.PopustVrijednost;
            this.Popust01 = original.Popust01;
            this.Popust02 = original.Popust02;
            this.Popust03 = original.Popust03;
            this.PopustNaziv01 = original.PopustNaziv01;
            this.PopustNaziv02 = original.PopustNaziv02;
            this.PopustNaziv03 = original.PopustNaziv03;
            this.IzlazNV = original.IzlazNV;
            this.IzlazMarzaVrijednost = original.IzlazMarzaVrijednost;
            this.IzlazVPV = original.IzlazVPV; //"CIJENABEZ") *("KOL")
            this.IzlazPorezVrijednost = original.IzlazPorezVrijednost;
            this.IzlazMPV = original.IzlazMPV;//("CIJENASA") * ("KOL")
            this.DodatniOpis = original.DodatniOpis;
            this.Garancija = original.Garancija;
            this.Otacid = original.Otacid;
            this.PosDoktorJMBG = original.PosDoktorJMBG;
            this.PosPrintOrder = original.PosPrintOrder;
            this.PosUdioZavoda = original.PosUdioZavoda;
            this.ReceptApoteke = original.ReceptApoteke;
            this.Status = original.Status;
            this.VezananaStaId = original.VezananaStaId;
            // this.CustomIzlazMPV = original.CustomIzlazMPV;
        }

        [MaxLength(13)]
        public string Sifra { get; set; } = null!;
        [MaxLength(150)]
        public string Naziv { get; set; } = null!;
        [MaxLength(30)]
        public string? KataloskiBroj { get; set; }
        [MaxLength(8)]
        public string? Jm { get; set; }
        [MaxLength(30)]
        public string? Zamjenski { get; set; }
        /// <summary>
        /// Nema ovog polja u bazi, samo se koristi za preracune
        /// </summary>
        public decimal MarzaPosto { get; set; }
        /// <summary>
        /// ovo mi treba samo kada saljem fiskalnim uredjajima i nikad vise ali sta ces !
        /// </summary>
        public string TarifaSifra { get; set; } = null!;

        /// <summary>
        /// Minimalna marza koja se provjerava na preracunu stavke
        /// </summary>
        public decimal MinMarza { get; init; }

        /// <summary>
        /// bivse polje Marka
        /// </summary>
        [MaxLength(100)]
        public string? Atribut01 { get; set; }
        /// <summary>
        /// bivse polje Rang
        /// </summary>
        [MaxLength(100)]
        public string? Atribut02 { get; set; }
        /// <summary>
        /// bivse polje Brend
        /// </summary>
        [MaxLength(100)]
        public string? Atribut03 { get; set; }
        /// <summary>
        /// bivse polje OrgBroj
        /// </summary>
        [MaxLength(100)]
        public string? Atribut04 { get; set; }
        /// <summary>
        /// bivse polje TekDok
        /// </summary>
        [MaxLength(100)]
        public string? Atribut05 { get; set; }
        /// <summary>
        /// bivse polje PLUVage
        /// </summary>
        [MaxLength(100)]
        public string? Atribut06 { get; set; }
        [MaxLength(100)]
        public string? Atribut07 { get; set; }
        [MaxLength(100)]
        public string? Atribut08 { get; set; }
        [MaxLength(100)]
        public string? Atribut09 { get; set; }
        [MaxLength(100)]
        public string? Atribut10 { get; set; }

        /// <summary>
        /// Na populate se postavlja na true ako je artikal u grupi koja ima slobodno zaokruzivanje ili od skladista
        /// </summary>
        public bool SlobodnoZaokruzivanje { get; set; }
        public int GrupaId { get; set; }
        public string GrupaNaziv { get; set; } = null!;
        
    }
