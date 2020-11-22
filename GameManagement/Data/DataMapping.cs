using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameManagement.Data
{
    public static class DataMapping
    {
        public static void Copy(GameServer.Notice source, GameServer.Notice target)
        {
            target.id = source.id;
            target.name = source.name;
            target.typeid = source.typeid;
            target.cycle = source.cycle;
            target.duration = source.duration;
            target.servers = source.servers;
            target.starttime = source.starttime;
            target.endtime = source.endtime;
            target.message = source.message;
        }

        public static void Copy(GameUser.Block source, GameUser.Block target)
        {
            target.id = source.id;
            target.userid = source.userid;
            target.typeid = source.typeid;
            target.starttime = source.starttime;
            target.endtime = source.endtime;
            target.comment = source.comment;
        }

        public static void Copy(GameUser.Goods source, GameUser.Goods target)
        {
            target.id = source.id;
            target.userid = source.userid;
            target.count = source.count;
            target.timelimit = source.timelimit;
        }

        public static void Copy(GameUser.Mail.Attached source, GameUser.Mail.Attached target)
        {
            target.id = source.id;
            target.cnt = source.cnt;
        }

        public static void Copy(GameUser.Mail source, GameUser.Mail target)
        {
            target.id = source.id;
            target.userid = source.userid;
            target.typeid = source.typeid;
            target.senderid = source.senderid;
            target.title = source.title;
            target.message = source.message;
            target.sentdate = source.sentdate;
            target.isrecv = source.isrecv;
            target.isread = source.isread;
            for (int i = 0; i < GameUser.Mail.MaxItemCount; i++)
                Copy(source.attached[i], target.attached[i]);
        }

        public static void Copy(GameChar source, GameChar target)
        {
            target.id = source.id;
            target.name = source.name;
            target.userid = source.userid;
            target.typeid = source.typeid;
            target.level = source.level;
            target.exp = source.exp;
            target.gold = source.gold;
            target.regdate = source.regdate;
            target.playtime = source.playtime;
        }

        public static void Copy(GameChar.Skill source, GameChar.Skill target)
        {
            target.id = source.id;
            target.charid = source.charid;
            target.level = source.level;
            target.cooltime = source.cooltime;
        }

        public static void Copy(GameChar.Item source, GameChar.Item target)
        {
            target.id = source.id;
            target.charid = source.charid;
            target.count = source.count;
            target.cooltime = source.cooltime;
        }
    }
}
