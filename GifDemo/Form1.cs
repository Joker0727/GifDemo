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
            int delayTime = 3000;

            if (string.IsNullOrEmpty(ResourcesFolder) || string.IsNullOrEmpty(targetFolder))
            {
                MessageBox.Show("文件夹路径不能为空！", "J·Y·T");
                return;
            }

            string[] picPathArr = ReadPic(ResourcesFolder);
            CreateGif(picPathArr, delayTime, targetFolder + @"\jyt.gif");

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
