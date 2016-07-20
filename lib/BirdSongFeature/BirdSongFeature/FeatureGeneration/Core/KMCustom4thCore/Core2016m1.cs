using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BirdSongFeature.FeatureGeneration.Core.KMCustom4thCore
{
    class Core2016m1 : KMCustom1stCore.Core2009mPlus
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

                // パワーの2次元データを1次元化
                int width = 16;
                double[] feature_vector = new double[this._timeSeriesPSD[0].Length * width];
                for (int i = 0; i < feature_vector.Length; i++) feature_vector[i] = 0.0 ;
                for (int i = 0; i < this._timeSeriesPSD.Count; i++)
                {
                    for (int j = 0; j < this._timeSeriesPSD[0].Length; j++)
                    {
                        int k = (int)((double)i / (this._timeSeriesPSD.Count + 0.1) * (double)width);
                        feature_vector[j * this._timeSeriesPSD[0].Length + k] += this._timeSeriesPSD[i][j];
                    }
                }

                // 最大値が1になるように正規化
                var ans = new PatternRecognition.Feature(feature_vector);
                ans.Normalizing();

                return ans;
            }
            else
                return null;
        }
        public Core2016m1():base()
        {

        }
    }
}
