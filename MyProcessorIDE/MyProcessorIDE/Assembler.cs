﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProcessorIDE
{
    static class Assembler
    {
        public static int MemoryDepth = 32;
        public static int MemoryWidth = 8;

        public static String Assemble(String assembleCode)
        {
            String[] lines = assembleCode.Split('\n');
            StringBuilder output = new StringBuilder();
            output.AppendLine("DEPTH = " + MemoryDepth.ToString() + ";");
            output.AppendLine("WIDTH = " + MemoryWidth.ToString() + ";");
            output.AppendLine("");
            output.AppendLine("ADDRESS_RADIX = bin;");
            output.AppendLine("DATA_RADIX = bin;");
            output.AppendLine("");
            output.AppendLine("CONTENT BEGIN");

            foreach (String line in lines)
            {
            }

            for (int index = 0; index < MemoryDepth; index++)
            {
                output.Append('\t');
                output.Append(prependLeadingZeros(Convert.ToString(index, 2), 5));
                output.Append(" : ");
                if (index < lines.Length && lines[index].Trim() != "")
                {
                    output.Append(commandToData(lines[index], index));
                }
                else
                {
                    output.Append("00000000");
                }
                output.AppendLine(";");
            }

            output.Append("END;");
            return output.ToString();
        }

        private static string commandToData(string command, int line)
        {
            String result = "";
            command = command.ToLower();

            if(command.IndexOf('%') != -1)
            {
                command = command.Substring(0, command.IndexOf('%'));
            }

            if (command.StartsWith("add"))
            {
                command = command.Substring(3);
                result = "000";
            }
            else if (command.StartsWith("sub"))
            {
                command = command.Substring(3);
                result = "001";
            }
            else if (command.StartsWith("mul"))
            {
                command = command.Substring(3);
                result = "010";
            }
            else if (command.StartsWith("div"))
            {
                command = command.Substring(3);
                result = "011";
            }
            else if (command.StartsWith("load"))
            {
                command = command.Substring(4);
                result = "100";
            }
            else if (command.StartsWith("save"))
            {
                command = command.Substring(4);
                result = "101";
            }
            else if (command.StartsWith("goto"))
            {
                command = command.Substring(4);
                result = "110";
            }
            else if (command.StartsWith("print"))
            {
                return "11100000";
            }
            else if (command.StartsWith("const"))
            {
                command = command.Substring(5).Trim();

                int constValue = Convert.ToInt16(command);
                if (constValue < Math.Pow(2, MemoryWidth) && constValue >= 0)
                {
                    String constString = Convert.ToString(constValue, 2);
                    constString = prependLeadingZeros(constString, MemoryWidth);

                    return constString;
                }
                else
                {
                    throw new CompilerErrorException("Constant out of range", line, 0);
                }
            }
            else
            {
                throw new CompilerErrorException("Unknown command", line, 0);
            }

            command = command.Trim();

            if (command.Length == 5)
            {
                try
                {
                    int arg = Convert.ToInt16(command, 2);
                    result += command;
                }
                catch (FormatException)
                {
                    throw new CompilerErrorException("Invalid argument format", line, 0);
                }
                catch (OverflowException)
                {
                    throw new CompilerErrorException("Invalid argument format", line, 0);
                }
            }
            else
            {
                try
                {
                    int arg = Convert.ToInt16(command, 10);
                    if (arg >= MemoryDepth)
                    {
                        throw new CompilerErrorException("Too big adress", line, 0);
                    }
                    else
                    {
                        String argString = Convert.ToString(arg, 2);
                        argString = prependLeadingZeros(argString, 5);
                        result += argString;
                    }
                }
                catch (FormatException)
                {
                    throw new CompilerErrorException("Invalid argument format", line, 0);
                }
                catch (OverflowException)
                {
                    throw new CompilerErrorException("Invalid argument format", line, 0);
                }
            }

            return result;
        }

        private static string prependLeadingZeros(string str, int length)
        {
            while (str.Length < length) str = "0" + str;
            return str;
        }
    }
}
