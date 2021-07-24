using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //This class represents an n bit register that can maintain an n bit number
    class MultiBitRegister : Gate
    {
        public WireSet Input { get; private set; }
        public WireSet Output { get; private set; }
        //A bit setting the register operation to read or write
        public Wire Load { get; private set; }

        //Word size - number of bits in the register
        public int Size { get; private set; }

        private SingleBitRegister[] bitRegisters;

        public MultiBitRegister(int iSize)
        {
            Size = iSize;
            Input = new WireSet(Size);
            Output = new WireSet(Size);
            Load = new Wire();
            //your code here
            this.Output = new WireSet(iSize);
            bitRegisters = new SingleBitRegister[iSize];
            for (int i = 0; i < iSize; i++)
            {
                bitRegisters[i] = new SingleBitRegister();
                bitRegisters[i].ConnectInput(this.Input[i]);
                bitRegisters[i].ConnectLoad(this.Load);
                Output[i].ConnectInput(bitRegisters[i].Output);
            }
        }

        public void ConnectInput(WireSet wsInput)
        {
            Input.ConnectInput(wsInput);
        }

        
        public override string ToString()
        {
            return Output.ToString();
        }


        public override bool TestGate()
        {
            Random rnd = new Random();
            int powNumber = (int)Math.Pow(2, Size - 1);
            int number = rnd.Next(powNumber * (-1), powNumber - 1);
            this.Input.Set2sComplement(number);
            this.Load.Value = 1;
            Clock.ClockDown();
            Clock.ClockUp();
            this.Load.Value = 0;
            Clock.ClockDown();
            Clock.ClockUp();
            if (this.Output.Get2sComplement() != number)
                return false;
            if (number == 0)
            {
                this.Input.Set2sComplement(1);
                if (Output.Get2sComplement() == 1)
                    return false;
                this.Load.Value = 1;
                Clock.ClockDown();
                Clock.ClockUp();
                
                if (Output.Get2sComplement() != 1)
                    return false;
                this.Load.Value = 0;
                Clock.ClockDown();
                Clock.ClockUp();
                
            }
            else
            {
                this.Input.Set2sComplement(0);
                if (Output.Get2sComplement() == 0)
                    return false;
                this.Load.Value = 1;
                Clock.ClockDown();
                Clock.ClockUp();
                
                if (Output.Get2sComplement() != 0)
                    return false;
                this.Load.Value = 0;
                Clock.ClockDown();
                Clock.ClockUp();
                
            }
            number = rnd.Next(powNumber * (-1), powNumber - 1);
            this.Input.Set2sComplement(number);
            this.Load.Value = 1;
            Clock.ClockDown();
            Clock.ClockUp();
            if (this.Output.Get2sComplement() != number)
                return false;
            this.Load.Value = 0;
            
            int number2 = rnd.Next(powNumber * (-1), powNumber - 1);
            Input.Set2sComplement(number2);
            Clock.ClockDown();
            Clock.ClockUp();
            if (this.Output.Get2sComplement() != number)
                return false;
            Clock.ClockDown();
            Clock.ClockUp();
            Clock.ClockDown();
            Clock.ClockUp();
            Clock.ClockDown();
            Clock.ClockUp();
            Clock.ClockDown();
            Clock.ClockUp();
            Clock.ClockDown();
            Clock.ClockUp();
            if (this.Output.Get2sComplement() != number)
                return false;

            return true;
        }
    }
}
