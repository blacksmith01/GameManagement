using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GameManagement.Services
{
    public class BulkAction
    {
        public enum State
        {
            None = 0,
            Wait,
            Running,
            Complete,
            Canceled,
        }
        public int id { get; set; }
        public string title { get; set; }
        public State state { get; set; }
        public DateTime requestTime { get; set; }
        public DateTime completeTime { get; set; }
    }

    static class BulkActionEx
    {
        public static bool IsFinished(this BulkAction.State state)
        {
            switch (state)
            {
                case BulkAction.State.Complete:
                case BulkAction.State.Canceled: return true;
                default: return false;
            }
        }
    }

    public interface IBulkActionService
    {
        Task<int> Add(string title, Func<CancellationToken, bool> request);
        int LastActionID();
        Task<IReadOnlyCollection<BulkAction>> RetrieveInfomations();
    }

    public class BulkActionService : IBulkActionService, IDisposable
    {
        const int MaxCompleteItemCount = 3;

        private object mutex = new object();
        Thread thread;
        AutoResetEvent arevent;
        bool markedShutdown;
        int idalloc;
        public int RunningActionID { get; private set; }
        public class BulkActionInternal : BulkAction, IDisposable
        {
            public Func<CancellationToken, bool> request;
            public CancellationTokenSource cts;
            public void Dispose()
            {
                cts?.Dispose();
            }
        }
        List<BulkActionInternal> actions = new List<BulkActionInternal>();

        public void Start()
        {
            arevent = new AutoResetEvent(false);
            thread = new Thread(Onthread);
            thread.Start();
        }
        public async Task Stop()
        {
            markedShutdown = true;
            arevent.Set();
            await Task.Run(() => thread.Join());
            arevent.Dispose();
            arevent = null;
            thread = null;
        }
        public void Onthread()
        {
            while (true)
            {
                arevent.WaitOne();

                BulkActionInternal running = null;
                bool isComplate = false;
                while (true)
                {
                    lock (mutex)
                    {
                        if (running != null)
                        {
                            running.state = isComplate ? BulkAction.State.Complete : BulkAction.State.Canceled;
                            running.completeTime = DateTime.Now;
                            running = null;
                            isComplate = false;
                            RunningActionID = 0;

                            var compCount = actions.Where(a => a.state.IsFinished()).Count();
                            if (compCount > MaxCompleteItemCount)
                            {
                                actions.RemoveRange(0, compCount - MaxCompleteItemCount);
                            }
                        }

                        if (markedShutdown)
                            break;

                        running = actions.Where(a => a.state == BulkAction.State.Wait).FirstOrDefault();
                        if (running == null)
                            break;

                        running.state = BulkAction.State.Running;
                        RunningActionID = running.id;
                    }

                    try
                    {
                        isComplate = running.request(running.cts.Token);
                    }
                    catch (Exception e)
                    {
                        //running.request.IsCanceled();
                    }
                }

                if (markedShutdown)
                {
                    break;
                }
            }
        }

        public async Task<int> Add(string title, Func<CancellationToken, bool> request)
        {
            var newaction = new BulkActionInternal()
            {
                title = title,
                requestTime = DateTime.Now,
                request = request,
                cts = new CancellationTokenSource(),
            };

            return await Task<int>.Run(() =>
            {
                int newid;
                bool isFirst = false;
                lock (mutex)
                {
                    newid = ++idalloc;
                    newaction.id = newid;
                    newaction.state = BulkAction.State.Wait;
                    actions.Add(newaction);
                    isFirst = actions.Where(a => a.state == BulkAction.State.Wait).Count() == 1;
                }

                if (isFirst)
                {
                    arevent.Set();
                }

                return newid;
            });
        }

        public async Task<IReadOnlyCollection<BulkAction>> RetrieveInfomations()
        {
            List<BulkAction> result = null;
            await Task.Run(() =>
            {
                lock (mutex)
                {
                    result = actions.Select(s =>
                    new BulkAction()
                    {
                        id = s.id,
                        title = s.title,
                        state = s.state,
                        requestTime = s.requestTime,
                        completeTime = s.completeTime,
                    }).ToList();
                }
            });
            return result;
        }

        public async Task<bool> Cancel(int id)
        {
            return await Task<bool>.Run(() =>
            {
                lock (mutex)
                {
                    var action = actions.Where(e => e.id == id).FirstOrDefault();
                    if (action == null || action.cts.Token.IsCancellationRequested)
                        return false;

                    action.cts.Cancel();
                    if (action.state == BulkAction.State.Wait)
                    {
                        actions.Remove(action);
                    }
                }
                return true;
            });
        }

        public int LastActionID()
        {
            return idalloc;
        }

        public void Dispose()
        {
            foreach (var elm in actions)
            {
                elm.Dispose();
            }
        }
    }
}
