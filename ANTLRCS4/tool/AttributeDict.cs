/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.tool.ast;

namespace org.antlr.v4.tool;



/** Track the attributes within retval, arg lists etc...
 *  <p>
 *  Each rule has potentially 3 scopes: return values,
 *  parameters, and an implicitly-named scope (i.e., a scope defined in a rule).
 *  Implicitly-defined scopes are named after the rule; rules and scopes then
 *  must live in the same name space--no collisions allowed.
 */
public class AttributeDict
{
    public string name;
    public GrammarAST ast;
    public DictType type;

    /** All {@link Token} scopes (token labels) share the same fixed scope of
     *  of predefined attributes.  I keep this out of the {@link Token}
     *  interface to avoid a runtime type leakage.
     */
    public static readonly AttributeDict predefinedTokenDict = new(DictType.TOKEN);
    static AttributeDict()
    {
        predefinedTokenDict.Add(new("text"));
        predefinedTokenDict.Add(new("type"));
        predefinedTokenDict.Add(new("line"));
        predefinedTokenDict.Add(new("index"));
        predefinedTokenDict.Add(new("pos"));
        predefinedTokenDict.Add(new("channel"));
        predefinedTokenDict.Add(new("int"));
    }

    public enum DictType : uint
    {
        ARG, RET, LOCAL, TOKEN,
        PREDEFINED_RULE, PREDEFINED_LEXER_RULE,
    }

    /** The list of {@link Attribute} objects. */

    public readonly Dictionary<string, Attribute> attributes =
        new();

    public AttributeDict() { }
    public AttributeDict(DictType type) => this.type = type;

    public Attribute Add(Attribute a)
    {
        a.dict = this;
        if (this.attributes.TryGetValue(a.name, out var ret))
        {
            return ret;
        }
        attributes[a.name] = a;
        return ret;
    }
    public Attribute Get(string name) => attributes.TryGetValue(name, out var r) ? r : null;

    public string Name => name;

    public int Count => attributes.Count;
    /** Return the set of keys that collide from
     *  {@code this} and {@code other}.
     */

    public HashSet<string> Intersection(AttributeDict other)
    {
        if (other == null || other.Count == 0 || Count == 0)
        {
            return new HashSet<string>();
        }

        HashSet<string> result = new(attributes.Keys);
        //result.retainAll(other.attributes.Keys);
        result.RemoveWhere(k => !other.attributes.ContainsKey(k));
        return result;
    }

    public override string ToString() => Name + ":" + attributes;
}
