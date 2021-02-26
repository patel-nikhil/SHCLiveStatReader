using System;

namespace SHC
{
    class State
    {
        readonly string name;
        readonly Func<bool> testActive;

        public State(string state, Func<bool> isActive)
        {
            this.name = state;
            this.testActive = isActive;
        }
        
        public bool IsActive()
        {
            return testActive();
        }

        public override string ToString()
        {
            return this.name;
        }

    }
}
