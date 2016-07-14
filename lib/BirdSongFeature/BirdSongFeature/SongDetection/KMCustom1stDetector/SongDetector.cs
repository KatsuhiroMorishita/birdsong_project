/****************************************************
 * SongDetector
 * ノイズフロアから飛び出た発声信号を検出するためのクラス
 * 複数の帯域を一度に監視可能とするために、SubDetectorクラスを使役します。
 * どの帯域で立ち上がるか不明なピークを検出・追跡することを念頭に設計しています。
 * 
 * [history]
 *      2012/5/21   引数なしのコンストラクタを設置し、それ以外のコンストラクタを削除した。
 *                  演算条件の設定はSetup()により行う。
 *      2013/2/2    コメントを修正
 *      2013/2/6    バグを訂正した。
 * **************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Signal.FrequencyAnalysis;

namespace BirdSongFeature.SongDetection.KMCustom1stDetector
{
    /// <summary>
    /// ノイズフロアから飛び出た発声信号を検出するためのクラス
    /// </summary>
    public class SongDetector
    {
        /******************* メンバ変数 *************************************/
        /// <summary>
        /// 帯域の分解数
        /// </summary>
        private int _numberOfPartitions;
        // <summary>正規化された時定数：</summary>
        //private int _timeConstant;
        /// <summary>
        /// 平均数：後進平均
        /// </summary>
        private int _numOfStock;
        /// <summary>
        /// 発声検出器
        /// </summary>
        private SubDetector[] sensor;
        /// <summary>
        /// 発声検出器の担当帯域
        /// </summary>
        private Signal.FrequencyAnalysis.Band[] band;
        /// <summary>
        /// 検出結果
        /// <para>検出されるとtrue</para>
        /// </summary>
        private Boolean _detection = false;
        /// <summary>
        /// 検出されたエッジ
        /// </summary>
        private Edge _edge = Edge.NA;
        /******************* プロパティ *************************************/
        /// <summary>
        /// 検出結果
        /// <para>鳥の声を検出するとtrue</para>
        /// </summary>
        public Boolean Detection 
        {
            get { return this._detection; } 
        }
        /// <summary>
        /// 検出されたエッジの種類
        /// <para>多数の検出器があるのに、どうやって知らせたらいいのか？</para>
        /// </summary>
        public Edge Edge 
        {
            get { return this._edge; } 
        }
        /// <summary>
        /// PSDの最低値検出窓の時間幅を設定する
        /// <para>
        /// 2011/11/18現在、未完成
        /// sensorオブジェクトを全て呼び出して設定する予定
        /// </para>
        /// </summary>
        public TimeSpan SetTimeSpan4minLeve 
        { 
            set { } 
        }
        /// <summary>
        /// セットアップ状況
        /// <para>true: セットアップ済み</para>
        /// </summary>
        public Boolean IsSetuped
        {
            get;
            private set;
        }
        /******************* メソッド *************************************/
        /// <summary>
        /// FFT処理後のデータをセットする
        /// </summary>
        /// <param name="data">FFT処理後のデータ</param>
        /// <exception cref="SystemException">演算条件がセットされていない場合にスロー</exception>
        public void Set(FFTresult data)
        {
            Boolean detection = false;

            if (this.IsSetuped == false) throw new SystemException("演算条件がセットされていません。Setup()を行ってください。");
            for (int i = 0; i < this.sensor.Length; i++)
            {
                //double psd = Math.Log10(data.GetPSD(this.band[i]));               // FFTの結果から、パワー密度を取得
                double psd = data.GetPSD(this.band[i]);                             // FFTの結果から、パワー密度を取得
                this.sensor[i].Set(psd);                                            // 検出器に掛ける
                if(this.sensor[i].FrontDetection)detection = true;                  // 検出結果をチェック（一つでも検出していたらture）
                this._edge = this.sensor[i].DetectedEdge;
            }
            this._detection = detection;                                            // 結果をバックアップ
            return;
        }
        /// <summary>
        /// 指定されたインデックスの帯域における状況をファイルに出力する
        /// <para>帯域とインデックスを結び付けるメソッドを用意しないと意味があまりないかも</para>
        /// </summary>
        /// <param name="fname">ファイル名</param>
        /// <param name="index">インデックス番号</param>
        /// <exception cref="SystemException">演算条件がセットされていない場合にスロー</exception>
        public void Save(string fname, int index = 0)
        {
            if (this.IsSetuped == false) throw new SystemException("演算条件がセットされていません。Setup()を行ってください。");
            if (index >= 0 && index < this.sensor.Length)
                this.sensor[index].Save(fname);
            else
                throw new System.FormatException("インデックスの値が不正です");     // 引数に不正があれば例外をスロー
            return;
        }
        /// <summary>
        /// 本クラスが内部に保持している帯域のリストを返す
        /// </summary>
        /// <exception cref="SystemException">演算条件がセットされていない場合にスロー</exception>
        /// <returns>帯域のリスト</returns>
        public Band[] GetBands()
        {
            if (this.IsSetuped == false) throw new SystemException("演算条件がセットされていません。Setup()を行ってください。");
            Signal.FrequencyAnalysis.Band[] bands = new Signal.FrequencyAnalysis.Band[this.band.Length];
            for (int i = 0; i < this.band.Length; i++ )                             // クローンを作って返す（Bandを構造体で設計しているのでこれでもよい）
                bands[i] = this.band[i];
            return bands;   
        }
        /// <summary>
        /// 発声を検出された帯域情報を返す
        /// </summary>
        /// <exception cref="SystemException">演算条件がセットされていない場合にスロー</exception>
        /// <returns>帯域情報</returns>
        public Band[] GetDetectedBands()
        {
            if (this.IsSetuped == false) throw new SystemException("演算条件がセットされていません。Setup()を行ってください。");
            int i = 0;
            for (int k = 0; k < this.sensor.Length; k++) if (this.sensor[k].FrontDetection == true) i++;    // 信号が検出された帯域数をカウント
            Signal.FrequencyAnalysis.Band[] bands = new Signal.FrequencyAnalysis.Band[i];                   // 信号が検出された数だけ帯域を示すクラスを生成
            i = 0;
            for (int k = 0; k < this.sensor.Length; k++)
            {
                if (this.sensor[k].FrontDetection == true)
                {
                    bands[i] = this.band[k];
                    i++;
                }
            }
            return bands;
        }
        /// <summary>
        /// 任意の帯域において発声が検出されているかどうかを返す
        /// 検査対象外の帯域だったり、検出されていなければfalseが返ります。
        /// </summary>
        /// <param name="band">指定帯域</param>
        /// <exception cref="SystemException">演算条件がセットされていない場合にスロー</exception>
        /// <returns>検出されていればtrue</returns>
        public Boolean CheckDetection(Signal.FrequencyAnalysis.Band band)
        {
            if (this.IsSetuped == false) throw new SystemException("演算条件がセットされていません。Setup()を行ってください。");
            Boolean ans = false;
            for (int i = 0; i < this.band.Length; i++)                                                      // 本クラス内に用意されている検査器を全てチェック
            {
                if (this.band[i].CenterFrequency > band.Min && this.band[i].CenterFrequency > band.Max && this.sensor[i].FrontDetection == true)
                {
                    ans = true;
                    break;
                }
            }
            return ans;
        }
        /// <summary>
        /// 強制的に、指定帯域で発声があることを認識させる
        /// 2011/11/18 呼び出し先が未実装・・・
        /// </summary>
        /// <param name="band">指定帯域</param>
        /// <exception cref="SystemException">演算条件がセットされていない場合にスロー</exception>
        public void RecognizeSignal(Signal.FrequencyAnalysis.Band band)
        {
            if (this.IsSetuped == false) throw new SystemException("演算条件がセットされていません。Setup()を行ってください。");
            for (int i = 0; i < this.band.Length; i++)                                                      // 本クラス内に用意されている検査器を全てチェック
            {
                if (this.band[i].CenterFrequency > band.Min && this.band[i].CenterFrequency > band.Max)
                {
                    this.sensor[i].RecognizeSignal();                                                       // 検査器に発声を認識させる
                }
            }
            return;
        }
        /// <summary>
        /// 演算条件をセットします
        /// </summary>
        /// <param name="numberOfPartitions">
        /// 用意する帯域数（分割数）
        /// <para>1以上を指定してください。</para>
        /// <para>分割数が多ければより狭い帯域の変化を監視することができます。（が、処理時間もかかります。）</para>
        /// </param>
        /// <param name="band">検査対象とする周波数帯域</param>
        /// <param name="timespan">
        /// 検査に利用する最小の時間幅
        /// <para>時間幅が大きいと追随性が悪くなりますが、短すぎると連続して長時間囀る鳥のノイズフロアをうまく捉えることが難しいこともあります。</para>
        /// <para>標準偏差を計算する関係上、最小 26 / samplingFreaquency [s]に置換されます。</para>
        /// <para>3秒を推奨します。</para>
        /// </param>
        /// <param name="samplingFreaquency">サンプリング周期<para>FFTを実施している周期を格納してください。</para></param>
        public void Setup(int numberOfPartitions, Band band, TimeSpan timespan, double samplingFreaquency)
        {
            if (numberOfPartitions <= 0) throw new Exception("SongDetectorクラスのSetupメソッドにおいてエラーがスローされました。帯域数は1以上をセットして下さい。");
            this._numOfStock = (int)(timespan.TotalSeconds * samplingFreaquency);               // 用意すべきストック数を計算
            if (this._numOfStock < 26) this._numOfStock = 26;
            if (numberOfPartitions > 0 && band.Max > 0 && band.Min >= 0 && band.Max > band.Min)
            {
                this._numberOfPartitions = numberOfPartitions;
                this.sensor = new SubDetector[numberOfPartitions];                              // 配列の大きさを定義
                this.band = new Signal.FrequencyAnalysis.Band[numberOfPartitions];              // 帯域の数を調整
                double delta_freq = (band.Max - band.Min) / (double)numberOfPartitions;         // 一つの帯域あたりの周波数幅を計算
                double lower_freq = band.Min;                                                   // 帯域の下限周波数
                // 指定された帯域数だけ、検出器を構成する
                for (int i = 0; i < this.sensor.Length; i++)
                {
                    this.sensor[i] = new SubDetector(this._numOfStock, samplingFreaquency, new TimeSpan(0, 0, 10)); // インスタンス生成
                    double upper_freq = delta_freq * (i + 1) + lower_freq;                      // 帯域の上限を計算
                    this.band[i] = new Band(upper_freq, lower_freq);
                    lower_freq = upper_freq;                                                    // 下限周波数を更新
                }
            }
            else
            {
                Console.WriteLine("SongDetectorクラスのSetup()にてエラーがスローされました。\n引数に不正が確認されています。");
                throw new System.FormatException("引数に不正が確認されました。");               // 引数に不正があれば例外をスロー
            }
            this.IsSetuped = true;
            return;
        }
        /// <summary>
        /// 初期化なしのコンストラクタ
        /// <para>インスタンスの確保後に、Setup()メソッドを呼び出して演算条件を設定してください。</para>
        /// </summary>
        public SongDetector()
        { 
            
        }
    }
}
