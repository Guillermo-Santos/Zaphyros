using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos.Core.Memory;
using Cosmos.System.Coroutines;

namespace Zaphyros.Core.Apps
{
    internal class TaskManager
    {
        private readonly Dictionary<int, Service> _services = new();
        private readonly Dictionary<int, Coroutine> _coroutines = new();
        private readonly Queue<(int PID, Service Service)> _queue = new();
        private readonly CoroutinePool _coroutinePool;
        private int PID_COUNT;

        public TaskManager()
        {
            _coroutinePool = CoroutinePool.Main;
            _coroutinePool.OnCoroutineCycle.Add(CheckStoppedServices);
            _coroutinePool.OnCoroutineCycle.Add(CheckNewServices);
            _coroutinePool.HeapCollectionInterval = 10_000;
        }

        public int RegisterService(Service service)
        {
            int PID;
            do
            {
                PID = PID_COUNT++;
            } 
            while (_services.ContainsKey(PID));

            return RegisterService(PID, service);
        }

        private int RegisterService(int PID, Service service)
        {
            service.PID = PID;
            _queue.Enqueue((PID, service));
            return PID;
        }

        private static IEnumerator<CoroutineControlPoint> CoroutineCicle(Service service)
        {
            service.Start();

            while (service.IsRunning)
            {
                try
                {
                    service.Update();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                    service.Stop();
                }
                yield return WaitFor.Nanoseconds(5);
            }
        }

        public void Run()
        {
            CheckNewServices();
            _coroutinePool.StartPool();
        }

        private void CheckStoppedServices()
        {
            //Sys.Global.Debugger.Send("Checking Stopped Services");
            Stack<int> keysToRemove = new();
            foreach (var service in _services)
            {
                if (service.Value.IsTermidated)
                {
                    Sys.Global.Debugger.Send("After Start");
                    try
                    {
                        service.Value.AfterStart();
                    }
                    catch(Exception ex)
                    {
                        Sys.Global.Debugger.Send(ex.ToString());
                    }
                    var PID = service.Key;
                    Sys.Global.Debugger.Send($"Adding Service to Remove: {PID}");
                    _coroutinePool.RemoveCoroutine(_coroutines[service.Key]);
                    keysToRemove.Push(PID);
                }
            }

            Sys.Global.Debugger.Send($"Removing Services");
            foreach (var key in keysToRemove)
            {
                Sys.Global.Debugger.Send($"Removing Service: {key}");
                _services.Remove(key);
                _coroutines.Remove(key);
            }
        }
        private void CheckNewServices()
        {
            Sys.Global.Debugger.Send("Checking New Services");
            while(_queue.Count > 0)
            {
                var service = _queue.Dequeue();
                _services.Add(service.PID, service.Service);
                var coroutine = new Coroutine(CoroutineCicle(service.Service));
                _coroutines.Add(service.PID, coroutine);
                _coroutinePool.AddCoroutine(coroutine);
            }
        }
    }
}
