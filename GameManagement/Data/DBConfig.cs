using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GameManagement.Data
{
    public class DBConfig
    {
        public class System
        {
            [Owned]
            public class EmailServer
            {
                public string Host { get; set; }
                public int Port { get; set; }
                public bool EnableSSL { get; set; }
                public string UserName { get; set; }
                public string Password { get; set; }
            }

            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int Id { get; set; }
            public bool UsingEmailConfirm { get; set; }
            public EmailServer Email { get; set; }
        }
    }
}
