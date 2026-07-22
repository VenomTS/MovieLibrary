namespace Models;

public class Stock
{
    public Guid MovieId { get; set; }
    public int Amount { get; set; }

    // Promijeniti tako da Stock bude input
    /*
     * 
     * Basically, Rentals je grupa svih izlazaka filma iz biblioteke
     * A Stock bi bio Id | MovieId | Datum | Kolicina - Znaci, za svaki dan se prati koliko filmova je uslo u biblioteku taj dan
     * Onda, kada neko pokusa rentati film,
     * uzmu se svi rentals koji NISU resolved (DateReturned == null) i SUM() WHERE MovieId == MovieId && Datum < Today 
     * (librarian moze unijeti filmove prijevremeno iz nekog razloga)
     * 
     * */

    public Movie Movie { get; set; }
}