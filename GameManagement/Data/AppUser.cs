using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GameManagement.Data
{
    public class AppUser
    {
        public const string DefaultAdminUserName = "Admin";
        public const string DefaultAdminUserPassword = "Admin1!";

        public class Role
        {
            public const string Admin = "Admin";
            public const string Dev = "Dev";
            public const string SeniorGM = "SeniorGM";
            public const string JuniorGM = "JuniorGM";
            public const string Staff = "Staff";
            public static string[] Names = new string[] { Admin, Dev, SeniorGM, JuniorGM, Staff };

            public static Dictionary<string, string> IdNameMap = new Dictionary<string, string>(); 
            public static Dictionary<string, string> NameIdMap = new Dictionary<string, string>();
        }

        [Table("AspNetUserExInformations")]
        public class ExtraInfo
        {
            [ForeignKey("AspNetUsersId")]
            public string ID { get; set; }

            public DateTime RegisterTime { get; set; }
        }

        [Table("AspNetUserHistories")]
        public class History
        {
            public enum Types
            {
                None = 0,
                
                GMLogin,
                GMLogout,
                EditConfig,
                
                NoticeCreate = 1000,
                NoticeRemove,
                NoticeUpdate,

                BlockCreate = 1010,
                BlockRemove,
                BlockUpdate,

                MailCreate = 1020,
                MailRemove,
                MailUpdate,

                GoodsCreate = 1030,
                GoodsRemove,
                GoodsUpdate,

                SkillCreate = 2000,
                SkillRemove,
                SkillUpdate,

                ItemCreate = 2010,
                ItemRemove,
                ItemUpdate,

                BulkMail = 3000,
            }

            [Key]
            public Int64 ID { get; set; }
            [Column(TypeName = "varchar(64)")]
            public string UserID { get; set; }
            [Column(TypeName = "varchar(20)")]
            public string UserName { get; set; }
            public Types TypeID { get; set; }
            public DateTime Time { get; set; }
            public string Detail { get; set; }
        }

    }
}
