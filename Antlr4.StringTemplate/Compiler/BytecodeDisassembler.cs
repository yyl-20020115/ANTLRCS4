/*
 * [The "BSD licence"]
 * Copyright (c) 2011 Terence Parr
 * All rights reserved.
 *
 * Conversion to C#:
 * Copyright (c) 2011 Sam Harwell, Tunnel Vision Laboratories, LLC
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 * 1. Redistributions of source code must retain the above copyright
 *    notice, this list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright
 *    notice, this list of conditions and the following disclaimer in the
 *    documentation and/or other materials provided with the distribution.
 * 3. The name of the author may not be used to endorse or promote products
 *    derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR
 * IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
 * OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
 * IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
 * INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
 * NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 * DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
 * THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
 * THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

namespace Antlr4.StringTemplate.Compiler;

using System.Collections.Generic;
using Antlr4.StringTemplate.Misc;
using ArgumentException = System.ArgumentException;
using BitConverter = System.BitConverter;
using StringBuilder = System.Text.StringBuilder;

public class BytecodeDisassembler
{
    private readonly CompiledTemplate code;

    public BytecodeDisassembler(CompiledTemplate code) => this.code = code;

    public virtual string GetInstructions()
    {
        var buffer = new StringBuilder();
        int ip = 0;
        while (ip < code.codeSize)
        {
            if (ip > 0)
                buffer.Append(", ");
            int opcode = code.instrs[ip];
            var I = Instruction.instructions[opcode];
            buffer.Append(I.name);
            ip++;
            for (int opnd = 0; opnd < I.nopnds; opnd++)
            {
                buffer.Append(' ');
                buffer.Append(GetShort(code.instrs, ip));
                ip += Instruction.OperandSizeInBytes;
            }
        }
        return buffer.ToString();
    }

    public virtual string Disassemble()
    {
        var buffer = new StringBuilder();
        int i = 0;
        while (i < code.codeSize)
        {
            i = DisassembleInstruction(buffer, i);
            buffer.AppendLine();
        }
        return buffer.ToString();
    }

    public virtual int DisassembleInstruction(StringBuilder buffer, int ip)
    {
        int opcode = code.instrs[ip];
        if (ip >= code.codeSize)
        {
            throw new ArgumentException("ip out of range: " + ip);
        }
        var I = Instruction.instructions[opcode];
        if (I == null)
        {
            throw new ArgumentException("no such instruction " + opcode + " at address " + ip);
        }
        string instrName = I.name;
        buffer.Append(string.Format("{0:0000}:\t{1,-14}", ip, instrName));
        ip++;
        if (I.nopnds == 0)
        {
            buffer.Append("  ");
            return ip;
        }

        var operands = new List<string>();
        for (int i = 0; i < I.nopnds; i++)
        {
            int opnd = GetShort(code.instrs, ip);
            ip += Instruction.OperandSizeInBytes;
            switch (I.type[i])
            {
            case OperandType.String:
                operands.Add(ShowConstantPoolOperand(opnd));
                break;

            case OperandType.Address:
            case OperandType.Int:
                operands.Add(opnd.ToString());
                break;

            default:
                operands.Add(opnd.ToString());
                break;
            }
        }

        for (int i = 0; i < operands.Count; i++)
        {
            var s = operands[i];
            if (i > 0)
                buffer.Append(", ");

            buffer.Append(s);
        }
        return ip;
    }

    private string ShowConstantPoolOperand(int poolIndex)
    {
        var buffer = new StringBuilder();
        buffer.Append('#');
        buffer.Append(poolIndex);
        string s = "<bad string index>";
        if (poolIndex < code.strings.Length)
        {
            if (code.strings[poolIndex] == null)
                s = "null";
            else
            {
                s = code.strings[poolIndex];
                if (code.strings[poolIndex] != null)
                {
                    s = Utility.ReplaceEscapes(s);
                    s = '"' + s + '"';
                }
            }
        }
        buffer.Append(':');
        buffer.Append(s);
        return buffer.ToString();
    }

    internal static int GetShort(byte[] memory, int index) 
        => BitConverter.ToInt16(memory, index);

    public virtual string GetStrings()
    {
        var buffer = new StringBuilder();
        int addr = 0;
        if (code.strings != null)
        {
            foreach (var o in code.strings)
            {
                if (o is string s)
                {
                    s = Utility.ReplaceEscapes(s);
                    buffer.AppendLine(string.Format("{0:0000}: \"{1}\"", addr, s));
                }
                else
                {
                    buffer.AppendLine(string.Format("{0:0000}: {1}", addr, o));
                }
                addr++;
            }
        }
        return buffer.ToString();
    }

    public virtual string GetSourceMap()
    {
        var buffer = new StringBuilder();
        int addr = 0;
        foreach (var interval in code.sourceMap)
        {
            if (interval != null)
            {
                var chunk = code.Template.Substring(interval.Start, interval.Length);
                buffer.AppendLine(string.Format("{0:0000}: {1}\t\"{2}\"", addr, interval, chunk));
            }
            addr++;
        }

        return buffer.ToString();
    }
}
