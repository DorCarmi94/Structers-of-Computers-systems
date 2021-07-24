using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //A mux has 2 inputs. There is a single output and a control bit, selecting which of the 2 inpust should be directed to the output.
    class MuxGate : TwoInputGate
    {
        public Wire ControlInput { get; private set; }
        //your code here
        private AndGate AandNotCtrl;
        private NotGate notCtrl;
        private AndGate BandCtrl;
        private OrGate  OrForAndGates;

        public MuxGate()
        {
            ControlInput = new Wire();
            AandNotCtrl = new AndGate();
            notCtrl = new NotGate();
            BandCtrl = new AndGate();
            OrForAndGates = new OrGate();

            notCtrl.ConnectInput (ControlInput);
            AandNotCtrl.ConnectInput1(notCtrl.Output);
            AandNotCtrl.ConnectInput2(this.Input1);
            BandCtrl.ConnectInput1(ControlInput);
            BandCtrl.ConnectInput2(this.Input2);
            OrForAndGates.ConnectInput1(AandNotCtrl.Output);
            OrForAndGates.ConnectInput2(BandCtrl.Output);
            this.Output = OrForAndGates.Output;

            //your code here

        }

        public void ConnectControl(Wire wControl)
        {
            ControlInput.ConnectInput(wControl);
        }


        public override string ToString()
        {
            return "Mux " + Input1.Value + "," + Input2.Value + ",C" + ControlInput.Value + " -> " + Output.Value;
        }



        public override bool TestGate()
        {
            // lsb- input1
            //      input2
            // msb  ctrl
            bool logic=true;
            for (int i = 0;logic&& i <= 7; i++)
            {
                int x = i;
                this.Input1.Value = x % 2;
                x = x / 2;
                this.Input2.Value = x % 2;
                x = x / 2;
                this.ControlInput.Value = x % 2;
                logic = testCurrentState();                
            }
            return logic;
        }
        private bool testCurrentState()
        {
            if(this.ControlInput.Value==0)
            {
                if(Output.Value==Input1.Value)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if(Output.Value==Input2.Value)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
