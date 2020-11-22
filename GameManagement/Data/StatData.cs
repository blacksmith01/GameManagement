using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GameManagement.Data
{
    public class StatData
    {
        public class Daily
        {
            [Key]
            [DataType(DataType.Date)]
            public DateTime date { get; set; }
            public int NRU { get; set; }
            public int AU { get; set; }
            public int CCU { get; set; }
            public int PU { get; set; }
            public int NPU { get; set; }
            public int Sales { get; set; }
        }
    }
}
