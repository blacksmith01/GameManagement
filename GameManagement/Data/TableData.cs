using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;


namespace GameManagement.Data
{
    using GMHistoryTID = AppUser.History.Types;
    using AgentEventTID = AgentEvent.TypeID;
    using NoticeTID = GameServer.Notice.TypeID;
    using BlockTID = GameUser.Block.TypeID;
    using MailTID = GameUser.Mail.TypeID;
    using GameLogTID = GameLog.TypeID;

    public class TableData
    {
        public enum SysMessegeTypeID
        {
            None,
            System_Update,
            Error_UserBlockState,
        }

        public class NameItem
        {
            public int Id { get { return (int)Id64; } set { Id64 = value; } }
            public Int64 Id64 { get; set; }
            public string Name { get; set; }
        }

        public class NameItem<T> where T : Enum
        {
            public T Id { get; set; }
            public string Name { get; set; }
        }
        public class NameMap<T> : Dictionary<T, NameItem<T>> where T : Enum
        {
            public NameMap(string header)
            {
                foreach (var e in EnumEx.GetEnumerable<T>())
                    TryAdd(e, new NameItem<T> { Id = e, Name = $"{header}_{e.ToString()}" });
            }
            public string GetValue(T e)
            {
                if (TryGetValue(e, out var v))
                    return v.Name;
                return $"UNKNOWN_{e.ToString()}";
            }
        }

        public static readonly NameMap<SysMessegeTypeID> sysmessages = new NameMap<SysMessegeTypeID>("SysMessage");
        public static readonly NameMap<AgentEventTID> agentevents = new NameMap<AgentEventTID>("Event");
        public static readonly NameMap<NoticeTID> notices = new NameMap<NoticeTID>("Notice");
        public static readonly NameMap<BlockTID> blocks = new NameMap<BlockTID>("Block");
        public static readonly NameMap<MailTID> mails = new NameMap<MailTID>("Mail");
        public static readonly NameMap<GameLogTID> gamelogs = new NameMap<GameLogTID>("GameLog");
        public static readonly NameMap<GMHistoryTID> gmhistories = new NameMap<GMHistoryTID>("Event");

        public static readonly List<NameItem> goodsList = Enumerable.Range(1, 10).Select(n => new NameItem { Id = n, Name = $"GoodsName_{n.ToString()}" }).ToList();
        public static readonly List<NameItem> charskillList = Enumerable.Range(1, 10).Select(n => new NameItem { Id = n, Name = $"CharSkillName_{n.ToString()}" }).ToList();
        public static readonly List<NameItem> charitemList = Enumerable.Range(1, 10).Select(n => new NameItem { Id = n, Name = $"CharItemName_{n.ToString()}" }).ToList();

        public static readonly Dictionary<int, NameItem> goods = goodsList.ToDictionary(n => n.Id);
        public static readonly Dictionary<int, NameItem> charskills = charskillList.ToDictionary(n => n.Id);
        public static readonly Dictionary<int, NameItem> charitems = charitemList.ToDictionary(n => n.Id);


        public static GMHistoryTID GetHistoryType(Defines.UserAssetType assetType, Defines.DialogAction action)
        {
            switch (assetType)
            {
                case Defines.UserAssetType.Block: 
                    switch(action)
                    {
                        case Defines.DialogAction.Create: return GMHistoryTID.BlockCreate;
                        case Defines.DialogAction.Remove: return GMHistoryTID.BlockRemove;
                        case Defines.DialogAction.Update: return GMHistoryTID.BlockUpdate;
                    }break;
                        
                case Defines.UserAssetType.Mail:
                    switch (action)
                    {
                        case Defines.DialogAction.Create: return GMHistoryTID.MailCreate;
                        case Defines.DialogAction.Remove: return GMHistoryTID.MailRemove;
                        case Defines.DialogAction.Update: return GMHistoryTID.MailUpdate;
                    }
                    break;
                case Defines.UserAssetType.Goods:
                    switch (action)
                    {
                        case Defines.DialogAction.Create: return GMHistoryTID.MailCreate;
                        case Defines.DialogAction.Remove: return GMHistoryTID.MailRemove;
                        case Defines.DialogAction.Update: return GMHistoryTID.MailUpdate;
                    }
                    break;
            }
            return GMHistoryTID.None;
        }
        public static GMHistoryTID GetHistoryType(Defines.CharAssetType assetType, Defines.DialogAction action)
        {
            switch (assetType)
            {
                case Defines.CharAssetType.Skill:
                    switch (action)
                    {
                        case Defines.DialogAction.Create: return GMHistoryTID.SkillCreate;
                        case Defines.DialogAction.Remove: return GMHistoryTID.SkillRemove;
                        case Defines.DialogAction.Update: return GMHistoryTID.SkillUpdate;
                    }
                    break;

                case Defines.CharAssetType.Item:
                    switch (action)
                    {
                        case Defines.DialogAction.Create: return GMHistoryTID.ItemCreate;
                        case Defines.DialogAction.Remove: return GMHistoryTID.ItemRemove;
                        case Defines.DialogAction.Update: return GMHistoryTID.ItemUpdate;
                    }
                    break;
            }
            return GMHistoryTID.None;
        }
    }
}
