using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.InventoryRecords
{
    public class InventoryRecordResponse
    {
        public Guid MovieId { get; set; }
        public List<InventoryRecordDataResponse> InventoryRecordData { get; set; } = [];
    }
}
