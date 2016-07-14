/*************************************************************************
 * FFTdata.cs
 * FFT処理前/後のデータを格納することを想定した構造体
 *  開発者：Katsuhiro Morishita(2010.5) Kumamoto-University
 * 
 * [名前空間]   Signal.FrequencyAnlysis
 * 
 * [更新情報]   
 *              2012/5/4    DFT.csより分割した。
 *              2012/11/1   Absの計算で冗長な部分があったので整理。ちっとだけ早くなったはず。
 *                          CompareTo()を実装し、比較に対応しました。これでSort()に対応しました。
 * ***********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Signal.FrequencyAnalysis
{
    /// <summary>
    /// FFT処理前/後のデータを格納することを想定した構造体
    /// </summary>
    public struct FFTdata : IComparable
    {
        /************** メンバ変数 *********************/
        /// <summary>実部</summary>
        public double Re;
        /// <summary>虚部</summary>
        public double Im;
        /// <summary>位相</summary>
        public double Phase;
        /// <summary>絶対値</summary>
        public double Abs;
        /// <summary>絶対値の対数</summary>
        public double Log;
        /// <summary>パワー<para>ところで、パワースペクトル密度 （Power Spectrum Density,PSD）は、Powerを周波数分解能で割れば求まります。</para></summary>
        public double Power;                            // FFTの結果を格納した場合、最小の分解能の周波数幅におけるパワーの積算値でもある
        /************** メソッド *********************/
        /// <summary>
        /// データをセットする
        /// </summary>
        /// <param name="_Re">実部</param>
        /// <param name="_Im">虚部</param>
        public void Set(double _Re, double _Im)
        {
            this.Re = _Re;
            this.Im = _Im;
            this.Phase = Math.Atan2(this.Im, this.Re);
            this.Power = Math.Pow(_Re, 2) + Math.Pow(_Im, 2);
            this.Abs = Math.Sqrt(this.Power);
            this.Log = Math.Log10(this.Abs);
            
        }
        /// <summary>
        /// パワーを用いてオブジェクトを比較します
        /// </summary>
        /// <param name="obj">比較したいオブジェクト</param>
        /// <returns>比較結果<para>自分自身がobjより小さいときはマイナスの数、大きいときはプラスの数、同じときは0を返します。</para></returns>
        public int CompareTo(object obj)
        {
            //nullより大きい
            if (obj == null)
            {
                return 1;
            }

            //違う型とは比較できない
            if (this.GetType() != obj.GetType())
            {
                throw new ArgumentException("別の型とは比較できません。", "obj");
            }

            return this.Power.CompareTo(((FFTdata)obj).Power);
        }
    }
}
