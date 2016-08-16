using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Signal.FrequencyAnalysis;
using System.Threading.Tasks;

namespace BirdSongFeature.FeatureGeneration.Core.KMCustom7thCore
{
    /// <summary>
    /// PSDの時系列データを帯域方向に結合した後にフーリエ変換する。
    /// </summary>
    class Core2016m3 : KMCustom1stCore.Core2009mPlus
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
                double maxPSDofBand = this._sumOfPSDbyBand.Max();     // get max value of sum of PSD by band.
                int indexOfMax = -1;                                  // -1としておくことで、↓2行のアルゴリズムにより最大となる帯域のインデックス-1を得る
                for (int i = 0; i < this._bandNum && this._sumOfPSDbyBand[i] != maxPSDofBand; i++) indexOfMax = i;// 最大となった帯域のインデックス-1を取得する
                indexOfMax++;                                         // 等しいと代入の前にループを抜けるので、抜けた後で加算してインデックスとする
                
                // SN比を求める。求まればよいが。
                double noiseLevelPSD = this._timeSeriesPSD[0][indexOfMax];    // フレームの初めはノイズレベルに近いはず
                double maxPSDofFlame = double.MinValue;
                for (int i = 0; i < this._timeSeriesPSD.Count; i++) if (maxPSDofFlame < this._timeSeriesPSD[i][indexOfMax]) maxPSDofFlame = this._timeSeriesPSD[i][indexOfMax];
                this._SNratio = 10.0 * Math.Log10(maxPSDofFlame / noiseLevelPSD);

                // データの並べ替え
                List<double> psd_array = new List<double>();
                for (int frame = 0; frame < this._timeSeriesPSD.Count; frame++)
                {
                    for (int band_id = 0; band_id < this._bandNum; band_id++)
                    {
                        psd_array.Add(this._timeSeriesPSD[frame][band_id]);
                    }
                }
                // zero-padding or cutting
                var _psd_array = psd_array.ToArray();
                var max_size = (int)this._condition.FrequencyOfFFT * 5 * this._bandNum;   // 5 is 5 sec.
                var add_size = max_size - this._timeSeriesPSD.Count * this._bandNum;      // number of zero padding frame.
                double[] __psd_array;
                if (add_size < 0)                                      // slice
                    __psd_array = _psd_array.Take(max_size).ToArray();
                else                                                   // zero padding
                {
                    var zero = new double[add_size];                   // add_sizeが0の時もうまくいく…と思う。
                    _psd_array = Window.Windowing(_psd_array, Window.WindowKind.Hanning); // windowning for FFT.
                    __psd_array = _psd_array.Concat(zero).ToArray();
                }
                __psd_array = Window.Windowing(__psd_array, Window.WindowKind.Hanning); // windowning for FFT.

                // FFT
                DFT fft = new DFT(__psd_array.Length, Window.WindowKind.Hanning); // FFTを実施する準備
                FFTresult spectrum = fft.FFT(__psd_array);
                var vector = spectrum.GetPowerArray();

                // 正規化
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
        public Core2016m3():base()
        {

        }
    }
}
