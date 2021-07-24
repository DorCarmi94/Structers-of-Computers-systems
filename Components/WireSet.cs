using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //This class represents a set of n wires (a cable)
    class WireSet
    {
        //Word size - number of bits in the register
        public int Size { get; private set; }
        
        public bool InputConected { get; private set; }

        //An indexer providing access to a single wire in the wireset
        public Wire this[int i]
        {
            get
            {
                return m_aWires[i];
            }
        }
        private Wire[] m_aWires;
        
        public WireSet(int iSize)
        {
            Size = iSize;
            InputConected = false;
            m_aWires = new Wire[iSize];
            for (int i = 0; i < m_aWires.Length; i++)
                m_aWires[i] = new Wire();
        }
        public override string ToString()
        {
            string s = "[";
            for (int i = m_aWires.Length - 1; i >= 0; i--)
                s += m_aWires[i].Value;
            s += "]";
            return s;
        }

        //Transform a positive integer value into binary and set the wires accordingly, with 0 being the LSB
        public void SetValue(int iValue)
        {
            int number = iValue;
            for (int j = 0; j < Size; j++)
            {
                this.m_aWires[j].Value = number % 2;
                number = number / 2;
            }
            
        }

            //Transform the binary code into a positive integer
        public int GetValue()
        {
            int num = 0;
            
            for (int i = 0; i < Size; i++)
            {
                if(this.m_aWires[i].Value!=0)
                {
                    num += Convert.ToInt32(Math.Pow(2, i));
                }

            }
            return num;
        }

        private int TurnValue(int[] arr)
        {
            int num = 0;

            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] != 0)
                {
                    num += Convert.ToInt32(Math.Pow(2, i));
                }

            }
            return num;
        }

        //Transform an integer value into binary using 2`s complement and set the wires accordingly, with 0 being the LSB
        public void Set2sComplement(int iValue)
        {
            if(iValue<0)
            {
                int number = iValue * (-1);
                this.SetValue(number);
                foreach (var wire in this.m_aWires)
                {
                    wire.Value = this.notBit(wire.Value);
                }
                this.incrementOne(0, 0);
            }
            else
            {
                this.SetValue(iValue);
            }
        }
        private int notBit(int input)
        {
            if(input==1)
            {
                return 0;
            }
            else if(input==0)
            {
                return 1;
            }
            else
            {
                return input;
            }
        }
        private void incrementOne(int[] arr,int bitNum, int carry)
        {
            if (carry == 0)
            {
                if (arr[bitNum] == 0)
                {
                    arr[bitNum] = 1;
                }
                else
                {
                    arr[bitNum] = 0;
                    incrementOne(arr,bitNum + 1, 1);
                }
            }
            else
            {
                if (arr[bitNum] == 0)
                {
                    arr[bitNum] = 1;
                }
                else
                {
                    arr[bitNum] = 0;
                    incrementOne(arr,bitNum + 1, 1);
                }
            }
            
        }

        private void incrementOne(int bitNum, int carry)
        {
            if (carry == 0)
            {
                if (this.m_aWires[bitNum].Value == 0)
                {
                    m_aWires[bitNum].Value = 1;
                }
                else
                {
                    m_aWires[bitNum].Value = 0;
                    incrementOne(bitNum + 1, 1);
                }
            }
            else
            {
                if (this.m_aWires[bitNum].Value == 0)
                {
                    m_aWires[bitNum].Value = 1;
                }
                else
                {
                    m_aWires[bitNum].Value = 0;
                    incrementOne(bitNum + 1, 1);
                }
            }

        }
        //Transform the binary code in 2`s complement into an integer
        public int Get2sComplement()
        {
           if(this.m_aWires[m_aWires.Length-1].Value==1)
           {
                int[] arr = new int[this.Size];
                for (int i = 0; i < this.m_aWires.Length; i++)
                {
                    arr[i] = m_aWires[i].Value;
                    arr[i] = notBit(arr[i]);
                }
                incrementOne(arr, 0, 0);
                int thePlusNumber = this.TurnValue(arr);
                
                return thePlusNumber * (-1);
           }
           else
            {
                return this.GetValue();
            }
        }

        public void ConnectInput(WireSet wIn)
        {
            if (InputConected)
                throw new InvalidOperationException("Cannot connect a wire to more than one inputs");
            if(wIn.Size != Size)
                throw new InvalidOperationException("Cannot connect two wiresets of different sizes.");
            for (int i = 0; i < m_aWires.Length; i++)
                m_aWires[i].ConnectInput(wIn[i]);

            InputConected = true;
            
        }

    }
}
