using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.InventoryRecords
{
    public class InventoryRecordDataResponse
    {
        public int Amount { get; set; }
        public DateOnly Date { get; set; }
    }
}
