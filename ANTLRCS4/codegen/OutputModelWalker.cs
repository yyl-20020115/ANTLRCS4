/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using Antlr4.StringTemplate;
using org.antlr.v4.codegen.model;
using org.antlr.v4.codegen.model.chunk;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.tool;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace org.antlr.v4.codegen;

/** Convert an output model tree to template hierarchy by walking
 *  the output model. Each output model object has a corresponding template
 *  of the same name.  An output model object can have nested objects.
 *  We identify those nested objects by the list of arguments in the template
 *  definition. For example, here is the definition of the parser template:
 *
 *  Parser(parser, scopes, funcs) ::= &lt;&lt;...&gt;&gt;
 *
 *  The first template argument is always the output model object from which
 *  this walker will create the template. Any other arguments identify
 *  the field names within the output model object of nested model objects.
 *  So, in this case, template Parser is saying that output model object
 *  Parser has two fields the walker should chase called a scopes and funcs.
 *
 *  This simple mechanism means we don't have to include code in every
 *  output model object that says how to create the corresponding template.
 */
public class OutputModelWalker {
	Tool tool;
	TemplateGroup templates;

	public OutputModelWalker(Tool tool,
							 TemplateGroup templates)
	{
		this.tool = tool;
		this.templates = templates;
	}

	public Template walk(OutputModelObject omo, bool header) {
		// CREATE TEMPLATE FOR THIS OUTPUT OBJECT
		Type cl = omo.GetType();
		String templateName = cl.Name;
		if ( templateName == null ) {
			tool.errMgr.toolError(ErrorType.NO_MODEL_TO_TEMPLATE_MAPPING, cl.Name);
			return new Template("["+templateName+" invalid]");
		}

		if (header) templateName += "Header";

        Template st = templates.GetInstanceOf(templateName);
		if ( st == null ) {
			tool.errMgr.toolError(ErrorType.CODE_GEN_TEMPLATES_INCOMPLETE, templateName);
			return new Template("["+templateName+" invalid]");
		}
		if ( st.impl.FormalArguments == null ) {
			tool.errMgr.toolError(ErrorType.CODE_TEMPLATE_ARG_ISSUE, templateName, "<none>");
			return st;
		}

		var formalArgs = st.impl.FormalArguments;

		// PASS IN OUTPUT MODEL OBJECT TO TEMPLATE AS FIRST ARG
		var argNames = formalArgs.Select(f=>f.Name).ToHashSet();
		var arg_it = argNames.GetEnumerator();

		arg_it.MoveNext();
		
		var modelArgName = arg_it.Current; // ordered so this is first arg
		st.Add(modelArgName, omo);

		// COMPUTE STs FOR EACH NESTED MODEL OBJECT MARKED WITH @ModelElement AND MAKE ST ATTRIBUTE
		HashSet<String> usedFieldNames = new HashSet<String>();
		FieldInfo[] fields = cl.GetFields();
		foreach(var fi in fields) {
			var me = fi.GetCustomAttribute<ModelElementAttribute>();
			if (me == null) continue;

			String fieldName = fi.Name;
			if (!usedFieldNames.Add(fieldName)) {
				tool.errMgr.toolError(ErrorType.INTERNAL_ERROR, "Model object " + omo.GetType().Name + " has multiple fields named '" + fieldName + "'");
				continue;
			}

			// Just don't set @ModelElement fields w/o formal arg in target ST
			if ( !formalArgs.Any(f=>f.Name==fieldName) ) continue;

			try {
				Object o = fi.GetValue(omo);
				if ( o is OutputModelObject nestedOmo) {  // SINGLE MODEL OBJECT?
                    Template nestedST = walk(nestedOmo, header);
					//Console.Out.WriteLine("set ModelElement "+fieldName+"="+nestedST+" in "+templateName);
					st.Add(fieldName, nestedST);
				}
				else if ( o is ICollection || o is OutputModelObject[] ) {
					// LIST OF MODEL OBJECTS?
					if ( o is OutputModelObject[] ) {
						o = Arrays.AsList((OutputModelObject[])o);
					}
					ICollection nestedOmos = (ICollection)o;
                    foreach (Object nomo in nestedOmos) {
						if (nomo == null ) continue;
						Template nestedST = walk((OutputModelObject)nomo, header);
						//Console.Out.WriteLine("set ModelElement "+fieldName+"="+nestedST+" in "+templateName);
						st.Add(fieldName, nestedST);
					} 
				}
				else if ( o is IDictionary nestedOmoMap) {
					Dictionary<Object, Template> m = new Dictionary<Object, Template>();
					var e = nestedOmoMap.GetEnumerator();

                    while(e.MoveNext())
					{
                        Template nestedST = walk((OutputModelObject)e.Value, header);
						//Console.Out.WriteLine("set ModelElement "+fieldName+"="+nestedST+" in "+templateName);
						m[e.Key] = nestedST;
					}
					
					st.Add(fieldName, m);
				}
				else if ( o!=null ) {
					tool.errMgr.toolError(ErrorType.INTERNAL_ERROR, "not recognized nested model element: "+fieldName);
				}
			}
			catch (Exception iae) {
				tool.errMgr.toolError(ErrorType.CODE_TEMPLATE_ARG_ISSUE, templateName, fieldName);
			}
		}
		//st.impl.dump();
		return st;
	}

}
