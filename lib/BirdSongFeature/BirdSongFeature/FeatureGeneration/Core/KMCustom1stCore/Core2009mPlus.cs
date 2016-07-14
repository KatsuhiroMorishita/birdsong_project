/***************************************************************************
 * [ソフトウェア名]
 *      Core20009mPlus.cs
 * [開発者]
 *      K.Morishita (Kumamoto-Univ. @ 2012.1)
 * [概要]
 *      鳥の鳴き声を用いて種の識別を実施するための特徴ベクトル生成器のコアです。
 *      新しい特徴ベクトルを試したいときは、このファイルを下地にして設計してください。
 * [参考資料]
 *      
 * [検討課題]
 *      後輩には全く新しい特徴ベクトル生成器を作って欲しい
 * [履歴]
 *      2012/1/17   三田研が伝統的に使用しているアルゴリズムに近いものを再現してみた。
 *      2012/3/10   コメントの見直し
 *                  IFeatureGeneratorCoreUnitを実装した。
 *      2012/5/4    FFT関連クラスの名前空間の変更に伴いコードを一部変更した。
 *      2012/6/16   Core20009mPlusへクラス名を変更した。
 *                  Condition類を、Core20009mPlusが内包する状態からCore20009mPlusと同じ名前空間レベルへ移した。
 * *************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PatternRecognition;
using Signal.FrequencyAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace BirdSongFeature.FeatureGeneration.Core.KMCustom1stCore
{
    /// <summary>
    /// ベクトル生成器のコアユニット
    /// <para>三田研が伝統的に使用してきたモジュールとほぼ同じ仕様にしています。</para>
    /// <para>性能改善のために特徴ベクトルの生成器を変えたい場合は、クラスを新規設計してください。</para>
    /// </summary>
    public class Core2009mPlus
    {
        // ---- メンバ変数 ---------------------------------------
        /// <summary>
        /// 帯域数
        /// <para>
        /// いじると帯域のカバー領域が増減します。
        /// 帯域数は16の場合に8kHzまでをカバーするように3角窓が構成されます。
        /// 減らすと広帯域をカットする方向に働き、特徴量が減ります。
        /// </para>
        /// </summary>
        protected int _bandNum;
        /// <summary>
        /// 帯域パワーの時系列データ
        /// </summary>
        protected List<double[]> _timeSeriesPSD = new List<double[]>(0);
        /// <summary>
        /// 帯域毎の周波数帯域パワーを格納する
        /// </summary>
        protected double[] _sumOfPSDbyBand;
        /// <summary>
        /// フィルタに使用する帯域情報
        /// </summary>
        protected Band[] _band;
        /// <summary>
        /// SN比
        /// </summary>
        protected double _SNratio;
        /// <summary>
        /// 計算条件
        /// </summary>
        protected Condition _condition;
        // ---- プロパティ ---------------------------------------
        /// <summary>
        /// 特徴ベクトルの出力用意
        /// <para>準備が整っていればture</para>
        /// <para>データストックがコンストラクタで指定したminTimeWidth以上溜まればtrueとしています。</para>
        /// <para>演算条件が入力されていない場合もfalseになりますので、ご確認ください。</para>
        /// </summary>
        public Boolean Ready
        {
            get
            {
                if (this.IsSetuped)
                {
                    if ((double)this._timeSeriesPSD.Count / this._condition.FrequencyOfFFT > this._condition.MinTimeWidth)
                        return true;
                }
                return false;
            }
        }
        /// <summary>
        /// ストックされているデータの時間幅
        /// <para>演算条件がセットされていない場合はNaNを返します。</para>
        /// </summary>
        public double TimeOfStock
        {
            get
            {
                if (this.IsSetuped)
                    return (double)this._timeSeriesPSD.Count / this._condition.FrequencyOfFFT;
                else
                    return double.NaN;
            }
        }
        /// <summary>
        /// エラー状況を示す
        /// <para>本来は、明らかにおかしい場合にtrueとする機能を実装する必要がある。</para>
        /// </summary>
        public Boolean Error
        {
            get { return false; }
        }
        /// <summary>
        /// SN比
        /// <para>定義はプログラムコードを読んで解読してください。</para>
        /// </summary>
        public double SN
        {
            get { return this._SNratio; }
        }
        /// <summary>
        /// セットアップ済みかどうかを表す
        /// <para>true: セットアップ済み</para>
        /// </summary>
        public Boolean IsSetuped
        {
            get;
            private set;
        }
        // ---- メソッド ---------------------------------------
        /// <summary>
        /// 特徴ベクトルを返す
        /// </summary>
        /// <returns>特徴ベクトル</returns>
        public virtual PatternRecognition.Feature GetFeature()
        {
            if (this.Ready)
            {
                // 最大のパワーとなった帯域を探すとその最大値を求める
                double maxPSDofBand = this._sumOfPSDbyBand.Max();                                           // 最大値を取得
                int indexOfMax = -1;                                                                        // -1としておくことで、↓2行のアルゴリズムにより最大となる帯域のインデックス-1を得る
                for (int i = 0; i < this._sumOfPSDbyBand.Length && this._sumOfPSDbyBand[i] != maxPSDofBand; i++) indexOfMax = i;// 最大となった帯域のインデックス-1を取得する
                indexOfMax++;                                                                               // 等しいと代入の前にループを抜けるので、抜けた後で加算してインデックスとする
                // SN比を求める。求まればよいが。
                double noiseLevelPSD = this._timeSeriesPSD[0][indexOfMax];                                  // フレームの初めはノイズレベルに近いはず
                double maxPSDofFlame = double.MinValue;
                for (int i = 0; i < this._timeSeriesPSD.Count; i++) if (maxPSDofFlame < this._timeSeriesPSD[i][indexOfMax]) maxPSDofFlame = this._timeSeriesPSD[i][indexOfMax];
                this._SNratio = 10.0 * Math.Log10(maxPSDofFlame / noiseLevelPSD);
                // 帯域パワーの最大値が1になるように正規化
                if (maxPSDofBand != 0.0) for (int i = 0; i < this._sumOfPSDbyBand.Length; i++) this._sumOfPSDbyBand[i] /= maxPSDofBand;
                
                // 変調スペクトルをもとめる
                double[] modulatedSongAtUniBand = new double[this._timeSeriesPSD.Count];
                for (int i = 0; i < modulatedSongAtUniBand.Length; i++) modulatedSongAtUniBand[i] = this._timeSeriesPSD[i][indexOfMax];// 最大であった帯域のパワーを取得する（変調されたパワー）
                DFT fft = new DFT(modulatedSongAtUniBand.Length, Window.WindowKind.Hanning);                // FFTを実施する準備
                FFTresult modulationSpectrum = fft.FFT(modulatedSongAtUniBand, this._condition.FrequencyOfFFT);
                double[] spectrum = new double[this._condition.MaxModulationSpectrumFrequency];
                //for (int i = 0; i < spectrum.Length; i++)
                Parallel.For(0, spectrum.Length, i =>
                {
                    double minFrequency = (double)i;
                    double maxFrequency = minFrequency + 1.0;
                    if (minFrequency == 0.0) minFrequency = 0.3;                                            // 直流に近い部分は除く
                    spectrum[i] = modulationSpectrum.GetPSD(minFrequency, maxFrequency);                    // 1 Hz帯域毎にコピー
                });               
                // コピーした変調スペクトルを正規化
                double maxSpectrum = spectrum.Max();
                if (spectrum.Length != 0 && maxSpectrum != 0.0)
                {
                    for (int i = 0; i < spectrum.Length; i++) spectrum[i] /= maxSpectrum;                   // 変調スペクトルを正規化
                }

                // 特徴ベクトルとしてまとめる
                var ans = new PatternRecognition.Feature(this._sumOfPSDbyBand);
                ans.Add(spectrum);
                
                return ans;
            }
            else
                return null;
        }
        /// <summary>
        /// バッファをクリア
        /// <para>未検出データを常に先頭に格納する必要がありますので、データを引き渡してください。</para>
        /// <param name="data">FFT済みのデータ</param>
        /// </summary>
        public void Init(FFTresult data)
        {
            for (int i = 0; i < this._sumOfPSDbyBand.Length; i++) this._sumOfPSDbyBand[i] = 0.0; // 帯域パワー積算値を初期化
            this._timeSeriesPSD.Clear();
            this.Add(data);                                                             // データを投げて処理・登録させる
            this._SNratio = double.NaN;
            return;
        }
        /// <summary>
        /// FFT後のデータを入れてください
        /// </summary>
        /// <param name="data">FFT済みのデータ</param>
        public void Add(FFTresult data)
        {
            double[] psdByBand = new double[_bandNum];                                  // 帯域毎の周波数帯域パワーを格納する

            //for (int i = 0; i < psdByBand.Length; i++)                                // 帯域パワーの計算と、積算を同時に実施する
            Parallel.For(0, psdByBand.Length, i =>
            {
                psdByBand[i] = data.GetPSD(this._band[i], Window.WindowKind.Triangle);  // 窓付きで帯域パワーを計算
                this._sumOfPSDbyBand[i] += psdByBand[i];                                // 帯域パワーを積算する
            });
            this._timeSeriesPSD.Add(psdByBand);
            return;
        }
        /// <summary>
        /// 演算条件をセットアップします
        /// <para>演算条件セットの後に本クラスを利用可能になります。</para>
        /// </summary>
        public void Setup(Object condition) 
        {
            if (condition is Condition)
            {
                this._condition = (Condition)condition;
                this.IsSetuped = true;
            }
            else 
            {
                throw new SystemException("認識できないオブジェクトを演算条件として引き渡されています。オブジェクトの型を確認してください。");
            }
            return;
        }
        /// <summary>
        /// 引数なしのコンストラクタ
        /// <para>後ほど、Setupメソッドを呼び出して演算条件をセットしてください。</para>
        /// </summary>
        public Core2009mPlus()
        {
            this._bandNum = 16;
            this._sumOfPSDbyBand = new double[this._bandNum];
            this._band = new Band[this._bandNum];

            this._timeSeriesPSD.Clear();
            this.IsSetuped = false;
            for (int i = 0; i < this._band.Length; i++)                                     // 帯域構造体を初期化
            {
                int f_low = (int)(15.275 * (double)(i * i) + 194.04 * (double)i + 33.8);    // 三田研が伝統的に使用している非対称三角形の帯域をおおむね模倣するように調整したパラメータを使う
                int f_high = (int)(17.901 * (double)(i * i) + 224.5 * (double)i + 538);
                this._band[i] = new Band(f_high, f_low);
            }
            this._SNratio = double.NaN;
        }
    }
}
