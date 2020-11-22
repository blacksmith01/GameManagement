using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameManagement
{
    public static class Defines
    {
        public enum DialogAction
        {
            None = 0,
            OK,
            Cancel,
            Create,
            Update,
            Remove,
        }
        public enum UserAssetType
        {
            None = 0,
            Block,
            Mail,
            Goods,
            Char
        }

        public enum CharAssetType
        {
            None = 0,
            Base,
            Skill,
            Item,
        }
    }
}
