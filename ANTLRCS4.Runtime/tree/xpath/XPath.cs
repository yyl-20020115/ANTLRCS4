/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.runtime.tree.xpath;

/**
 * Represent a subset of XPath XML path syntax for use in identifying nodes in
 * parse trees.
 *
 * <p>
 * Split path into words and separators {@code /} and {@code //} via ANTLR
 * itself then walk path elements from left to right. At each separator-word
 * pair, find set of nodes. Next stage uses those as work list.</p>
 *
 * <p>
 * The basic interface is
 * {@link XPath#findAll ParseTree.findAll}{@code (tree, pathString, parser)}.
 * But that is just shorthand for:</p>
 *
 * <pre>
 * {@link XPath} p = new {@link XPath#XPath XPath}(parser, pathString);
 * return p.{@link #evaluate evaluate}(tree);
 * </pre>
 *
 * <p>
 * See {@code org.antlr.v4.test.TestXPath} for descriptions. In short, this
 * allows operators:</p>
 *
 * <dl>
 * <dt>/</dt> <dd>root</dd>
 * <dt>//</dt> <dd>anywhere</dd>
 * <dt>!</dt> <dd>invert; this must appear directly after root or anywhere
 * operator</dd>
 * </dl>
 *
 * <p>
 * and path elements:</p>
 *
 * <dl>
 * <dt>ID</dt> <dd>token name</dd>
 * <dt>'string'</dt> <dd>any string literal token from the grammar</dd>
 * <dt>expr</dt> <dd>rule name</dd>
 * <dt>*</dt> <dd>wildcard matching any node</dd>
 * </dl>
 *
 * <p>
 * Whitespace is not allowed.</p>
 */
public class XPath {
	public static readonly String WILDCARD = "*"; // word not operator/separator
	public static readonly String NOT = "!"; 	   // word for invert operator

	protected String path;
	protected XPathElement[] elements;
	protected Parser parser;

	public XPath(Parser parser, String path) {
		this.parser = parser;
		this.path = path;
		elements = split(path);
//		System.out.println(Arrays.toString(elements));
	}

	// TODO: check for invalid token/rule names, bad syntax

	public class XPathLexerWithoutRecovery : XPathLexer
	{
		public XPathLexerWithoutRecovery(ANTLRInputStream stream)
			:base(stream)
		{

		}
        public void recover(LexerNoViableAltException e) { throw e; }

    }

    public XPathElement[] split(String path) {
		ANTLRInputStream @in;
		try {
			@in = new ANTLRInputStream(new StringReader(path));
		}
		catch (IOException ioe) {
			throw new ArgumentException("Could not read path: "+path, ioe);
		}
		XPathLexer lexer = new XPathLexerWithoutRecovery(@in);

		lexer.removeErrorListeners();
		lexer.addErrorListener(new XPathLexerErrorListener());
		CommonTokenStream tokenStream = new CommonTokenStream(lexer);
		try {
			tokenStream.fill();
		}
		catch (LexerNoViableAltException e) {
			int pos = lexer.getCharPositionInLine();
			String msg = "Invalid tokens or characters at index "+pos+" in path '"+path+"'";
			throw new ArgumentException(msg, e);
		}

		List<Token> tokens = tokenStream.getTokens();
//		System.out.println("path="+path+"=>"+tokens);
		List<XPathElement> elements = new ();
		int n = tokens.Count;
		int i=0;
loop:
		while ( i<n ) {
			Token el = tokens[(i)];
			Token next = null;
			switch ( el.getType() ) {
				case XPathLexer.ROOT :
				case XPathLexer.ANYWHERE :
					bool anywhere = el.getType() == XPathLexer.ANYWHERE;
					i++;
					next = tokens[(i)];
					bool invert = next.getType()==XPathLexer.BANG;
					if ( invert ) {
						i++;
						next = tokens[(i)];
					}
					XPathElement pathElement = getXPathElement(next, anywhere);
					pathElement.invert = invert;
					elements.Add(pathElement);
					i++;
					break;

				case XPathLexer.TOKEN_REF :
				case XPathLexer.RULE_REF :
				case XPathLexer.WILDCARD :
					elements.Add( getXPathElement(el, false) );
					i++;
					break;

				case Token.EOF :
					goto exit_loop;

				default :
					throw new ArgumentException("Unknowth path element "+el);
			}
		}
exit_loop:
		return elements.ToArray();
	}

	/**
	 * Convert word like {@code *} or {@code ID} or {@code expr} to a path
	 * element. {@code anywhere} is {@code true} if {@code //} precedes the
	 * word.
	 */
	protected XPathElement getXPathElement(Token wordToken, bool anywhere) {
		if ( wordToken.getType()==Token.EOF ) {
			throw new ArgumentException("Missing path element at end of path");
		}
		String word = wordToken.getText();
		int ttype = parser.getTokenType(word);
		int ruleIndex = parser.getRuleIndex(word);
		switch ( wordToken.getType() ) {
			case XPathLexer.WILDCARD :
				return anywhere ?
					new XPathWildcardAnywhereElement() :
					new XPathWildcardElement();
			case XPathLexer.TOKEN_REF :
			case XPathLexer.STRING :
				if ( ttype==Token.INVALID_TYPE ) {
					throw new ArgumentException(word+
													   " at index "+
													   wordToken.getStartIndex()+
													   " isn't a valid token name");
				}
				return anywhere ?
					new XPathTokenAnywhereElement(word, ttype) :
					new XPathTokenElement(word, ttype);
			default :
				if ( ruleIndex==-1 ) {
					throw new ArgumentException(word+
													   " at index "+
													   wordToken.getStartIndex()+
													   " isn't a valid rule name");
				}
				return anywhere ?
					new XPathRuleAnywhereElement(word, ruleIndex) :
					new XPathRuleElement(word, ruleIndex);
		}
	}


	public static ICollection<ParseTree> findAll(ParseTree tree, String xpath, Parser parser) {
		XPath p = new XPath(parser, xpath);
		return p.evaluate(tree);
	}

	/**
	 * Return a list of all nodes starting at {@code t} as root that satisfy the
	 * path. The root {@code /} is relative to the node passed to
	 * {@link #evaluate}.
	 */
	public ICollection<ParseTree> evaluate(ParseTree t) {
        var dummyRoot = new ParserRuleContext
        {
            children = new List<ParseTree>()// Collections.singletonList(t); // don't set t's parent.
        };

        ICollection<ParseTree> work = new List<ParseTree> { dummyRoot };// Collections.<ParseTree>singleton(dummyRoot);

		int i = 0;
		while ( i < elements.Length ) {
            ICollection<ParseTree> next = new HashSet<ParseTree>();
			foreach (ParseTree node in work) {
				if ( node.getChildCount()>0 ) {
					// only try to match next element if it has children
					// e.g., //func/*/stat might have a token node for which
					// we can't go looking for stat nodes.
					ICollection<ParseTree> matching = elements[i].evaluate(node);
					foreach(var m in matching) next.Add(m); 
				}
			}
			i++;
			work = next;
		}

		return work;
	}
}
