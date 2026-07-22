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
    }
}
