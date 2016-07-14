/*************************************
 * 
 * [履歴]
 *          2012/6/16   "長さ"となっていたLengthのコメントを次元数へ変更
 *                      ベクトル長を返すNormを整備
 *                      ベクトル成分を、成分の追加が容易になるようにList型へ変更
 *                      正規化メソッドNormalizing()を新設
 *                      全てのメソッド・コンストラクタでインスタンスをコピーするように変更し、完全にデータを本クラス内に閉じた。
 *                      足し算と引き算を実装した。
 *                      うまく行くかわからんが、スカラー積と商を実装してみた。->うまく行った。
 *          2012/6/19   コピーコンストラクタを整備
 * ***********************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PatternRecognition
{
    /// <summary>
    /// ベクトルクラス
    /// </summary>
    public class Vector
    {
        /************** メンバ変数 **********************/
        /// <summary>
        /// ベクトル要素
        /// </summary>
        private List<double> vector = new List<double>();
        /* 演算子 ***************************************************/
        /// <summary>
        /// 二項+演算子（これで足し算が簡単にできる）
        /// </summary>
        /// <param name="c1">被加算値</param>
        /// <param name="c2">加算値</param>
        /// <returns>2値を加算した結果</returns>
        public static Vector operator +(Vector c1, Vector c2)
        {
            if (c1 == null || c2 == null) throw new Exception("インスタンスの確保されていないベクトルの演算を検出しました。");
            if (c1.Length != c2.Length) throw new Exception("次元数の異なるベクトルの足し算が行われました。");
            var data = new double[c1.Length];
            for (int i = 0; i < data.Length; i++) data[i] = c1[i] + c2[i];
            return new Vector(data);
        }
        /// <summary>
        /// 二項-演算子（これで引き算が簡単にできる）
        /// </summary>
        /// <param name="c1">被減算値</param>
        /// <param name="c2">減算値</param>
        /// <returns>2値の引き算の結果</returns>
        public static Vector operator -(Vector c1, Vector c2)
        {
            if (c1 == null || c2 == null) throw new Exception("インスタンスの確保されていないベクトルの演算を検出しました。");
            if (c1.Length != c2.Length) throw new Exception("次元数の異なるベクトルの引き算が行われました。");
            var data = new double[c1.Length];
            for (int i = 0; i < data.Length; i++) data[i] = c1[i] - c2[i];
            return new Vector(data);
        }
        /// <summary>
        /// 二項*演算子（これでスカラー積が簡単にできる）
        /// </summary>
        /// <param name="c1">積算値</param>
        /// <param name="c2">被積算値</param>
        /// <returns>演算結果</returns>
        public static Vector operator *(double c1, Vector c2)
        {
            if (c2 == null) throw new Exception("インスタンスの確保されていないベクトルの演算を検出しました。");
            var data = new double[c2.Length];
            for (int i = 0; i < data.Length; i++) data[i] = c1 * c2[i];
            return new Vector(data);
        }
        /// <summary>
        /// 二項/演算子（これでスカラー除法が簡単にできる）
        /// </summary>
        /// <param name="c1">被除数</param>
        /// <param name="c2">除数（法）</param>
        /// <returns>演算結果</returns>
        public static Vector operator /(Vector c1, double c2)
        {
            if (c1 == null) throw new Exception("インスタンスの確保されていないベクトルの演算を検出しました。");
            var data = new double[c1.Length];
            for (int i = 0; i < data.Length; i++) data[i] = c1[i] / c2;
            return new Vector(data);
        }
        /// <summary>
        /// ベクトルの各成分を指定乗する
        /// <para>破壊的メソッド</para>
        /// </summary>
        /// <param name="c">乗数</param>
        /// <returns>演算結果（被演算インスタンス）</returns>
        public Vector Pow(double c)
        {
            for (int i = 0; i < this.Length; i++) this[i] = Math.Pow(this[i], c);
            return this;
        }
        /************** プロパティ **********************/
        /// <summary>
        /// 次元数
        /// </summary>
        public int Length { get { return this.vector.Count; } }
        /// <summary>
        /// ノルム
        /// </summary>
        public double Norm
        {
            get
            {
                double squareNnorm = 0.0;
                foreach (double data in this.vector) squareNnorm += Math.Pow(data, 2.0);
                return Math.Sqrt(squareNnorm);
            }
        }
        /// <summary>
        /// 各要素の合計値
        /// </summary>
        public double Total
        {
            get 
            {
                double sum = 0.0;
                foreach (double data in this.vector) sum += data;
                return sum;
            }
        }
        /************** インデクサ **********************/
        /// <summary>
        /// インデクサ
        /// <para>配列と同様にアクセスできます。</para>
        /// </summary>
        /// <param name="index">要素番号</param>
        /// <returns>指定されたベクトル要素を返す</returns>
        public double this[int index]
        {
            get { if (index >= 0 && index < this.vector.Count) return this.vector[index]; else throw new SystemException("範囲外の要素が指定されました。"); }
            set { if (index >= 0 && index < this.vector.Count) this.vector[index] = value; else throw new SystemException("範囲外の要素が指定されました。"); }
        }
        /************** メソッド **********************/
        /// <summary>
        /// 正規化
        /// <para>ノルムが0だと動作しません。</para>
        /// </summary>
        public void Normalizing()
        {
            double norm = this.Norm;
            if (norm != 0.0)
            {
                for (int i = 0; i < this.vector.Count; i++) this.vector[i] /= norm;
            }
            return;
        }
        /// <summary>
        /// ベクトル要素を文字列として返す
        /// <para>出力フォーマット付</para>
        /// </summary>
        /// <param name="format">出力フォーマット</param>
        /// <returns>ベクトル要素の中身</returns>
        public string ToString(string format = "E4")
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(300);
            for (int i = 0; i < this.vector.Count; i++) sb.Append(this.vector[i].ToString(format)).Append(",");
            return sb.ToString();
        }
        /// <summary>
        /// ベクトル要素を追加する
        /// <para>後追加となる</para>
        /// </summary>
        /// <param name="add">追加したい要素</param>
        public void Add(double add)
        {
            this.vector.Add(add);
            return;
        }
        /// <summary>
        /// ベクトル要素を追加する
        /// <para>後追加となる</para>
        /// </summary>
        /// <param name="addtionalVector">追加したいベクトル</param>
        public void Add(double[] addtionalVector)
        {
            for (int i = 0; i < addtionalVector.Length; i++)
                this.Add(addtionalVector[i]);

            return;
        }
        /// <summary>
        /// ベクトル情報をdouble型配列で返す
        /// </summary>
        /// <returns>配列表現の特徴ベクトル</returns>
        public double[] ToArray()
        {
            return this.vector.ToArray();
        }
        #region /* コンストラクタ関係 -----------------------------------------------------------------*/
        /// <summary>
        /// double型配列を使った初期化付コンストラクタ
        /// <para>配列はディープコピーされます。</para>
        /// </summary>
        /// <param name="_vector">double型配列</param>
        /// <exception cref="SystemException">配列がnullならスロー</exception>
        public Vector(double[] _vector)
        {
            if (_vector == null) throw new SystemException("配列がnullです。");
            this.Add(_vector);
        }
        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="_vector">Vectorクラスオブジェクト</param>
        public Vector(Vector _vector)
        {
            if (_vector == null) throw new SystemException("オブジェクトはnullです。");
            this.Add(_vector.ToArray());
        }
        /// <summary>
        /// 文字列を利用したコンストラクタ
        /// <para>カンマ・タブ・半角スペースで区切られた文字列を渡して下さい。</para>
        /// <para>解析に成功すると文字列に含まれていた数値がセットされます。</para>
        /// </summary>
        /// <param name="_vector">文字列表現のベクトル成分</param>
        public Vector(string _vector)
        {
            string[] field = _vector.Split(',');                        // 文字列を分割処理
            if (field.Length == 1) field = _vector.Split('\t');
            if (field.Length == 1) field = _vector.Split(' ');
            double value;
            for (int i = 0; i < field.Length; i++)                      // 文字列を解析して配列要素に格納していく
            {
                Boolean success = double.TryParse(field[i], out value);
                if (success)
                {
                    this.Add(value);
                }
            }
            return;
        }
        /// <summary>
        /// サイズ指定のコンストラクタ
        /// <para>指定したサイズのベクトルを生成します。生成後に一つ一つの要素にアクセスする用途を想定しています。</para>
        /// </summary>
        /// <param name="size">ベクトルの次元数</param>
        public Vector(int size)
        {
            for (int i = 0; i < size; i++ ) this.Add(0.0);
            return;
        }
        /// <summary>
        /// 要素数1のベクトルを生成します
        /// <para>関数近似などの用途を想定しています。</para>
        /// </summary>
        /// <param name="value">格納したい数</param>
        public Vector(double value)
        {
            this.Add(value);
            return;
        }
        /// <summary>
        /// 初期化なしのコンストラクタ
        /// <para>空のベクトルを生成します。適宜要素を追加するような用途での使用を想定しています。</para>
        /// </summary>
        public Vector()
        {
            
        }
        #endregion
    }
}
