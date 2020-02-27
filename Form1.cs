using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using IWshRuntimeLibrary;

//https://blog.csdn.net/FairyStepWGL/article/details/52904339


namespace Bandizip一键分包安装工具
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            if (System.IO.File.Exists("path.txt"))
            {
                using (StreamReader streamReader = new StreamReader("path.txt"))
                {
                    txt_bandizipPath.Text = streamReader.ReadToEnd();
                }
            }
            if (!(txt_bandizipPath.Text == ""))
                return;
            DialogResult dialogResult = startOpenFileDialog("请指定Bandizip.exe", false, "Bandizip|Bandizip.exe", "Bandizip.exe");
            if (dialogResult == DialogResult.OK)
                txt_bandizipPath.Text = openFileDialog1.FileName;
            using (StreamWriter streamWriter = new StreamWriter("path.txt"))
            {
                streamWriter.Write(txt_bandizipPath.Text);
                streamWriter.Flush();
            }
            //防止窗体最小化显示
            TopMost = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = startOpenFileDialog("选择文件",true,"","");
            if (dialogResult == DialogResult.OK)
            {
                foreach (var item in openFileDialog1.FileNames)
                {
                    listBox1.Items.Add(item);
                }
            }
            if (listBox1.Items.Count >= 2)
            {
                txt_outName.Text = "";
                txt_outName.Enabled = false;
            }
        }

        private DialogResult startOpenFileDialog(string title,bool multiselect,string filter,string fileName)
        {
            openFileDialog1.Title = title;
            openFileDialog1.Multiselect = multiselect;
            openFileDialog1.Filter = filter;
            openFileDialog1.FileName = fileName;
            DialogResult dialogResult = openFileDialog1.ShowDialog();
            return dialogResult;
        }

        private void Creatlnk(string shortcutName, string targetPath)
        {
            string shortcutPath = string.Format("{0}.lnk", shortcutName);
            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);//创建快捷方式对象
            shortcut.TargetPath = targetPath;//指定目标路径
            shortcut.WorkingDirectory = Path.GetDirectoryName(targetPath);//设置起始位置
            shortcut.Save();//保存快捷方式
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(!System.IO.File.Exists(Path.GetFileName(txt_bandizipPath.Text)+Path.GetExtension(txt_bandizipPath.Text)+".ink"))
            {
                Creatlnk("Bandizip.exe", txt_bandizipPath.Text);
            }
            foreach (var item in listBox1.Items)
            {
                string outname;
                if (Path.GetExtension(item.ToString())==".exe")
                    outname = Path.GetFileNameWithoutExtension(item.ToString()) + "_zip";
                else
                    outname = txt_outName.Text == "" ? Path.GetFileNameWithoutExtension(item.ToString()) : txt_outName.Text;
                try
                {
                    ProcessStartInfo processStartInfo= new ProcessStartInfo();
                    processStartInfo.FileName = "Bandizip.exe";
                    processStartInfo.Arguments = String.Format("c -fmt:{0} -v:{1} \"{2}\" \"{3}\"", txt_extraName.Text, txt_size.Text, txt_outPath.Text+ outname, item.ToString());
                    processStartInfo.WindowStyle = ProcessWindowStyle.Normal;
                    Process proc = Process.Start(processStartInfo);
                    if(proc !=null)
                    {
                        proc.WaitForExit();

                        DirectoryInfo directoryInfo = new DirectoryInfo(txt_outPath.Text==""?Environment.CurrentDirectory:txt_outPath.Text);
                        foreach (FileInfo item2 in directoryInfo.GetFiles())
                        {
                            string[] nameParts = item2.Name.Split('.');
                            if (nameParts.Length != 2)
                                continue;
                            if (nameParts[0] == outname && nameParts[1] != "exe" && nameParts[1].StartsWith("e"))
                            {
                                item2.MoveTo(item2.FullName+".exe");
                            }
                        }

                        
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult= startOpenFileDialog("选择文件", false, "exe|*.exe", "");
            if(dialogResult ==DialogResult.OK)
            {
                txt_mainFile.Text = openFileDialog1.FileName;
                DirectoryInfo directoryInfo = new DirectoryInfo(Environment.CurrentDirectory);
                foreach (FileInfo item in directoryInfo.GetFiles())
                {
                    string[] nameParts = item.Name.Split('.');
                    if (nameParts.Length != 3)
                        continue;
                    if(nameParts[0] == Path.GetFileNameWithoutExtension(txt_mainFile.Text)&&nameParts[1]!="exe"&&nameParts[1].StartsWith("e")&&nameParts[2]=="exe")
                    {
                        listBox2.Items.Add(item.Name);
                    }
                }
            }
        }
    }
}
