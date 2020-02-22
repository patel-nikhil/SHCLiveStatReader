using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SHC
{
    class Program
    {
        public static void Main()
        {
            while(true){
                try
                {
                    StateMachine.Update();
                } catch (IndexOutOfRangeException)
                {
                    StateMachine.Reset();
                }
            } 
        }
    }
}
