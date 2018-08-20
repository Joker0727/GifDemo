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
        private void button1_Click(object sender, EventArgs e)
        {
            string ResourcesFolder = this.textBox1.Text;
            string targetFolder = this.textBox2.Text;
            string time = this.textBox3.Text;

            if (string.IsNullOrEmpty(ResourcesFolder) || string.IsNullOrEmpty(targetFolder) || string.IsNullOrEmpty(time))
            {
                MessageBox.Show("文件夹路径不能为空！", "J·Y·T");
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
                
               // this.pictureBox1.Image = picBox;
            }
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
                gif.AddFrame(Image.FromFile(picPathArr[i]));
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
                frame.Save(outputPath + Guid.NewGuid().ToString() + ".png", ImageFormat.Png);
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


    }
}
