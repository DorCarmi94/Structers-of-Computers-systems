using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //A demux has 2 outputs. There is a single input and a control bit, selecting whether the input should be directed to the first or second output.
    class Demux : Gate
    {
        public Wire Output1 { get; private set; }
        public Wire Output2 { get; private set; }
        public Wire Input { get; private set; }
        public Wire Control { get; private set; }


        //your code here
        private NotGate notCtrl;
        private AndGate InAndNotCtrl;
        private AndGate InAndCtrl;
        

        public Demux()
        {
            Input = new Wire();
            Control = new Wire();
            //your code here
            notCtrl = new NotGate();
            InAndNotCtrl = new AndGate();
            InAndCtrl = new AndGate();
            notCtrl.ConnectInput(this.Control);
            //out bit 00
            this.InAndNotCtrl.ConnectInput1(this.Input);
            this.InAndNotCtrl.ConnectInput2(notCtrl.Output);
            this.Output1 = InAndNotCtrl.Output;
            //out bit 01
            InAndCtrl.ConnectInput1(this.Input);
            InAndCtrl.ConnectInput2(this.Control);
            this.Output2 = InAndCtrl.Output;

        }

        public void ConnectControl(Wire wControl)
        {
            Control.ConnectInput(wControl);
        }
        public void ConnectInput(Wire wInput)
        {
            Input.ConnectInput(wInput);
        }



        public override bool TestGate()
        {
            this.Input.Value = 0;
            this.Control.Value = 0;
            if(Output1.Value!=Input.Value)
            {
                return false;
            }

            this.Input.Value = 0;
            this.Control.Value = 1;
            if (Output2.Value != Input.Value)
            {
                return false;
            }

            this.Input.Value = 1;
            this.Control.Value = 0;
            if (Output1.Value != Input.Value)
            {
                return false;
            }

            this.Input.Value = 1;
            this.Control.Value = 1;
            if (Output2.Value != Input.Value)
            {
                return false;
            }

            return true;

        }
    }
}
