﻿/*
 * [The "BSD licence"]
 * Copyright (c) 2005-2008 Terence Parr
 * All rights reserved.
 *
 * Conversion to C#:
 * Copyright (c) 2008 Sam Harwell, Pixel Mine, Inc.
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

namespace Antlr3.Runtime.Tree;

using Exception = System.Exception;

#if !PORTABLE
using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
using StreamingContext = System.Runtime.Serialization.StreamingContext;
#endif

/** <summary>Ref to ID or expr but no tokens in ID stream or subtrees in expr stream</summary> */
[System.Serializable]
public class RewriteEmptyStreamException : RewriteCardinalityException
{
    public RewriteEmptyStreamException()
    {
    }

    public RewriteEmptyStreamException(string elementDescription)
        : base(elementDescription)
    {
    }
    
    public RewriteEmptyStreamException(string elementDescription, Exception innerException)
        : base(elementDescription, innerException)
    {
    }

    public RewriteEmptyStreamException(string message, string elementDescription)
        : base(message, elementDescription)
    {
    }

    public RewriteEmptyStreamException(string message, string elementDescription, Exception innerException)
        : base(message, elementDescription, innerException)
    {
    }

#if !PORTABLE
    protected RewriteEmptyStreamException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
#endif
}
