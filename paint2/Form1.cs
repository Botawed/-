using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace paint2
{

    public partial class Form1 : Form
    {
        
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
       
        List<Image> History; //Список для истории
        public Form1()
        {
            InitializeComponent();
            drawing = false; //Переменная, ответственная за рисование
             ColorPen = Color.Red;

            picDrawingSurface.Width = trackBar2.Value;
            picDrawingSurface.Height = trackBar3.Value;

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

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void openToolStripMenuItem_Click(object sender, EventArgs e)//
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

        private void picDrawingSurface_MouseMove(object sender, MouseEventArgs e)
        {
            
            label1.Text = e.X.ToString() + "X,Y" + e.Y.ToString();
            if (drawing)
            {
                x2 = e.X;
                y2 = e.Y;


                Graphics g = Graphics.FromImage(picDrawingSurface.Image);
                g.Clear(Color.White);
                g.DrawRectangle(currentPen, x1, y1, x2 - x1, y2 - y1);
                g.Dispose();
                picDrawingSurface.Invalidate();
                
                //Graphics g = Graphics.FromImage(picDrawingSurface.Image);

                //currentPath.AddLine(oldLocation, e.Location);//GraphicsPath currentPath;
                //g.DrawPath(currentPen, currentPath);
                //oldLocation = e.Location;

                //g.Dispose();
                //picDrawingSurface.Invalidate();

            }

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

        }

        private void dotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentPen.DashStyle = DashStyle.Dot; // задания стиля пира
            solidToolStripMenuItem.Checked = false;
            dotToolStripMenuItem.Checked = true;
            dashDotDotToolStripMenuItem.Checked = false;
        }

        private void dashDotDotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentPen.DashStyle = DashStyle.DashDot; // задания стиля пира
            solidToolStripMenuItem.Checked = false;
            dotToolStripMenuItem.Checked = false;

            dashDotDotToolStripMenuItem.Checked = true;
        }

        private void colorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {

                currentPen.Color = colorDialog1.Color;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            picDrawingSurface.Width = trackBar2.Value;
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            picDrawingSurface.Height = trackBar3.Value;
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