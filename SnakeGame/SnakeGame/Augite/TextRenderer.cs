using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Augite
{
    using System.Drawing;

    class TextRenderer
    {
        public int fontSize = 24;
        public Color textColor = Color.White;
        public FontFamily font = null;       

        public Color borderColor = Color.Black;
        public float borderWidth = 0.0f;

        private StringFormat _strFmt;

        public StringFormat stringFormat { get { return _strFmt; } }

        public TextRenderer()
        {
            _strFmt = new System.Drawing.StringFormat();
            _strFmt.Alignment = StringAlignment.Center;
            _strFmt.LineAlignment = StringAlignment.Center;
        }
        
        public virtual System.Drawing.Brush createBrush()
        {
           

            return new System.Drawing.SolidBrush(textColor);
        }       


        public void render(System.Drawing.Graphics g, string text, System.Drawing.Rectangle rect)
        {
            //string text = string.Format("SCORE: {0}", _scoreValue);
            var gp = new System.Drawing.Drawing2D.GraphicsPath();


            System.Drawing.FontFamily family = font;

            if (family == null)
            {
                family = System.Drawing.SystemFonts.DefaultFont.FontFamily;//
            }
            
          
            int fontStyle = (int)System.Drawing.FontStyle.Bold;
            gp.AddString(text, family, fontStyle, fontSize, rect, _strFmt);


            if(borderWidth > 0)
            {
                g.DrawPath(new Pen(borderColor, borderWidth), gp);
            }


            var brush = createBrush();

            g.FillPath(brush, gp);
        }

    }
}
