using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //This gate implements the or operation. To implement it, follow the example in the And gate.
    class OrGate : TwoInputGate
    {
        //your code here 
        private NAndGate Nand;

        private NotGate NotA;
        private NotGate NotB;
        public OrGate()
        {
            //your code here 
            //init the gates
            Nand = new NAndGate();
            NotA = new NotGate();
            NotB = new NotGate();

            Nand.ConnectInput1(NotA.Output);
            Nand.ConnectInput2(NotB.Output);
            Wire w1 = NotA.Output;
            Wire w2 = NotB.Output;
            Wire w3 = Nand.Output;

            Output = Nand.Output;
            Input1 = NotA.Input;
            Input2 = NotB.Input;
        }


        public override string ToString()
        {
            return "Or " + Input1.Value + "," + Input2.Value + " -> " + Output.Value;
        }

        public override bool TestGate()
        {
            bool flag = true;
            Input1.Value = 0;
            Input2.Value = 0;
            if (Output.Value != 0)
                flag= false;
            Input1.Value = 0;
            Input2.Value = 1;
            if (Output.Value != 1)
                flag = false;
            Input1.Value = 1;
            Input2.Value = 0;
            if (Output.Value != 1)
                flag = false;
            Input1.Value = 1;
            Input2.Value = 1;
            if (Output.Value != 1)
                flag = false;
            return flag;

        }
    }

}
