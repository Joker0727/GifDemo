using Gif.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GifDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.MaximizeBox = false;
        }
        private void textBox1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog targetFolder = new FolderBrowserDialog();

            if (targetFolder.ShowDialog() == DialogResult.OK)
            {
                this.textBox1.Text = targetFolder.SelectedPath;
            }
        }

        private void textBox2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog targetFolder = new FolderBrowserDialog();

            if (targetFolder.ShowDialog() == DialogResult.OK)
            {
                this.textBox2.Text = targetFolder.SelectedPath;
            }
        }
        private void textBox4_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;//该值确定是否可以选择多个文件
            dialog.Title = "请选择文件夹";
            dialog.Filter = "动态图(*.gif)|*.gif";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
              this.textBox4.Text = dialog.FileName;
            }
            // Filter 属性 赋值为一字符串 用于过滤文件类型;
            //字符串说明如下：
            //‘|’分割的两个，一个是注释，一个是真的Filter，显示出来的是那个注释。如果要一次显示多中类型的文件，用分号分开。
            //如：
            //Open1.Filter = "图片文件(*.jpg,*.gif,*.bmp)|*.jpg;*.gif;*.bmp";
            //           则过滤的文件类型为 “|”号 右边的 *.jpg; *.gif; *.bmp 三种类型文件，在OpenDialog / SaveDialog中显示给用户看的文件类型字符串则是 ：“|”号 左边的 图片文件(*.jpg, *.gif, *.bmp)。
            //再如：
            //Open1.Filter = "图像文件(*.jpg;*.jpg;*.jpeg;*.gif;*.png)|*.jpg;*.jpeg;*.gif;*.png";
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string ResourcesFolder = this.textBox1.Text;
            string targetFolder = this.textBox2.Text;
            string time = this.textBox3.Text;

            if (string.IsNullOrEmpty(ResourcesFolder) || string.IsNullOrEmpty(targetFolder) || string.IsNullOrEmpty(time))
            {
                MessageBox.Show("文件夹路径不能为空！", "OldWang");
                return;
            }

            int delayTime = int.Parse((double.Parse(time) * 1000).ToString());
            string[] picPathArr = ReadPic(ResourcesFolder);
            string targetPath = targetFolder + @"\jyt.gif";
            if (File.Exists(targetPath))
                File.Delete(targetPath);
            CreateGif(picPathArr, delayTime, targetPath);
            if (File.Exists(targetPath))
            {
                Bitmap picBox = new Bitmap(targetPath);
                Form2 f2 = new Form2();
                f2.Show();
                f2.pictureBox1.Image = picBox;
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            string targetFolder = this.textBox2.Text;
            string gifPath = this.textBox4.Text;

            if (string.IsNullOrEmpty(gifPath) || string.IsNullOrEmpty(targetFolder))
            {
                MessageBox.Show("文件夹路径不能为空！", "OldWang");
                return;
            }

            ExtractGif(gifPath, targetFolder);
        }
        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 0x20) e.KeyChar = (char)0;  //禁止空格键
            if ((e.KeyChar == 0x2D) && (((TextBox)sender).Text.Length == 0)) return;   //处理负数
            if (e.KeyChar > 0x20)
            {
                try
                {
                    double.Parse(((TextBox)sender).Text + e.KeyChar.ToString());
                }
                catch
                {
                    e.KeyChar = (char)0;   //处理非法字符
                }
            }
        }
        /// <summary>
        /// 生成Gif
        /// </summary>
        /// <param name="picPathArr"></param>
        /// <param name="delayTime"></param>
        /// <param name="outputFilePath"></param>
        public static void CreateGif(string[] picPathArr, int delayTime, string outputFilePath)
        {
            AnimatedGifEncoder gif = new AnimatedGifEncoder();
            gif.Start(outputFilePath);
            gif.SetDelay(delayTime);//设置延迟         
            gif.SetRepeat(0); //-1:不循环,0:始终循环
            //gif.SetSize(300,200);

            for (int i = 0, count = picPathArr.Length; i < count; i++)
            {
                gif.AddFrame(DealWithPic(picPathArr[i]));
            }
            gif.Finish();
        }
        /// <summary>
        /// 提取Gif中的没一张图片
        /// </summary>
        public static void ExtractGif(string gifFullPath, string outputPath)
        {
            GifDecoder gifDecoder = new GifDecoder();
            gifDecoder.Read(gifFullPath);
            for (int i = 0, count = gifDecoder.GetFrameCount(); i < count; i++)
            {
                Image frame = gifDecoder.GetFrame(i); // frame i
                frame.Save(outputPath +@"\"+ Guid.NewGuid().ToString() + ".png", ImageFormat.Png);
            }
        }
        /// <summary>
        /// 读取图片路径
        /// </summary>
        /// <param name="picFolder"></param>
        /// <returns></returns>
        public static string[] ReadPic(string picFolder)
        {
            if (!Directory.Exists(picFolder))
                Directory.CreateDirectory(picFolder);

            string[] tempArr = Directory.GetFiles(picFolder);
            List<string> resultList = new List<string>();
            foreach (var item in tempArr)
            {
                string ExteName = Path.GetExtension(item);
                if (ExteName.ToLower() == ".jpg" || ExteName.ToLower() == ".png" || ExteName.ToLower() == ".bmp")
                    resultList.Add(item);
            }
            return resultList.ToArray();
        }
        /// <summary>
        /// 处理图片
        /// </summary>
        /// <param name="picPath"></param>
        /// <returns></returns>
        public static Bitmap DealWithPic(string picPath)
        {
            Bitmap bitmap = new Bitmap(picPath);
            return bitmap;
        }
        /// <summary>
        /// 缩放类型枚举
        /// </summary>
        public enum ZoomType { NearestNeighborInterpolation, BilinearInterpolation }
        /// <summary>
        /// 图像缩放
        /// </summary>
        /// <param name="srcBmp">原始图像</param>
        /// <param name="width">目标图像宽度</param>
        /// <param name="height">目标图像高度</param>
        /// <param name="dstBmp">目标图像</param>
        /// <param name="GetNearOrBil">缩放选用的算法</param>
        /// <returns>处理成功 true 失败 false</returns>
        public static bool Zoom(Bitmap srcBmp, double ratioW, double ratioH, out Bitmap dstBmp, ZoomType zoomType)
        {
            //ZoomType为自定义的枚举类型
            if (srcBmp == null)
            {
                dstBmp = null;
                return false;
            }
            //若缩放大小与原图一样，则返回原图不做处理
            if ((ratioW == 1.0) && ratioH == 1.0)
            {
                dstBmp = new Bitmap(srcBmp);
                return true;
            }
            //计算缩放高宽
            double height = ratioH * (double)srcBmp.Height;
            double width = ratioW * (double)srcBmp.Width;
            dstBmp = new Bitmap((int)width, (int)height);

            BitmapData srcBmpData = srcBmp.LockBits(new Rectangle(0, 0, srcBmp.Width, srcBmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData dstBmpData = dstBmp.LockBits(new Rectangle(0, 0, dstBmp.Width, dstBmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            unsafe
            {
                byte* srcPtr = null;
                byte* dstPtr = null;
                int srcI = 0;
                int srcJ = 0;
                double srcdI = 0;
                double srcdJ = 0;
                double a = 0;
                double b = 0;
                double F1 = 0;//横向插值所得数值
                double F2 = 0;//纵向插值所得数值
                if (zoomType == ZoomType.NearestNeighborInterpolation)
                {//邻近插值法

                    for (int i = 0; i < dstBmp.Height; i++)
                    {
                        srcI = (int)(i / ratioH);//srcI是此时的i对应的原图像的高
                        srcPtr = (byte*)srcBmpData.Scan0 + srcI * srcBmpData.Stride;
                        dstPtr = (byte*)dstBmpData.Scan0 + i * dstBmpData.Stride;
                        for (int j = 0; j < dstBmp.Width; j++)
                        {
                            dstPtr[j * 3] = srcPtr[(int)(j / ratioW) * 3];//j / ratioW求出此时j对应的原图像的宽
                            dstPtr[j * 3 + 1] = srcPtr[(int)(j / ratioW) * 3 + 1];
                            dstPtr[j * 3 + 2] = srcPtr[(int)(j / ratioW) * 3 + 2];
                        }
                    }
                }
                else if (zoomType == ZoomType.BilinearInterpolation)
                {//双线性插值法
                    byte* srcPtrNext = null;
                    for (int i = 0; i < dstBmp.Height; i++)
                    {
                        srcdI = i / ratioH;
                        srcI = (int)srcdI;//当前行对应原始图像的行数
                        srcPtr = (byte*)srcBmpData.Scan0 + srcI * srcBmpData.Stride;//指原始图像的当前行
                        srcPtrNext = (byte*)srcBmpData.Scan0 + (srcI + 1) * srcBmpData.Stride;//指向原始图像的下一行
                        dstPtr = (byte*)dstBmpData.Scan0 + i * dstBmpData.Stride;//指向当前图像的当前行
                        for (int j = 0; j < dstBmp.Width; j++)
                        {
                            srcdJ = j / ratioW;
                            srcJ = (int)srcdJ;//指向原始图像的列
                            if (srcdJ < 1 || srcdJ > srcBmp.Width - 1 || srcdI < 1 || srcdI > srcBmp.Height - 1)
                            {//避免溢出（也可使用循环延拓）
                                dstPtr[j * 3] = 255;
                                dstPtr[j * 3 + 1] = 255;
                                dstPtr[j * 3 + 2] = 255;
                                continue;
                            }
                            a = srcdI - srcI;//计算插入的像素与原始像素距离（决定相邻像素的灰度所占的比例）
                            b = srcdJ - srcJ;
                            for (int k = 0; k < 3; k++)
                            {//插值    公式：f(i+p,j+q)=(1-p)(1-q)f(i,j)+(1-p)qf(i,j+1)+p(1-q)f(i+1,j)+pqf(i+1, j + 1)
                                F1 = (1 - b) * srcPtr[srcJ * 3 + k] + b * srcPtr[(srcJ + 1) * 3 + k];
                                F2 = (1 - b) * srcPtrNext[srcJ * 3 + k] + b * srcPtrNext[(srcJ + 1) * 3 + k];
                                dstPtr[j * 3 + k] = (byte)((1 - a) * F1 + a * F2);
                            }
                        }
                    }
                }
            }
            srcBmp.UnlockBits(srcBmpData);
            dstBmp.UnlockBits(dstBmpData);
            return true;
        }
    }
}
