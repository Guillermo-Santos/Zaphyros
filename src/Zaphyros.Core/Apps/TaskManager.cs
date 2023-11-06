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
        private readonly Dictionary<string, Service> _services = new();
        private readonly Dictionary<string, Coroutine> _coroutines = new();
        private readonly Queue<(string key, Service Service)> _queue = new();
        private readonly CoroutinePool _coroutinePool;

        public TaskManager()
        {
            _coroutinePool = CoroutinePool.Main;
            _coroutinePool.OnCoroutineCycle.Add(CheckStoppedServices);
            _coroutinePool.OnCoroutineCycle.Add(CheckNewServices);
            _coroutinePool.HeapCollectionInterval = 10_000;
        }

        public string RegisterService(Service service)
        {
            string key;
            do
            {
                key = Guid.NewGuid().ToString();
            } 
            while (_services.ContainsKey(key));

            return RegisterService(key, service);
        }

        public string RegisterService(string key, Service service)
        {
            _queue.Enqueue((key, service));
            //_services.Add(key, service);
            //_coroutinePool.AddCoroutine(_coroutines[key]);
            return key;
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
            Stack<string> keysToRemove = new();
            foreach (var service in _services)
            {
                if (service.Value.IsTermidated)
                {
                    Sys.Global.Debugger.Send("After Start");
                    service.Value.AfterStart();
                    var key = service.Key;
                    Sys.Global.Debugger.Send($"Adding Service to Remove: {key}");
                    _coroutinePool.RemoveCoroutine(_coroutines[service.Key]);
                    keysToRemove.Push(key);
                }
            }

            //Sys.Global.Debugger.Send($"Removing Services");
            foreach (var key in keysToRemove)
            {
                Sys.Global.Debugger.Send($"Removing Service: {key}");
                _services.Remove(key);
                _coroutines.Remove(key);
            }
        }
        private void CheckNewServices()
        {
            //Sys.Global.Debugger.Send("Checking New Services");
            while(_queue.Count > 0)
            {
                var service = _queue.Dequeue();
                _services.Add(service.key, service.Service);
                var coroutine = new Coroutine(CoroutineCicle(service.Service));
                _coroutines.Add(service.key, coroutine);
                _coroutinePool.AddCoroutine(coroutine);
            }
        }
    }
}
