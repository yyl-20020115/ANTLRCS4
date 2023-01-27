/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.gui;

/**
 *
 * @author Sam Harwell
 */
public class SystemFontMetrics : BasicFontMetrics {
	protected readonly Font font;

	public SystemFontMetrics(String fontName) {
		BufferedImage img = new BufferedImage(40, 40, BufferedImage.TYPE_4BYTE_ABGR);
		Graphics2D graphics = GraphicsEnvironment.getLocalGraphicsEnvironment().createGraphics(img);
		FontRenderContext fontRenderContext = graphics.getFontRenderContext();
		this.font = new Font(fontName, Font.PLAIN, 1000);
		double maxHeight = 0;
		for (int i = 0; i < 255; i++) {
			TextLayout layout = new TextLayout(Character.toString((char)i), font, fontRenderContext);
			maxHeight = Math.max(maxHeight, layout.getBounds().getHeight());
			super.widths[i] = (int)layout.getAdvance();
		}

		super.maxCharHeight = (int)Math.round(maxHeight);
	}

	public Font getFont() {
		return font;
	}
}
