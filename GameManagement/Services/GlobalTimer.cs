using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GameManagement.Services
{
    public class GlobalTimer
    {
        public abstract class Object : IComparable
        {
            public abstract void Ontimer();

            public int CompareTo(object obj)
            {
                var o = (obj as Object);
                if (timerTimeTicks > o.timerTimeTicks)
                    return 1;
                else if (o.timerTimeTicks > timerTimeTicks)
                    return -1;
                else if (timerSequence > o.timerSequence)
                    return 1;
                else if (o.timerSequence > timerSequence)
                    return -1;
                return 0;
            }

            public Int64 timerTimeTicks; /* GlobalTimer internal */
            public Int64 timerSequence; /* GlobalTimer internal */
        }
        private class FakeTimerObject : Object
        {
            public override void Ontimer()
            {
            }
        }

        private object mutex = new object();
        private SortedDictionary<Object, Object> dic = new SortedDictionary<Object, Object>();
        private Int64 topTime;
        private AutoResetEvent eventHandle = new AutoResetEvent(false);
        private Int64 timerSequence;
        public bool IsMarkedShutdown { get; private set; }
        public bool IsShutdownFinished { get; private set; }

        public GlobalTimer()
        {
            var ft = new FakeTimerObject { timerTimeTicks = Int64.MaxValue, timerSequence = Int64.MaxValue };
            dic.Add(ft, ft);
            topTime = Int64.MaxValue;
        }

        public void Push(Object obj, Int64 dueTimeMS)
        {
            obj.timerTimeTicks = DateTime.Now.Ticks + dueTimeMS * 10000;
            lock (mutex)
            {
                if(!IsMarkedShutdown)
                {
                    obj.timerSequence = ++timerSequence;
                    dic.Add(obj, obj);
                    if (topTime > obj.timerTimeTicks)
                    {
                        topTime = obj.timerTimeTicks;
                        eventHandle.Set();
                    }
                }
            }
        }

        public void Shutdown()
        {
            lock (mutex)
            {
                IsMarkedShutdown = true;
                eventHandle.Set();
            }   
        }

        public void Run()
        {
            new Thread(Run_Impl).Start();
        }

        private void Run_Impl()
        {
            while (true)
            {
                var currTime = DateTime.Now.Ticks;
                var waitTime = topTime - currTime;
                if (waitTime <= 0)
                {
                    lock (mutex)
                    {
                        while (dic.Count > 0)
                        {
                            var top = dic.First();
                            waitTime = top.Key.timerTimeTicks - currTime;

                            if (waitTime > 0)
                            {
                                topTime = top.Key.timerTimeTicks;
                                break;
                            }

                            topTime = 0;
                            var obj = top.Value;
                            dic.Remove(top.Key);
                            Task.Run(() => { obj.Ontimer(); });
                        }
                    }
                }

                var timeout = (waitTime / 10000);
                if (timeout > (Int64)int.MaxValue)
                    timeout = (Int64)int.MaxValue;

                var result = eventHandle.WaitOne((int)timeout);

                if (IsMarkedShutdown)
                    break;
            }

            lock (mutex)
            {
                foreach (var v in dic)
                {
                    Task.Run(() => { v.Value.Ontimer(); });
                }
                dic.Clear();

                IsShutdownFinished = true;
            }
        }

        
    }
}
