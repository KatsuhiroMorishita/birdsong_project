using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace wavePlayerForCheck
{
    public partial class Info : Form
    {
        public Info()
        {
            InitializeComponent();
        }

        private void info_Load(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 本フォームに、表示したいファイルがあればそのファイル名を渡してください。
        /// </summary>
        /// <param name="filePath"></param>
        public void Setup(string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                using (System.IO.StreamReader sr = new System.IO.StreamReader(filePath))
                {
                    string txt = sr.ReadToEnd();
                    label1.Text = txt;
                }
            }
            return;
        }
    }
}
