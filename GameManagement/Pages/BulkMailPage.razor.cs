using FluentValidation;
using GameManagement.Data;
using GameManagement.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GameManagement.Pages
{
    public partial class BulkMailPage
    {
        public class ModelBulkMail
        {
            public bool IsTargetAll { get; set; } = true;
            public string TargetQuery { get; set; }
            public Dictionary<Int64, string> Users { get; set; } = new Dictionary<long, string>();
            public GameUser.Mail Mail { get; set; } = new GameUser.Mail();
        }

        public class ModelBulkMailValidator : AbstractValidator<ModelBulkMail>
        {
            public ModelBulkMailValidator(IStringLocalizer<SharedResources> L, IValidator<GameUser.Mail> mailValidator)
            {
                RuleFor(x => x.TargetQuery).NotEmpty().When(x => x.IsTargetAll == false);
                RuleFor(x => x.Mail).SetValidator(mailValidator);
            }
        }

        class SendMailRequest
        {
            public DbContextOptions<GameDbContext> dbo_game;
            public GameUser.Mail mail;
            public List<Int64> targetUsers;

            public bool Action(CancellationToken ct)
            {
                if (ct.IsCancellationRequested)
                {
                    return false;
                }

                if (IEnumerableEx.IsNullOrEmpty(targetUsers))
                {
                    using (var db = new GameDbContext(dbo_game))
                    {
                        db.Database.ExecuteSqlRaw(
                            "INSERT INTO `game_mails`(`userid`, `typeid`, `title`, `message`, `sentdate`, " +
                            "`item1id`, `item2id`, `item3id`, `item4id`, `item5id`, `item1cnt`, `item2cnt`, `item3cnt`, `item4cnt`, `item5cnt`) " +
                            "SELECT `id`, {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13} FROM `game_users`",
                            (int)mail.typeid, mail.title, mail.message, mail.sentdate.ToDBFormatString(), mail.item1id, mail.item2id, mail.item3id, mail.item4id, mail.item5id,
                            mail.item1cnt, mail.item2cnt, mail.item3cnt, mail.item4cnt, mail.item5cnt);

                        db.agent_events.Add(AgentEvent.Create(AgentEvent.TypeID.MailCreateAll));
                        db.SaveChanges();
                    }
                }
                else
                {
                    var mails = new List<GameUser.Mail>();
                    foreach (var userid in targetUsers)
                    {
                        var newmail = new GameUser.Mail();
                        DataMapping.Copy(mail, newmail);
                        newmail.userid = userid;
                        mails.Add(newmail);
                    }

                    using (var db = new GameDbContext(dbo_game))
                    {
                        db.ChangeTracker.AutoDetectChangesEnabled = false;
                        db.game_mails.AddRange(mails);
                        db.ChangeTracker.AutoDetectChangesEnabled = true;

                        if (ct.IsCancellationRequested)
                        {
                            return false;
                        }
                        db.SaveChanges();

                        db.agent_events.Add(AgentEvent.Create(AgentEvent.TypeID.MailCreate, mails.First().id, mails.Last().id));
                        db.SaveChanges();
                    }
                }

                return true;
            }
        }
    }
}
