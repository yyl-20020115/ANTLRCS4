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
public class AttributeDict {
    public String name;
    public GrammarAST ast;
	public DictType type;

    /** All {@link Token} scopes (token labels) share the same fixed scope of
     *  of predefined attributes.  I keep this out of the {@link Token}
     *  interface to avoid a runtime type leakage.
     */
    public static readonly AttributeDict predefinedTokenDict = new (DictType.TOKEN);
    static AttributeDict(){
        predefinedTokenDict.add(new Attribute("text"));
        predefinedTokenDict.add(new Attribute("type"));
        predefinedTokenDict.add(new Attribute("line"));
        predefinedTokenDict.add(new Attribute("index"));
        predefinedTokenDict.add(new Attribute("pos"));
        predefinedTokenDict.add(new Attribute("channel"));
        predefinedTokenDict.add(new Attribute("int"));
    }

    public enum DictType {
        ARG, RET, LOCAL, TOKEN,
		PREDEFINED_RULE, PREDEFINED_LEXER_RULE,
    }

    /** The list of {@link Attribute} objects. */

    public readonly Dictionary<String, Attribute> attributes =
        new ();

	public AttributeDict() {}
	public AttributeDict(DictType type) { this.type = type; }

	public Attribute add(Attribute a) { 
        a.dict = this; 
        if(this.attributes.TryGetValue(a.name,out var ret))
        {
            return ret;
        }
        attributes[a.name] = a;
        return ret;
    }
    public Attribute get(String name) { return attributes.TryGetValue(name,out var r)?r:null; }

    public String getName() {
        return name;
    }

    public int size() { return attributes.Count; }

    /** Return the set of keys that collide from
     *  {@code this} and {@code other}.
     */

    public HashSet<String> intersection(AttributeDict other) {
        if ( other==null || other.size()==0 || size()==0 ) {
            return new HashSet<string>();
        }

        HashSet<String> result = new HashSet<String>(attributes.Keys);
		//result.retainAll(other.attributes.Keys);
        result.RemoveWhere(k=>!other.attributes.ContainsKey(k));
		return result;
    }

    //@Override
    public override String ToString() {
        return getName()+":"+attributes;
    }
}
