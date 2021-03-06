using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleComponents
{
    public class AndGate : BooleanGate
    {

        public AndGate()
        {
        }


        public override string ToString()
        {
            return "And " + Input1.Value + "," + Input2.Value + " -> " + Output.Value;
        }

        

        public override bool TestGate()
        {
            Input1.Value = 0;
            Input2.Value = 0;
             if (Output.Value != 0)
                return false;
            Input1.Value = 0;
            Input2.Value = 1;
            if (Output.Value != 0)
                return false;
            Input1.Value = 1;
            Input2.Value = 0;
            if (Output.Value != 0)
                return false;
            Input1.Value = 1;
            Input2.Value = 1;
            if (Output.Value != 1)
                return false;
            return true;
        }

        public override void Compute()
        {
            if (Input1.Value == 1 && Input2.Value == 1)
                Output.Value = 1;
            else
                Output.Value = 0;

        }
    }
}
