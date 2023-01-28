/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.codegen.model;

/** Indicate field of OutputModelObject is an element to be walked by
 *  OutputModelWalker.
 */
//@Retention(RetentionPolicy.RUNTIME)
//public interface ModelElement {
//}
[System.AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
sealed class ModelElementAttribute : Attribute
{
    // See the attribute guidelines at 
    //  http://go.microsoft.com/fwlink/?LinkId=85236
    // This is a positional argument
    public ModelElementAttribute()
    {
    }
}
