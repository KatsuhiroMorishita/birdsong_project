/***************************************************************************
 * [ソフトウェア名]
 *      Discriminator.cs
 * [開発者]
 *      K.Morishita (Kumamoto-Univ. @ 2011.12)
 *      
 * [履歴]
 *      2012/5/9    ソースコードの分割によりファイルとして独立した。
 *      2012/5/10   コメントを一部見直し。
 *      2012/6/22   ニューラルネットの初期化方法を見直して整理したのに伴い、一部のコードを変更した。
 *                  また、コンストラクタを見直してニューラルネット用のパラメータをメンバ変数から削除した。
 * *************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PatternRecognition.ArtificialNeuralNetwork;

namespace PatternRecognition
{
    /// <summary>
    /// 識別クラス
    /// <para>ニューラルネットワークを利用した識別を行う。</para>
    /// </summary>
    public class Discriminator
    {
        /* 構造体 ********************************************************************/
        /// <summary>
        /// 識別結果を簡単に表現するための構造体
        /// <para>クラス名を表すIDと、尤度を格納する。</para>
        /// </summary>
        public struct IDandLikelihood
        {
            /* メンバ変数 ***********************************/
            /// <summary>
            /// ID名
            /// </summary>
            private readonly string id;
            /// <summary>
            /// 尤度
            /// </summary>
            private readonly double likelihood;
            /* プロパティ ***********************************/
            /// <summary>
            /// ID名
            /// </summary>
            public string ID { get { return this.id; } }
            /// <summary>
            /// 尤度
            /// </summary>
            public double Likelihood { get { return this.likelihood; } }
            /* メソッド ***********************************/
            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="_id">ID名</param>
            /// <param name="_likelihood">尤度</param>
            public IDandLikelihood(string _id, double _likelihood)
            {
                this.id = _id;
                this.likelihood = _likelihood;
            }
        }
        /* メンバ変数 ********************************************************************/
        /// <summary>
        /// ニューラルネットクラス
        /// </summary>
        private NeuralNetwork _NNet;
        /// <summary>
        /// ID名
        /// </summary>
        private string[] _IDnames;
        /* プロパティ ********************************************************************/
        /// <summary>
        /// 利用準備状況
        /// </summary>
        public Boolean Ready
        {
            get
            {
                if (this._IDnames != null && this._NNet != null)
                    return true;
                else
                    return false;
            }
        }
        /* メソッド **********************************************************************/
        /// <summary>
        /// 識別用ニューラルネットの設定ファイルを読み込む
        /// <para>デフォルトのファイル名は"discriminator.ini"です。</para>
        /// </summary>
        /// <param name="fname">ファイル名</param>
        public void Setup(string fname = "discriminator.ini")
        {
            if (System.IO.File.Exists(fname) == false) throw new SystemException("指定されたファイルは存在しません。");
            try
            {
                // モデルID名をファイルより読み込む
                using (System.IO.StreamReader sr = new System.IO.StreamReader(fname, System.Text.Encoding.UTF8))
                {
                    Boolean idFound = false;
                    while (sr.EndOfStream == false)
                    {
                        string line = sr.ReadLine();                            // 一行読み込み
                        if (idFound)
                        {
                            string[] field = line.Split(',');
                            this._IDnames = new string[field.Length];
                            for (int i = 0; i < this._IDnames.Length; i++) this._IDnames[i] = field[i];
                            idFound = false;
                        }
                        if (line.IndexOf("ID") >= 0 && line.IndexOf("/ID") < 0) idFound = true;
                    }
                }
                // ニューラルネット復元
                this._NNet = new NeuralNetwork();
                this._NNet.Setup(fname);
            }
            catch (Exception e)
            {
                throw new SystemException(e.GetType().FullName + "\nDiscriminatorクラスのSetup()にてファイル読み込み時にエラーが発生しました。以下詳細：\n" + e.Message);
            }
            return;
        }
        /// <summary>
        /// ニューラルネットの演算結果を分かり易くIDと尤度にまとめて返す
        /// </summary>
        /// <param name="outputVector">ニューラルネットの出力ベクトル</param>
        /// <param name="productionVolume">配列の生成数（1以上かつ出力ベクトルの次元を超えないようにして下さい）</param>
        /// <returns>ID・尤度の組を配列に加工したもの<para>本クラスのIDnamesメンバがセットされていない場合は""が返ります。</para></returns>
        /// <exception cref="SystemException">要求サイズに異常があるとスロー</exception>
        /// <example>
        /// 以下にサンプルコードを示します。
        /// このコードは処理の流れを大まかに示しています。
        /// 実際にはnnclassは結合係数等をセットしなければ識別は行えません。
        /// <code>
        /// NeuralNetwork.Parameter para = new NeuralNetwork.Parameter(2, 6, 1, 1, 0.01);           // ニューラルネットのパラメータ設定
        /// NeuralNetwork nnclass = new NeuralNetwork(para);                                        // ニューラルネットクラスのインスタンス生成
        /// double[] feature10 = new double[2] { 1, 0 };                                            // 特徴ベクトル生成（XORを意識している）
        /// 
        /// IDandLikelihood[] result = nnclass.ToIDandLikelihood(nnclass.Recognize(feature10), 3);  // 出力ベクトルをリレーさせて、構造体配列に変換している
        /// </code>
        /// </example>
        private IDandLikelihood[] GetIDandLikelihood(Vector outputVector, int productionVolume)
        {
            IDandLikelihood[] ans = new IDandLikelihood[productionVolume];
            double val = double.MaxValue;

            if (productionVolume == 0 || productionVolume > outputVector.Length) throw new SystemException("要求サイズが異常です。");
            for (int i = 0; i < productionVolume; i++)  // 大きい順に並び替えながら要素番号も把握する
            {
                double max = 0.0;
                int index = -1;
                for (int k = 0; k < outputVector.Length; k++)
                {
                    if (max < outputVector[k] && val > outputVector[k])
                    {
                        max = outputVector[k];
                        index = k;
                    }
                }
                val = max;
                if (this._IDnames != null && index != -1)
                    ans[i] = new IDandLikelihood(this._IDnames[index], max);
                else
                    ans[i] = new IDandLikelihood("Non", max);
            }
            return ans;
        }
        /// <summary>
        /// 識別結果をIDと尤度で返す
        /// </summary>
        /// <param name="feature">特徴ベクトル</param>
        /// <returns>IDと尤度をセットにした識別結果</returns>
        public IDandLikelihood Discriminate(Feature feature)
        {
            IDandLikelihood ans = new IDandLikelihood();
            if (this.Ready)
            {
                Vector result = this._NNet.Recognize(feature);
                ans = this.GetIDandLikelihood(result, 1)[0];
            }
            return ans;
        }
        /// <summary>
        /// 識別結果をIDと尤度の配列で返す
        /// </summary>
        /// <param name="feature">特徴ベクトル</param>
        /// <param name="productionVolume">配列の生成数（1以上かつ出力ベクトルの次元を超えないようにして下さい）</param>
        /// <returns>IDと尤度をセットにした識別結果の配列</returns>
        public IDandLikelihood[] Discriminate(Feature feature, int productionVolume)
        {
            IDandLikelihood[] ans = null;
            if (this.Ready)
            {
                Vector result = this._NNet.Recognize(feature);
                ans = this.GetIDandLikelihood(result, productionVolume);
            }
            return ans;
        }
        /// <summary>
        /// ニューラルネットのインスタンスとモデル名のリストを渡して初期化するコンストラクタ
        /// </summary>
        /// <param name="NNet">ニューラルネットワーククラスオブジェクト</param>
        /// <param name="IDnames">モデルIDのリスト<para>リストの要素番号とモデル名は関係があるので適当な順序で渡さない様に。あくまで学習器(NeuralNetTeacher)から取得して下さい。</para></param>
        /// <exception cref="SystemException">ニューラルネットのインスタンスが確保されていない場合と、モデルIDの数と出力層ユニット数が一致しなければスロー</exception>
        public Discriminator(NeuralNetwork NNet, string[] IDnames)
        {
            if (NNet == null) throw new SystemException("ニューラルネットのインスタンスが確保されていません。");
            this._IDnames = new string[IDnames.Length];
            for (int i = 0; i < IDnames.Length; i++) this._IDnames[i] = IDnames[i];
            this._NNet = NNet;
            if (this._NNet.OutputVectorLength != this._IDnames.Length) throw new SystemException("モデルIDの数と出力層ユニット数が一致しません。");
        }
        /// <summary>
        /// 引数なしのコンストラクタ
        /// <para>Setup()を呼び出して初期化して下さい。</para>
        /// </summary>
        public Discriminator()
        {

        }
    }
}
