﻿/*
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

using Antlr3.Runtime;
using Antlr3.Runtime.Tree;
using Antlr4.StringTemplate.Misc;

partial class CodeGenerator
{
    /// <summary>Name of overall template</summary>
    private readonly string outermostTemplateName;
    /// <summary>Overall template token</summary>
    private readonly IToken templateToken;
    /// <summary>Overall template text</summary>
    private readonly string _template;
    private readonly TemplateCompiler _compiler;
    private CompiledTemplate outermostImpl;

    public CodeGenerator(ITreeNodeStream input, TemplateCompiler compiler, string name, string template, IToken templateToken)
        : this(input, new RecognizerSharedState())
    {
        this._compiler = compiler;
        this.outermostTemplateName = name;
        this._template = template;
        this.templateToken = templateToken;
    }

    public ErrorManager ErrMgr => _compiler.ErrorManager;

    public TemplateGroup Group => _compiler.Group;

    public CompilationState CompilationState 
        => template_stack == null || template_stack.Count == 0 ? null : template_stack.Peek().state;

    // convience funcs to hide offensive sending of emit messages to
    // CompilationState temp data object.

    public void Emit1(CommonTree opAST, Bytecode opcode, int arg) => CompilationState.Emit1(opAST, opcode, arg);

    public void Emit1(CommonTree opAST, Bytecode opcode, string arg) => CompilationState.Emit1(opAST, opcode, arg);

    public void Emit2(CommonTree opAST, Bytecode opcode, int arg, int arg2) => CompilationState.Emit2(opAST, opcode, arg, arg2);

    public void Emit2(CommonTree opAST, Bytecode opcode, string s, int arg2) => CompilationState.Emit2(opAST, opcode, s, arg2);

    public void Emit(CommonTree opAST, Bytecode opcode) => CompilationState.Emit(opAST, opcode);

    private void Indent(CommonTree indent) => CompilationState.Indent(indent);

    private void Dedent() => CompilationState.Emit(Bytecode.INSTR_DEDENT);

    public void Insert(int addr, Bytecode opcode, string s) => CompilationState.Insert(addr, opcode, s);

    public void SetOption(CommonTree id) => CompilationState.SetOption(id);

    public void Write(int addr, short value) => CompilationState.Write(addr, value);

    public int Address() => CompilationState.ip;

    public void Func(CommonTree id) => CompilationState.Function(templateToken, id);

    public void RefAttr(CommonTree id) => CompilationState.ReferenceAttribute(templateToken, id);

    public int DefineString(string s) => CompilationState.DefineString(s);
}
