using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sound.WAVE;

namespace WaveReaderChecker
{
    public partial class FormOfWaveReaderCheker : Form
    {
        /// <summary>
        /// 開いたパスを記憶しておく
        /// </summary>
        private string _path = "";
        /// <summary>
        /// 
        /// </summary>
        public FormOfWaveReaderCheker()
        {
            InitializeComponent();
        }

        private void FormOfWaveReaderCheker_Load(object sender, EventArgs e)
        {

        }

        private void buttonOfStart_Click(object sender, EventArgs e)
        {
            int outCounter = 0;
            int wavefileNum = 0;

            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.SelectedPath = this._path;
            if(dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this._path = dialog.SelectedPath;
                string[] files = System.IO.Directory.GetFiles(dialog.SelectedPath);
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter("WaveReader out list.txt", false, Encoding.UTF8))
                {
                    for (int i = 0; i < files.Length; i++)
                    {
                        string fname = System.IO.Path.GetFileName(files[i]);
                        string[] field = fname.Split('.');
                        if (field[field.Length - 1] == "wave" || field[field.Length - 1] == "wav" || field[field.Length - 1] == "WAV")
                        {
                            wavefileNum++;
                            WaveReader reader = new WaveReader();
                            reader.Open(files[i]);
                            if (reader.IsOpen == false)
                            {
                                outCounter++;
                                sw.WriteLine(files[i] + "\t" + reader.ErrorMessage);
                            }
                            reader.Close();
                            reader = null;
                        }
                    }
                }
            }
            MessageBox.Show(wavefileNum.ToString() + "件中、" + outCounter.ToString() + "件の開けないファイルが確認されました。");
            return;
        }
    }
}
