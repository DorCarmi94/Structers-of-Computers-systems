using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //Multibit gates take as input k bits, and compute a function over all bits - z=f(x_0,x_1,...,x_k)

    class MultiBitOrGate : MultiBitGate
    {
        //your code here
        private OrGate[] m_orGates;
        private int inputCountParam;
        public MultiBitOrGate(int iInputCount)
            : base(iInputCount)
        {
            //your code here
            inputCountParam = iInputCount;
            int numberOfGatesThatShouldBe = iInputCount - 1;

            m_orGates = new OrGate[numberOfGatesThatShouldBe];
            m_orGates[0] = new OrGate();
            m_orGates[0].ConnectInput1(this.m_wsInput[0]);
            m_orGates[0].ConnectInput2(this.m_wsInput[1]);

            int inputCount = 2;
            int GateCount = 1;


            for (int i = GateCount; i < numberOfGatesThatShouldBe; i++)
            {
                m_orGates[i] = new OrGate();
                m_orGates[i].ConnectInput1(m_orGates[i - 1].Output);
                m_orGates[i].ConnectInput2(this.m_wsInput[inputCount]);
                inputCount++;
            }
            Output = m_orGates[numberOfGatesThatShouldBe - 1].Output;

        }

        public override bool TestGate()
        {
            return recursionTest(this.inputCountParam - 1, false);
        }

        private bool recursionTest(int inputWireTst, bool shouldBeTrue)
        {
            if (inputWireTst == -1)
            {
                Console.WriteLine(this.m_wsInput);
                Console.WriteLine(this.Output.Value);
                if (shouldBeTrue && Output.Value == 1)
                {
                    return true;
                }
                else if (!shouldBeTrue && Output.Value == 0)
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
                m_wsInput[inputWireTst].Value = 0;
                bool a = recursionTest(inputWireTst - 1, shouldBeTrue | false);
                m_wsInput[inputWireTst].Value = 1;
                bool b = recursionTest(inputWireTst - 1, shouldBeTrue | true);
                return a & b;
            }
        }
    }
}
