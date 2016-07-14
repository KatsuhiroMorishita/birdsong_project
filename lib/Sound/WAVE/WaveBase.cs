/*********************************************
 * waveファイルを扱うための基底クラスと構造体など
 *  開発者：Katsuhiro Morishita(2011.12) Kumamoto-University
 * 
 * [名前空間]   Sound.WAVE
 * 
 * [更新情報]   2011/12/5   開発開始
 *                          名前空間をWave_FileからWaveFileに変更
 *                          MusicDataクラスに、インデクサを追加。また、コンストラクタでディープコピーするように変更した。
 *              某月某日    WaveReaderクラスとファイルを分離
 *                          また、名前空間をSound.WAVEへ変更した。
 *              2012/5/4    Sound.WAVE.Channelに値を付けた。
 *                          以前はモノラルが0だったのを1にしたのでどっかで不具合が出るかな？
 *              2012/7/12   MusicDataクラスのインデクサで、if (i < this.Length) i = this.Length - 1;となっていたが、if (i > this.Length) i = this.Length - 1;が正しいとの指摘を受けて、訂正しました。
 *              2012/7/13   コメントと実装の不一致を訂正しました。
 *                          また、WaveBaseクラスをそのままインスタンスが作れないように、abstruct修飾子を付けました。
*********************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/************************************************
 * Waveファイルを扱うクラスを集めた名前空間です。
 * **********************************************/
namespace Sound.WAVE
{
    /// <summary>
    /// モノラル/ステレオを表す列挙体
    /// </summary>
    public enum Channel : int
    {
        /// <summary>モノラル</summary>
        Monoral = 1,
        /// <summary>ステレオ</summary>
        Stereo = 2
    }
    /// <summary>
    /// 一つの“音”を格納する構造体
    /// </summary>
    public struct MusicUnit
    {
        /// <summary>左の音</summary>
        public int Left;
        /// <summary>右の音</summary>
        public int Right;
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="left">左の音</param>
        /// <param name="right">右の音</param>
        public MusicUnit(int left, int right)
        {
            this.Left = left;
            this.Right = right;
            return;
        }
    }
    /// <summary>
    /// 音声データを格納するクラス
    /// <para>
    /// 取得した・セットした音声データを取り扱いやすくする
    /// </para>
    /// </summary>
    public class MusicData
    {
        /****** 列挙体 *******/
        /// <summary>利用可能なチャンネルを示すのに使用する</summary>
        public enum UsableChannel
        {
            /// <summary>右だけに存在</summary>
            Right,
            /// <summary>左だけに存在</summary>
            Left,
            /// <summary>左右両方に存在</summary>
            Both,
            /// <summary>音声なし</summary>
            NA
        }
        /// <summary>左右を指定するために使用する</summary>
        public enum Channel
        {
            /// <summary>右</summary>
            Right,
            /// <summary>左</summary>
            Left
        }
        /****** メンバ変数 *******/
        /// <summary>
        /// 音声データを格納する配列
        /// </summary>
        private MusicUnit[] data;
        /****** プロパティ *******/
        /// <summary>
        /// データ長
        /// </summary>
        public int Length { get { return this.data.Length; } }
        /// <summary>
        /// 使用可能なチャンネル
        /// </summary>
        public UsableChannel UsableCH
        {
            get
            {
                int i;
                double l = 0, r = 0;

                for (i = 0; i < this.Length && i < 100; i++)  // 最大100データを検査
                {
                    l += Math.Abs((double)this.data[i].Left);
                    r += Math.Abs((double)this.data[i].Right);
                }
                if (l > 100 && r > 100)
                    return UsableChannel.Both;
                else if (l > 100)
                    return UsableChannel.Left;
                else if (r > 100)
                    return UsableChannel.Right;
                else
                    return UsableChannel.NA;
            }
        }
        /****** メソッド *******/
        /// <summary>
        /// インデクサを利用して、内部の要素へアクセス
        /// </summary>
        /// <param name="i">要素番号（インデックス）</param>
        /// <returns>指定した要素番号に該当する音素データ</returns>
        public MusicUnit this[int i]
        {
            get {
                if (i < 0) i = 0;
                if (i > this.Length) i = this.Length - 1;
                return this.data[i]; 
            }
        }
        /// <summary>
        /// 指定された片方のデータを返す
        /// </summary>
        /// <param name="ch">Channel列挙体</param>
        /// <returns>double型の配列</returns>
        public double[] GetData(Channel ch)
        {
            int i;
            double[] ans = new double[this.Length];

            for (i = 0; i < this.Length; i++)
            {
                if (ch == Channel.Left)
                    ans[i] = data[i].Left;
                else
                    ans[i] = data[i].Right;
            }
            return ans;
        }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="_data">音声データ配列</param>
        public MusicData(MusicUnit[] _data)
        {
            this.data = new MusicUnit[_data.Length];
            for (int i = 0; i < _data.Length; i++) this.data[i] = _data[i];
            return;
        }
    }

    /// <summary>
    /// Waveファイルを操作するのに必要な基底クラス
    /// </summary>
    public abstract class WaveBase
    {
        /// <summary>
        /// waveファイルのヘッダ情報を格納する構造体
        /// </summary>
        protected struct WaveFileHeader
        {
            /// <summary>
            /// データ形式（リニアPCMなら1）
            /// </summary>
            public Int16   ID;
            /// <summary>
            /// チャネル数（モノラル/ステレオ）
            /// </summary>
            public Channel Channel;
            /// <summary>
            /// サンプリングレート[sampling/sec]
            /// </summary>
            public Int32   SamplingRate;
            /// <summary>
            /// データレート[byte/sec](チャネル数×ブロックサイズ)
            /// </summary>
            public Int32   DataRate;
            /// <summary>
            /// ブロックサイズ：1サンプル当たりのサイズ[byte/sample]<para>(分解能で決まる1データサイズ×チャネル)</para>
            /// </summary>
            public Int16   BlockSize;
            /// <summary>
            /// 分解能：1サンプル当たりのビット数[bit/sample]
            /// </summary>
            public Int16   ResolutionBit;
            /// <summary>
            /// 波形データサイズ[byte]
            /// </summary>
            public UInt32  WaveDataSize;
            /*****　メソッド　*****/
            /// <summary>初期化する</summary>
            public void Init()
            {
                this.ID = 0;
                this.Channel = Channel.Stereo;
                this.SamplingRate = 0;
                this.DataRate = 0;
                this.BlockSize = 0;
                this.ResolutionBit = 0;
                this.WaveDataSize = 0;
                return;
            }
        }
    }
}
