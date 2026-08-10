using System.ComponentModel.DataAnnotations;

namespace Models.OFS;

public class FaktStaDTO : RMStaBazna
    {
        private FaktStaDTO? original;
        public FaktStaDTO() { }
        
        public FaktStaDTO(FaktStaDTO original)
        {
            this.original = original;

            Id = original.Id;
            RMZagId = original.RMZagId;
            ArtikalId = original.ArtikalId;
            UneseniBarkod = original.UneseniBarkod;
            FC = original.FC;
            ZavisniNivelacija = original.ZavisniNivelacija;
            NC = original.NC;
            Marza = original.Marza;
            VPC = original.VPC;
            MPC = original.MPC;
            TarifaId = original.TarifaId;
            PorezPosto = original.PorezPosto;
            Porez = original.Porez;
            DodatniOpis = original.DodatniOpis;
            Status = original.Status;
            Otacid = original.Otacid;
            VezananaStaId = original.VezananaStaId;

            Izlaz = original.Izlaz;
            PopustPosto = original.PopustPosto;
            PopustVrijednost = original.PopustVrijednost;
            Popust01 = original.Popust01;
            Popust02 = original.Popust02;
            Popust03 = original.Popust03;
            PopustNaziv01 = original.PopustNaziv01;
            PopustNaziv02 = original.PopustNaziv02;
            PopustNaziv03 = original.PopustNaziv03;
            IzlazNV = original.IzlazNV;
            IzlazMarzaVrijednost = original.IzlazMarzaVrijednost;
            IzlazVPV = original.IzlazVPV;
            IzlazPorezVrijednost = original.IzlazPorezVrijednost;
            IzlazMPV = original.IzlazMPV;
            ReceptApoteke = original.ReceptApoteke;
            Garancija = original.Garancija;
            PosPrintOrder = original.PosPrintOrder;
            PosUdioZavoda = original.PosUdioZavoda;
            PosDoktorJMBG = original.PosDoktorJMBG;
        }

        //18,6
        public decimal Izlaz { get; set; }
        //18,6
        public decimal PopustPosto { get; set; }
        //18,2
        public decimal PopustVrijednost { get; set; }
        //9,2
        public decimal? Popust01 { get; set; }
        public decimal? Popust02 { get; set; }
        public decimal? Popust03 { get; set; }

        [MaxLength(38)]
        public string? PopustNaziv01 { get; set; }
        [MaxLength(38)]
        public string? PopustNaziv02 { get; set; }
        [MaxLength(38)]
        public string? PopustNaziv03 { get; set; }
        //18,2
        public decimal IzlazNV { get; set; }
        //18,2
        public decimal IzlazMarzaVrijednost { get; set; }
        //18,2
        public decimal IzlazVPV { get; set; }
        //18,2
        public decimal IzlazPorezVrijednost { get; set; }
        //18,2
        public decimal IzlazMPV { get; set; }

        [MaxLength(20)]
        public string? ReceptApoteke { get; set; }
        [MaxLength(50)]
        public string? Garancija { get; set; }
        public short? PosPrintOrder { get; set; }
        //18,6
        public decimal? PosUdioZavoda { get; set; }
        [MaxLength(13)]
        public string? PosDoktorJMBG { get; set; }
    }
