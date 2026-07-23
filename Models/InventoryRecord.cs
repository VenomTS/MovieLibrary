using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class InventoryRecord
    {
        public Guid Id { get; set; }
        public Guid MovieId { get; set; }
        public int Amount { get; set; }
        public DateOnly Date { get; set; }


        public Movie Movie { get; set; }
        
        // Promijeniti tako da Stock bude input
        /*
         *
         * Basically, Rentals je grupa svih izlazaka filma iz biblioteke
         * A Stock bi bio Id | MovieId | Datum | Kolicina - Znaci, za svaki dan se prati koliko filmova je uslo u biblioteku taj dan
         * Onda, kada neko pokusa rentati film,
         * uzmu se svi rentals koji NISU resolved (DateReturned == null) i SUM() WHERE MovieId == MovieId && Datum < Today
         * (librarian moze unijeti filmove prijevremeno iz nekog razloga)
         * NOTE: Rental Returns NE UDATE-uju Stock, jer ce to bit implicit kroz DateReturned != Null
         *
         * 10 Filmova in Library
         * 2 su rentala
         * 5 Filmova dodano (doslo iz Indonezije)
         * 1 je vracen
         * 3 Filma dodana (doslo iz Pakistana)
         * 1 pokusava rentat
         * Isto tako, dobro bi bilo dodati opciju da budu izgubljeni / osteceni
         *
         * Available: 10 - 2 + 1 = 9 => 10 - 1 (jer 10 filmova in library, 2 su rentana ali je jedan DateReturned != NULL, sto znaci 1 je rentan)
         *
         * Rename to InventoryEntry il nesto
         * */
    }
}
