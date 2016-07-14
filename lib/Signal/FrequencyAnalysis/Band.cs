/*************************************************************************
 * Band.cs
 * 周波数の帯域を表す構造体
 *  開発者：Katsuhiro Morishita(2010.5) Kumamoto-University
 * 
 * [名前空間]   Signal.FrequencyAnlysis
 * 
 * [更新情報]   
 *              2012/5/4    DFT.csより分割した。
 *              2012/6/16   最低周波数が負値の場合に0をセットするように変更した。
 * ***********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Signal.FrequencyAnalysis
{
    /// <summary>
    /// 周波数帯域を定義する構造体
    /// </summary>
    public struct Band
    {
        /************ メンバ変数 ****************/
        /// <summary>
        /// 最大周波数[Hz]
        /// </summary>
        public readonly double Max;
        /// <summary>
        /// 最小周波数[Hz]
        /// </summary>
        public readonly double Min;
        /************ プロパティ ****************/
        /// <summary>
        /// バンド幅[Hz]
        /// </summary>
        public double BandWidth { get { return this.Max - this.Min; } }
        /// <summary>
        /// 中心周波数[Hz]
        /// </summary>
        public double CenterFrequency { get { return (this.Max - this.Min) / 2 + this.Min; } }
        /************ メソッド ******************/
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="max">最大周波数[Hz]</param>
        /// <param name="min">最小周波数[Hz]<para>負値であった場合は0をセットします。</para></param>
        /// <exception cref="SystemException">引数の大小関係が異常である場合にスローします。</exception>
        public Band(double max, double min)
        {
            if (min < 0.0) min = 0.0;
            if (max > min && min >= 0)
            {
                this.Max = max;
                this.Min = min;
            }
            else
            {
                throw new SystemException("引数の大小関係が異常です。");
            }
            return;
        }
    }
}
