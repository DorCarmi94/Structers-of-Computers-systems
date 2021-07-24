using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //This class implements a FullAdder, taking as input 3 bits - 2 numbers and a carry, and computing the result in the output, and the carry out.
    class FullAdder : TwoInputGate
    {
        public Wire CarryInput { get; private set; }
        public Wire CarryOutput { get; private set; }

        //your code here
        HalfAdder   firstHA;
        HalfAdder   secondHA;
        OrGate      orGate;

        public FullAdder()
        {
            CarryInput = new Wire();
            //your code here
            firstHA = new HalfAdder();
            secondHA = new HalfAdder();
            orGate = new OrGate();


            this.firstHA.ConnectInput1(this.Input1);
            this.firstHA.ConnectInput2(this.Input2);
            this.secondHA.ConnectInput1(firstHA.Output);
            this.secondHA.ConnectInput2(this.CarryInput);
            this.Output = secondHA.Output;
            this.orGate.ConnectInput1(firstHA.CarryOutput);
            this.orGate.ConnectInput2(secondHA.CarryOutput);
            this.CarryOutput = orGate.Output;
        }


        public override string ToString()
        {
            return Input1.Value + "+" + Input2.Value + " (C" + CarryInput.Value + ") = " + Output.Value + " (C" + CarryOutput.Value + ")";
        }

        public override bool TestGate()
        {
            this.Input1.Value = 0;
            this.Input2.Value = 0;
            this.CarryInput.Value = 0;
            if(this.Output.Value!=0 || this.CarryOutput.Value!=0)
            {
                return false;
            }

            this.Input1.Value = 0;
            this.Input2.Value = 0;
            this.CarryInput.Value = 1;
            if (this.Output.Value != 1 || this.CarryOutput.Value != 0)
            {
                return false;
            }

            this.Input1.Value = 0;
            this.Input2.Value = 1;
            this.CarryInput.Value = 0;
            if (this.Output.Value != 1 || this.CarryOutput.Value != 0)
            {
                return false;
            }

            this.Input1.Value = 0;
            this.Input2.Value = 1;
            this.CarryInput.Value = 1;
            if (this.Output.Value != 0 || this.CarryOutput.Value != 1)
            {
                return false;
            }

            this.Input1.Value = 1;
            this.Input2.Value = 0;
            this.CarryInput.Value = 0;
            if (this.Output.Value != 1 || this.CarryOutput.Value != 0)
            {
                return false;
            }

            this.Input1.Value = 1;
            this.Input2.Value = 0;
            this.CarryInput.Value = 1;
            if (this.Output.Value != 0 || this.CarryOutput.Value != 1)
            {
                return false;
            }

            this.Input1.Value = 1;
            this.Input2.Value = 1;
            this.CarryInput.Value = 0;
            if (this.Output.Value != 0 || this.CarryOutput.Value != 1)
            {
                return false;
            }

            this.Input1.Value = 1;
            this.Input2.Value = 1;
            this.CarryInput.Value = 1;
            if (this.Output.Value != 1 || this.CarryOutput.Value != 1)
            {
                return false;
            }
            return true;
        }
    }
}
