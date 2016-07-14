/***************************************************************************
 * [ソフトウェア名]
 *      Song Checker
 * [開発者]
 *      K.Morishita (Kumamoto-Univ. @ 2012.1)
 * [概要]
 *      別ソフトで作成された発声部分リストを基に、Waveファイルを再生させるソフトウェアです。
 *      検出部分の鳥の種類を人が聞いて判断するのに使用してください。
 *      リスト中に記載されたタイムスタンプを使って一つのファイル中の複数個所を再生させます。
 *      リストのタイムスタンプのフォーマットは、"ID,start time,time width,etc."とします。
 *      再生対象とするファイル名をファイルの1行目に記載してください。
 *      日本語を使用する場合、解析対象ファイルはUTF-8で記述されている必要があります。
 * [参考資料]
 *      
 * [検討課題]
 *      
 * [履歴]
 *      2012/1/18   開発開始。3時間で何とか形になった。
 *      2012/11/16  最新のフォーマットに合わせて更新した。
 *                  懸案だったデリミタの件は片付いた。
 *                  ただし、新フォーマットへの完全移行にはついて行っていない。
 *                  タイムオーバーかなぁ。
 *      2013/1/15   新フォーマットへの完全移行を果たした。
 *                  旧フォーマットも処理可能。
 *                  ただし、帯域名の後にチェックを記入するためのスペースは必須とする。
 *      2013/1/17   コメントを追加し、一部の処理を変更した。
 *      2013/2/15   結果を保存するファイルにファイルパスを書き込んだ後、改行コードを入れるのを忘れていたのを修正した。
 *      2013/3/16   処理する文字列にコメントを付加できるように改良した。
 *                  そのために、FeatureTextLineParseクラスを取り込んで、TimeLine構造体内の記述方法が若干変更になっています。
 *      2013/3/22    FeatureTextLineParseクラスをBirdSongFeatureプロジェクトに移して、これを使うように変更した。
 * *************************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;   // for Regex

using Sound.WAVE;
using BirdSongFeature.Feature;

namespace wavePlayerForCheck
{
    /// <summary>
    /// 再生時刻を格納する構造体
    /// </summary>
    public struct TimeLine
    {
        /// <summary>
        /// 再生開始時刻[s]
        /// </summary>
        private double _startTime;
        /// <summary>
        /// 再生終了時刻[s]
        /// </summary>
        private double _endTime;
        /// <summary>
        /// 音源ファイルのパス
        /// </summary>
        private string _musicFilePath;
        /// <summary>
        /// コンストラクタ時に渡された被解析文字列
        /// </summary>
        private string _line;
        /// <summary>
        /// エラー状況
        /// <para>true: error</para>
        /// </summary>
        private readonly bool _error;
        /// <summary>
        /// メッセージ
        /// <para>以上時にメッセージを残します。</para>
        /// </summary>
        private string _message;
        // ---プロパティ----------------------------------
        /// <summary>
        /// 再生開始時刻
        /// <para>ファイル先頭からの時間[s]です。</para>
        /// </summary>
        public double StartTime
        {
            get 
            {
                if (this.Error)
                    return 0.0;
                else
                    return _startTime; 
            }
        }
        /// <summary>
        /// 再生終了時刻
        /// </summary>
        public double EndTime
        {
            get 
            {
                if (this.Error)
                    return 0.0;
                else
                    return _endTime; 
            }
        }
        /// <summary>
        /// 音源ファイル名
        /// </summary>
        public string MusicFilePath
        {
            get
            {
                if (this.Error)
                    return "";
                else
                    return this._musicFilePath; 
            }
        }
        /// <summary>
        /// エラー
        /// <para>true: エラーあり</para>
        /// </summary>
        public bool Error
        {
            get { return this._error; }
        }
        /// <summary>
        /// メッセージ
        /// </summary>
        public string Message
        {
            get { return this._message; }
        }
        // ---メソッド----------------------------------
        /// <summary>
        /// 文字列に識別結果(タグ)を挿入して返す
        /// </summary>
        /// <param name="tag">付加したいタグ</param>
        /// <returns>タグを付けた文字列<para>エラーがある場合は、""を返します。</para></returns>
        public string GetLineAsTagPlus(string tag)
        {
            if (this.Error == false)
            {
                FeatureTextLineParser parser = new FeatureTextLineParser(this._line);
                var delimiter = this.GetDelimiter(parser.Feature);
                if (delimiter.HasValue)
                {
                    string[] field = this._line.Split(new char[] { delimiter.Value }, 3);
                    string line = field[0] + delimiter.Value.ToString() + tag + delimiter.Value.ToString() + field[2];
                    return line;
                }
                else
                    return "";
            }
            else
                return "";
        }
        /// <summary>
        /// 渡された文字列を解析して、デリミタ文字を返します
        /// <para>検査されるのは、',', '\t'の2つです。</para>
        /// <para>一つの行のデータに複数のデリミタを混ぜないでください。</para>
        /// </summary>
        /// <param name="txt">被解析文字列</param>
        /// <returns>デリミタ文字<para>もし、デリミタ文字が発見できなかった場合はnullを返します。</para></returns>
        private char? GetDelimiter(string txt)
        {
            string[] field = txt.Split(',');
            if (field.Length != 1)
                return ',';
            field = txt.Split('\t');
            if (field.Length != 1)
                return '\t';

            return null;
        }
        /// <summary>
        /// 指定した音声ファイルの長さ[s]を取得する
        /// <para>ファイルを検査できなければ0を返す。</para>
        /// </summary>
        /// <param name="path">ファイルのパス</param>
        /// <returns>音源ファイルの長さ[s]</returns>
        private double GetSoundLength(string path)
        {
            double ans = 0.0;
            WaveReader wave_file = new WaveReader();
            wave_file.Open(path);
            if (wave_file.IsOpen)
                ans = wave_file.SoundLength;
            else
                this._message += "*WAVEファイルの長さを得ることができませんでした。ファイルが存在しないか、フォーマットが対応していないか、アクセスに制限がかかっている可能性があります。\n";
            wave_file.Close();
            wave_file = null;
            return ans;
        }
        /// <summary>
        /// 引数を解析して、音源ファイルのパスと再生時刻をメンバ変数にセットします
        /// </summary>
        /// <param name="line">解析したい文字列</param>
        /// <param name="fpath">音源ファイルのパス</param>
        /// <returns>[bool] true: エラーあり</returns>
        private bool ParseArg(string line, string fpath)
        {
            bool ans = false;
            this._musicFilePath = fpath;
            Console.WriteLine(fpath);

            if (System.IO.File.Exists(fpath))
            {
                FeatureTextLineParser parser = new FeatureTextLineParser(line);
                if (parser.Success)
                {
                    var len = this.GetSoundLength(fpath);
                    if (len == 0.0)
                        ans = true;
                    else
                    {
                        double startTime = double.Parse(parser.StartTime);
                        double endTime = startTime + double.Parse(parser.TimeWidth);
                        if (endTime > len)
                        {
                            endTime = len;
                            this._message += "*リスト中に記載されている再生時刻が音源ファイルの長さを超えました。確認して下さい。\n";
                        }
                        this._startTime = startTime;
                        this._endTime = endTime;

                        if (startTime > endTime || endTime == 0.0)
                        {
                            ans = true;
                            this._message += "*再生開始時刻が再生終了時刻よりも遅れています。\n";
                        }
                    }
                }
                else
                {
                    this._message += "*テキストフォーマットの不正により解析できませんでした。\n";
                    ans = true;
                }
            }
            else
            {
                this._message += "*音源ファイルが存在しませんでした。\n";
                ans = true;
            }
            return ans;
        }
        /// <summary>
        /// コンストラクタ
        /// <para>一行の中にファイル名が入っているケース用です。</para>
        /// </summary>
        /// <param name="line">被解析文字列</param>
        public TimeLine(string line)
        {
            this._error = false;
            this._startTime = 0.0;
            this._endTime = 0.0;
            this._musicFilePath = "";
            this._line = line;
            this._message = "";

            FeatureTextLineParser parser = new FeatureTextLineParser(line);
            if (parser.Success && parser.FilePath != "")
            {
                this._error = this.ParseArg(line, parser.FilePath);
            }
            else
            {
                this._error = true;
                this._message += "*文字列から音源ファイル名を取得できませんでした。\n";
            }
        }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="line">解析したい文字列</param>
        /// <param name="fpath">音源ファイルのパス</param>
        public TimeLine(string line, string fpath)
        {
            this._error = false;
            this._startTime = 0.0;
            this._endTime = 0.0;
            this._musicFilePath = "";
            this._line = line;
            this._message = "";

            this._error = this.ParseArg(line, fpath);
        }
    }
    public partial class Form1 : Form
    {
        // ---- メンバ変数 -----------------------------------------------------------
        /// <summary>
        /// 再生時刻のリスト
        /// <para>再生が終わると要素からどんどん削除する</para>
        /// </summary>
        List<TimeLine> _playList = new List<TimeLine>(0);
        /// <summary>
        /// 再生リストの何番目を再生しているかを示す
        /// <para>表示では1～とする</para>
        /// </summary>
        private int _playCount = 0;
        /// <summary>
        /// 再生リストのサイズを記憶しておく
        /// </summary>
        private int _totalCount = 0;
        /// <summary>
        /// ログを保存するためのファイル名
        /// </summary>
        private string _fnameForSave = "";
        /// <summary>
        /// ファイルへチェック内容を書き込んだかどうかを表すフラグ
        /// </summary>
        private Boolean _savedFlag = true;
        /// <summary>
        /// テキストの内容を表示するためのフォーム
        /// </summary>
        private Info infoForm;
        /// <summary>
        /// マッチさせたいコードがあればこれを利用する
        /// </summary>
        private KeyCodeMatching _codeList;
        // ---- メソッド -------------------------------------------------------------
        /// <summary>
        /// コンストラクタ
        /// <para>パラメータの引き渡しがあればここを使います。</para>
        /// </summary>
        public Form1()
        {
            InitializeComponent();
        }
        /// <summary>
        /// フォームのインスタンスが生成された後に呼び出されます
        /// <para>インターフェイスの変更はここでやってください。</para>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            // URLプロパティが指定されたら自動的に再生されるようにしない
            this.axWindowsMediaPlayer1.settings.autoStart = false;
            // コンボボックスの設定
            this.Setting.SelectedIndex = 0;
            // 
            this.infoForm = new Info();
            this.infoForm.Show();
            this.infoForm.Setup("bird list.ini");

            // コードマッチの準備
            this._codeList = new KeyCodeMatching("bird list.ini");
        }
        /// <summary>
        /// 文字列を解析して、ファイルパスを取得する
        /// </summary>
        /// <param name="line">解析対象文字列</param>
        /// <returns>文字列から抜き出したファイルパス<para>パスを取得できなければ""が返ります。</para></returns>
        private string GetPath(string line)
        {
            Regex re = new Regex(@"File Path(\t|,)(?<filepath>(\w|\\|:|\.|\s|-)+)");
            string fpath = "";
            Match m = re.Match(line);                // マッチチェック
            if (m.Success)
            {
                fpath = m.Groups["filepath"].Value;
                Console.WriteLine(fpath);
            }
            return fpath;
        }
        /// <summary>
        /// 音源再生のための準備を行う
        /// <para>既にセットされた音源であれば、特に何もしない。</para>
        /// </summary>
        /// <param name="fpath"></param>
        private void OpenSound(string fpath)
        {
            if (System.IO.File.Exists(fpath))
            {
                // オーディオファイルを開く準備
                if (this.axWindowsMediaPlayer1.URL != fpath)
                {
                    this.axWindowsMediaPlayer1.URL = fpath;
                }
            }
            return;
        }
        /// <summary>
        /// ファイルを開きます
        /// <para>エラー処理はあまり入っていないので注意</para>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();                       // ファイルを開く準備
            dialog.Filter = "解析ファイル|*.csv";
            if(dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                using (System.IO.StreamReader sr = new System.IO.StreamReader(dialog.FileName, Encoding.UTF8))//
                {
                    try
                    {
                        this._playList.Clear();                                 // 変数初期化
                        this._playCount = 0;

                        List<string> txt = new List<string>(0);
                        while (sr.EndOfStream == false) txt.Add(sr.ReadLine());
                        var path = this.GetPath(txt[0]);
                        
                        // 再生のためのリストを作る
                        var errorMsg = "";
                        for (int i = 0; i < txt.Count; i++)
                        {
                            TimeLine timeLine = new TimeLine();
                            if(path != "")
                                timeLine = new TimeLine(txt[i], path);
                            else
                                timeLine = new TimeLine(txt[i]);
                            if (timeLine.Error == false)                        // エラーチェック
                                this._playList.Add(timeLine);
                            else
                                errorMsg = timeLine.Message;
                        }
                        // 登録件数のチェック
                        if (this._playList.Count == 0)
                        {
                            var msg = "正常に認識された登録件数は0件でした。\n";
                            msg += "[エラーメッセージの一部]\n";
                            msg += errorMsg;
                            MessageBox.Show(msg);
                        }
                        else
                        {
                            this._totalCount = this._playList.Count;
                            MessageBox.Show(this._playList.Count.ToString() + "件の再生を行います。");
                            var fname = System.IO.Path.GetFileNameWithoutExtension(dialog.FileName);
                            var originDir = System.Windows.Forms.Application.StartupPath;
                            this._fnameForSave = originDir + @"\" + fname + " checked.csv";
                            if (path != "")                                                     // 必要なら、ファイルパスを書き込む
                                this.Save(this._fnameForSave, false, txt[0] + System.Environment.NewLine);
                            MessageBox.Show("キーボードの'n':next, 'r':rejectを押すと次々と再生します。");
                        }
                    }
                    catch (Exception error)
                    {
                        MessageBox.Show(error.Message);
                    }
                }
            }
        }
        /// <summary>
        /// 結果をファイルへ保存します
        /// </summary>
        /// <param name="fname">ファイル名</param>
        /// <param name="append">追記の有無<para>falseだと上書きします。</para></param>
        /// <param name="line">書き込み内容</param>
        private void Save(string fname, Boolean append, string line)
        {
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(this._fnameForSave, append, Encoding.UTF8))
            {
                sw.Write(line);
            }
            return;
        }
        /// <summary>
        /// タイマーの処理
        /// <para>再生中のプレイヤーの停止時刻をチェックして、停止させる</para>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerForCheckOfStop_Tick(object sender, EventArgs e)
        {
            if (this.axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPlaying)
            {
                if (this._playList[0].EndTime <= this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition)// 再生時間をチェック
                {
                    this.axWindowsMediaPlayer1.Ctlcontrols.pause();
                    this.timerForCheckOfStop.Enabled = false;                                       // タイマー停止
                }
            }
        }
        /// <summary>
        /// キーイベントを担当
        /// <para>動作中に入力されたキーに合わせて判別結果をファイルへ保存します。</para>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void axWindowsMediaPlayer1_KeyPressEvent(object sender, AxWMPLib._WMPOCXEvents_KeyPressEvent e)
        {
            if (this._codeList.IsContain(((char)e.nKeyAscii).ToString()))
            {
                if (this._savedFlag == false)
                {
                    string code = this._codeList.GetID(((char)e.nKeyAscii).ToString());             // コードを取得
                    this.judgeLabel.Text = "No." + this._playCount.ToString() + " is " + code + ".";

                    this.Save(this._fnameForSave, true, this._playList[0].GetLineAsTagPlus(code) + System.Environment.NewLine);  // 保存する
                    this._savedFlag = true;                                                         // 保存したという行為を覚えておく
                    if (e.nKeyAscii == 'r')                                                         // 色を変える
                        this.playNumLabel.BackColor = Color.Red;
                    else
                        this.playNumLabel.BackColor = Color.Green;
                    this._playList.RemoveAt(0);
                }
                if (this._playList.Count != 0 && this.axWindowsMediaPlayer1.playState != WMPLib.WMPPlayState.wmppsPlaying)
                {
                    this._playCount++;                                                  // 再生番号をカウントアップ
                    this.playNumLabel.Text = "再生番号：" + this._playCount.ToString() + "/" + this._totalCount.ToString();
                    this.playNumLabel.BackColor = Color.Transparent;
                    this.OpenSound(this._playList[0].MusicFilePath);
                    this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition = this._playList[0].StartTime; // 再生開始時刻をセット
                    this.axWindowsMediaPlayer1.Ctlcontrols.play();                      // 再生開始
                    this.timerForCheckOfStop.Enabled = true;                            // (時間になったら止めるために)タイマースタート
                    this._savedFlag = false;
                }
                if (this._playList.Count == 0)
                {
                    MessageBox.Show("再生リストは空です。");
                }
            }
        }
        /// <summary>
        /// コンボボックスの設定が行われた際の動作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingChange(object sender, EventArgs e)
        {
            if ((string)this.Setting.SelectedItem == "Auto")
            {
                MessageBox.Show("未対応です。\n人力で頑張ってください。");
                this.Setting.SelectedIndex = 0;
            }
            return;
        }
    }
}
