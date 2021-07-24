using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //This gate implements the xor operation. To implement it, follow the example in the And gate.
    class XorGate : TwoInputGate
    {
        //your code here
        private NAndGate m_A_notB;
        private NAndGate m_notA_B;
        private NAndGate m_nandTwoGates;
        private NotGate notA;
        private NotGate notB;
        public XorGate()
        {
            //your code here

             m_A_notB=new NAndGate();
             m_notA_B=new NAndGate();
             m_nandTwoGates = new NAndGate();
            notA = new NotGate();
            notB=new NotGate();

            //internal connection
            m_nandTwoGates.ConnectInput1(m_A_notB.Output);
            m_nandTwoGates.ConnectInput2(m_notA_B.Output);
            Input1 = notA.Input;
            Input2 = notB.Input;
            m_A_notB.ConnectInput1(Input1);
            m_A_notB.ConnectInput2(notB.Output);
            m_notA_B.ConnectInput1(notA.Output);
            m_notA_B.ConnectInput2(Input2);
            Output = m_nandTwoGates.Output;

        }

        //an implementation of the ToString method is called, e.g. when we use Console.WriteLine(xor)
        //this is very helpful during debugging
        public override string ToString()
        {
            return "Xor " + Input1.Value + "," + Input2.Value + " -> " + Output.Value;
        }


        //this method is used to test the gate. 
        //we simply check whether the truth table is properly implemented.
        public override bool TestGate()
        {
            Input1.Value = 0;
            Input2.Value = 0;
            if (Output.Value != 0)
                return false;
            Input1.Value = 0;
            Input2.Value = 1;
            if (Output.Value != 1)
                return false;
            Input1.Value = 1;
            Input2.Value = 0;
            if (Output.Value != 1)
                return false;
            Input1.Value = 1;
            Input2.Value = 1;
            if (Output.Value != 0)
                return false;
            return true;
        }
    }
}
