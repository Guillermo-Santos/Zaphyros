using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos.System.Coroutines;

namespace Zaphyros.Core.Apps
{
    internal class TaskManager
    {
        private readonly Dictionary<string, Service> _services = new();
        private readonly Dictionary<string, Coroutine> _coroutines = new();
        private readonly CoroutinePool _coroutinePool;

        public TaskManager()
        {
            _coroutinePool = CoroutinePool.Main;
            _coroutinePool.OnCoroutineCycle.Add(AfterCicle);
            _coroutinePool.HeapCollectionInterval = 10_000;
        }

        public string RegisterService(Service service) => RegisterService(Guid.NewGuid().ToString(), service);
        public string RegisterService(string key, Service service)
        {
            _services.Add(key, service);
            _coroutines.Add(key, new(CoroutineCicle(service)));
            return key;
        }

        private static IEnumerator<CoroutineControlPoint> CoroutineCicle(Service service)
        {
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
        private void BeforeRun()
        {
            foreach (var coroutine in _coroutines)
            {
                _coroutinePool.AddCoroutine(coroutine.Value);
            }
        }
        public void Run()
        {
            BeforeRun();
            _coroutinePool.StartPool();
        }

        private void AfterCicle()
        {
            foreach (var service in _services)
            {
                if (service.Value.IsTermidated)
                {
                    _coroutinePool.RemoveCoroutine(_coroutines[service.Key]);
                    _coroutines.Remove(service.Key);
                    _services.Remove(service.Key);
                }
            }
        }
    }
}
