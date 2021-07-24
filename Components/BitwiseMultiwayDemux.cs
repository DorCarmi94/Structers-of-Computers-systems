using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //This class implements a demux with k outputs, each output with n wires. The input also has n wires.

    class BitwiseMultiwayDemux : Gate
    {
        //Word size - number of bits in each output
        public int Size { get; private set; }

        //The number of control bits needed for k outputs
        public int ControlBits { get; private set; }

        public WireSet Input { get; private set; }
        public WireSet Control { get; private set; }
        public WireSet[] Outputs { get; private set; }
        private int NumberOfOuptus;

        //your code here
        private BitwiseDemux[] demuxesTree;
        public BitwiseMultiwayDemux(int iSize, int cControlBits)
        {
            Size = iSize;
            Input = new WireSet(Size);
            Control = new WireSet(cControlBits);
            Outputs = new WireSet[(int)Math.Pow(2, cControlBits)];
            this.NumberOfOuptus = (int)Math.Pow(2, cControlBits);
            for (int i = 0; i < Outputs.Length; i++)
            {
                Outputs[i] = new WireSet(Size);
            }




            //your code here
            int powOf = NumberOfOuptus;
            this.ControlBits = cControlBits;
            demuxesTree = new BitwiseDemux[powOf];
            int demuxCounter = 0;
            int outputsCounter = 0;
            int controlCount = 0;
            int demuxFrom;
            int demuxUntil;

            //first phase- leafs
            for (int i = 0; i < powOf; i += 2)
            {
                demuxesTree[demuxCounter] = new BitwiseDemux(Size);
                for (int j = 0; j < this.Size; j++)
                {
                    this.Outputs[outputsCounter][j].ConnectInput(demuxesTree[demuxCounter].Output1[j]);
                    this.Outputs[outputsCounter + 1][j].ConnectInput(demuxesTree[demuxCounter].Output2[j]);
                    //demuxesTree[demuxCounter].Output2[j].ConnectInput(this.Outputs[outputsCounter + 1][j]);
                }
                
                demuxesTree[demuxCounter].ConnectControl(this.Control[controlCount]);
                outputsCounter += 2;
                demuxCounter++;

            }
            controlCount++;
            demuxFrom = 0;
            demuxUntil = demuxCounter;
            for (int i = 1; i < cControlBits; i++)
            {
                int j;
                int start = demuxCounter;
                for (j = demuxFrom; j < demuxUntil; j += 2)
                {
                    demuxesTree[demuxCounter] = new BitwiseDemux(Size);
                    for (int k = 0; k < this.Size; k++)
                    {
                        demuxesTree[j].Input[k].ConnectInput(demuxesTree[demuxCounter].Output1[k]);
                        demuxesTree[j + 1].Input[k].ConnectInput(demuxesTree[demuxCounter].Output2[k]);
                    }
                    
                    //demuxesTree[demuxCounter].Output1.ConnectInput(demuxesTree[j].Input);
                    //demuxesTree[demuxCounter].Output2.ConnectInput(demuxesTree[j + 1].Input);
                    demuxesTree[demuxCounter].ConnectControl(this.Control[controlCount]);
                    demuxCounter++;

                }
                demuxFrom = start;
                demuxUntil = demuxCounter;
                controlCount++;
            }
            for (int k = 0; k < this.Size; k++)
            {
                this.demuxesTree[demuxFrom].Input[k].ConnectInput(Input[k]);
               
            }
            
        }


        public void ConnectInput(WireSet wsInput)
        {
            Input.ConnectInput(wsInput);
        }
        public void ConnectControl(WireSet wsControl)
        {
            Control.ConnectInput(wsControl);
        }

        public override string ToString()
        {
            String str = "MultiwayDemux: ";
            str += "Control: {" + this.Control.GetValue() + "}\n";
            str += "Input: ->" + this.Input.ToString()+"\n";
            for (int i = 0; i < this.NumberOfOuptus; i++)
            {
                str += " Output" + i + " -> " + this.Outputs[i].ToString() + "\n";
            }
            

            return str;
        }
        public override bool TestGate()
        {
            bool ans = true;
            int count = 0;
            //Set inputs
            int powOfSize=(int)Math.Pow(2,Size);

            //Check all control options
            for (int i = 0; i < NumberOfOuptus; i++)
            {
                int ctrl = i;
                for (int j = 0; j < ControlBits; j++)
                {
                    this.Control[j].Value = ctrl % 2;
                    ctrl = ctrl / 2;
                }

                //Check all input options by input size
                for (int j = 0; j < powOfSize; j++)
                {
                    int inputTst = i;
                    for (int k = 0; k < Size; k++)
                    {
                        this.Input[k].Value = inputTst % 2;
                        inputTst = inputTst / 2;
                    }
                }

                count++;
                //Console.WriteLine(this.ToString());
                //foreach (var demux in demuxesTree)
                //{
                //    if(demux!=null)
                //        Console.WriteLine(demux.ToString());
                //}
                //Test Output
                for (int m = 0; ans && m < this.Size; m++)
                {
                    int chosenOutput = Control.GetValue();
                    if (Outputs[chosenOutput][m].Value != Input[m].Value)
                    {
                        ans = false;
                    }
                }
                //Console.WriteLine(ans);
            }

            return ans;
        }
    }
}
