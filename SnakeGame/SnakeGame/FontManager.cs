using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SnakeGame
{
    class FontManager
    {

        private System.Drawing.Text.PrivateFontCollection _default;
        private System.Drawing.Text.PrivateFontCollection _button;
        private System.Drawing.Text.PrivateFontCollection _title;
        private System.Drawing.Text.PrivateFontCollection _message;
        private System.Drawing.Text.PrivateFontCollection _label;

        public System.Drawing.FontFamily defaultFont
        {
            get { return _default.Families[0]; }
        }

        public System.Drawing.FontFamily titleFont
        {
            get { return _title.Families[0]; }
        }

        public System.Drawing.FontFamily buttonFont
        {
            get { return _button.Families[0]; }
        }

        public System.Drawing.FontFamily messageFont
        {
            get { return _message.Families[0]; }
        }

        public System.Drawing.FontFamily labelFont
        {
            get { return _label.Families[0]; }
        }

        public FontManager()
        {

        }


        private unsafe IntPtr _bytes2Ptr(byte[] data)
        {
            fixed(byte* ptr  = data)
            {
                return (IntPtr)ptr;
            }
        }

        private System.Drawing.Text.PrivateFontCollection load(byte[] data)
        {
            var fonts = new System.Drawing.Text.PrivateFontCollection();

            fonts.AddMemoryFont(_bytes2Ptr(data), data.Length);

            return fonts;
        }

        public void load()
        {
            _default = load(FontResources.Inconsolata_ttf);
            _button = load(FontResources.atari_full);
            _title = load(FontResources.tlpsmb);
            _message = load(FontResources.Inconsolata_ttf);
            _label = load(FontResources.Antique_526);

            
        }

    }
}
