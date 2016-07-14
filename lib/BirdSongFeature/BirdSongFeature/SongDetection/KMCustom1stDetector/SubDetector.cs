/****************************************************
 * SubDetector
 * ノイズフロアから飛び出た発声信号を検出するためのクラス
 * 時系列のPSDを入力すると、統計的に有意な鳴き声を検出します。
 * SongDetectorとは異なり、帯域情報は保持していません。
 * 
 * [history]
 *      2012/5/21頃 新設
 *      2013/2/2    コメントを修正
 *                  利用していないコードをコメントアウト
 *                  _thresholdを6.0へ変更した。テストは未実施。
 * **************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BirdSongFeature.SongDetection.KMCustom1stDetector
{
    /// <summary>
    /// ノイズフロアから飛び出た発声信号を検出するためのサブクラス
    /// <para>
    /// 2011/11/15時点では、一度検出されるとその後は検出の感度が鈍るという副作用がある。
    /// これは検出時には値をセットされてもストックしないことで対応可能といえば可能
    /// ただし、グローバル変数にセットされた値を一時的に保持する必要がある。
    /// </para>
    /// </summary>
    internal class SubDetector
    {
        /********* メンバ変数 ************************************************************/
        /// <summary>
        /// 音声検出閾値（この値の1が1σに相当）
        /// <para>
        /// この値が大きいと、感度が下がる傾向にある。
        /// データ先頭からずっと鳴きっぱなしの音声データでは、
        /// 　「信号の標準偏差が非常に大きな音声として認識されるため、かなり大きな鳴き声でも鳴き声と判定されずに通常音声として処理されることが原因で」
        /// フィルタの学習が進まなくなるので注意してください。
        /// 感度の調整は、下のマージンの方でも若干ながら可能です。
        /// </para>
        /// </summary>
        private readonly double _threshold = 6.0;
        /// <summary>
        /// 無音区間学習用閾値
        /// <para>
        /// 雑音ではなく、「音声」として認識させるためにたった一つの閾値（_threshold）を使用すると、徐々に信号の推定標準偏差が小さくなっていって最終的に感度が高くなり過ぎてしまう。
        /// そこで、「音声」として認識されたとしても若干なら通常音声として処理する必要がある。
        /// そのためのマージンです。
        /// 
        /// この値が大きいと、やっぱり感度が下がる傾向があります。
        /// この値が大きいと、データ先頭からずっと鳴きっぱなしのデータを入力すると通常音声として処理されるデータが増えてしまい、計測したいノイズの推定標準偏差が大きくなりすぎてしまう。
        /// 結果として、いつまでたっても「音声」として認識されないという問題が生じます。
        /// 
        /// 鳴く頻度が低い鳥ならこの値を50程度と大き目にするとノイズフロアのドリフトへの応答が早くなります。
        /// しかし、
        /// 鳴く頻度が高い鳥なら、0.5程度が適正です。
        /// </para>
        /// </summary>
        private readonly double _thresholdMargin = 0.5;
        /// <summary>
        /// 検出されたエッジ
        /// </summary>
        private Edge _edge = Edge.NA;
        /// <summary>
        /// 前方検出フラグ
        /// </summary>
        private Boolean _frontDetection = false;
        /// <summary>
        /// 後方検出フラグ
        /// </summary>
        private Boolean _rearDetection = false;
        /// <summary>
        /// フィルタリングされた平均
        /// </summary>
        private double _meanLevel = 0.0;
        /// <summary>
        /// フィルタリングされた標準偏差
        /// </summary>
        private double _standardDeviation = 0.0;
        /// <summary>
        /// 閾値によってフィルタリングされない平均
        /// </summary>
        private double _noFiltMeanLevel = 0.0;
        /// <summary>
        /// 閾値によってフィルタリングされない標準偏差
        /// </summary>
        private double _noFilterStandardDeviation = 0.0;
        /// <summary>
        /// 加算数
        /// </summary>
        private double _addCounter = 0;
        /// <summary>
        /// 書き込みポインタ（といっても、言語のポインタではない）
        /// </summary>
        private int _wp = 0;
        /// <summary>
        /// 読み込みポインタ（といっても、言語のポインタではない）
        /// </summary>
        private int _rp = 0;
        /// <summary>
        /// 閾値によってフィルタリングされないデータストックのための書き込みポインタ（といっても、言語のポインタではない）
        /// </summary>
        private int _wp4noFilter = 0;
        /// <summary>
        /// 平均数：後進平均
        /// </summary>
        private int _numOfMean;
        /// <summary>
        /// データストックのためのストレージ
        /// </summary>
        private double[] _stock;
        /// <summary>
        /// 閾値によってフィルタリングされないデータストックのためのストレージ
        /// </summary>
        private double[] _stock4noFilter;
        /// <summary>
        /// PSDの最低値を検出するのに使用するバッファのサイズを決定する時間　デフォルトだと10秒
        /// </summary>
        private TimeSpan _timespan4minLevel;
        /// <summary>
        /// 設定されたサンプリング周波数に基づいて、値を追加した数だけ時間をカウントする
        /// </summary>
        private double _time = 0.0;
        /// <summary>
        /// サンプリング周波数[Hz]
        /// <para>Waveファイルから0.1秒毎に抜いたデータで計算したパワーをセットするなら、1/0.1=10Hzです。</para>
        /// </summary>
        private double _samplingFrequency = 0.0;
        // バックアップ用の変数
        private double backup_z;
        private double backup_psd = double.PositiveInfinity;
        private double bottom_psd;
        /********* プロパティ ************************************************************************/
        /// <summary>
        /// 前方音声検出フラグ
        /// 検出されると、tureとなる。
        /// </summary>
        public Boolean FrontDetection { get { return this._frontDetection; } }
        /// <summary>
        /// 後方音声検出フラグ
        /// 検出されると、tureとなる。
        /// </summary>
        public Boolean RearDetection { get { return this._rearDetection; } }
        /// <summary>
        /// 検出されたエッジの種類
        /// </summary>
        public Edge DetectedEdge { get { return this._edge; } }
        /// <summary>
        /// フィルタリングされないデータストレージの中での最小値
        /// </summary>
        private double MinLevel
        {
            get
            {
                double min = double.PositiveInfinity;
                foreach (double v in _stock4noFilter)
                    if (v < min && v != 0.0) min = v;
                return min;
            }
        }
        /********* メソッド ************************************************************************/
        /// <summary>
        /// 強制的に現状を音声が入力されていると認識させる
        /// 2011/11/18現在、未実装
        /// </summary>
        public void RecognizeSignal()
        {
            // 現状におけるデータストックから、分散を計算して、入力値が小さいとみられるデータのみで平均・分散を再計算の後に再度スクリーニングを行い、
            // データストックを再構成する。
            // これで強制的にパラメータを調整可能なはず。
            return;
        }
        /// <summary>
        /// 平均値を計算する
        /// </summary>
        /// <param name="buff">バッファ</param>
        private double CalcMean(double[] buff)
        {
            double sum = 0.0;
            double cnt = 0.0;

            foreach (double v in buff)
            {
                if (v != 0.0)
                {
                    sum += v;
                    cnt += 1.0;
                }
            }
            return sum / cnt;
        }
        /// <summary>
        /// 標準偏差を計算する
        /// </summary>
        /// <param name="buff">バッファ</param>
        /// <param name="mean">平均値</param>
        private double CalcSD(double[] buff, double mean)
        {
            double sum = 0.0;
            double cnt = 0.0;

            foreach (double v in buff)
            {
                if (v != 0.0)
                {
                    sum += Math.Pow((v - mean), 2.0);
                    cnt += 1.0;
                }

            }
            double variance = sum / (cnt - 1.0);
            return Math.Sqrt(variance);
        }
        /// <summary>
        /// 信号に音声が入っているのかを検出する
        /// 平均値は鳥が頻繁になくと信頼性がなくなるので、最小値を用いる方がずっと良い。
        /// なお、チェックは御検出を避けるために規定数以上のデータをセットされた後となる。
        /// </summary>
        /// <param name="PSD">パワースペクトル密度[V^2/Hz]</param>
        /// <returns>入力されたPSDの正規化されたレベル</returns>
        private double Check(double PSD)
        {
            //double z = (PSD - this._meanLevel) / this._standardDeviation;       // ↓の最低値を使った方が確実な気がする。応答は、初めの数秒間を除けば平均の方が早いほどだが。。
            double z = (PSD - this.MinLevel) / this._standardDeviation;       // 分かり易くするために、正規化 ファイルの先頭から鳴き続けるデータに対してはこちらの方が有効
            if (z > this._threshold)
                this._frontDetection = true;
            else
                this._frontDetection = false;
            return z;
        }
        /// <summary>
        /// 2つの統計量を比較して、2つの分布が分離するならtrueを返す
        /// </summary>
        /// <param name="mean1">分布1の平均</param>
        /// <param name="mean2">分布2の平均</param>
        /// <param name="sd1">分布1の標準偏差</param>
        /// <param name="sd2">分布2の標準偏差</param>
        /// <returns>2つの分布が分離するならtrue</returns>
        private Boolean CheckStatistic(double mean1, double mean2, double sd1, double sd2)
        {
            Boolean ans = false;
            //double u1, u2, d1, d2;
            //double u2, d2;

            //u1 = 1.0;
            //d1 = -1.0;
            //u2 = (mean2 - mean1) / sd1 + sd2 / sd1 * 1.0;
            //d2 = (mean2 - mean1) / sd1 - sd2 / sd1 * 1.0;
            //double amplitude = Math.Log10(sd2);
            // 統計的なものと、分散の比に大きなかい離が生じた場合に更新を促すように設計している
            //if (!((u1 > d2 && d1 < d2) || (u1 > u2 && d1 < u2) || (u1 > u2 && d1 < d2) || (u1 < u2 && d1 > d2)))
            //    ans = true;
            if (Math.Abs(Math.Log10(sd2 / sd1)) > 6.0)
                ans = true;
            return ans;
        }
        /// <summary>
        /// データをセットする
        /// </summary>
        /// <param name="PSD">パワースペクトル密度[V^2/Hz]</param>
        public void Set(double PSD)
        {
            // とにかくストックするバッファの処理　下の処理より先行する必要がある
            this._stock4noFilter[this._wp4noFilter] = PSD;                                              // ストレージに格納
            this._wp4noFilter = (this._wp4noFilter + 1) % this._stock4noFilter.Length;                  // ライトポイントを更新
            this._noFiltMeanLevel = this.CalcMean(this._stock4noFilter);                                // 平均値を計算させておく（標準偏差よりも先に計算すること）
            this._noFilterStandardDeviation = this.CalcSD(this._stock4noFilter, this._noFiltMeanLevel); // 標準偏差を計算
            // 分布の検定
            // 検定の結果、統計量に無視できない乖離が認められるとフィルタ値を更新する
            if (this.CheckStatistic(this._meanLevel, this._noFiltMeanLevel, this._standardDeviation, this._noFilterStandardDeviation))
            {
                this._meanLevel = this._noFiltMeanLevel;                                                // 
                this._standardDeviation = this._noFilterStandardDeviation;
            }
            // 観測データをチェックして、正規化した値を取得
            double zf = this.Check(PSD);
            this.backup_z = zf;
            // デバッグ用パラメータ取得
            if (PSD < this.backup_psd)
                this.bottom_psd = PSD;
            this.backup_psd = PSD;                                                                      // バックアップ
            // バイアスの把握のためのデータストック
            // 条件は順に、異様に大きくない入力信号であること，発散している場合，必要なデータ量が足りない場合，最低値が平均よりも高い場合
            if ((zf < this._threshold + this._thresholdMargin || zf == double.PositiveInfinity || zf == double.NegativeInfinity) || this._addCounter < this._numOfMean || this.MinLevel > this._meanLevel)  // 学習条件
            {
                this._stock[this._wp] = PSD;                                                            // ストレージに格納
                this._rp = this._wp;                                                                    // 最新データの書き込み位置を記憶しておく
                this._wp = (this._wp + 1) % this._stock.Length;                                         // ライトポイントを更新
                this._meanLevel = this.CalcMean(this._stock);                                           // 平均値を計算させておく（標準偏差よりも先に計算すること）
                this._standardDeviation = this.CalcSD(this._stock, this._meanLevel);                    // 標準偏差を計算
                this._addCounter += 1.0;                                                                // 加算数を加算
            }
            // 時間をカウントする
            this._time += (1.0 / this._samplingFrequency);
            return;
        }
        /// <summary>
        /// フィルタ状況を文字列で出力
        /// </summary>
        /// <returns>フィルタ情報</returns>
        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(400);
            sb.Append(this._time.ToString("0.00"));
            sb.Append(",").Append(this._meanLevel.ToString("e3"));
            sb.Append(",").Append(this._standardDeviation.ToString("e3"));
            sb.Append(",").Append(this._noFiltMeanLevel.ToString("e3"));
            sb.Append(",").Append(this._noFilterStandardDeviation.ToString("e3"));
            sb.Append(",").Append(this.backup_z.ToString("0.00"));
            sb.Append(",").Append(this.backup_psd.ToString("e3"));
            sb.Append(",").Append(this.MinLevel.ToString("e3"));
            sb.Append(",").Append(this.bottom_psd.ToString("e3"));
            if (this.FrontDetection)
                sb.Append(",").Append(this._threshold.ToString());              // 閾値を使うことで、zとの関係上グラフが見やすい
            else
                sb.Append(",").Append("0.001");
            sb.Append("\n");
            return sb.ToString();
        }
        /// <summary>
        /// 結果をファイルに保存する
        /// </summary>
        /// <param name="fname">ファイル名</param>
        public void Save(string fname = "hoge.csv")
        {
            try
            {
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fname, true, System.Text.Encoding.GetEncoding("shift_jis")))
                {
                    sw.Write(this.ToString());
                }
            }
            catch (SystemException e)
            {
                Console.WriteLine("SoundDetectionクラスのSaveメソッドにてエラーがスローされました。");
                Console.WriteLine(e.Message);
            }
            return;
        }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="numOfMean">平均と標準偏差等の計算に用いる数</param>
        /// <param name="samplingFrequency">サンプリング周波数[Hz]<para>Waveファイルから0.1秒毎に抜いたデータで計算したパワーをセットするなら、1/0.1=10Hzです。</para></param>
        /// <param name="timespan4minlevel">
        /// 最低値を検出するための時間幅
        /// <para>一鳴きの長さが指定値以上だと正常に判定ができません。</para>
        /// </param>
        public SubDetector(int numOfMean, double samplingFrequency, TimeSpan timespan4minlevel)
        {
            this._numOfMean = numOfMean;
            this._timespan4minLevel = timespan4minlevel;
            this._stock = new double[numOfMean];                                                            // 配列サイズを指定
            this._stock4noFilter = new double[(int)(timespan4minlevel.TotalSeconds * samplingFrequency)];   // 配列サイズを指定
            this._samplingFrequency = samplingFrequency;
        }
    }
}
