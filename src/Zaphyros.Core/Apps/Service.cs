using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos.System.Coroutines;

namespace Zaphyros.Core.Apps
{
    internal abstract class Service
    {
        private bool isRunning;
        private bool isTermidated;

        public bool IsRunning => isRunning;

        public bool IsTermidated => isTermidated;

        protected Service()
        {
            isRunning = false; // Initialize to a known state
        }

        public virtual void BeforeStart() { }
        public void Start()
        {
            if (!isRunning)
            {
                BeforeStart();
                isRunning = true;
            }
        }
        public abstract void Update();
        public virtual void AfterStart() { }
        public void Stop()
        {
            if (!isTermidated)
            {
                isTermidated = true;
                isRunning = false;
            }
        }

        public void Suspend()
        {
            if (isRunning)
            {
                isRunning = false;
            }
        }

        public void Resume()
        {
            if (!isRunning)
            {
                isRunning = true;
            }
        }
    }

}
