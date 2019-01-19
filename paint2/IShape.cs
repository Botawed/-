using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace paint2
{
    public interface IShape
    {
        GraphicsPath  GetPath();
        bool HitTest(Point p);
        void Draw(Graphics g);
        void Move(Point d);
    }
}
