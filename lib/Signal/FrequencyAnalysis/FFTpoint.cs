/*************************************************************************
 * FFTpoint.cs
 * FFTデータ幅を表す列挙体
 *  開発者：Katsuhiro Morishita(2010.5) Kumamoto-University
 * 
 * [名前空間]   Signal.FrequencyAnlysis
 * 
 * [更新情報]   
 *              2012/5/4    DFT.csより分割した。
 * ***********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Signal.FrequencyAnalysis
{
    /// <summary>
    /// FFTデータ幅
    /// <para>データサイズ(2^n)をこの列挙体を使って宣言すれば間違えない.</para>
    /// </summary>
    public enum FFTpoint : int
    {
        /// <summary>データ幅32ポイント</summary>
        size32 = 32,
        /// <summary>データ幅64ポイント</summary>
        size64 = 64,
        /// <summary>データ幅128ポイント</summary>
        size128 = 128,
        /// <summary>データ幅256ポイント</summary>
        size256 = 256,
        /// <summary>データ幅512ポイント</summary>
        size512 = 512,
        /// <summary>データ幅1024ポイント</summary>
        size1024 = 1024,
        /// <summary>データ幅2048ポイント</summary>
        size2048 = 2048,
        /// <summary>データ幅4096ポイント</summary>
        size4096 = 4096,
        /// <summary>データ幅8192ポイント</summary>
        size8192 = 8192
    }
}
