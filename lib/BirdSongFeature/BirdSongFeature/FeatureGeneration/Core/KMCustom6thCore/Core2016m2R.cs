using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Signal.FrequencyAnalysis;
using System.Threading.Tasks;

namespace BirdSongFeature.FeatureGeneration.Core.KMCustom6thCore
{
    /// <summary>
    /// 帯域ごとに変調スペクトルを求め、結合したベクトルを正規化する。
    /// </summary>
    class Core2016m2R : KMCustom1stCore.Core2009mPlus
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
                double maxPSDofBand = this._sumOfPSDbyBand.Max();     // 最大値を取得
                int indexOfMax = -1;                                  // -1としておくことで、↓2行のアルゴリズムにより最大となる帯域のインデックス-1を得る
                for (int i = 0; i < this._sumOfPSDbyBand.Length && this._sumOfPSDbyBand[i] != maxPSDofBand; i++) indexOfMax = i;// 最大となった帯域のインデックス-1を取得する
                indexOfMax++;                                         // 等しいと代入の前にループを抜けるので、抜けた後で加算してインデックスとする
                
                // SN比を求める。求まればよいが。
                double noiseLevelPSD = this._timeSeriesPSD[0][indexOfMax];    // フレームの初めはノイズレベルに近いはず
                double maxPSDofFlame = double.MinValue;
                for (int i = 0; i < this._timeSeriesPSD.Count; i++) if (maxPSDofFlame < this._timeSeriesPSD[i][indexOfMax]) maxPSDofFlame = this._timeSeriesPSD[i][indexOfMax];
                this._SNratio = 10.0 * Math.Log10(maxPSDofFlame / noiseLevelPSD);

                // 全帯域で変調スペクトルを求める
                List<double> feature_vector = new List<double>();
                for (int band_id = 0; band_id < this._sumOfPSDbyBand.Length; band_id++)
                {
                    double[] modulatedSongAtUniBand = new double[this._timeSeriesPSD.Count];
                    for (int i = 0; i < modulatedSongAtUniBand.Length; i++) modulatedSongAtUniBand[i] = this._timeSeriesPSD[i][band_id];// band_id帯域の時系列のパワーを取得する（変調されたパワー）
                    DFT fft = new DFT(modulatedSongAtUniBand.Length, Window.WindowKind.Hanning);                // FFTを実施する準備
                    FFTresult modulationSpectrum = fft.FFT(modulatedSongAtUniBand, this._condition.FrequencyOfFFT);
                    double[] spectrum = new double[(int)(this._condition.MaxModulationSpectrumFrequency / 2)];
                    //for (int i = 0; i < spectrum.Length; i++)
                    Parallel.For(0, spectrum.Length, i =>
                    {
                        double minFrequency = (double)i * 2;
                        double maxFrequency = minFrequency + 2.0;
                        if (minFrequency == 0.0) minFrequency = 0.3;                                            // 直流に近い部分は除く
                        spectrum[i] = modulationSpectrum.GetPSD(minFrequency, maxFrequency);                    // 2 Hz帯域毎にコピー
                    });
                    // 変調スペクトルを格納
                    foreach (var val in spectrum) feature_vector.Add(val);
                }
                // 正規化
                var vector = feature_vector.ToArray();
                double maxSpectrum = vector.Max();
                if (vector.Length != 0 && maxSpectrum != 0.0)
                {
                    for (int i = 0; i < vector.Length; i++) vector[i] /= maxSpectrum;
                }

                // 呼び出し元に返す
                var ans = new PatternRecognition.Feature(vector);
                return ans;
            }
            else
                return null;
        }
        public Core2016m2R():base()
        {

        }
    }
}
