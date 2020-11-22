using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameManagement.Data
{
    public class GameUserGoodsValidator : AbstractValidator<GameUser.Goods>
    {
        public GameUserGoodsValidator()
        {
            RuleFor(x => x.id).NotEmpty();
            RuleFor(x => x.userid).NotEmpty();
            RuleFor(x => x.count).NotEmpty();
        }
    }
    public class GameUserMailValidator : AbstractValidator<GameUser.Mail>
    {
        public GameUserMailValidator()
        {
            RuleFor(x => x.typeid).NotEmpty();
            RuleFor(x => x.title).NotEmpty();
            RuleFor(x => x.item1cnt).GreaterThan(0).When(x => x.item1id > 0);
            RuleFor(x => x.item2cnt).GreaterThan(0).When(x => x.item2id > 0);
            RuleFor(x => x.item3cnt).GreaterThan(0).When(x => x.item3id > 0);
            RuleFor(x => x.item4cnt).GreaterThan(0).When(x => x.item4id > 0);
            RuleFor(x => x.item5cnt).GreaterThan(0).When(x => x.item5id > 0);
        }
    }

    public class DBConfigSystemMailValidator : AbstractValidator<DBConfig.System.EmailServer>
    {
        public DBConfigSystemMailValidator()
        {
            RuleFor(x => x.Host).NotEmpty();
            RuleFor(x => x.Port).GreaterThan(0).LessThan(UInt16.MaxValue+1);

            RuleFor(x => x.UserName).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
        }
    }

    public class DBConfigSystemValidator : AbstractValidator<DBConfig.System>
    {
        public DBConfigSystemValidator(IValidator<DBConfig.System.EmailServer> mailValidator)
        {
            RuleFor(x => x.Email).SetValidator(mailValidator).When(x=>x.UsingEmailConfirm==true);
        }
    }
}
