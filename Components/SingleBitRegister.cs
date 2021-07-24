using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //This class implements a register that can maintain 1 bit.
    class SingleBitRegister : Gate
    {
        public Wire Input { get; private set; }
        public Wire Output { get; private set; }
        //A bit setting the register operation to read or write
        public Wire Load { get; private set; }

        private DFlipFlopGate dff;
        private MuxGate singleBitMux;
        public SingleBitRegister()
        {
            
            Input = new Wire();
            Load = new Wire();
            Output = new Wire();
            //your code here 
            singleBitMux = new MuxGate();
            dff = new DFlipFlopGate();
            singleBitMux.Input2.ConnectInput(this.Input);
            
            singleBitMux.ConnectInput1(this.Output);
            singleBitMux.ControlInput.ConnectInput(this.Load);
            this.dff.ConnectInput(singleBitMux.Output);
            this.Output.ConnectInput(dff.Output);
        }

        public void ConnectInput(Wire wInput)
        {
            Input.ConnectInput(wInput);
        }

      

        public void ConnectLoad(Wire wLoad)
        {
            Load.ConnectInput(wLoad);
        }


        public override bool TestGate()
        {
            //Check clock
            Load.Value = 1;
            Input.Value = 0;
            Clock.ClockDown();
            Clock.ClockUp();
            if (Output.Value != 0)
                return false;
            Input.Value = 1;
            if (Output.Value != 0)
                return false;
            Clock.ClockDown();
            Clock.ClockUp();
            if (Output.Value != 1)
                return false;

            //CheckLoad
            Load.Value = 0;
            Input.Value = 0;
            Clock.ClockDown();
            Clock.ClockUp();
            if (Output.Value != 1)
                return false;
            Input.Value = 1;
            if (Output.Value != 1)
                return false;
            Clock.ClockDown();
            Clock.ClockUp();
            if (Output.Value != 1)
                return false;

            return true;
        }
    }
}
