using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //This class is used to implement the ALU
    //The specification can be found at https://b1391bd6-da3d-477d-8c01-38cdf774495a.filesusr.com/ugd/56440f_2e6113c60ec34ed0bc2035c9d1313066.pdf slides 48,49
    class ALU : Gate
    {
        //The word size = number of bit in the input and output
        public int Size { get; private set; }

        //Input and output n bit numbers
        public WireSet InputX { get; private set; }
        public WireSet InputY { get; private set; }
        public WireSet Output { get; private set; }

        //Control bit 
        public Wire ZeroX { get; private set; }
        public Wire ZeroY { get; private set; }
        public Wire NotX { get; private set; }
        public Wire NotY { get; private set; }
        public Wire F { get; private set; }
        public Wire NotOutput { get; private set; }

        //Bit outputs
        public Wire Zero { get; private set; }
        public Wire Negative { get; private set; }


        //your code here
        //----x:
        private BitwiseMux      muxZx; //in: x -> out: x'
        private BitwiseMux      muxNx;// in: x' -> out: x''
        private BitwiseNotGate  notXtag; //Not x' 

        //----y:
        private BitwiseMux      muxZy; //in: y -> out: y'
        private BitwiseMux      muxNy;// in: y' -> out: y''
        private BitwiseNotGate  notYtag; //Not y' 

        //----f:
        private BitwiseAndGate  XtagaimAndYtgaim;// x'' and y''
        private MultiBitAdder   XtagimADDYtagim;// x'' + y''
        private BitwiseMux      AndMuxAdd;

        //no:
        private BitwiseNotGate  Not2Output;
        private BitwiseMux      OuputOrNotOutput;

        //zr:
        private NotGate[]       notsOutput;
        private MultiBitAndGate NotOutpusAndZr;

        //Wires:
        private WireSet zeroWire;

        public WireSet w_xtag;
        public WireSet w_ytag;
        public WireSet w_xtagim;
        public WireSet w_ytagim;
        public WireSet w_xtagimANDytagim;
        public WireSet w_xtagimADDytagim;
        public WireSet w_ftag;
        public WireSet w_No;
        public WireSet NoTag;




        public ALU(int iSize)
        {
            Size = iSize;
            InputX = new WireSet(Size);
            InputY = new WireSet(Size);
            ZeroX = new Wire();
            ZeroY = new Wire();
            NotX = new Wire();
            NotY = new Wire();
            F = new Wire();
            NotOutput = new Wire();
            Negative = new Wire();            
            Zero = new Wire();


            //Create and connect all the internal components
            muxZx = new BitwiseMux(iSize);
            muxNx= new BitwiseMux(iSize);
            notXtag= new BitwiseNotGate(iSize);


            muxZy= new BitwiseMux(iSize);
            muxNy=  new BitwiseMux(iSize);
            notYtag = new BitwiseNotGate(iSize); 


            XtagaimAndYtgaim= new BitwiseAndGate(iSize);
            XtagimADDYtagim = new MultiBitAdder(iSize);
            AndMuxAdd= new BitwiseMux(iSize);


            Not2Output= new BitwiseNotGate(iSize);
            OuputOrNotOutput= new BitwiseMux(iSize);


            notsOutput= new NotGate[iSize];
            NotOutpusAndZr= new MultiBitAndGate(iSize);


            zeroWire = new WireSet(iSize);
            zeroWire.SetValue(0);

            //Decalre wires:
            w_xtag= new WireSet(iSize);
            w_ytag= new WireSet(iSize);
            w_xtagim= new WireSet(iSize); 
            w_ytagim= new WireSet(iSize); 
            w_xtagimANDytagim= new WireSet(iSize); 
            w_xtagimADDytagim= new WireSet(iSize);
            w_ftag=new WireSet(iSize);
            w_No= new WireSet(iSize);
            this.Output = new WireSet(iSize);

            //InputX.SetValue(3);
            //InputY.SetValue(2);
            //NotX.Value = 0;
            //NotY.Value = 0;
            //F.Value = 1;
            //NotOutput.Value = 0;

            //Zx
            muxZx.ConnectInput1(this.InputX);
            muxZx.ConnectInput2(this.zeroWire);
            muxZx.ConnectControl(this.ZeroX);
            this.w_xtag.ConnectInput(muxZx.Output);

            //Zy
            muxZy.ConnectInput1(this.InputY);
            muxZy.ConnectInput2(this.zeroWire);
            muxZy.ConnectControl(this.ZeroY);
            this.w_ytag.ConnectInput(muxZy.Output);

            //Nx:
            notXtag.ConnectInput(w_xtag);
            muxNx.ConnectInput1(w_xtag);
            muxNx.ConnectInput2(notXtag.Output);
            muxNx.ConnectControl(NotX);
            w_xtagim.ConnectInput(muxNx.Output);

            //Ny:
            notYtag.ConnectInput(w_ytag);
            muxNy.ConnectInput1(w_ytag);
            muxNy.ConnectInput2(notYtag.Output);
            muxNy.ConnectControl(NotY);
            w_ytagim.ConnectInput(muxNy.Output);

            //AndAdd
            this.XtagaimAndYtgaim.ConnectInput1(w_xtagim);
            this.XtagaimAndYtgaim.ConnectInput2(w_ytagim);
            this.w_xtagimANDytagim.ConnectInput(XtagaimAndYtgaim.Output);

            this.XtagimADDYtagim.ConnectInput1(w_xtagim);
            this.XtagimADDYtagim.ConnectInput2(w_ytagim);
            this.w_xtagimADDytagim.ConnectInput(XtagimADDYtagim.Output);

            this.AndMuxAdd.ConnectInput1(w_xtagimANDytagim);
            this.AndMuxAdd.ConnectInput2(w_xtagimADDytagim);
            this.AndMuxAdd.ConnectControl(F);
            this.w_ftag.ConnectInput(AndMuxAdd.Output);

            //No:
            this.OuputOrNotOutput.ConnectInput1(w_ftag);
            this.Not2Output.ConnectInput(w_ftag);
            this.OuputOrNotOutput.ConnectInput2(Not2Output.Output);
            this.OuputOrNotOutput.ConnectControl(NotOutput);
            
            for (int i = 0; i < Size; i++)
            {
                this.w_No[i].ConnectInput(OuputOrNotOutput.Output[i]);
                this.Output[i].ConnectInput(w_No[i]);
            }
            

            //Zr:
            NoTag = new WireSet(iSize);
            for (int i = 0; i < iSize; i++)
            {
                notsOutput[i] = new NotGate();
                notsOutput[i].ConnectInput(w_No[i]);
                NoTag[i].ConnectInput(notsOutput[i].Output);
                
            }
            this.NotOutpusAndZr.ConnectInput(NoTag);
            this.Zero.ConnectInput(NotOutpusAndZr.Output);

            //ng:
            this.Negative.ConnectInput(w_No[iSize-1]);
            
            


        }

        public override bool TestGate()
        {
            Random rnd = new Random();
            int powNumber = (int)Math.Pow(2, Size-1);
            //Console.WriteLine("Start Test");
            for (int i = 0; i < 20; i++)
            {
                int a = rnd.Next(powNumber * (-1), powNumber-1);
                int b = rnd.Next(powNumber * (-1), powNumber-1);
                WireSet wa = new WireSet(this.Size);
                WireSet wb = new WireSet(this.Size);
                wa.Set2sComplement(a);
                wb.Set2sComplement(b);
                for (int j = 0; j < Size; j++)
                {
                    InputX[j].Value = wa[j].Value;
                    InputY[j].Value = wb[j].Value;
                }
                //Console.WriteLine(wa.ToString());
                //Console.WriteLine(wb.ToString());

                int outputVal;
                int inX_Val;
                int inY_Val;

                //Solo zero
                this.ZeroX.Value =      1;
                this.NotX.Value =       0;
                this.ZeroY.Value =      1;
                this.NotY.Value =       0;
                this.F.Value =          1;
                this.NotOutput.Value =  0;
                if(this.Output.Get2sComplement()!=0)
                {
                    //Console.WriteLine("Failed: solo zero, input1={" + this.InputX.ToString() + "} , input2={" + this.InputY.ToString());
                    return false;
                }

                //Solo One
                this.ZeroX.Value = 1;
                this.NotX.Value = 1;
                this.ZeroY.Value = 1;
                this.NotY.Value = 1;
                this.F.Value = 1;
                this.NotOutput.Value = 1;
                if (this.Output.Get2sComplement() != 1)
                {
                    //Console.WriteLine("Failed: solo one, input1={"+this.InputX.ToString() + "} , input2={"+this.InputY.ToString());
                    return false;
                }

                //Solo Minus One
                this.ZeroX.Value = 1;
                this.NotX.Value = 1;
                this.ZeroY.Value = 1;
                this.NotY.Value = 0;
                this.F.Value = 1;
                this.NotOutput.Value = 0;
                if (this.Output.Get2sComplement() != -1)
                {
                    //Console.WriteLine("Failed: solo minus one, input1={" + this.InputX.ToString() + "} , input2={" + this.InputY.ToString());
                    return false;
                }


                //output= X
                this.ZeroX.Value = 0;
                this.NotX.Value = 0;
                this.ZeroY.Value = 1;
                this.NotY.Value = 1;
                this.F.Value = 0;
                this.NotOutput.Value = 0;
                if (this.Output.Get2sComplement() != this.InputX.Get2sComplement())
                {
                    //Console.WriteLine("Failed: X output, input1={" + this.InputX.ToString() + "} , input2={" + this.InputY.ToString());
                    return false;
                }

                //output= Y
                this.ZeroX.Value = 1;
                this.NotX.Value = 1;
                this.ZeroY.Value = 0;
                this.NotY.Value = 0;
                this.F.Value = 0;
                this.NotOutput.Value = 0;
                if (this.Output.Get2sComplement() != this.InputY.Get2sComplement())
                {
                    //Console.WriteLine("Failed: Y output, input1={" + this.InputX.ToString() + "} , input2={" + this.InputY.ToString());
                    return false;
                }

                //output=!X
                this.ZeroX.Value = 0;
                this.NotX.Value = 0;
                this.ZeroY.Value = 1;
                this.NotY.Value = 1;
                this.F.Value = 0;
                this.NotOutput.Value = 1;
                if (this.Output.Get2sComplement() != (~this.InputX.Get2sComplement()))
                {
                    //Console.WriteLine("Failed: not X output, input1={" + this.InputX.ToString() + "} , input2={" + this.InputY.ToString());
                    return false;
                }


                //output=!Y
                this.ZeroX.Value = 1;
                this.NotX.Value = 1;
                this.ZeroY.Value = 0;
                this.NotY.Value = 0;
                this.F.Value = 0;
                this.NotOutput.Value = 1;
                if (this.Output.Get2sComplement() != (~this.InputY.Get2sComplement()))
                {
                    //Console.WriteLine("Failed: not Y output, input1={" + this.InputX.ToString() + "} , input2={" + this.InputY.ToString());
                    return false;
                }

                //output=-X
                this.ZeroX.Value = 0;
                this.NotX.Value = 0;
                this.ZeroY.Value = 1;
                this.NotY.Value = 1;
                this.F.Value = 1;
                this.NotOutput.Value = 1;
                outputVal = this.Output.Get2sComplement();
                inX_Val = this.InputX.Get2sComplement();
                if (outputVal != ((-1)* inX_Val))
                {
                    if (inX_Val == powNumber)
                    {
                        //Console.WriteLine("Failed: -X output, input1={" + this.InputX.ToString() + "} , input2={" + this.InputY.ToString());
                        return false;
                    }
                }

                //output=-Y
                this.ZeroX.Value = 1;
                this.NotX.Value = 1;
                this.ZeroY.Value = 0;
                this.NotY.Value = 0;
                this.F.Value = 1;
                this.NotOutput.Value = 1;
                outputVal = this.Output.Get2sComplement();
                inY_Val = this.InputX.Get2sComplement();
                if (outputVal != ((-1)* inY_Val))
                {
                    if (inY_Val == powNumber)
                    {
                        //Console.WriteLine("Failed: -Y output, input1={" + this.InputY.ToString() + "} , input2={" + this.InputY.ToString());
                        return false;
                    }
                }

                //output=X+1
                this.ZeroX.Value = 0;
                this.NotX.Value = 1;
                this.ZeroY.Value = 1;
                this.NotY.Value = 1;
                this.F.Value = 1;
                this.NotOutput.Value = 1;
                outputVal = this.Output.Get2sComplement();
                inX_Val = this.InputX.Get2sComplement();
                
                if (outputVal != (1+ inX_Val))
                {
                    if ((inX_Val + 1) < powNumber && (inX_Val + 1) >= (-1) * powNumber)
                    {
                        //Console.WriteLine("Failed: X+1 output, input1={" + this.InputX.ToString() + "} , input2={" + this.InputY.ToString());
                        return false;
                    }
                }

                //output=Y+1
                this.ZeroX.Value = 1;
                this.NotX.Value = 1;
                this.ZeroY.Value = 0;
                this.NotY.Value = 1;
                this.F.Value = 1;
                this.NotOutput.Value = 1;
                outputVal = this.Output.Get2sComplement();
                
                inY_Val = this.InputY.Get2sComplement();
                if (outputVal != (1+ inY_Val))
                {
                    if ((inY_Val + 1) < powNumber && (inY_Val + 1) >= (-1) * powNumber)
                    {
                        //Console.WriteLine("Failed: Y+1 output, input1={" + this.InputX.ToString() + "} , input2={" + this.InputY.ToString());
                        return false;
                    }
                }

                //output=X-1
                this.ZeroX.Value = 0;
                this.NotX.Value = 0;
                this.ZeroY.Value = 1;
                this.NotY.Value = 1;
                this.F.Value = 1;
                this.NotOutput.Value = 0;
                outputVal = this.Output.Get2sComplement();
                inX_Val = this.InputX.Get2sComplement();
                inY_Val = this.InputY.Get2sComplement();
                if (outputVal != (inX_Val-1))
                {
                    if ((inX_Val - 1) < powNumber && (inX_Val - 1) >= (-1) * powNumber)
                    {
                        //Console.WriteLine("Failed: X-1 output, input1={" + this.InputX.ToString() + "} , input2={" + this.InputY.ToString());
                        return false;
                    }
                }

                //output=Y-1
                this.ZeroX.Value = 1;
                this.NotX.Value = 1;
                this.ZeroY.Value = 0;
                this.NotY.Value = 0;
                this.F.Value = 1;
                this.NotOutput.Value = 0;
                outputVal = this.Output.Get2sComplement();
                
                inY_Val = this.InputY.Get2sComplement();
                if (outputVal != ((-1) + this.InputY.Get2sComplement()))
                {
                    if ((inY_Val-1) < powNumber && (inY_Val - 1) >= (-1) * powNumber)
                    {
                        //Console.WriteLine("Failed: Y-1 output, input1={" + this.InputX.ToString() + "} , input2={" + this.InputY.ToString());
                        return false;
                    }
                }

                //output=X+Y
                this.ZeroX.Value = 0;
                this.NotX.Value = 0;
                this.ZeroY.Value = 0;
                this.NotY.Value = 0;
                this.F.Value = 1;
                this.NotOutput.Value = 0;
                outputVal = this.Output.Get2sComplement();
                inX_Val = this.InputX.Get2sComplement();
                inY_Val = this.InputY.Get2sComplement();
                if (outputVal != (inX_Val+inY_Val))
                {
                    if (inX_Val + inY_Val < powNumber && inX_Val + inY_Val >= (-1) * powNumber)
                    {
                        //Console.WriteLine("Failed: X+Y output, input1={" + this.InputX.ToString() + "} , input2={" + this.InputY.ToString());
                        //Console.WriteLine("Input X: {" + inX_Val + "} , Input Y : {" + inY_Val + "} , Output: {" + outputVal + "}");
                        return false;
                    }
                    
                }

                //output=X-Y
                this.ZeroX.Value = 0;
                this.NotX.Value = 1;
                this.ZeroY.Value = 0;
                this.NotY.Value = 0;
                this.F.Value = 1;
                this.NotOutput.Value = 1;
                outputVal = this.Output.Get2sComplement();
                inX_Val = this.InputX.Get2sComplement();
                inY_Val = this.InputY.Get2sComplement();
                if (outputVal != (inX_Val - inY_Val))
                {
                    if (inX_Val - inY_Val < powNumber && inX_Val - inY_Val >= (-1) * powNumber)
                    {
                        //Console.WriteLine("Failed: X-Y output, input1={" + this.InputX.ToString() + "} , input2={" + this.InputY.ToString());
                        return false;
                    }
                }

                //output=Y-X
                this.ZeroX.Value = 0;
                this.NotX.Value = 0;
                this.ZeroY.Value = 0;
                this.NotY.Value = 1;
                this.F.Value = 1;
                this.NotOutput.Value = 1;
                outputVal = this.Output.Get2sComplement();
                inX_Val = this.InputX.Get2sComplement();
                inY_Val = this.InputY.Get2sComplement();
                if (outputVal != (inY_Val - inX_Val))
                {
                    if (inY_Val - inX_Val < powNumber && inY_Val - inX_Val >= (-1) * powNumber)
                    {
                        //Console.WriteLine("Failed: Y-X output, input1={" + this.InputX.ToString() + "} , input2={" + this.InputY.ToString());
                        return false;
                    }
                }

                //output=X&Y
                this.ZeroX.Value = 0;
                this.NotX.Value = 0;
                this.ZeroY.Value = 0;
                this.NotY.Value = 0;
                this.F.Value = 0;
                this.NotOutput.Value = 0;

                if (this.Output.Get2sComplement() != (this.InputX.Get2sComplement() & this.InputY.Get2sComplement()))
                {
                    //Console.WriteLine("Failed: X&Y output, input1={" + this.InputX.ToString() + "} , input2={" + this.InputY.ToString());
                    return false;
                }

                //output=X|Y
                this.ZeroX.Value = 0;
                this.NotX.Value = 1;
                this.ZeroY.Value = 0;
                this.NotY.Value = 1;
                this.F.Value = 0;
                this.NotOutput.Value = 1;
                if (this.Output.Get2sComplement() != (this.InputX.Get2sComplement() | this.InputY.Get2sComplement()))
                {
                    //Console.WriteLine("Failed: X|Y output, input1={" + this.InputX.ToString() + "} , input2={" + this.InputY.ToString());
                    return false;
                }


                //WireSet ctrl = new WireSet(6);
                //ctrl.SetValue(j);
                //this.ZeroX.Value = ctrl[0].Value;
                //this.NotX.Value = ctrl[1].Value;
                //this.ZeroY.Value = ctrl[2].Value;
                //this.NotY.Value = ctrl[3].Value;
                //this.F.Value = ctrl[4].Value;
                //this.NotOutput.Value = ctrl[5].Value;




            }
            return true;
        }

        
    }
}
