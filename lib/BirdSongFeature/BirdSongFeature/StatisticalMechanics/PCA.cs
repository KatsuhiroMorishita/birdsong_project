using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BirdSongFeature.StatisticalMechanics
{
    /// <summary>
    /// 主成分分析クラス
    /// <para>2012/5/18時点では空のクラスです。</para>
    /// </summary>
    public class PCA
    {
        /// <summary>
        /// 主成分分析の成果を利用してベクトルを整理する
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static double[] Convert(double[] vector)
        {
            return vector;
        }
        /// <summary>
        /// ファイル指定による初期化付きコンストラクタ
        /// <para>ファイル名を指定して、ベクトル演算に必要なパラメータを読み込ませてください。</para>
        /// </summary>
        /// <param name="fname"></param>
        public PCA(string fname = "")
        {
            if (System.IO.File.Exists(fname))
            {

            }
            return;
        }
    }
}
