using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //This class implements a memory unit, containing k registers, each of size n bits.
    class Memory : SequentialGate
    {
        //The address size determines the number of registers
        public int AddressSize { get; private set; }
        //The word size determines the number of bits in each register
        public int WordSize { get; private set; }

        //Data input and output - a number with n bits
        public WireSet Input { get; private set; }
        public WireSet Output { get; private set; }
        //The address of the active register
        public WireSet Address { get; private set; }
        //A bit setting the memory operation to read or write
        public Wire Load { get; private set; }

        //your code here
        MultiBitRegister[] memoryRegister;
        BitwiseMultiwayMux multiwayMuxOutput;
        BitwiseMultiwayDemux multiwayDemuxLoad;
        //NotGate[] notsForLoads;
        public Memory(int iAddressSize, int iWordSize)
        {
            AddressSize = iAddressSize;
            WordSize = iWordSize;

            Input = new WireSet(WordSize);
            Output = new WireSet(WordSize);
            Address = new WireSet(AddressSize);
            Load = new Wire();

            //your code here
            
            int k = AddressSize;
            multiwayDemuxLoad = new BitwiseMultiwayDemux(1, k);
            multiwayDemuxLoad.Input[0].ConnectInput(Load);
            multiwayDemuxLoad.Control.ConnectInput(Address);
            int pow2k = (int)Math.Pow(2, k);
            this.memoryRegister = new MultiBitRegister[pow2k];
            //notsForLoads = new NotGate[pow2k];
            this.multiwayMuxOutput = new BitwiseMultiwayMux(WordSize, k);
            multiwayMuxOutput.Control.ConnectInput(Address);
            for (int i = 0; i < pow2k; i++)
            {
                memoryRegister[i]= new MultiBitRegister(WordSize);
                memoryRegister[i].ConnectInput(Input);
                //notsForLoads[i] = new NotGate();
                //notsForLoads[i].ConnectInput(multiwayDemuxLoad.Outputs[i][0]);
                //memoryRegister[i].Load.ConnectInput(notsForLoads[i].Output);
                memoryRegister[i].Load.ConnectInput(multiwayDemuxLoad.Outputs[i][0]);
                multiwayMuxOutput.Inputs[i].ConnectInput(memoryRegister[i].Output);
            }
            this.Output.ConnectInput(multiwayMuxOutput.Output);
            

        }

        public void ConnectInput(WireSet wsInput)
        {
            Input.ConnectInput(wsInput);
        }
        public void ConnectAddress(WireSet wsAddress)
        {
            Address.ConnectInput(wsAddress);
        }


        public override void OnClockUp()
        {
        }

        public override void OnClockDown()
        {
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }

        public override bool TestGate()
        {
            bool flag = true;
            for (int i = 0; flag && i < 100; i++)
            {
                //Write to random register in memory
                //Load=0: Read
                //Load=1: Write
                this.Load.Value = 0;//Begin not writing
                
                Random rnd = new Random();
                int randomAddress;
                int randomInput;

                int addressPowNumber = (int)Math.Pow(2, AddressSize);
                int inputPowNumber = (int)Math.Pow(2, WordSize-1);

                
                randomAddress=  rnd.Next(0, addressPowNumber - 1);
                randomInput =  rnd.Next(inputPowNumber * (-1), inputPowNumber - 1);

                this.Address.SetValue(randomAddress);
                this.Input.Set2sComplement(randomInput);
                this.Load.Value = 1;
                Clock.ClockDown();
                Clock.ClockUp();

                this.Load.Value = 0;
                Clock.ClockDown();
                Clock.ClockUp();
                //Console.WriteLine("Writing the number {"+randomInput+"} to address {"+randomAddress+"}");
                int memoryRegValue = this.memoryRegister[randomAddress].Output.Get2sComplement();
                if (memoryRegValue != randomInput)
                {
                    flag = false;
                    //Console.WriteLine("A: " + i);
                }
                int outputValue = this.Output.Get2sComplement();
                if (flag && this.Output.Get2sComplement() != randomInput)
                {
                    flag = false;
                    //Console.WriteLine("B: " + i);
                }

                //Console.WriteLine("The output is: {" + this.Output.ToString() + "} := " + Output.Get2sComplement());
                int randomAddress2= rnd.Next(0, addressPowNumber - 1);
                //Console.WriteLine("Writing the number {" + randomInput + "} to address {" + randomAddress2+"}");
                this.Address.SetValue(randomAddress2);
                this.Load.Value = 1;//Write
                Clock.ClockDown();
                Clock.ClockUp();
                this.Load.Value = 0;
                Clock.ClockDown();
                Clock.ClockUp();
                //Console.WriteLine("The output is: {" + this.Output.ToString() + "} := " + Output.Get2sComplement());
                if (flag && this.memoryRegister[randomAddress2].Output.Get2sComplement() != randomInput)
                {
                    flag = false;
                    //Console.WriteLine("C: "+i);
                }
                if (flag && this.Output.Get2sComplement() != randomInput)
                {
                    flag = false;
                    //Console.WriteLine("D: " + i);
                }
                //Console.WriteLine("Stopping to write: Load=1");
                
                //don't write
                int randomInput2 = rnd.Next(inputPowNumber * (-1), inputPowNumber - 1);
                this.Input.Set2sComplement(randomInput2);
                Clock.ClockDown();
                Clock.ClockUp();
                //Console.WriteLine("Trying to write the number {" + randomInput2 + "} to address {" + randomAddress2+"}");
                //Console.WriteLine("The output is: {" + this.Output.ToString() + "} := " + Output.Get2sComplement());
                if (flag && this.memoryRegister[randomAddress2].Output.Get2sComplement() != randomInput)
                {
                    flag = false;
                    //Console.WriteLine("E: " + i);
                }
                if (flag && this.Output.Get2sComplement() != randomInput)
                {
                    flag = false;
                    //Console.WriteLine("F: " + i);
                }
                this.Address.SetValue(randomAddress);
                
                Clock.ClockDown();
                Clock.ClockUp();
                //Console.WriteLine("Reading from address {" + randomAddress + "} when trying to write the input {" + randomInput2 + "}");
                //Console.WriteLine("The output is: {" + this.Output.ToString() + "} := "+Output.Get2sComplement());
                if (flag && this.memoryRegister[randomAddress].Output.Get2sComplement() != randomInput)
                {
                    flag = false;
                    //Console.WriteLine("G: " + i);
                }

                if (flag && this.Output.Get2sComplement() != randomInput)
                {
                    flag = false;
                    //Console.WriteLine("H: " + i);
                }
                //Console.WriteLine("///////////////////////////////////////////");
            }
            return flag;
        }
    }
}
