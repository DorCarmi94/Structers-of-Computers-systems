using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler
{
    public class Assembler
    {
        private const int WORD_SIZE = 16;

        private Dictionary<string, int[]> m_dControl, m_dJmp, m_dDest; //these dictionaries map command mnemonics to machine code - they are initialized at the bottom of the class
        private Dictionary<string, int> m_dSymbols;
        private Dictionary<string, int> m_dLinesLabels;
        private string ENGLISH_ALEF_BET = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private string ENGLISH_ALEF_BET_NUMBERS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_";
        private string DIGITS = "0123456789";
        private Dictionary<string, Func<List<string>,List<string>>> macrosDictionary;
        private int nextLocationInRAMforSymbol=16;

        //more data structures here (symbol map, ...)

        public Assembler()
        {
            InitCommandDictionaries();
            
        }

        //this method is called from the outside to run the assembler translation
        public void TranslateAssemblyFile(string sInputAssemblyFile, string sOutputMachineCodeFile)
        {
            //read the raw input, including comments, errors, ...
            StreamReader sr = new StreamReader(sInputAssemblyFile);
            List<string> lLines = new List<string>();
            while (!sr.EndOfStream)
            {
                lLines.Add(sr.ReadLine());
            }
            sr.Close();
            //translate to machine code
            List<string> lTranslated = TranslateAssemblyFile(lLines);
            //write the output to the machine code file
            StreamWriter sw = new StreamWriter(sOutputMachineCodeFile);
            foreach (string sLine in lTranslated)
                sw.WriteLine(sLine);
            sw.Close();
            
        }

        //TODO: Delete
        private void WriteToFile(List<string> lTranslated, string sOutputMachineCodeFile)
        {
            //write the output to the machine code file
            StreamWriter sw = new StreamWriter(sOutputMachineCodeFile);
            foreach (string sLine in lTranslated)
                sw.WriteLine(sLine);
            sw.Close();

        }

        //translate assembly into machine code
        private List<string> TranslateAssemblyFile(List<string> lLines)
        {
            //implementation order:
            //first, implement "TranslateAssemblyToMachineCode", and check if the examples "Add", "MaxL" are translated correctly.
            //next, implement "CreateSymbolTable", and modify the method "TranslateAssemblyToMachineCode" so it will support symbols (translating symbols to numbers). check this on the examples that don't contain macros
            //the last thing you need to do, is to implement "ExpendMacro", and test it on the example: "SquareMacro.asm".
            //init data structures here 

            //expand the macros
            List<string> lAfterMacroExpansion = ExpendMacros(lLines);
            //this.WriteToFile(lAfterMacroExpansion, @"D:\University\3rd Simester\Structers of Computers systems\Homework\Ex2\Ex2.3\Code\Assembly examples\SquareNoMacros.asm");
            //first pass - create symbol table and remove lable lines
            CreateSymbolTable(lAfterMacroExpansion);

            //second pass - replace symbols with numbers, and translate to machine code
            List<string> lAfterTranslation = TranslateAssemblyToMachineCode(lAfterMacroExpansion);
            return lAfterTranslation;
        }

        
        //first pass - replace all macros with real assembly
        private List<string> ExpendMacros(List<string> lLines)
        {
            
            //Console.WriteLine("\n\nMacros expend:\n\n");
            //You do not need to change this function, you only need to implement the "ExapndMacro" method (that gets a single line == string)
            List<string> lAfterExpansion = new List<string>();
            for (int i = 0; i < lLines.Count; i++)
            {
                //remove all redudant characters
                string sLine = CleanWhiteSpacesAndComments(lLines[i]);
                if (sLine == "")
                    continue;
                //if the line contains a macro, expand it, otherwise the line remains the same
                List<string> lExpanded = ExapndMacro(sLine);
                //we may get multiple lines from a macro expansion
                foreach (string sExpanded in lExpanded)
                {
                    lAfterExpansion.Add(sExpanded);
                }
            }
            return lAfterExpansion;
        }

        //expand a single macro line
        private List<string> ExapndMacro(string sLine)
        {
            
            List<string> lExpanded = new List<string>();

            if (IsCCommand(sLine))
            {
                string sDest, sCompute, sJmp;
                GetCommandParts(sLine, out sDest, out sCompute, out sJmp);
                //your code here - check for indirect addessing and for jmp shortcuts
                //check for first macro
                if (sLine.Contains("++"))
                {
                    string nameOfVal = sLine.Substring(0, sLine.IndexOf('+'));
                    if (nameOfVal.Length > 0 && nameOfVal.All((char ch) => { return this.ENGLISH_ALEF_BET_NUMBERS.IndexOf(ch) != -1; }))
                    {
                        List<string> args = new List<string>();
                        args.Add(nameOfVal);
                        lExpanded.AddRange(this.macrosDictionary["number++"].Invoke(args));
                    }
                    else
                    {
                        throw new FormatException("Cannot parse macro: " + ": " + sLine);
                    }
                    
                }
                if(sLine.Contains("--"))
                {
                    string nameOfVal = sLine.Substring(0, sLine.IndexOf('-'));
                    if (nameOfVal.Length > 0 && nameOfVal.All((char ch) => { return this.ENGLISH_ALEF_BET_NUMBERS.IndexOf(ch) != -1; }))
                    {
                        List<string> args = new List<string>();
                        args.Add(nameOfVal);
                        lExpanded.AddRange(this.macrosDictionary["number--"].Invoke(args));
                    }
                    else
                    {
                        throw new FormatException("Cannot parse macro: " + ": " + sLine);
                    }

                }
                if(sLine.Contains("+")&& (sLine.Count((char ch)=> { return ch == '+'; }))==1)
                {
                    char leftPlus = sLine.ElementAt(sLine.IndexOf('+') - 1);
                    char rightPlus = sLine.ElementAt(sLine.IndexOf('+') + 1);
                    string leftEqSign = sLine.Substring(0, sLine.IndexOf('='));

                    if (this.m_dDest.Keys.Contains(leftPlus.ToString()) && leftPlus!='D' && rightPlus == 'D')
                    {
                        
                        List<string> args = new List<string>();
                        args.Add(leftEqSign);
                        args.Add(leftPlus.ToString());
                        lExpanded.AddRange(this.macrosDictionary["REG1=REG2+D"].Invoke(args));
                    }
                }
                if(sLine.Contains('=') && !sLine.Contains(';'))
                {
                    string left = sLine.Substring(0, sLine.IndexOf('='));
                    string right = sLine.Substring(sLine.IndexOf('=')+1);
                    if(this.m_dDest.Keys.Contains(left) && !this.m_dDest.Keys.Contains(right))
                    {
                        //Left is register and right is label
                        //(char ch, out ans) <= { this.ENGLISH_ALEF_BET.IndexOf(ch) != -1};
                        if (right.All((char ch) => { return this.ENGLISH_ALEF_BET_NUMBERS.IndexOf(ch) != -1; }))
                        {
                            List<String> args = new List<string>();
                            args.Add(left);
                            args.Add(right);
                            lExpanded.AddRange(this.macrosDictionary["Reg=Number"].Invoke(args));
                        }
                    }
                    
                    if (!this.m_dDest.Keys.Contains(left) && this.m_dDest.Keys.Contains(right))
                    {
                        
                        List<String> args = new List<string>();
                        args.Add(left);
                        args.Add(right);
                        lExpanded.AddRange(this.macrosDictionary["Number=Reg"].Invoke(args));
                    }
                    if(!this.m_dDest.Keys.Contains(right) && !this.m_dDest.Keys.Contains(left))
                    {
                        if(left.All((char ch) => { return this.ENGLISH_ALEF_BET_NUMBERS.IndexOf(ch) != -1; }) &&
                            right.All((char ch) => { return this.DIGITS.IndexOf(ch) != -1; }))
                        {
                            List<String> args = new List<string>();
                            args.Add(left);
                            args.Add(right);
                            lExpanded.AddRange(this.macrosDictionary["NUMBER=VAL"].Invoke(args));
                        }
                        else if (left.All((char ch) => { return this.ENGLISH_ALEF_BET_NUMBERS.IndexOf(ch) != -1; }) &&
                            right.All((char ch) => { return this.ENGLISH_ALEF_BET_NUMBERS.IndexOf(ch) != -1; }))
                        {
                            List<String> args = new List<string>();
                            args.Add(left);
                            args.Add(right);
                            lExpanded.AddRange(this.macrosDictionary["Number=Number"].Invoke(args));
                        }
                    }
                }
                if(sLine.Contains(':'))
                {
                    string left = sLine.Substring(0, sLine.IndexOf(':'));
                    string right = sLine.Substring(sLine.IndexOf(':') + 1);
                    List<string> args = new List<string>();
                    args.Add(left);
                    args.Add(right);

                    lExpanded.AddRange(this.macrosDictionary["JMP:LABEL"].Invoke(args));

                }
                //read the word file to see all the macros you need to support
                /*if(lExpanded.Count>0)
                {
                    Console.WriteLine("\nMacro:");
                    Console.WriteLine(sLine + " -> \n");
                }
                for (int i = 0; i < lExpanded.Count; i++)
                {
                    Console.WriteLine(lExpanded[i].ToString());
                }*/
                    
            }
            if (lExpanded.Count == 0)
                lExpanded.Add(sLine);
            return lExpanded;
        }

        //second pass - record all symbols - labels and variables
        private void CreateSymbolTable(List<string> lLines)
        {
            int linesCount = 0;
            string sLine = "";
            for (int i = 0; i < lLines.Count; i++)
            {
                if(i==51)
                    Console.WriteLine("");
                sLine = lLines[i];
                if (IsLabelLine(sLine))
                {
                    int open = sLine.IndexOf('(');
                    int close = sLine.IndexOf(')');
                    string str = sLine.Substring(open+1, close-1);
                    if (this.m_dSymbols.Keys.Contains(str))
                    {                        
                        if(this.m_dLinesLabels.Keys.Contains(str))
                        {
                            
                            throw new FormatException("Cannot parse line " + i + ": " + lLines[i] + " : dual name for label");
                        }
                        else
                        {
                            this.m_dSymbols[str] = -1;
                            this.m_dLinesLabels.Add(str, linesCount);
                        }
                        
                    }
                    else
                    {
                        this.m_dSymbols.Add(str, linesCount);
                        this.m_dLinesLabels.Add(str, linesCount);
                    }
                }
                else if (IsACommand(sLine))
                {
                    
                    int indexOf = sLine.IndexOfAny(ENGLISH_ALEF_BET.ToCharArray());
                    if (indexOf!=-1)
                    {
                        if(indexOf>1)
                            throw new FormatException("Cannot parse line " + i + ": " + lLines[i]);
                        string newName = sLine.Substring(1);
                        if(!this.m_dSymbols.Keys.Contains(newName))
                        {
                            
                            this.m_dSymbols.Add(newName, this.nextLocationInRAMforSymbol);
                            nextLocationInRAMforSymbol++;
                            if (nextLocationInRAMforSymbol == this.m_dSymbols["SCREEN"] || nextLocationInRAMforSymbol == this.m_dSymbols["KEYBOARD"])
                                nextLocationInRAMforSymbol++;
                        }
                    }
                    linesCount++;
                }
                else if (IsCCommand(sLine))
                {
                    linesCount++;
                    //do nothing here
                }
                else
                    throw new FormatException("Cannot parse line " + i + ": " + lLines[i]);
            }
          
        }
        
        //third pass - translate lines into machine code, replacing symbols with numbers
        private List<string> TranslateAssemblyToMachineCode(List<string> lLines)
        {
            
            //Console.WriteLine("\n\nTurning to Binary:\n\n");   
            string sLine = "";
            List<string> lAfterPass = new List<string>();
            bool changed = false;
            for (int i = 0; i < lLines.Count; i++)
            {
                changed = true;
                //if(i==3)
                   // Console.WriteLine("");
                sLine = lLines[i];
                
                if (IsACommand(sLine))
                {
                    //translate an A command into a sequence of bits
                    string strWithoutAt = sLine.Substring(1);
                    int strInt = -1;
                    if (strWithoutAt.IndexOfAny(ENGLISH_ALEF_BET.ToCharArray()) != -1)
                    {
                        if (this.m_dSymbols.ContainsKey(strWithoutAt) && this.m_dSymbols[strWithoutAt]!=-1)
                        {
                            strInt = m_dSymbols[strWithoutAt];
                        }
                        else if(this.m_dLinesLabels.ContainsKey(strWithoutAt))
                        {
                            strInt = m_dLinesLabels[strWithoutAt];
                        }
                        else
                        {
                            //Unknown symbol
                            throw new FormatException("Unknown symbol in line: " + i + ": " + lLines[i]);
                        }

                    }
                    else
                    {
                        strInt = Convert.ToInt32(strWithoutAt);
                    }
                    string ans = ToBinary(strInt);
                    lAfterPass.Add(ans);

                }
                else if (IsCCommand(sLine))
                {

                    string sDest, sControl, sJmp;
                    GetCommandParts(sLine, out sDest, out sControl, out sJmp);
                    //translate an C command into a sequence of bits
                    //take a look at the dictionaries m_dControl, m_dJmp, and where they are initialized (InitCommandDictionaries), to understand how to you them here
                    string function = "111";
                    if (this.m_dControl.ContainsKey(sControl))
                    {
                        String str = String.Join("", m_dControl[sControl].Select(p => p.ToString()));
                        function += str;
                    }
                    else
                    {
                        throw new FormatException("Cannot parse line " + i + ": " + lLines[i]);
                    }
                    if (this.m_dDest.ContainsKey(sDest))
                    {
                        String str = String.Join("", m_dDest[sDest].Select(p => p.ToString()));
                        function += str;
                    }
                    else
                    {
                        throw new FormatException("Cannot parse line " + i + ": " + lLines[i]);
                    }
                    if (this.m_dJmp.ContainsKey(sJmp))
                    {
                        String str = String.Join("", m_dJmp[sJmp].Select(p => p.ToString()));
                        function += str;
                    }
                    else
                    {
                        throw new FormatException("Cannot parse line " + i + ": " + lLines[i]);
                    }

                    lAfterPass.Add(function);

                }
                else if (IsLabelLine(sLine))
                {
                    changed = false;
                    //for label checking

                    int open = sLine.IndexOf('(');
                    int close = sLine.IndexOf(')');
                    string strWithoutBreakets = sLine.Substring(open + 1, close - 1);
                    //if (this.m_dSymbols.Keys.Contains(strWithoutBreakets) && !this.m_dLinesLabels.Keys.Contains(strWithoutBreakets))
                    //{
                    //    if()
                    //    string ans = this.ToBinary(val);
                    //    //lAfterPass.Add(ans);
                    //}
                }
                else
                    throw new FormatException("Cannot parse line " + i + ": " + lLines[i]);
                //if(lAfterPass.Count>0 && changed)
                //    Console.WriteLine(sLine + " -> " + lAfterPass.Last());
            }
            //Console.ReadLine();
            return lAfterPass;
            
        }

        //helper functions for translating numbers or bits into strings
        private string ToString(int[] aBits)
        {
            string sBinary = "";
            for (int i = 0; i < aBits.Length; i++)
                sBinary += aBits[i];
            return sBinary;
        }

        private string ToBinary(int x)
        {
            string sBinary = "";
            for (int i = 0; i < WORD_SIZE; i++)
            {
                sBinary = (x % 2) + sBinary;
                x = x / 2;
            }
            return sBinary;
        }


        //helper function for splitting the various fields of a C command
        private void GetCommandParts(string sLine, out string sDest, out string sControl, out string sJmp)
        {
            if (sLine.Contains('='))
            {
                int idx = sLine.IndexOf('=');
                sDest = sLine.Substring(0, idx);
                sLine = sLine.Substring(idx + 1);
            }
            else
                sDest = "";
            if (sLine.Contains(';'))
            {
                int idx = sLine.IndexOf(';');
                sControl = sLine.Substring(0, idx);
                sJmp = sLine.Substring(idx + 1);

            }
            else
            {
                sControl = sLine;
                sJmp = "";
            }
        }

        private bool IsCCommand(string sLine)
        {
            return !IsLabelLine(sLine) && sLine[0] != '@';
        }

        private bool IsACommand(string sLine)
        {
            return sLine[0] == '@';
        }

        private bool IsLabelLine(string sLine)
        {
            if (sLine.StartsWith("(") && sLine.EndsWith(")"))
                return true;
            return false;
        }

        private string CleanWhiteSpacesAndComments(string sDirty)
        {
            string sClean = "";
            for (int i = 0 ; i < sDirty.Length ; i++)
            {
                char c = sDirty[i];
                if (c == '/' && i < sDirty.Length - 1 && sDirty[i + 1] == '/') // this is a comment
                    return sClean;
                if (c > ' ' && c <= '~')//ignore white spaces
                    sClean += c;
            }
            return sClean;
        }


        private void InitCommandDictionaries()
        {
            m_dControl = new Dictionary<string, int[]>();

            m_dControl["0"] = new int[] { 0, 1, 0, 1, 0, 1, 0 };
            m_dControl["1"] = new int[] { 0, 1, 1, 1, 1, 1, 1 };
            m_dControl["-1"] = new int[] { 0, 1, 1, 1, 0, 1, 0 };
            m_dControl["D"] = new int[] { 0, 0, 0, 1, 1, 0, 0 };
            m_dControl["A"] = new int[] { 0, 1, 1, 0, 0, 0, 0 };
            m_dControl["!D"] = new int[] { 0, 0, 0, 1, 1, 0, 1 };
            m_dControl["!A"] = new int[] { 0, 1, 1, 0, 0, 0, 1 };
            m_dControl["-D"] = new int[] { 0, 0, 0, 1, 1, 1, 1 };
            m_dControl["-A"] = new int[] { 0, 1, 1, 0, 0,1, 1 };
            m_dControl["D+1"] = new int[] { 0, 0, 1, 1, 1, 1, 1 };
            m_dControl["A+1"] = new int[] { 0, 1, 1, 0, 1, 1, 1 };
            m_dControl["D-1"] = new int[] { 0, 0, 0, 1, 1, 1, 0 };
            m_dControl["A-1"] = new int[] { 0, 1, 1, 0, 0, 1, 0 };
            m_dControl["D+A"] = new int[] { 0, 0, 0, 0, 0, 1, 0 };
            m_dControl["D-A"] = new int[] { 0, 0, 1, 0, 0, 1, 1 };
            m_dControl["A-D"] = new int[] { 0, 0, 0, 0, 1,1, 1 };
            m_dControl["D&A"] = new int[] { 0, 0, 0, 0, 0, 0, 0 };
            m_dControl["D|A"] = new int[] { 0, 0, 1, 0,1, 0, 1 };

            m_dControl["M"] = new int[] { 1, 1, 1, 0, 0, 0, 0 };
            m_dControl["!M"] = new int[] { 1, 1, 1, 0, 0, 0, 1 };
            m_dControl["-M"] = new int[] { 1, 1, 1, 0, 0, 1, 1 };
            m_dControl["M+1"] = new int[] { 1, 1, 1, 0, 1, 1, 1 };
            m_dControl["M-1"] = new int[] { 1, 1, 1, 0, 0, 1, 0 };
            m_dControl["D+M"] = new int[] { 1, 0, 0, 0, 0, 1, 0 };
            m_dControl["D-M"] = new int[] { 1, 0, 1, 0, 0, 1, 1 };
            m_dControl["M-D"] = new int[] { 1, 0, 0, 0, 1, 1, 1 };
            m_dControl["D&M"] = new int[] { 1, 0, 0, 0, 0, 0, 0 };
            m_dControl["D|M"] = new int[] { 1, 0, 1, 0, 1, 0, 1 };

            m_dDest = new Dictionary<string, int[]>();
            m_dDest[""] = new int[] { 0, 0, 0 };
            m_dDest["M"] = new int[] { 0, 0, 1 };
            m_dDest["D"] = new int[] { 0, 1, 0 };
            m_dDest["MD"] = new int[] { 0, 1, 1 };
            m_dDest["A"] = new int[] { 1, 0, 0 };
            m_dDest["AM"] = new int[] { 1, 0, 1 };
            m_dDest["AD"] = new int[] { 1, 1, 0 };
            m_dDest["AMD"] = new int[] { 1, 1, 1 };

            m_dJmp = new Dictionary<string, int[]>();

            m_dJmp[""] = new int[] { 0, 0, 0 };
            m_dJmp["JGT"] = new int[] { 0, 0, 1 };
            m_dJmp["JEQ"] = new int[] { 0, 1, 0 };
            m_dJmp["JGE"] = new int[] { 0, 1, 1 };
            m_dJmp["JLT"] = new int[] { 1, 0, 0 };
            m_dJmp["JNE"] = new int[] { 1, 0, 1 };
            m_dJmp["JLE"] = new int[] { 1, 1, 0 };
            m_dJmp["JMP"] = new int[] { 1, 1, 1 };

            this.m_dSymbols = new Dictionary<string, int>();
            m_dSymbols.Add("R0", 0);
            m_dSymbols.Add("R1", 1);
            m_dSymbols.Add("R2", 2);
            m_dSymbols.Add("R3", 3);
            m_dSymbols.Add("R4", 4);
            m_dSymbols.Add("R5", 5);
            m_dSymbols.Add("R6", 6);
            m_dSymbols.Add("R7", 7);
            m_dSymbols.Add("R8", 8);
            m_dSymbols.Add("R9", 9);
            m_dSymbols.Add("R10", 10);
            m_dSymbols.Add("R11", 11);
            m_dSymbols.Add("R12", 12);
            m_dSymbols.Add("R13", 13);
            m_dSymbols.Add("R14", 14);
            m_dSymbols.Add("R15", 15);
            m_dSymbols.Add("SCREEN", 0x4000);
            m_dSymbols.Add("KEYBOARD", 0x6000);


            macrosDictionary = new Dictionary<string, Func<List<string>, List<string>>>();
            macrosDictionary["number++"] = (List<string> nameOfVal) =>
            {
                List<string> result = new List<string>();
                result.Add("@" + nameOfVal[0]);
                result.Add("M=M+1");
                return result;
            };

            macrosDictionary["number--"] = (List<string> nameOfVal) =>
            {
                List<string> result = new List<string>();
                result.Add("@" + nameOfVal[0]);
                result.Add("M=M-1");
                return result;
            };

            macrosDictionary["Number=Reg"] = (List<string> nameOfVal) =>
            {
                //0- number
                //1- reg
                List<string> result = new List<string>();
                result.Add("@" + nameOfVal[0]);
                result.Add("M="+nameOfVal[1]);
                return result;
            };

            macrosDictionary["Reg=Number"] = (List<string> nameOfVal) =>
            {
                //0-reg
                //1-number
                List<string> result = new List<string>();
                result.Add("@" + nameOfVal[1]);
                result.Add(nameOfVal[0]+"=M");
                return result;
            };

            macrosDictionary["Number=Number"] = (List<string> nameOfVal) =>
            {
                //x=y
                //0-left number
                //1-right number
                List<string> result = new List<string>();
                result.Add("@" + nameOfVal[1]);
                result.Add("D=M");//D=y
                result.Add("@" + nameOfVal[0]);
                result.Add("M=D");//x=D
                return result;
            };

            macrosDictionary["JMP:LABEL"] = (List<string> nameOfVal) =>
            {
                //0-Dest=Comp;JMP
                //1-Label
                List<string> result = new List<string>();
                result.Add("@" + nameOfVal[1]);
                result.Add(nameOfVal[0]);
                return result;
            };

            macrosDictionary["REG1=REG2+D"] = (List<string> nameOfVal) =>
            {
                //0- REG 1
                //1- REG 2
                string reg1 = nameOfVal[0];
                string reg2 = nameOfVal[1];
                List<string> result = new List<string>();
                result.Add(reg1 + "=D+" + reg2);
                return result;
            };
            macrosDictionary["NUMBER=VAL"] = (List<string> nameOfVal) =>
            {
                //0- LABEL
                //1- NUMBER
                string label = nameOfVal[0];
                string number = nameOfVal[1];
                List<string> result = new List<string>();
                result.Add("@"+number);
                result.Add("D=A");
                result.Add("@"+label);
                result.Add("M=D");
                return result;
            };

            m_dLinesLabels = new Dictionary<string, int>();
        }
    }
}
