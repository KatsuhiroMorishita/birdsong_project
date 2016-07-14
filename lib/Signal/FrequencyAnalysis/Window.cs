/*********************************************
 * Window.cs
 * 窓関数に関するクラス
 *  開発者：Katsuhiro Morishita(2011.12) Kumamoto-University
 * 
 * [名前空間]   SignalProcess
 * [クラス名]   Window
 * 
 * [使用方法]   まず、プロジェクトに本ファイルを追加した後に、ソースヘッダに以下のコードを追加して下さい。
 *              using SignalProcess;
 *              
 * [更新情報]   2011/12/5   開発開始。
 *                          今後の拡張予定は現時点ではない。
*********************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Signal.FrequencyAnalysis
{
    /// <summary>
    /// 窓関数に関するクラス
    /// </summary>
    /// <example>
    /// <code>
    /// double[] hoge = new double[100];
    /// // hogeに対してデータを格納する処理
    /// hoge = Window.Windowing(hoge, Window.WindowKind.Hanning); // これで窓が掛かる
    /// </code>
    /// </example>
    public static class Window
    {
        /// <summary>
        /// 窓関数の種類
        /// </summary>
        public enum WindowKind
        {
            /// <summary>ハミング窓</summary>
            Hanning,
            /// <summary>パルツェン窓</summary>
            Perzen,
            /// <summary>矩形窓</summary>
            NoWindow,
            /// <summary>三角窓</summary>
            Triangle
        }
        /// <summary>
        /// 窓関数を掛ける
        /// <para>静的メソッドですのでインスタンスを生成することなく利用可能です。</para>
        /// </summary>
        /// <param name="data_array">窓を掛けたいdouble型一次元配列</param>
        /// <param name="windowKind">窓の種類</param>
        /// <returns>窓関数を掛けた結果（引数として渡した配列自体が変形するので注意）</returns>
        /// <example>
        /// 以下にWindowingメソッドの使用例を示します。
        /// <code>
        /// double[] hoge = new double[100];
        /// // hogeに対してデータを格納する処理
        /// hoge = Window.Windowing(hoge, Window.WindowKind.Hanning); // これで窓が掛かる
        /// </code>
        /// </example>
        public static double[] Windowing(double[] data_array, WindowKind windowKind)
        {
            double wj = 1.0;

            for (int j = 0; j < data_array.Length; j++)
            {
                if (windowKind == WindowKind.Hanning)       // 窓種類毎に計数を計算
                    wj = 0.5 * (1.0 - Math.Cos(2.0 * Math.PI * (double)j / ((double)data_array.Length - 1)));
                else if (windowKind == WindowKind.Perzen)
                    wj = 1.0 - Math.Abs(((double)j - 0.5 * ((double)data_array.Length - 1)) / (0.5 * ((double)data_array.Length + 1)));
                else if (windowKind == WindowKind.Triangle)
                {
                    if (j < data_array.Length / 2)
                        wj = 2.0 / data_array.Length * (double)j;
                    else if (j >= data_array.Length / 2)
                        wj = -2.0 / data_array.Length * (double)j + 2.0;
                }
                data_array[j] *= wj;
            }
            return data_array;
        }
    }
}
