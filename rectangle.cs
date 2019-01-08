using System;

public class Class1
{
	public Class1()
	{
	    Graphics g = Graphics.FromImage(picDrawingSurface.Image);
	    g.Clear(Color.White);
	    //g.DrawRectangle(currentBrush, x1, y1, x2 - x1, y2 - y1);
	    g.DrawRectangle(currentPen, x1, y1, x2 - x1, y2 - y1);
	    g.FillRectangle(currentBrush, x1, y1, x2 - x1, y2 - y1);
	    g.Dispose();
	    picDrawingSurface.Invalidate();
    }
}
