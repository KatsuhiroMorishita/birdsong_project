/*************************************************************************************************************************************
 * DFT.cs
 * FFT/IFFTクラス及び、処理結果を操作するクラス
 *  開発者：Katsuhiro Morishita(2010.5) Kumamoto-University
 * 
 * [名前空間]   Signal.FrequencyAnlysis
 * [クラス名]   FFTresult, DFT
 * 
 * 
 * [使用方法]   まず、プロジェクトに本ファイルを追加した後に、ソースヘッダに以下のコードを追加して下さい。
 *              using SignalProcess;
 * 
 * [参考文献]   ニューメリカル・レシピ・イン・シー
 *              用語はこの書籍に準ずる。
 * 
 * [更新情報]   2010/5/23   森下整備開始 （関係ないけど、初めてのC#だったりする）
 *              2011/2/18   名前空間を名づけた
 *                          今後：渡されたデータ幅がFFTデータ幅を超えた場合でも、可能なだけ結果を返すようにしたい。
 *              2011/2/26   コンストラクタでのエラー対策と、コメントを入れる。
 *                          拡張予定：　帯域指定でのパワー計算値を返すメソッド（引数にサンプリング周波数情報が必須）
 *                          窓関数に三角窓も追加予定
 *              2011/9/30   コメントの見直しを実施
 *              2011/11/15  コメントの見直しを更に実施
 *                          関数の一部整理
 *                          サンプリング周波数に関してメソッドを追加
 *                          FFT後のデータを取り扱いやすくするため、FFTresultクラスを整備
 *                          FFTdata構造体を少し改良
 *              2011/12/6   Windowing()メソッドで、矩形窓を指定した場合に全部0になるバグがあったのを修正
 *                          名前空間をSignal_ProcessからSignalProcessに変更
 *                          Windowingは他の処理でも使うことがあるので、同じ名前空間にWindowクラスとして独立させた。
 *                          それに伴い、ファイルを分割したので注意のこと。
 *                          Band構造体のコンストラクタ生成時に、引数の異常を検出して例外をスローするように変更
 *                          窓関数付の帯域パワー取得メソッド GetPSD(Band band, Window.WindowKind windowKind) を新設
 *              2011/12/8   コメントの一部見直し
 *                          IFFTをするポテンシャルはあるのに、実施するためのメソッドが用意されていないことに気が付いた。必要があれば作る予定。
 *                          FFTdataに位相情報を追加
 *              2012/1/17   FFTウィンドウ幅を最小32になるように調整した。
 *                          以前は条件によるものの、最小で2とされエラーがスローされることがあった。。。
 *                          帯域を指定してパワーを得られる、GetPowerArray(Band band)とGetFilterdResult(Band band)を新設。
 *              2012/1/18   周波数の取り扱いをdoubleに変更
 *              2012/5/4    名前空間の変更とファイルの分割を実施
 * *********************************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/******************************************
 * 信号処理を行うクラスを集めた名前空間です。
 * ****************************************/
namespace Signal.FrequencyAnalysis
{
    /// <summary>
    /// <para>FFT若しくはIFFTを実施するためのクラスです。</para>
    /// </summary>
    /// <example>
    /// DFTクラスの使用例です。
    /// <code>
    /// DFT fft = new DFT[4096];
    /// 
    /// FFTresult result = fft_arr[i].FFT(sound, 44100); // soundはdouble型又はFFTdata型の一次元配列で、処理したいデータが入っている
    /// </code>
    /// </example>
    public class DFT
    {
        /***********************　列挙体の宣言  *******************************/
        /// <summary>
        /// FFT / IFFT
        /// </summary>
        private enum FFTorIFFT : int { 
            /// <summary>
            /// フーリエ変換
            /// </summary>
            FFTconvert = 1,
            /// <summary>
            /// 逆フーリエ変換
            /// </summary>
            IFFTconvert = -1
        }
        /***********************　グローバル変数の宣言  *******************************/
        /// <summary>FFTデータ幅</summary>
        private int __FFTpoint;
        /// <summary>使用する窓の種類</summary>
        private Window.WindowKind __WindowKind;
        /// <summary>計算後の配列</summary>
        private FFTdata[] calced_datas;
        /// <summary>外部より渡されたデータを格納する配列</summary>
        private FFTdata[] set_datas;
        /// <summary>データのサンプリング周波数</summary>
        private double frequencyOfSample = 0.0;
        /****************************　プロパティ　************************************/
        /// <summary>
        /// 窓関数の種別
        /// </summary>
        public Window.WindowKind WindowKind
        {
            get { return this.__WindowKind; }
            set { this.__WindowKind = value; }
        }
        /// <summary>
        /// FFT/IFFTデータ幅
        /// </summary>
        public int FFTsize { get { return this.__FFTpoint; } }
        /// <summary>
        ///  変換後の配列に含まれる最大値を返す
        /// </summary>
        public double Max
        {
            get
            {
                double max = 0;
                int i;

                for (i = 0; i < this.calced_datas.Length; i++) {
                    if (max < this.calced_datas[i].Abs) max = this.calced_datas[i].Abs;
                }
                return max;
            }
        }
        /// <summary>
        /// 変換後の配列に含まれる平均値を返す
        /// </summary>
        public double Mean
        {
            get
            {
                double mean = 0;
                int i;

                for (i = 0; i < this.calced_datas.Length; i++)
                {
                    mean += this.calced_datas[i].Abs;
                }
                return mean / this.calced_datas.Length;
            }
        }

        /****************************　メソッド　************************************/
        /// <summary>
        /// データをセットするメソッド。
        /// <para>
        /// 指定された窓関数をかけて保持する。
        /// データサイズよりFFTサイズの方が大きい場合は、0で埋める。
        /// 反対に、小さい場合は入るだけしか受け取らない。
        /// </para>
        /// </summary>
        private void Dataset(FFTdata[] arr) {
            int i;
            int size;
            double[] temp = new double[arr.Length];

            size = arr.Length;
            if (this.__FFTpoint < arr.Length) size = this.__FFTpoint;   // フィルタにかけるサイズを決定する
            // Reパート
            for (i = 0; i < arr.Length; i++) temp[i] = arr[i].Re;
            temp = Window.Windowing(temp, this.__WindowKind);           // 窓を掛ける
            for (i = 0; i < this.__FFTpoint; i++)                       // 受け渡されたデータをコピー
            {
                if( arr.Length > i )                                    // ある限りコピー
                    this.set_datas[i].Re = temp[i];
                else
                    this.set_datas[i].Re = 0.0;                         // 余ったら0を詰める
            }

            // Imパート
            for (i = 0; i < arr.Length; i++) temp[i] = arr[i].Im;
            temp = Window.Windowing(temp, this.__WindowKind);           // 窓を掛ける
            for (i = 0; i < this.__FFTpoint; i++)                       // 受け渡されたデータをコピー
            {
                if (arr.Length > i)
                    this.set_datas[i].Im = temp[i];                     // ある限りコピー
                else
                    this.set_datas[i].Im = 0.0;                         // 余ったら0を詰める
            }
            return;
        } 
        /// <summary>
        /// データをセットするメソッド2
        /// <para>
        /// 指定された窓関数をかけて保持する。
        /// データサイズよりFFTサイズの方が大きい場合は、0で埋める。
        /// </para>
        /// </summary>
        private void Dataset(double[] arr) {
            FFTdata[] copy = new FFTdata[arr.Length];

            for (int k = 0; k < arr.Length; k++)
            { 
                copy[k].Re = arr[k];
                copy[k].Im = 0.0;
            }
            this.Dataset(copy);
            return;
        }
        /// <summary>
        /// FFT/IFFTメソッド
        /// </summary>
        /// <param name="data">
        /// 被変換変数。結果が格納される。長さnnの複素配列（実部と虚部を一列に並べたもの）。
        /// 配列の中には、実部・虚部・実部・虚部・実部・虚部・実部・虚部・…と並んでおればよい。
        /// 原本では要素番号1から詰めていることを仮定している。
        /// 実部と虚部が入れ替わっても特に影響は無いはず。
        /// </param>
        /// <param name="nn">data[]に格納されたデータ長を表す。要素数の半分である。2の整数乗である必要がある。</param>
        /// <param name="isign">isign　 1：FFT変換を行う　-1：IFFT変換を行う。結果はnn倍される。</param>
        private void Calc(double[] data, long nn , int isign ){
            long n, mmax, m, j, istep, i;
            double wtemp, wr, wpr, wpi, wi, theta;              // 三角関数の漸化式用
            double tempr, tempi;

            n = nn << 1;                                        // n:データポインタ数の2倍
            j = 1;
            for (i = 1; i < n; i += 2) {                        // ビット反転アルゴリズム
                if (j > i) {                                    // 複素数を交換
                    tempr = data[j]; data[j] = data[i]; data[i] = tempr;                    // 原本では実部を表す
                    tempi = data[j + 1]; data[j + 1] = data[i + 1]; data[i + 1] = tempi;    // 原本では虚部を表す
                }
                m = n >> 1;
                while (m >= 2 && j > m) {
                    j -= m;
                    m >>= 1;
                }
                j += m;
            }
            mmax = 2;                                           // 以下はDanielson-Lanczosアルゴリズムを採用
            while (n > mmax) {                                  // 外側のループはlog2 nn回実行される
                istep = mmax << 1;
                theta = isign * (6.28318530717959 / mmax);      // 三角関数の漸化式の初期値. 2*pi = 6.28…
                wtemp = Math.Sin (0.5 * theta);
                wpr = -2.0 * wtemp * wtemp;
                wpi = Math.Sin(theta);
                wr = 1.0;
                wi = 0.0;
                for (m = 1; m < mmax; m += 2) {
                    for (i = m; i <= n; i += istep) {
                        j = i + mmax;                           // Danielson-Lanczos公式
                        tempr = wr * data[j] - wi * data[j + 1];// 原本通りならreal part
                        tempi = wr * data[j + 1] + wi * data[j];// 原本通りならimaginary part
                        data[j] = data[i] - tempr;
                        data[j + 1] = data[i + 1] - tempi;
                        data[i] += tempr;
                        data[i+1] += tempi;
                    }                                           // 三角関数の漸化式
                    wr = (wtemp = wr) * wpr - wi * wpi + wr;    // ここは処理をひっくり返せばwtempをそのままwrに置き換えられるが、恐らくrealパートから計算したいのだろう。
                    wi = wi * wpr + wtemp * wpi + wi;
                }
                mmax = istep;
            }
            return;
        }
        #region "FFTの実行インターフェイス"
        /// <summary>
        /// double型一次元配列に対して、FFTを実行する
        /// </summary>
        /// <example>
        /// <code>
        /// DFT a = new DFT();                              // このクラスを宣言して、実態も確保
        /// FFTresult result = a.FFT(temp);                 // double型配列を渡してFFT演算を実行。配列サイズがFFTデータ幅とする必要はない。
        /// </code>
        /// </example>
        /// <param name="data">double型の配列に解析したいデータを格納してください</param>
        /// <returns>FFT処理データ</returns>
        public FFTresult FFT(double[] data)
        {
            int i,k;
            double[] data_for_calc = new double[this.__FFTpoint * 2 + 1];

            this.Dataset(data);                                                     // データをセットする
            for (i = 0; i < this.__FFTpoint; i++)
            {
                k = 2 * i + 1;
                data_for_calc[k] = this.set_datas[i].Re;                            // 解析するデータを格納する
                data_for_calc[k + 1] = 0.0;                                         // こっちはimaginaryパート
            }
            this.Calc(data_for_calc, this.__FFTpoint, (int)FFTorIFFT.FFTconvert);   // FFT演算
            for (i = 0; i < this.__FFTpoint; i++)
            {
                k = 2 * i + 1;
                this.calced_datas[i].Set(data_for_calc[k], data_for_calc[k + 1]);   // 結果を格納する
            }
            return new FFTresult(this.calced_datas, this.frequencyOfSample, this.__FFTpoint);
        }
        /// <summary>
        /// double型一次元配列に対して、FFTを実行する
        /// <para>サンプリング周期をセットする機能付き（結果の整理が楽になります）</para>
        /// </summary>
        /// <param name="data">解析したいデータ配列</param>
        /// <param name="frequency">サンプリング周波数[Hz]</param>
        /// <returns>FFT処理データ</returns>
        public FFTresult FFT(double[] data, double frequency)
        {
            this.frequencyOfSample = frequency;
            return this.FFT(data);
        }
        /// <summary>
        /// FFTdata型一次元配列に対して、FFTを実行する
        /// <para>実部・虚部を合わせて解析するFFTメソッド</para>
        /// </summary>
        /// <param name="data">FFTdatas型の配列に解析したいデータを格納してください</param>
        /// <returns>FFT処理データ</returns>
        public FFTresult FFT(FFTdata[] data) 
        {
            int i, k;
            double[] data_for_calc = new double[this.__FFTpoint * 2 + 1];

            this.Dataset(data);                                                     // データをセットする
            for (i = 0; i < this.__FFTpoint; i++)
            {
                k = 2 * i + 1;
                data_for_calc[k] = this.set_datas[i].Re;                            // 解析するデータを格納する
                data_for_calc[k + 1] = this.set_datas[i].Im;                        // こっちはimaginaryパート
            }
            this.Calc(data_for_calc, this.__FFTpoint, (int)FFTorIFFT.FFTconvert);   // FFT演算
            for (i = 0; i < this.__FFTpoint; i++)
            {
                k = 2 * i + 1;
                this.calced_datas[i].Set(data_for_calc[k], data_for_calc[k + 1]);   // 結果を格納する
            }
            return new FFTresult(this.calced_datas, this.frequencyOfSample, this.__FFTpoint);
        }
        /// <summary>
        /// FFTdata型一次元配列に対して、FFTを実行する
        /// <para>実部・虚部を合わせて解析するFFT/IFFTメソッド</para>
        /// <para>サンプリング周期をセットする機能付き（結果の整理が楽になります）</para>
        /// </summary>
        /// <param name="data">解析したいデータ配列</param>
        /// <param name="frequency">サンプリング周波数[Hz]</param>
        /// <returns>FFT処理データ</returns>
        public FFTresult FFT(FFTdata[] data, double frequency)
        {
            this.frequencyOfSample = frequency;
            return this.FFT(data);
        }
        #endregion
        /****************************　初期化/削除　************************************/
        /*****************
         * このクラスのコンストラクタ
         * 
         * 初期化時に使用します。
         * ***************/
        /// <summary>
        /// FFTpointを利用したコンストラクタ
        /// <para>デフォルトでは4096ポイントFFT/IFFT，窓関数の種別としてハミングウィンドウがセットされます。</para>
        /// </summary>
        /// <param name="FFT_point">FFT/IFFTポイント数</param>
        /// <param name="window_kind">窓関数の種類</param>
        public DFT(FFTpoint FFT_point = FFTpoint.size4096, Window.WindowKind window_kind = Window.WindowKind.Hanning)
        {
            this.__FFTpoint = (int)FFT_point;
            this.__WindowKind = window_kind;

            this.calced_datas = new FFTdata[this.__FFTpoint];       // サイズに合わせて、インスタンスを生成
            this.set_datas = new FFTdata[this.__FFTpoint];
        }
        /// <summary>
        /// int型の引数を用いてFFTデータ幅を決定するコンストラクタ
        /// <para>デフォルトでは4096ポイントFFT/IFFT，窓関数の種別としてハミングウィンドウがセットされます。</para>
        /// <para>指定ポイント数が2^Nではない場合、FFT_point＜2^N かつ32以上となる最小の2^Nを採用しますので注意してください。</para>
        /// </summary>
        /// <param name="FFT_point">FFT/IFFTポイント数</param>
        /// <param name="window_kind">窓関数の種類</param>
        public DFT(int FFT_point = 4096, Window.WindowKind window_kind = Window.WindowKind.Hanning)
        {
            int FFT_point2 = 1;

            while (FFT_point2 < FFT_point || FFT_point2 < (int)FFTpoint.size32) FFT_point2 <<= 1;        // サイズ調整
            this.__FFTpoint = FFT_point2;
            this.__WindowKind = window_kind;
            
            this.calced_datas = new FFTdata[this.__FFTpoint];       // サイズに合わせて、インスタンスを生成
            this.set_datas = new FFTdata[this.__FFTpoint];
        }
    }
}
