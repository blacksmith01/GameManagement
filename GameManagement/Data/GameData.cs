using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;

namespace GameManagement.Data
{
    public class AgentEvent
    {
        public enum TypeID
        {
            None = 0,

            NoticeCreate, // p1: notice_id
            NoticeRemove, // p1: notice_id
            NoticeUpdate, // p1: notice_id

            BlockCreate, // p1: block_id
            BlockRemove, // p1: block_id
            BlockUpdate, // p1: block_id

            MailCreate, // p1: mail_id
            MailRemove, // p1: mail_id
            MailUpdate, // p1: mail_id
            MailCreateAll, // p1: mail_id
        }
        public Int64 id { get; set; }
        public TypeID typeid { get; set; }
        public Int64 param1 { get; set; }
        public Int64 param2 { get; set; }
        public Int64 param3 { get; set; }
        public Int64 param4 { get; set; }
        public Int64 param5 { get; set; }

        public static AgentEvent Create(TypeID _typeID)
        {
            return new AgentEvent() { typeid = _typeID };
        }
        public static AgentEvent Create(TypeID _typeID, Int64 _p1)
        {
            return new AgentEvent() { typeid = _typeID, param1 = _p1 };
        }
        public static AgentEvent Create(TypeID _typeID, Int64 _p1, Int64 _p2)
        {
            return new AgentEvent() { typeid = _typeID, param1 = _p1, param2 = _p2 };
        }
    }
    public class GameServer
    {
        public enum TypeID
        {
            None = 0,
            Auth,
            Lobby,
            Game,
        }
        public enum State
        {
            Unavailable = 0,
            Active,
            Mainternance,
        }
        public int id { get; set; }
        public string name { get; set; }
        public TypeID typeid { get; set; }
        public string ip { get; set; }
        public int port { get; set; }
        public State state { get; set; }
        public int usercount { get; set; }
        public DateTime lastupdate { get; set; }

        public class Notice
        {
            public enum TypeID
            {
                Screen = 0,
                Chat,
            }

            public int id { get; set; }
            [Required]
            [StringLength(maximumLength: 50, MinimumLength = 5)]
            public string name { get; set; }
            public TypeID typeid { get; set; }
            public int cycle { get; set; }
            [Required]
            [Range(1, 60)]
            public int duration { get; set; }
            [Required]
            public string servers { get; set; }
            public DateTime starttime { get; set; }
            public DateTime endtime { get; set; }
            [Required]
            [StringLength(255)]
            public string message { get; set; }
        }
    }

    public class GameUser
    {
        public enum Grade
        {
            None = 0,
            User,
            GM,
            Dev
        }

        public Int64 id { get; set; }
        public string name { get; set; }
        public Grade grade { get; set; }
        public int loginseq { get; set; }
        public bool isblocked { get; set; }
        public DateTime regdate { get; set; }
        public DateTime lastlogin { get; set; }
        public DateTime lastlogout { get; set; }

        public bool IsLogon()
        {
            return lastlogin > lastlogout;
        }

        public List<Goods> Goodses { get; set; }
        public List<Block> Blocks { get; set; }
        public List<Mail> Mails { get; set; }
        public List<GameChar> Chars { get; set; }
        public class Goods
        {
            [Required]
            [Range(1, Int16.MaxValue)]
            public int id { get; set; }
            public Int64 userid { get; set; }
            [Required]
            [Range(1, 255)]
            public int count { get; set; }
            public int timelimit { get; set; }
        }
        public class Block
        {
            public enum TypeID
            {
                None = 0,
                Auth,
                Chat,
                Matching,
            }
            public Int64 id { get; set; }
            public Int64 userid { get; set; }
            [Required]
            [Range(1, int.MaxValue)]
            public TypeID typeid { get; set; }
            public DateTime starttime { get; set; }
            public DateTime endtime { get; set; }
            [Required]
            public string comment { get; set; }
        }
        public class Mail
        {
            public enum TypeID
            {
                None = 0,
                System,
                Maintenance,
                Block,
                Event,
                Private,
            }
            public const int MaxItemCount = 5;
            public class Attached
            {
                public int id { get; set; }
                public int cnt { get; set; }
            }

            public Int64 id { get; set; }
            public Int64 userid { get; set; }
            [Required]
            [Range(1, int.MaxValue)]
            public TypeID typeid { get; set; }
            public Int64 senderid { get; set; }
            [Required]
            public string title { get; set; }
            public string message { get; set; }
            public DateTime sentdate { get; set; }
            public bool isrecv { get; set; }
            public bool isread { get; set; }
            public int item1id { get => attached[0].id; set => attached[0].id = value; }
            public int item2id { get => attached[1].id; set => attached[1].id = value; }
            public int item3id { get => attached[2].id; set => attached[2].id = value; }
            public int item4id { get => attached[3].id; set => attached[3].id = value; }
            public int item5id { get => attached[4].id; set => attached[4].id = value; }
            public int item1cnt { get => attached[0].cnt; set => attached[0].cnt = value; }
            public int item2cnt { get => attached[1].cnt; set => attached[1].cnt = value; }
            public int item3cnt { get => attached[2].cnt; set => attached[2].cnt = value; }
            public int item4cnt { get => attached[3].cnt; set => attached[3].cnt = value; }
            public int item5cnt { get => attached[4].cnt; set => attached[4].cnt = value; }
            [NotMapped]
            public Attached[] attached = ArrayEx.CreateWithNew<Attached>(MaxItemCount);
        }
    }

    public class GameChar
    {
        public const int MinCharName = 2;
        public const int MaxCharName = 10;

        public enum TypeID
        {
            Newbie = 0,
            Warrior,
            Archer,
            Mage
        }
        public Int64 id { get; set; }
        [Required]
        [StringLength(MaxCharName, MinimumLength = MinCharName)]
        public string name { get; set; }
        public Int64 userid { get; set; }
        [Required]
        public TypeID typeid { get; set; }
        public int level { get; set; }
        public Int64 exp { get; set; }
        public Int64 gold { get; set; }
        public DateTime regdate { get; set; }
        public int playtime { get; set; }

        public class Skill
        {
            [Required]
            [Range(1, Int16.MaxValue)]
            public int id { get; set; }
            public Int64 charid { get; set; }
            [Required]
            [Range(1, 10)]
            public int level { get; set; }
            public int cooltime { get; set; }
        }
        public class Item
        {
            [Required]
            [Range(1, Int16.MaxValue)]
            public int id { get; set; }
            public Int64 charid { get; set; }
            [Required]
            [Range(1, int.MaxValue)]
            public int count { get; set; }
            public int cooltime { get; set; }
        }
    }

    public class GameLog
    {
        public enum TypeID
        {
            None = 0,
            UserRegister,
            CharCreate,
            CharRemove,
        }

        public Int64 id { get; set; }
        public TypeID typeid { get; set; }
        public Int64 userid { get; set; }
        public Int64 charid { get; set; }
        public DateTime logtime { get; set; }
        public string detail { get; set; }
    }
}
