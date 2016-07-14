/***************************************************************************
 * [クラス名]
 *      Core20009mPlusPlus.cs
 * [開発者]
 *      K.Morishita (Kumamoto-Univ. @ 2012.1)
 * [概要]
 *      鳥の鳴き声を用いて種の識別を実施するための特徴ベクトル生成器のコアです。
 * [参考資料]
 *      
 * [検討課題]
 *      後輩には全く新しい特徴ベクトル生成器を作って欲しい
 * [履歴]
 *      2012/6/16   Core20009mPlusのバージョン違いとして整備
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

namespace BirdSongFeature.FeatureGeneration.Core.KMCustom2ndCore
{
    /// <summary>
    /// Core20009mPlusを継承したベクトル生成器のコアユニット
    /// <para>
    /// Core20009mPlusクラスとの違いは、特徴ベクトルの正規化の方法と、帯域数をコンストラクタのコードをいじることで可変にした点にあります。
    /// Core20009mPlusクラスでは帯域パワー・変調スペクトルをそれぞれ最大値を1としています。
    /// 本クラスではそれぞれでまずノルムを1とし、合成後にさらにノルムを1としています。
    /// コンストラクタでは帯域数を2009mで使用している16以外に設定可能としています。
    /// あまり細かくしても意味がありませんので、試す程度にとどめておいた方が無難です。
    /// </para>
    /// </summary>
    public class Core2009mPlusPlus : KMCustom1stCore.Core2009mPlus
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
        public Core2009mPlusPlus()
            : base()
        {
            // 帯域パワーをいじる場合のコードを以下に記す
            // 標準帯域数は16である。0～8 kHzを帯域数で分割する。
            // 帯域数が16より小さいときは高い周波数側の帯域数が減る。16よりも大きいときは8 kHzまでの帯域をより細かく取ろうとします。
            this._bandNum = 20;
            this._sumOfPSDbyBand = new double[this._bandNum];                               // 親クラスの設定をやり直す
            this._band = new Band[this._bandNum];

            if (this._bandNum <= 0) throw new Exception("Core20009mPlusPlusのコンストラクタにてエラーがスローされました。帯域数が不正です。");
            for (int i = 0; i < this._band.Length; i++)                                     // 帯域構造体を初期化
            {
                int f_low = (int)(15.275 * (double)(i * i) + 194.04 * (double)i + 33.8);    // 三田研が伝統的に使用している非対称三角形の帯域をおおむね模倣するように調整したパラメータを使う
                int f_high = (int)(17.901 * (double)(i * i) + 224.5 * (double)i + 538);
                if (this._bandNum <= 16)
                    this._band[i] = new Band(f_high, f_low);
                else
                    this._band[i] = new Band(f_high * 16.0 / (double)this._bandNum, f_low * 16.0 / (double)this._bandNum);
            }
        }
    }
}
