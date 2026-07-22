using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.InventoryRecords
{
    public class CreateInventoryRecordRequest
    {
        public Guid MovieId { get; set; }
        public int Amount { get; set; }
        public DateOnly? Date { get; set; }
    }
}
