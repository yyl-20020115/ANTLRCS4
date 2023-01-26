/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using System.Runtime.Serialization;

namespace org.antlr.v4
{
    [Serializable]
    internal class Error : Exception
    {
        public Error()
        {
        }

        public Error(string? message) : base(message)
        {
        }

        public Error(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected Error(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}