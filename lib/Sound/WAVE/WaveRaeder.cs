/*********************************************
 * waveファイルを読み込むためのクラス
 *  開発者：Katsuhiro Morishita(2011.2) Kumamoto-University
 * 
 * [対応フォーマット]   8/16/24/32 bit ステレオ/モノラル　無圧縮リニアPCM
 * [名前空間]   Sound.WAVE
 * [クラス名]   WaveReader
 * 
 * [参考文献]   waveフォーマット：http://www.kk.iij4u.or.jp/~kondo/wave/
 *              リニアPCM       ：http://e-words.jp/w/E383AAE3838BE382A2PCM.html
 * 
 * [使用方法]   [ソースコードレベルで追加する場合]
 *              ご自身で作成されたプロジェクトに本ファイルとWaveBase.csを追加して下さい。
 *              
 *              [ソースコードレベル…だけど、プロジェクト単位で追加する場合]
 *              ご自身で作成されたプロジェクトに、Soundプロジェクトを参照登録します。
 *              下記のリンクが参考になります。
 *              http://www.geocities.jp/gakaibon/tips/csharp2008/classlibrary-make.html#sec3
 *              
 *              [ライブラリレベルで、dllを追加する場合]
 *              ご自身で作成されたプロジェクトに、Sound.dllを参照登録します。
 *              下記のリンクが参考になります。言語は異なるものの、同じVisual Studioなのでほぼ同じ工程で追加できます。
 *              http://morimori2008.web.fc2.com/contents/PCprograming/Cplusplus/HowToImportCSLib.html
 * 
 *              [以下は共通]
 *              ソースヘッダに以下のコードを追加して下さい。
 *              using Sound.WAVE;
 *              次に、任意のスコープ位置でクラスオブジェクトのインスタンスを生成します。コード例を以下に示す。
 *              usingを宣言済みであれば、下記のコードから「Sound.WAVE」は省略可能です。
 *              例：Sound.WAVE.WaveReader wave = new Sound.WAVE.WaveReader()
 *              
 * [既知の出るかも知れないエラー]
 *              [SystemにはWindows名前空間は含まれません]
 *              コンソールアプリからライブラリを使用すると、上記のようなエラーメッセージが出ることがあります。
 *              その場合、ソリューションエクスプローラーのプロジェクト内にある参照設定に、「System.Windows.Forms」を追加して下さい。
 * 
 * [更新情報]   2011/2/18   開発開始
 *              2011/2/21   読み込みファイルのチャンネル（モノラル／ステレオ）を外部から参照できるように解放
 *              2011/2/27   File_check()の中身を、byte[] to string エンコーディングメソッドを使用してすっきりさせる
 *              2011/9/30   コメントを一部改訂した
 *              2011/10/7   コメントを充実させた。
 *                          チャンネルが定数で宣言されていたのを列挙体に変更した。
 *                          Open()をファイル名orパスで実行できるメソッドを追加。
 *                          WaveFileHeader構造体に初期化メソッドを追加。
 *                          MusicDatasという恥ずかしい構造体名をMusicUnitに変更。
 *                          音声データを取り扱う新たなクラスMusicDataを新設（機能がでかくなるようなら独立させる予定）
 *                          これに伴い、MusicDataクラスを返すメソッドも新設
 *                          今までは8又は16ビットデータしか処理できなかったが、新たに24/32ビットデータも扱えるように拡張
 *                          拡張のために、音を格納するデータサイズを16bitから32ビットへ変更（int型はデフォルトだと32bitなのだが、もし設定をいじってあればエラーの基。。）
 *              2011/10/8   FLLRチャンクにも対応した。
 *              2011/11/16  ファイル名を参照できるようにプロパティを追加
 *              2011/12/5   waveファイルの書き出し用のWaveWriterクラス整備に向けて、共通項っぽいものをWaveBase.csへ掃き出した。
 *              2011/12/7   外部に公開しているポイント指定のReadメソッドを、MusicDataを返すように変更
 *              2012/1/12   LISTチャンクに対応した
 *              2012/1/19   JUNKとfactチャンクに対応した
 *                          ファイルを開けなかった場合にその理由がわかるように、エラーメッセージを取得できるように改造した。
 *              2012/2/1    Disposeを追加
 *              2012/5/4    微妙に修正した。機能の追加やコメントは変わっていない。
 *                          CheckFile()をはじめいくつかのメソッドの一部を読みやすく訂正した。
 *              2012/7/13   コメントと実装の不一致を訂正しました。
 *              2012/7/20   体裁と誤字の訂正
 *              2013/3/16    dataチャンクの前に出現することがあるデータチャンクを読み飛ばすアルゴリズムを若干変更した。
 *                          dataチャンクの証しである、“data”が出てこないファイルの場合は異常終了するはずだが、WAVEの仕様上はあり得ない。
 *                          今日の段階では放っておく。
*********************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;                                        //ファイル操作に必要


/************************************************
 * WAVEファイルを扱うクラスを集めた名前空間です。
 * **********************************************/
namespace Sound.WAVE
{
    /// <summary>
    /// waveファイルを読み込むためのクラス
    /// <para>
    /// <remarks>
    /// ファイル名を指定することで開きます。
    /// 後はRead()メソッドを使用することで順次データを読み込むことができます。
    /// </remarks>
    /// </para>
    /// </summary>
    public class WaveReader : WaveBase, IDisposable
    {
        /*************** グローバル変数の宣言 ************/
        /// <summary>
        /// waveファイルのファイル名
        /// </summary>
        private string fname = "";
        /// <summary>
        /// waveファイルのヘッダ情報を格納する構造体
        /// </summary>
        private WaveFileHeader fileInfo;
        /// <summary>
        /// ファイルが開けていればture
        /// </summary>
        private Boolean isOpenStatus = false;
        /// <summary>
        /// ファイル読み込みに使用するストリームオブジェクト
        /// </summary>
        private FileStream fs;
        /// <summary>
        /// ファイルをバイナリで読み込むためのオブジェクト
        /// </summary>
        private BinaryReader reader;
        /// <summary>
        /// 読み込み済みの波形データサイズ[Byte]
        /// </summary>
        private UInt32 ReadWaveDataSize = 0;
        /// <summary>
        /// エラーメッセージ
        /// </summary>
        private string _errorMessage = "";
        /****************************　プロパティ　************************************/
        #region "プロパティ"
        /// <summary>ファイル名</summary>
        public string FileName { get { return this.fname; } }
        /// <summary>
        /// ファイルを開けたか状態をチェック
        /// </summary>
        public Boolean IsOpen {
            get {
                return this.isOpenStatus;
            }
        }
        /// <summary>
        /// 読み込んだファイルがステレオかどうかを示す
        /// </summary>
        public Channel Ch
        {
            get
            {
                return this.fileInfo.Channel;
            }
        }
        /// <summary>
        /// データを最後まで読み切るとtrue
        /// </summary>
        public Boolean ReadLimit
        {
            get 
            {
                Boolean ans;

                if (this.ReadWaveDataSize < this.fileInfo.WaveDataSize){
                    ans = false;
                }
                else {
                    ans = true;
                }
                return ans;
            }
        }
        /// <summary>
        /// 読み出し位置までの経過時刻[s]を返す
        /// </summary>
        public double NowTime 
        {
            get { return (double)ReadWaveDataSize / (double)this.fileInfo.BlockSize / (double)this.fileInfo.SamplingRate; }
        }
        /// <summary>
        /// サンプリング周波数
        /// </summary>
        public int SamplingRate
        {
            get { return (int)this.fileInfo.SamplingRate; }
        }
        /// <summary>
        /// 量子化分解能
        /// </summary>
        public int Resolution
        {
            get { return (int)this.fileInfo.ResolutionBit; }
        }
        /// <summary>
        /// 音源ファイルの長さ[s]
        /// </summary>
        public double SoundLength
        {
            get { return this.fileInfo.WaveDataSize / this.fileInfo.DataRate; }
        }
        /// <summary>
        /// エラーメッセージ
        /// </summary>
        public string ErrorMessage
        {
            get { return this._errorMessage; }
        }
        #endregion
        /****************************　メソッド　************************************/
        #region "メソッド"
        /// <summary>
        /// 指定サイズのデータ量が音源にして何秒であるかを返す
        /// </summary>
        /// <param name="size">指定サイズ[point]</param>
        /// <returns>指定サイズの表す時間[sec]</returns>
        public double GetTimeAtSpecifiedSize(int size)
        {
            return (double)size / (double)this.fileInfo.SamplingRate;
        }
        /// <summary>
        /// ファイルを閉じる
        /// </summary>
        public void Close(){
            if (this.reader != null)
            {
                this.reader.Close();    // こちらを先に閉じる
                this.fs.Close();        // これが後。リソースを解放しているのか少し怪しい。
                this.fs.Dispose();
                this.fileInfo.Init();   // ファイル情報を初期化
            }
            this.isOpenStatus = false;
            return;
        }
        /// <summary>
        /// 資源を開放します
        /// </summary>
        public void Dispose() {
            this.Close();
            return;
        }
        /// <summary>
        /// wave音楽ファイルであるかをチェックする
        /// </summary>
        /// <returns>8/16 bit リニアPCM形式のwave音楽ファイルでなければ、falseを返す</returns>
        private Boolean CheckFile(){
            byte[] buf;
            string txt;
            Boolean ans = true;
            int fmt_chunk_size = 0, fmt_read_size = 0;

            txt = Encoding.ASCII.GetString(this.reader.ReadBytes(4));
            if (ans && txt != "RIFF") { ans = false; this._errorMessage = "RIFF識別子が見つかりません。"; }
            if (ans) buf = this.reader.ReadBytes(4);                                        // 読み飛ばし
            if (ans) txt = Encoding.ASCII.GetString(this.reader.ReadBytes(4));
            if (ans && txt != "WAVE") { ans = false; this._errorMessage = "WAVEファイルではありません。"; }
            if (ans) txt = Encoding.ASCII.GetString(this.reader.ReadBytes(4));              // 
            if (ans && (txt == "LIST"))                                                     // 著作権情報LISTチャンクがあれば読み飛ばす
            {
                Int32 listSize = this.reader.ReadInt32();
                buf = this.reader.ReadBytes(listSize);                                      // 読み飛ばし
                txt = Encoding.ASCII.GetString(this.reader.ReadBytes(4));                   // 続くfmt のために読み込み
            }
            if (ans && txt != "fmt ") { ans = false; this._errorMessage = "fmt チャンクが見当たりません。"; }
            if (ans) fmt_chunk_size = this.reader.ReadInt32();                              // fmtチャンクのサイズを取得.リニアPCMであれば16.
            if (ans) this.fileInfo.ID = this.reader.ReadInt16();
            if (ans && this.fileInfo.ID != 1) { ans = false; this._errorMessage = "リニアPCMではありません。"; }   // リニアPCMであるかをチェック
            if (ans) {                                                                      // チャンネル（ステレオ/モノラルの判定）
                int channelKind = (int)this.reader.ReadInt16();
                if (channelKind == (int)Channel.Monoral) 
                    this.fileInfo.Channel = Channel.Monoral; 
                else 
                    this.fileInfo.Channel = Channel.Stereo; 
            }
            if (ans) this.fileInfo.SamplingRate  = this.reader.ReadInt32();
            if (ans) this.fileInfo.DataRate      = this.reader.ReadInt32();
            if (ans) this.fileInfo.BlockSize     = this.reader.ReadInt16();
            if (ans) this.fileInfo.ResolutionBit = this.reader.ReadInt16();
            if (ans && this.fileInfo.ResolutionBit != 8 && this.fileInfo.ResolutionBit != 16 && this.fileInfo.ResolutionBit != 24 && this.fileInfo.ResolutionBit != 32) 
                { ans = false;  this._errorMessage = "分解能が8, 16, 24, 32 bitでは無いので対応できません。"; }
            while (ans && (fmt_chunk_size - 16 - fmt_read_size) > 0) {                      // fmtチャンクの拡張部分を読み飛ばす
                buf = this.reader.ReadBytes(1);                                             // ID == 1なら、存在しないので必要ないが、拡張に備えて設置
                fmt_read_size++;
            }
            if (ans) txt = Encoding.ASCII.GetString(this.reader.ReadBytes(4));              // 
            while (ans && txt != "data")                                                    // "data"ではなく、FLLRチャンクやJUNKチャンクが続くことがあるようだ。
            {
                Int32 read_size = this.reader.ReadInt32();                                  // チャンクサイズを取得
                buf = this.reader.ReadBytes((int)read_size);                                // 読み飛ばし
                txt = Encoding.ASCII.GetString(this.reader.ReadBytes(4));
            }
            if (ans && txt != "data") { ans = false;  this._errorMessage = "dataチャンクが見当たりません。"; } // "data"チャンクかどうかチェック
            if (ans) this.fileInfo.WaveDataSize = this.reader.ReadUInt32();                 // 波形データサイズを取得
            
            return ans;
        }
        /// <summary>
        /// ダイアログを使ってファイルを開く
        /// </summary>
        /// <remarks>ファイルオープンの状態は、IsOpenプロパティに反映されます</remarks>
        /// <param name="dialog">ダイアログオブジェクト</param>
        /// <returns>開けたかどうかをtrue/falseで返す。成功ならtrue。</returns>
        public Boolean Open(OpenFileDialog dialog){
            Boolean format_match;

            if (this.isOpenStatus == true) this.Close();                            // もしファイルを既に開いているなら、一度閉じる
            this.fs = new FileStream(dialog.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            this.reader = new BinaryReader(this.fs);
            format_match = CheckFile();
            if (format_match == false) this.Close();
            if (format_match)
            {
                this.isOpenStatus = true;
                this.fname = System.IO.Path.GetFileName(dialog.FileName);           // ファイル名をバックアップ
            }
            this.ReadWaveDataSize = 0;
            return format_match;
        }
        /// <summary>
        /// ファイル名（またはパス）の指定でファイルを開く
        /// </summary>
        /// <param name="fname">ファイル名（絶対パス・相対パスでも良い）</param>
        /// <returns>開いたファイルのフォーマットが一致しなかった場合、falseが返ります</returns>
        /// <exception cref="SystemException">指定ファイルが存在しない場合にスロー</exception>
        public Boolean Open(string fname)
        {
            Boolean format_match = false;

            if (this.isOpenStatus == true) this.Close();                            // もしファイルを既に開いているなら、一度閉じる
            if (File.Exists(fname))                                                 // ファイルの存在を確認
            {
                this.fs = new FileStream(fname, FileMode.Open, FileAccess.Read, FileShare.Read);
                this.reader = new BinaryReader(this.fs);
                format_match = CheckFile();
                if (format_match == false) this.Close();                            // フォーマットチェック
                if (format_match)
                {
                    this.isOpenStatus = true;
                    this.fname = fname;
                }
                this.ReadWaveDataSize = 0;
            }
            else 
            {
                throw new SystemException("指定されたファイルは存在しません。");
            }
            return format_match;
        }
        /// <summary>
        /// バイナリデータから、リトルエンディアンの24ビット整数を得る
        /// </summary>
        /// <remarks>
        /// C#のint型が32ビットとうち決めた方法で計算する。
        /// とりあえず結合した後、負であれば残りの上位ビットを全部1で埋めればよい。
        /// <para>
        /// ex. 24 bit -3 == 0b(1111 1111 1111 1111 1111 1101)
        ///     これが32ビットに拡張しても、0b(1111 1111 1111 1111 1111 1111 1111 1101)となるだけ。
        /// </para>
        /// 正の数の場合は代わりに0で埋めればよい（つまり、何もしなくて良い）。
        /// </remarks>
        /// <param name="r">バイナリのリーダーオブジェクト（既に開いていること）</param>
        /// <returns>int型の変数として返します。</returns>
        private int ReadInt24(BinaryReader r)
        { 
            int ans;

            byte[] hoge = r.ReadBytes(3);                                           // 24 bit分として3 Byte読み込む
            uint sign = (uint)hoge[2] >> 7;                                         // 7ビット右シフトして、最上位の符号ビットを得る
            uint temp = (uint)hoge[0] + ((uint)hoge[1] << 8) + ((uint)hoge[2] << 16);// データを足し合わせて32ビット整数に変換する
            if (sign == (uint)1)                                                    // 符号ビットが1なら負
                ans = (int)(temp | (uint)0xff000000);                               // 負なら、最上位バイトを1で埋める
            else
                ans = (int)temp;
            return ans;
        }
        /// <summary>
        /// 指定ポイント数の音声データを読み込む。
        /// <para>モノラルなら、Left/Rightどちらにも格納される。</para>
        /// </summary>
        /// <param name="size">データサイズ[point]</param>
        /// <returns>ファイルを完読すると、0を書き込んで返す</returns>
        private MusicUnit[] ReadFile(int size)
        {
            MusicUnit[] music = new MusicUnit[size];  // これにデータを格納して返す

            if (this.isOpenStatus) {
                for (int i = 0; i < size && this.ReadWaveDataSize < this.fileInfo.WaveDataSize; i++){
                    if (this.ReadWaveDataSize < this.fileInfo.WaveDataSize)
                    {
                        // まず左を埋める
                        if (this.fileInfo.ResolutionBit == 8) music[i].Left = (int)this.reader.ReadSByte();
                        if (this.fileInfo.ResolutionBit == 16) music[i].Left = (int)this.reader.ReadInt16();
                        if (this.fileInfo.ResolutionBit == 24) music[i].Left = this.ReadInt24 (this.reader);
                        if (this.fileInfo.ResolutionBit == 32) music[i].Left = (int)this.reader.ReadInt32();
                        // 次に、ステレオ音源なら右を埋める
                        switch (this.fileInfo.Channel)
                        {
                            case Channel.Stereo:
                                if (this.fileInfo.ResolutionBit == 8) music[i].Right = (int)this.reader.ReadSByte();
                                if (this.fileInfo.ResolutionBit == 16) music[i].Right = (int)this.reader.ReadInt16();
                                if (this.fileInfo.ResolutionBit == 24) music[i].Right = this.ReadInt24(this.reader);
                                if (this.fileInfo.ResolutionBit == 32) music[i].Right = (int)this.reader.ReadInt32();
                                break;
                            case Channel.Monoral:
                                music[i].Right = music[i].Left;
                                break;
                        }
                        this.ReadWaveDataSize += (UInt32)this.fileInfo.BlockSize;   // 読み込んだデータ量を更新
                    }
                    else {
                        music[i].Left = 0;                                          // 読めるデータが無くなると、0を書き込んで返す
                        music[i].Right = 0;
                    }
                }
            }
            return music;
        }
        /// <summary>
        /// 指定時間分のデータを読み込むのに必要となる配列サイズを返す
        /// </summary>
        /// <param name="time_width">指定時間幅[s]</param>
        /// <returns>指定時間のデータを格納するに足るデータサイズ</returns>
        public int GetArrSize(double time_width) {
            return (int)((double)this.fileInfo .SamplingRate * time_width);
        }
        /// <summary>
        /// 指定時間分の音声データを返す
        /// </summary>
        /// <remarks>
        /// waveファイルより、指定された時間だけデータを読み込んで返します。
        /// 読み出し開始位置は、既読部分の続きになります。
        /// </remarks>
        /// <example>
        /// waveというWaveReaderクラスオブジェクトを生成した場合、下記のコードを実行すれば1.50秒分のサウンドデータを取得できます。
        /// <code>
        /// MusicData sound = wave.Read(1.50);
        /// </code>
        /// </example>
        /// <param name="time_width">指定時間幅[s]</param>
        /// <returns>音声データクラス</returns>
        public MusicData Read(double time_width)
        {
            return new MusicData(this.ReadFile(this.GetArrSize(time_width)));
        }
        /// <summary>
        /// 指定ポイント数の音声データを返す
        /// </summary>
        /// <param name="size">データサイズ[point]</param>
        /// <returns>音声データクラス</returns>
        public MusicData Read(int size)
        {
            return new MusicData(this.ReadFile(size));
        }
        /****************************　初期化/削除　************************************/
        /*****************
         * このクラスのデストラクタ
         * ***************/
        /// <summary>
        /// 　デストラクタ.
        /// </summary>
        ~WaveReader(){
            this.Close();
        }
        #endregion
    }
}
