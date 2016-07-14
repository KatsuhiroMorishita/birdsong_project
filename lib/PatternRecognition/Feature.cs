/*************************************
 * Feature
 * Vectorクラスのラッパー
 * 
 * クラス名から特徴ベクトルであることを連想しやすいようにラップした。
 * 
 * [履歴]
 *          2012/6/19   コピーコンストラクタを整備
 * ***********************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PatternRecognition
{
    /// <summary>
    /// 特徴ベクトル
    /// <para>Vectorクラスのラッパー</para>
    /// <para>クラス名から特徴ベクトルであることを連想しやすいようにラップした。</para>
    /// </summary>
    public class Feature : Vector
    {
        #region /* コンストラクタ関係 -----------------------------------------------------------------*/
        /// <summary>
        /// double型配列を使った初期化付コンストラクタ
        /// <para>配列はディープコピーされます。</para>
        /// </summary>
        /// <param name="_vector">double型配列による特徴ベクトル</param>
        /// <exception cref="SystemException">配列がnullならスロー</exception>
        public Feature(double[] _vector)
            : base(_vector)
        {
        }
        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="_vector">Featureクラスオブジェクト</param>
        /// <exception cref="SystemException">配列がnullならスロー</exception>
        public Feature(Feature _vector)
            : base(_vector)
        {
        }
        /// <summary>
        /// 文字列を利用したコンストラクタ
        /// <para>カンマ・タブ・半角スペースで区切られた文字列を渡して下さい。</para>
        /// <para>解析に成功すると文字列に含まれていた数値がセットされます。</para>
        /// </summary>
        /// <param name="_vector">文字列で表現した特徴ベクトル</param>
        public Feature(string _vector)
            : base(_vector)
        {
        }
        /// <summary>
        /// サイズ指定のコンストラクタ
        /// <para>指定したサイズのベクトルを生成します。生成後に一つ一つの要素にアクセスする用途を想定しています。</para>
        /// </summary>
        /// <param name="size">ベクトルサイズ</param>
        public Feature(int size)
            : base(size)
        {
        }
        /// <summary>
        /// 要素数1のベクトルを生成します
        /// <para>関数近似などの用途を想定しています。</para>
        /// </summary>
        /// <param name="value">格納したい数</param>
        public Feature(double value)
            : base(value)
        {
        }
        /// <summary>
        /// 初期化なしのコンストラクタ
        /// <para>空のベクトルを生成します。適宜要素を追加するような用途での使用を想定しています。</para>
        /// </summary>
        public Feature()
            : base()
        {
        }
        #endregion
    }
}
