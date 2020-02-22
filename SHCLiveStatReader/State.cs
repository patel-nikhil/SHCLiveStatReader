using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHC
{
    class State
    {
        readonly String name;

        Func<bool> testActive;

        public State(String state, Func<bool> isActive)
        {
            this.name = state;
            this.testActive = isActive;
        }
        
        public bool isActive()
        {
            return testActive();
        }

        public override String ToString()
        {
            return this.name;
        }

    }
}
