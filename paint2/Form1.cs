using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.Remoting.Proxies;
using System.Windows.Forms;

namespace paint2
{

    public partial class Form1 : Form
    {
        List<Point> points;
        string selectedTool;
        int x1;
        int y1;
        int x2;
        int y2;
        Color ColorPen;
        bool drawing;
        int historyCounter; //Счетчик истории
        GraphicsPath currentPath;
        Point oldLocation;
        public Pen currentPen;
        public SolidBrush currentBrush;
        List<Image> History; //Список для истории
        public Form1()
        {
            InitializeComponent();
            drawing = false; //Переменная, ответственная за рисование
             ColorPen = Color.Red;

            points = new List<Point>();

            picDrawingSurface.Width = trackBar2.Value; //изменение ширины
            picDrawingSurface.Height = trackBar3.Value; // изменение высоты 

            currentBrush = new SolidBrush(BackColor);

            currentPen = new Pen(ColorPen); //Инициализация пера с черным цветом
            currentPen.Width = trackBar1.Value; //Инициализация толщины пера
            History = new List<Image>(); //Инициализация списка для истории
        }
       
    private void picDrawingSurface_MouseDown(object sender, MouseEventArgs e)
        {
            if (picDrawingSurface.Image == null)
            {
                MessageBox.Show("Сначала создайте новый файл!");
                return;
            }

            if (e.Button == MouseButtons.Left)
            {
                points.Add(new Point(e.X, e.Y));
                x1 = e.X;
                y1 = e.Y;
                drawing = true;
                oldLocation = e.Location;
                currentPath = new GraphicsPath();
            }

            

        }

        private void toolStripButton2_Click(object sender, EventArgs e)// сохранения с боку
        {
            saveToolStripMenuItem_Click( sender,  e);
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)// new создание
        {
            if (picDrawingSurface.Image != null)
            {
                var result = MessageBox.Show("Сохранить текущее изображение перед созданием нового рисунка ? ", "Предупреждение", MessageBoxButtons.YesNoCancel);
                switch (result)
                {
                    case DialogResult.No: break;
                    case DialogResult.Yes: saveToolStripMenuItem_Click(sender, e); break;
                    case DialogResult.Cancel: return;
                }
            }
            points.Clear();
            History.Clear();
            historyCounter = 0;
            Bitmap pic = new Bitmap(trackBar2.Value, trackBar3.Value);
            picDrawingSurface.Image = pic;
            History.Add(new Bitmap(picDrawingSurface.Image));
           
        }

        private void toolStripButton1_Click(object sender, EventArgs e)//сохдание с боку
        {
            newToolStripMenuItem_Click( sender,  e);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)//сохранение
        {
            SaveFileDialog SaveDlg = new SaveFileDialog();
            SaveDlg.Filter = "JPEG Image|*.jpg|Bitmap Image|*.bmp|GIF Image|*.gif|PNG Image | *.png";
            SaveDlg.Title = "Save an Image File";
            SaveDlg.FilterIndex = 4; //По умолчанию будет выбрано последнее расширение *.png
            SaveDlg.ShowDialog();
            if (SaveDlg.FileName != "") //Если введено не пустое имя
            {
                System.IO.FileStream fs =
                    (System.IO.FileStream) SaveDlg.OpenFile();
                switch (SaveDlg.FilterIndex)
                {
                    case 1:
                        picDrawingSurface.Image.Save(fs, ImageFormat.Jpeg);
                        break;
                    case 2:
                        picDrawingSurface.Image.Save(fs, ImageFormat.Bmp);
                        break;
                    case 3:
                        picDrawingSurface.Image.Save(fs, ImageFormat.Gif);
                        break;
                    case 4:
                        picDrawingSurface.Image.Save(fs, ImageFormat.Png);
                        break;
                }

                fs.Close();
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)//открытие
        {
            OpenFileDialog OP = new OpenFileDialog();
            OP.Filter = "JPEG Image|*.jpg|Bitmap Image|*.bmp|GIF Image|*.gif|PNG Image | *.png";
            OP.Title = "Open an Image File";
            OP.FilterIndex = 4; //По умолчанию будет выбрано первое расширение *.jpg
     //И, когда пользователь укажет нужный путь к картинке, ее нужно будет загрузить в PictureBox:
            if (OP.ShowDialog() != DialogResult.Cancel)
                picDrawingSurface.Load(OP.FileName);
            picDrawingSurface.AutoSize = true;

        }

        private void picDrawingSurface_MouseUp(object sender, MouseEventArgs e)
        {

            //Очистка ненужной истории
            History.RemoveRange(historyCounter + 1, History.Count - historyCounter - 1);
            History.Add(new Bitmap(picDrawingSurface.Image));
            if (historyCounter + 1 < 6) historyCounter++;
            if (History.Count - 1 == 6) History.RemoveAt(0);
            drawing = false;
            try
            {

                currentPath.Dispose();
            }
            catch { };

        }

        public void picDrawingSurface_MouseMove(object sender, MouseEventArgs e)
        {
            
            label1.Text = e.X.ToString() + "X,Y" + e.Y.ToString();
            x2 = e.X;
            y2 = e.Y;
            switch (selectedTool)
            {
                case "Circle":
                    if (drawing )
                    {
                        Graphics g = Graphics.FromImage(picDrawingSurface.Image);
                        g.Clear(Color.White);
                        g.DrawEllipse(currentPen, x1, y1, x2 - x1, y2 - y1);
                        g.FillEllipse(currentBrush, x1, y1, x2 - x1, y2 - y1);
                        g.Dispose();
                        picDrawingSurface.Invalidate();
                        
                    }
                    break;

                case "Rectanglee":
                    if (drawing )
                    {
                        
                        Graphics g = Graphics.FromImage(picDrawingSurface.Image);
                        g.Clear(Color.White);
                        //g.DrawRectangle(currentBrush, x1, y1, x2 - x1, y2 - y1);
                        g.DrawRectangle(currentPen, x1, y1, x2 - x1, y2 - y1);
                        g.FillRectangle(currentBrush, x1, y1, x2 - x1, y2 - y1);
                        g.Dispose();
                         picDrawingSurface.Invalidate();
                       

                    }
                    break;

                case "Pencil":
                    if (drawing)
                    {
                        Graphics g = Graphics.FromImage(picDrawingSurface.Image);
                        currentPath.AddLine(oldLocation, e.Location);//GraphicsPath currentPath;
                        g.DrawPath(currentPen, currentPath);
                        oldLocation = e.Location;
                        g.Dispose();
                        picDrawingSurface.Invalidate();
                    }
                    break;

                case "Line":
                    if (drawing)
                    {
                        Graphics g = Graphics.FromImage(picDrawingSurface.Image);
                        g.Clear(Color.White);
                        g.DrawLine(currentPen, x1, y1, x2, y2);
                        g.Dispose();
                        picDrawingSurface.Invalidate();
                    }
                    break;

                case "Polygon":
                    if (drawing)
                    {
                        Graphics g = Graphics.FromImage(picDrawingSurface.Image);
                        g.Clear(Color.White);
                        
                        Point[] pnts = new Point[points.Count];
                        for (int i = 0; i < points.Count; i++)
                        {
                            pnts[i] = points[i];
                        }
                        g.DrawPolygon(currentPen, pnts);
                        g.FillPolygon(currentBrush, pnts);
                        g.Dispose();
                        picDrawingSurface.Invalidate();
                    }
                    break;
                default:
                    break;
            }


            //if (drawing)
            //{
            //    x2 = e.X;
            //    y2 = e.Y;


            //    Graphics g = Graphics.FromImage(picDrawingSurface.Image);
            //    g.Clear(Color.White);
            //    //g.DrawRectangle(currentBrush, x1, y1, x2 - x1, y2 - y1);
            //    g.DrawRectangle(currentPen, x1, y1, x2 - x1, y2 - y1);
            //    g.FillRectangle(currentBrush, x1, y1, x2 - x1, y2 - y1);
            //    g.Dispose();
            //    picDrawingSurface.Invalidate();
                
            //    //Graphics g = Graphics.FromImage(picDrawingSurface.Image);

            //    //currentPath.AddLine(oldLocation, e.Location);//GraphicsPath currentPath;
            //    //g.DrawPath(currentPen, currentPath);
            //    //oldLocation = e.Location;

            //    //g.Dispose();
            //    //picDrawingSurface.Invalidate();

            //}

        }

        private void trackBar1_Scroll(object sender, EventArgs e)//ВЕЛЕЧИНА РИСУНКА
        {
            currentPen.Width = trackBar1.Value;
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)//ДЕЙСТВИЕ НАЗАД
        {
            if (History.Count != 0 && historyCounter != 0)
            {
                picDrawingSurface.Image = new Bitmap(History[--historyCounter]);
            }
            else MessageBox.Show("История пуста");

        }

        private void renoToolStripMenuItem_Click(object sender, EventArgs e)//ДЕЙСТВИЕ ВПЕРЕД
        {
            if (historyCounter < History.Count - 1)
            {
                picDrawingSurface.Image = new Bitmap(History[++historyCounter]);
            }
            else MessageBox.Show("История пуста");
        }

        private void solidToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentPen.DashStyle = DashStyle.Solid; // задания стиля пира
            solidToolStripMenuItem.Checked = true;
            dotToolStripMenuItem.Checked = false;
            dashDotDotToolStripMenuItem.Checked = false;

        }//стиль пера

        private void dotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentPen.DashStyle = DashStyle.Dot; // задания стиля пира
            solidToolStripMenuItem.Checked = false;
            dotToolStripMenuItem.Checked = true;
            dashDotDotToolStripMenuItem.Checked = false;
        }//стиль пера

        private void dashDotDotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentPen.DashStyle = DashStyle.DashDot; // задания стиля пира
            solidToolStripMenuItem.Checked = false;
            dotToolStripMenuItem.Checked = false;

            dashDotDotToolStripMenuItem.Checked = true;
        }//стиль пера

        private void colorToolStripMenuItem_Click(object sender, EventArgs e)// цвет
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {

                currentPen.Color = colorDialog1.Color;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)//сохранение перед выходом
        {
            if (picDrawingSurface.Image != null)
            {
                var result = MessageBox.Show("Сохранить текущее изображение перед созданием нового рисунка ? ", "Предупреждение", MessageBoxButtons.YesNoCancel);
                switch (result)
                {
                    case DialogResult.No: break;
                    case DialogResult.Yes: saveToolStripMenuItem_Click(sender, e); break;
                    case DialogResult.Cancel: return;
                }
            }
            Application.Exit();
        }

        private void toolStripButton3_Click(object sender, EventArgs e) //открытие с боку
        {
            openToolStripMenuItem_Click(sender, e);
        }

        private void toolStripButton5_Click(object sender, EventArgs e)//выход с боку
        {
            exitToolStripMenuItem_Click(sender, e);
        }

        private void toolStripButton4_Click(object sender, EventArgs e)//выбор цвета
        {
            colorToolStripMenuItem_Click( sender,  e);
        }

        private void trackBar2_Scroll(object sender, EventArgs e)//задание размера пикчерсбокс
        {
            picDrawingSurface.Width = trackBar2.Value;
        }

        private void trackBar3_Scroll(object sender, EventArgs e)//задание размера пикчерсбокс
        {
            picDrawingSurface.Height = trackBar3.Value;
        }

        private void toolStripButton7_Click(object sender, EventArgs e)//цвет заливки
        {
            if (colorDialog2.ShowDialog() == DialogResult.OK)
            {
                
                currentBrush.Color = colorDialog2.Color;
            }
        }


        private void Circle_Click(object sender, EventArgs e)//выбор фигуры
        {
            foreach (ToolStripButton btn in toolStrip1.Items)
            {
                btn.Checked = false;
            }

            ToolStripButton btnClicked = sender as ToolStripButton;
            btnClicked.Checked = true;
            selectedTool = btnClicked.Name;
        }
    }
}
//e.Graphics.DrawLine(currentPen, x1, y1, x2, y2);//линия
//e.Graphics.DrawEllipse(currentPen, x1, y1, x2 - x1, y2 - y1);//элипс
//e.Graphics.DrawRectangle(currentPen, x1, y1, x2 - x1, y2 - y1);//прямоугольник
//Graphics g = Graphics.FromImage(picDrawingSurface.Image);
//g.DrawRectangle(currentPen, 20, 20, 40, 40);
//g.DrawEllipse(currentPen, 80, 80, 60, 60);
//currentPath.AddLine(oldLocation, e.Location);//GraphicsPath currentPath;
//g.DrawPath(currentPen, currentPath);
//oldLocation = e.Location;

//g.Dispose();
//picDrawingSurface.Invalidate();