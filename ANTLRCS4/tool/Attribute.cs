/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime;

namespace org.antlr.v4.tool;


/** Track the names of attributes defined in arg lists, return values,
 *  scope blocks etc...
 */
public class Attribute
{
    /** The entire declaration such as "string foo" or "x:int" */
    public string decl;

    /** The type; might be empty such as for Python which has no static typing */
    public string type;

    /** The name of the attribute "foo" */
    public string name;

    /** A {@link Token} giving the position of the name of this attribute in the grammar. */
    public Token token;

    /** The optional attribute initialization expression */
    public string initValue;

    /** Who contains us? */
    public AttributeDict dict;

    public Attribute() { }

    public Attribute(string name) : this(name, null) {; }

    public Attribute(string name, string decl)
    {
        this.name = name;
        this.decl = decl;
    }

    public override string ToString()
        => initValue != null ? name + ":" + type + "=" + initValue : type != null ? name + ":" + type : name;
}
