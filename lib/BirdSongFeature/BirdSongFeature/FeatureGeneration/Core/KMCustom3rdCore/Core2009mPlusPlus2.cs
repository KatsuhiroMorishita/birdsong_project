/***************************************************************************
 * [クラス名]
 *      Core20009mPlusPlus2
 * [開発者]
 *      K.Morishita (Kumamoto-Univ. @ 2012.1)
 * [履歴]
 *      2012/6/16   Core20009mPlusPlusのバージョン違いとして新設した。
 *                  帯域パワーと変調スペクトルを求めるコードを関数として分解したほうが良いかもしれない。
 * *************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PatternRecognition;
using Signal.FrequencyAnalysis;
using System.Threading;
using System.Threading.Tasks;
using BirdSongFeature.FeatureGeneration.Core;

namespace BirdSongFeature.FeatureGeneration.Core.KMCustom3rdCore
{
    /// <summary>
    /// Core20009mPlusPlusを継承したベクトル生成器のコアユニット
    /// <para>
    /// Core20009mPlusPlusクラスとの違いは、変調スペクトルの分解能を0.1 Hzとした点です。
    /// それに直流分も含めています。
    /// </para>
    /// </summary>
    public class Core2009mPlusPlus2 : KMCustom2ndCore.Core2009mPlusPlus
    {
        // ---- メソッド ---------------------------------------
        /// <summary>
        /// 特徴ベクトルを返す
        /// </summary>
        /// <returns>特徴ベクトル</returns>
        public override PatternRecognition.Feature GetFeature()
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
                var fea1 = new PatternRecognition.Feature(this._sumOfPSDbyBand);
                fea1.Normalizing();
                
                // 変調スペクトルをもとめる
                double[] modulatedSongAtUniBand = new double[this._timeSeriesPSD.Count];
                for (int i = 0; i < modulatedSongAtUniBand.Length; i++) modulatedSongAtUniBand[i] = this._timeSeriesPSD[i][indexOfMax];// 最大であった帯域のパワーを取得する（変調されたパワー）
                DFT fft = new DFT(modulatedSongAtUniBand.Length, Window.WindowKind.Hanning);                // FFTを実施する準備
                FFTresult modulationSpectrum = fft.FFT(modulatedSongAtUniBand, this._condition.FrequencyOfFFT);
                double[] spectrum = new double[this._condition.MaxModulationSpectrumFrequency * 10];
                //for (int i = 0; i < spectrum.Length; i++)
                Parallel.For(0, spectrum.Length, i =>
                {
                    double minFrequency = (double)i * 0.1;
                    double maxFrequency = minFrequency + 0.1;
                    spectrum[i] = modulationSpectrum.GetPSD(minFrequency, maxFrequency);                    // 帯域毎にコピー
                });
                // コピーした変調スペクトルを正規化
                var fea2 = new PatternRecognition.Feature(spectrum);
                fea2.Normalizing();

                // 特徴ベクトルとしてまとめる
                var ans = new PatternRecognition.Feature(fea1.ToArray());
                ans.Add(fea2.ToArray());
                ans.Normalizing();
                
                return ans;
            }
            else
                return null;
        }
        /// <summary>
        /// 引数なしのコンストラクタ
        /// <para>後ほど、Setupメソッドを呼び出して演算条件をセットしてください。</para>
        /// </summary>
        public Core2009mPlusPlus2()
            : base()
        {

        }
    }
}
