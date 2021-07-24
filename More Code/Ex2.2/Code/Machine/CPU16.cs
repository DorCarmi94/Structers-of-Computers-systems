using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleComponents;

namespace Machine
{
    public class CPU16 
    {
        //this "enum" defines the different control bits names
        public const int J3 = 0, J2 = 1, J1 = 2, D3 = 3, D2 = 4, D1 = 5, C6 = 6, C5 = 7, C4 = 8, C3 = 9, C2 = 10, C1 = 11, A = 12, X2 = 13, X1 = 14, Type = 15;

        public int Size { get; private set; }

        //CPU inputs
        public WireSet Instruction { get; private set; }
        public WireSet MemoryInput { get; private set; }
        public Wire Reset { get; private set; }

        //CPU outputs
        public WireSet MemoryOutput { get; private set; }
        public Wire MemoryWrite { get; private set; }
        public WireSet MemoryAddress { get; private set; }
        public WireSet InstructionAddress { get; private set; }

        //CPU components
        private ALU m_gALU;
        private Counter m_rPC;
        private MultiBitRegister m_rA, m_rD;
        private BitwiseMux m_gAMux, m_gMAMux;

        //here we initialize and connect all the components, as in Figure 5.9 in the book
        public CPU16()
        {
            Size =  16;

            Instruction = new WireSet(Size);
            MemoryInput = new WireSet(Size);
            MemoryOutput = new WireSet(Size);
            MemoryAddress = new WireSet(Size);
            InstructionAddress = new WireSet(Size);
            MemoryWrite = new Wire();
            Reset = new Wire();

            m_gALU = new ALU(Size);
            m_rPC = new Counter(Size);
            m_rA = new MultiBitRegister(Size);
            m_rD = new MultiBitRegister(Size);

            m_gAMux = new BitwiseMux(Size);
            m_gMAMux = new BitwiseMux(Size);

            m_gAMux.ConnectInput1(Instruction);
            m_gAMux.ConnectInput2(m_gALU.Output);

            m_rA.ConnectInput(m_gAMux.Output);

            m_gMAMux.ConnectInput1(m_rA.Output);
            m_gMAMux.ConnectInput2(MemoryInput);
            m_gALU.InputY.ConnectInput(m_gMAMux.Output);

            m_gALU.InputX.ConnectInput(m_rD.Output);

            m_rD.ConnectInput(m_gALU.Output);

            MemoryOutput.ConnectInput(m_gALU.Output);
            MemoryAddress.ConnectInput(m_rA.Output);

            InstructionAddress.ConnectInput(m_rPC.Output);
            m_rPC.ConnectInput(m_rA.Output);
            m_rPC.ConnectReset(Reset);

            //now, we call the code that creates the control unit
            ConnectControls();
        }

        //add here components to implement the control unit 
        private BitwiseMultiwayMux m_gJumpMux;//an example of a control unit compnent - a mux that controls whether a jump is made


        private AndGate and_D;
        private NotGate not_type;
        private OrGate or_d1_notType;
        private AndGate and_type_d3;
        private NotGate not_zero_alu;
        private NotGate not_negative_alu;
        private AndGate and_not_zero_not_negative_alu;
        private OrGate or_zeroAlu_NotNegative;
        private OrGate or_zeroAlu_NegativeAlu;
        private AndGate and_type_JMPmux;

        private void ConnectControls()
        {
            //1. connect control of mux 1 (selects entrance to register A)
            
            this.m_gAMux.ConnectControl(this.Instruction[Type]);


            //2. connect control to mux 2 (selects A or M entrance to the ALU)
            this.m_gMAMux.ConnectControl(this.Instruction[A]);

            //3. consider all instruction bits only if C type instruction (MSB of instruction is 1)
            

            //4. connect ALU control bits
            this.m_gALU.ZeroX.ConnectInput(this.Instruction[C1]);
            this.m_gALU.NotX.ConnectInput(this.Instruction[C2]);
            this.m_gALU.ZeroY.ConnectInput(this.Instruction[C3]);
            this.m_gALU.NotY.ConnectInput(this.Instruction[C4]);
            this.m_gALU.F.ConnectInput(this.Instruction[C5]);
            this.m_gALU.NotOutput.ConnectInput(this.Instruction[C6]);


            //5. connect control to register D (very simple)
            and_D = new AndGate();
            and_D.ConnectInput1(this.Instruction[Type]);
            and_D.ConnectInput2(this.Instruction[D2]);
            this.m_rD.Load.ConnectInput(and_D.Output);

            //6. connect control to register A (a bit more complicated)
            not_type = new NotGate();
            not_type.ConnectInput(this.Instruction[Type]);
            or_d1_notType = new OrGate();
            or_d1_notType.ConnectInput1(Instruction[D1]);
            or_d1_notType.ConnectInput2(not_type.Output);
            m_rA.Load.ConnectInput(or_d1_notType.Output);

            //7. connect control to MemoryWrite
            and_type_d3 = new AndGate();
            and_type_d3.ConnectInput1(Instruction[Type]);
            and_type_d3.ConnectInput2(Instruction[D3]);
            this.MemoryWrite.ConnectInput(and_type_d3.Output);


            //8. create inputs for jump mux
            //JGT
            not_negative_alu = new NotGate();
            not_zero_alu = new NotGate();
            not_zero_alu.ConnectInput(this.m_gALU.Zero);
            not_negative_alu.ConnectInput(this.m_gALU.Negative);
            and_not_zero_not_negative_alu = new AndGate();
            and_not_zero_not_negative_alu.ConnectInput1(not_zero_alu.Output);
            and_not_zero_not_negative_alu.ConnectInput2(not_negative_alu.Output);

            //JGE
            or_zeroAlu_NotNegative = new OrGate();
            or_zeroAlu_NotNegative.ConnectInput1(not_negative_alu.Output);
            or_zeroAlu_NotNegative.ConnectInput2(m_gALU.Zero);

            //JLE
            or_zeroAlu_NegativeAlu = new OrGate();
            or_zeroAlu_NegativeAlu.ConnectInput1(m_gALU.Zero);
            or_zeroAlu_NegativeAlu.ConnectInput2(m_gALU.Negative);
            

            //9. connect jump mux (this is the most complicated part)
            m_gJumpMux = new BitwiseMultiwayMux(1, 3);
            m_gJumpMux.Control[0].ConnectInput(Instruction[J3]);
            m_gJumpMux.Control[1].ConnectInput(Instruction[J2]);
            m_gJumpMux.Control[2].ConnectInput(Instruction[J1]);
            m_gJumpMux.Inputs[0].Value = 0;
            m_gJumpMux.Inputs[1][0].ConnectInput(and_not_zero_not_negative_alu.Output);
            m_gJumpMux.Inputs[2][0].ConnectInput(m_gALU.Zero);
            m_gJumpMux.Inputs[3][0].ConnectInput(or_zeroAlu_NotNegative.Output);
            m_gJumpMux.Inputs[4][0].ConnectInput(m_gALU.Negative);
            m_gJumpMux.Inputs[5][0].ConnectInput(not_zero_alu.Output);
            m_gJumpMux.Inputs[6][0].ConnectInput(or_zeroAlu_NegativeAlu.Output);
            m_gJumpMux.Inputs[7][0].Value = 1;





            //10. connect PC load control
            and_type_JMPmux = new AndGate();
            and_type_JMPmux.ConnectInput1(Instruction[Type]);
            and_type_JMPmux.ConnectInput2(m_gJumpMux.Output[0]);
            this.m_rPC.Load.ConnectInput(and_type_JMPmux.Output);
        }

        public override string ToString()
        {
            return "A=" + m_rA + ", D=" + m_rD + ", PC=" + m_rPC + ",Ins=" + Instruction;
        }

        private string GetInstructionString()
        {
            if (Instruction[Type].Value == 0)
                return "@" + Instruction.GetValue();
            return Instruction[Type].Value + "XX " +
               "a" + Instruction[A] + " " +
               "c" + Instruction[C1] + Instruction[C2] + Instruction[C3] + Instruction[C4] + Instruction[C5] + Instruction[C6] + " " +
               "d" + Instruction[D1] + Instruction[D2] + Instruction[D3] + " " +
               "j" + Instruction[J1] + Instruction[J2] + Instruction[J3];
        }

        //use this function in debugging to print the current status of the ALU. Feel free to add more things for printing.
        public void PrintState()
        {
           Console.WriteLine("CPU state:");
           Console.WriteLine("PC=" + m_rPC + "=" + m_rPC.Output.GetValue());
           Console.WriteLine("A=" + m_rA + "=" + m_rA.Output.GetValue());
           Console.WriteLine("D=" + m_rD + "=" + m_rD.Output.GetValue());
           Console.WriteLine("Ins=" + GetInstructionString());
           Console.WriteLine("ALU=" + m_gALU);
           Console.WriteLine("inM=" + MemoryInput);
           Console.WriteLine("outM=" + MemoryOutput);
           Console.WriteLine("addM=" + MemoryAddress);



            //J3 = 0, J2 = 1, J1 = 2, D3 = 3, D2 = 4, D1 = 5, C6 = 6, C5 = 7, C4 = 8, C3 = 9, C2 = 10, C1 = 11, A = 12, X2 = 13, X1 = 14, Type = 15;
            Console.WriteLine("Instruction[Type]: " + Instruction[Type]);
            Console.WriteLine("Instruction[A]: " + Instruction[A]);
            Console.WriteLine("Instruction[C1]: " + Instruction[C1]);
            Console.WriteLine("ZeroX" + this.m_gALU.ZeroX);
            Console.WriteLine("Instruction[C2]: " + Instruction[C2]);
            Console.WriteLine("NotX" + this.m_gALU.NotX);
            Console.WriteLine("Instruction[C3]: " + Instruction[C3]);
            Console.WriteLine("ZeroY" + this.m_gALU.ZeroY);
            Console.WriteLine("Instruction[C4]: " + Instruction[C4]);
            Console.WriteLine("NotY" + this.m_gALU.NotY);
            Console.WriteLine("Instruction[C5]: " + Instruction[C5]);
            Console.WriteLine("F" + this.m_gALU.F);
            Console.WriteLine("Instruction[C6]: " + Instruction[C6]);
            Console.WriteLine("NotOut" + this.m_gALU.NotOutput);


            Console.WriteLine("Instruction[D1]: " + Instruction[D1]);
            Console.WriteLine("Instruction[D2]: " + Instruction[D2]);
            Console.WriteLine("Instruction[D3]: " + Instruction[D3]);
            Console.WriteLine("Instruction[J1]: " + Instruction[J1]);
            Console.WriteLine("Instruction[J2]: " + Instruction[J2]);
            Console.WriteLine("Instruction[J3]: " + Instruction[J3]);
            
        }
    }
}
