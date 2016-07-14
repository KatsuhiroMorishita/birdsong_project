/***************************************************************************
 * [ソフトウェア名]
 *      NeuralNetwork.cs
 * [開発者]
 *      K.Morishita (Kumamoto-Univ. @ 2011.12)
 * [概要]
 *      ニューラルネットワークを利用した識別器のクラスです。
 *      ソースコードの改造が容易になるように、とことんオブジェクトに分解しました。
 *      おかげで学習は遅くなったようですが、可読性と可塑性が増したのではないかと思います。
 *      ニューラルネットに関してはシロートが作っていますので変なところがあるかもしれません。
 *      XORを学習できることは確認済みです。
 *      
 * [参考資料]
 *      浅川伸一, バックプロパゲーション, http://www.cis.twcu.ac.jp/~asakawa/waseda2002/bp.pdf ,2011/12. （少し誤字があるけど）
 *      
 * [履歴]
 *      2011/12/8   開発開始
 *                  コンセプトは、使い易く書きやすく。。。
 *                  効率的で高速な演算とは縁遠い構造をしています。
 *                  オブジェクトや演算の関係が分かり易ければ良いという考えの基で作りました。
 *      2011/12/9   基本動作の確認が行えるまでは組めた。
 *                  今後の予定：学習終了トリガとなる構造体の宣言
 *                              学習モードの整備
 *                              保存されたファイルから文字列を解析して、NeuralNetTeacherクラスの再構成・NNクラスの再構成を実施するメソッドを整備
 *      2011/12/11  昨日に引き続き、デバッグとコメント見直しを行った。
 *      2011/12/24  昨日より再度整備を開始
 *                  ニューラルネットの核はほぼできたと思う。ただし、関数近似に使用できない。どうすればよいのだろう？
 *                  今日は学習用のクラスを整備中
 *      2011/12/25  学習用のクラスはファイル保存機能を除いてほぼ完全に整備できたと思う。
 *                  次は保存メソッドと、識別機能利用時のファイルからの復元機能を設計する必要がある。
 *      2011/12/27  昨日の内に保存ファイルからの復帰機能を実装できた。
 *                  今日はUIの改善と動作テストを行った。
 *                  本日で基本的な機能の実装は終わったと思う。
 *                  無論、学習の方法や評価の方法に思うところはあるのだけどこれで一つの区切りとする。
 *      2011/12/30  結局気になっていたので学習器の学習アルゴリズムを、モデル毎にまとめて学習する方法から均等に学習する（引数によってモデル毎にもできる）方法に変えた。
 *                  それに伴いモデルクラスも改良した。
 *                  結果はというと…鳥の学習ではとりわけよくなった。
 *                  鳥だと、特定の種だけ2倍学習させただけで途端に成績が悪くなる。
 *      2012/3/10   GetIDandLikelihoodにおいて、入力された出力層の値が非値だった場合にエラーが発生していたのを訂正
 *      2012/3/13   コンソールアプリだとメッセージボックスが邪魔であったのでこれを削除し、代わりに例外をスローするように変更した。
 *      2012/5/9    ライブラリをプロジェクト単位で切り出して、ソースコードを分割した。
 *      2012/6/18   結合係数を初期化するInit()メソッドを整備した。
 *                  メモ：Setup()の呼び出し時にどうにかしてパラメータを復元するか、もしくはLayerに対して初期化命令を出せるようにする必要がある。
 *                  今後を考えると、パラメータ復元の方が良い。
 *      2012/6/22   汎用性確保のためにコンストラクタから引数をなくした。
 *                  Setup()メソッドも見直したことで、18日のメモにある、「Setup()の呼び出し時にどうにかしてパラメータを復元するか・・・」の問題はなくなった。
 *                  また、設定パラメータに不正を検出した際にエラーをスローするようにした。
 *                  汎用化の一環のため、OutputVectorLengthプロパティを整備した。
 * *************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PatternRecognition.ArtificialNeuralNetwork
{
    /// <summary>
    /// ニューラルネットワーク処理クラス
    /// </summary>
    public partial class NeuralNetwork : PatternRecognition.ArtificialNeuralNetwork.ILearningMachine
    {
        /************** メンバ変数 **********************/
        /// <summary>
        /// 本クラスの構造を規定するパラメータ
        /// <para>中間層数などを格納する。</para>
        /// </summary>
        private Parameter parameter;
        /// <summary>
        /// 演算処理を実行するレイヤークラス
        /// <para>再帰構造になっているので一つのオブジェクトで演算は完結している。</para>
        /// </summary>
        private Layer layer;
        /************** プロパティ **********************/
        /// <summary>
        /// 出力誤差の変化量
        /// </summary>
        public double VariationOfError 
        { 
            get { return this.layer.VariationOfError; } 
        }
        /// <summary>
        /// 出力誤差の合計
        /// <para>絶対値をとった出力層における各ユニットの誤差を合計したものです。</para>
        /// </summary>
        public double TotalOutputError 
        { 
            get { return this.layer.TotalOutputError; } 
        }
        /// <summary>
        /// 出力するベクトルの次元数
        /// <para>ニューラルネットにとっては出力層のユニット数に該当します。</para>
        /// </summary>
        public int OutputVectorLength
        {
            get { return this.parameter.NumOfUnitInOutputLayer; }
        }
        /************** メソッド **********************/
        /// <summary>
        /// ニューラルネットワークの構成パラメータを返す
        /// </summary>
        /// <returns>ニューラルネットワークの構成パラメータ</returns>
        public Parameter GetParameter()
        {
            return this.parameter; 
        }
        /// <summary>
        /// ニューラルネットの構成パラメータ及び結合係数を文字列として出力する。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(10000);
            sb.Append(this.parameter.ToString());
            sb.Append(this.layer.ToString());
            return sb.ToString();
        }
        /// <summary>
        /// 指定ファイル名でニューラルネットの状態量をテキストファイルへ保存する
        /// <para>デフォルトのファイル名は「NN.ini」です。</para>
        /// </summary>
        /// <param name="fname">ファイル名</param>
        /// <param name="append">追記の有無<para>true：追記</para></param>
        public void Save(string fname = "NN.ini", Boolean append = true)
        {
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fname, append, System.Text.Encoding.UTF8))
            {
                sw.WriteLine(this.ToString());
            }
            return;
        }
        /// <summary>
        /// 特徴ベクトルを基に識別を行う
        /// </summary>
        /// <param name="featureVector">特徴ベクトル</param>
        /// <returns>識別結果</returns>
        public double[] Recognize(double[] featureVector)
        {
            if (this.parameter.NumOfUnitInInputLayer != featureVector.Length) throw new SystemException("入力層のユニット数と特徴ベクトルの長さが一致しません。");
            Vector result = this.layer.GetResult(new Vector(featureVector), null);
            return result.ToArray();
        }
        /// <summary>
        /// 特徴ベクトルを基に識別を行う
        /// </summary>
        /// <param name="featureVector">特徴ベクトル</param>
        /// <returns>識別結果</returns>
        public Vector Recognize(Feature featureVector)
        {
            if (this.parameter.NumOfUnitInInputLayer != featureVector.Length) throw new SystemException("入力層のユニット数と特徴ベクトルの長さが一致しません。");
            Vector result = this.layer.GetResult((Vector)featureVector, null);
            return result;
        }
        /// <summary>
        /// 特徴データを基に識別を行う
        /// <para>入力ユニットが1つの場合に使用します。出力層の第0ユニットの出力を返します。関数近似の用途を想定しています。</para>
        /// </summary>
        /// <param name="feature">特徴データ</param>
        /// <returns>返値<para>出力層の第0ユニットの出力</para></returns>
        public double Recognize(double feature)
        {
            if (this.parameter.NumOfUnitInInputLayer != 1) throw new SystemException("入力層のユニット数と特徴ベクトルの長さが一致しません。");
            Vector result = this.layer.GetResult(new Vector(feature), null);
            return result[0];
        }
        /// <summary>
        /// 特徴ベクトルを基に、学習を行う
        /// </summary>
        /// <param name="featureVector">特徴ベクトル</param>
        /// <param name="NeuralNetTeacher">教師ベクトル</param>
        /// <returns>識別結果</returns>
        public double[] Learn(double[] featureVector, double[] NeuralNetTeacher)
        {
            if (this.parameter.NumOfUnitInInputLayer != featureVector.Length) throw new SystemException("入力層のユニット数と特徴ベクトルの長さが一致しません。");
            if (this.parameter.NumOfUnitInOutputLayer != NeuralNetTeacher.Length) throw new SystemException("出力層のユニット数と教師ベクトルの長さが一致しません。");
            Vector result = this.layer.GetResult(new Vector(featureVector), new Vector(NeuralNetTeacher));
            return result.ToArray();
        }
        /// <summary>
        /// 特徴ベクトルを基に、学習を行う
        /// </summary>
        /// <param name="featureVector">特徴ベクトル</param>
        /// <param name="NeuralNetTeacher">教師ベクトル</param>
        /// <returns>識別結果</returns>
        public Vector Learn(Feature featureVector, Vector NeuralNetTeacher)
        {
            if (this.parameter.NumOfUnitInInputLayer != featureVector.Length) throw new SystemException("入力層のユニット数と特徴ベクトルの長さが一致しません。");
            if (this.parameter.NumOfUnitInOutputLayer != NeuralNetTeacher.Length) throw new SystemException("出力層のユニット数と教師ベクトルの長さが一致しません。");
            Vector result = this.layer.GetResult((Vector)featureVector, NeuralNetTeacher);
            return result;
        }
        /// <summary>
        /// 特徴ベクトルを基に、学習を行う
        /// <para>出力が一つしかない場合に特に便利です。例えばXORの学習時に利用してください。</para>
        /// </summary>
        /// <param name="featureVector">特徴ベクトル</param>
        /// <param name="NeuralNetTeacher">教師データ</param>
        /// <returns>演算結果</returns>
        public double Learn(Feature featureVector, double NeuralNetTeacher)
        {
            if (this.parameter.NumOfUnitInInputLayer != featureVector.Length) throw new SystemException("入力層のユニット数と特徴ベクトルの長さが一致しません。");
            if (this.parameter.NumOfUnitInOutputLayer != 1) throw new SystemException("出力層のユニット数と教師ベクトルの長さが一致しません。");
            Vector result = this.layer.GetResult((Vector)featureVector, new Vector(NeuralNetTeacher));
            return result[0];
        }
        /// <summary>
        /// 特徴ベクトルを基に、学習を行う
        /// <para>出力が一つしかない場合に特に便利です。例えばXORの学習時に利用してください。特徴ベクトルにはdouble型配列を用います。</para>
        /// </summary>
        /// <param name="featureVector">特徴ベクトル</param>
        /// <param name="NeuralNetTeacher">教師データ</param>
        /// <returns>演算結果</returns>
        public double Learn(double[] featureVector, double NeuralNetTeacher)
        {
            if (this.parameter.NumOfUnitInInputLayer != featureVector.Length) throw new SystemException("入力層のユニット数と特徴ベクトルの長さが一致しません。");
            if (this.parameter.NumOfUnitInOutputLayer != 1) throw new SystemException("出力層のユニット数と教師ベクトルの長さが一致しません。");
            Vector result = this.layer.GetResult(new Vector(featureVector), new Vector(NeuralNetTeacher));
            return result[0];
        }
        /// <summary>
        /// 特徴データを基に、学習を行う
        /// <para>関数近似に利用可能です。</para>
        /// </summary>
        /// <param name="feature">特徴データ</param>
        /// <param name="NeuralNetTeacher">教師データ</param>
        /// <returns>演算結果</returns>
        public double Learn(double feature, double NeuralNetTeacher)
        {
            if (this.parameter.NumOfUnitInInputLayer != 1) throw new SystemException("入力層のユニット数と特徴ベクトルの長さが一致しません。");
            if (this.parameter.NumOfUnitInOutputLayer != 1) throw new SystemException("出力層のユニット数と教師ベクトルの長さが一致しません。");
            Vector result = this.layer.GetResult(new Vector(feature), new Vector(NeuralNetTeacher));
            return result[0];
        }
        /// <summary>
        /// 入力層・中間層・出力層のインスタンスを新規に生成するとともに、全ユニットを初期化する
        /// </summary>
        public void Init()
        {
            this.layer = new Layer(this.parameter, this.parameter.TotalLayer - 1);
            this.layer.LearningCoefficient = this.parameter.LearningCoefficient;
            return;
        }
        /// <summary>
        /// ファイルを指定して結合係数を読み込ませる
        /// <para>ファイル名はデフォルトでは"NN.ini"です。</para>
        /// </summary>
        /// <param name="fname">初期化用のパラメータの書かれたファイルのパス</param>
        public void Setup(string fname = "NN.ini")
        {
            if (System.IO.File.Exists(fname) == false) throw new SystemException("NeuralNetworkクラスのSetup()メソッドにおいてエラーがスローされました。指定されたファイルは存在しません。");
            Parameter para = new Parameter(fname);
            this.parameter = para;
            if (this.parameter.Error) throw new Exception("NeuralNetworkクラスのSetup()メソッドにおいてエラーがスローされました。設定パラメータの値が不正です。");
            this.Init();
            this.layer.Setup(fname);                        // 実際の処理はlayerに丸投げ
            return;
        }
        /// <summary>
        /// パラメータ構造体で初期化する
        /// </summary>
        /// <param name="_parameter">ニューラルネットワーク構造を規定するパラメータ</param>
        public void Setup(Parameter _parameter)
        {
            this.parameter = _parameter;
            if (this.parameter.Error) throw new Exception("NeuralNetworkクラスのSetup()メソッドにおいてエラーがスローされました。設定パラメータの値が不正です。");
            this.Init();
            return;
        }
        /// <summary>
        /// 最小パラメータ構造体及び入力層と出力層のユニット数で初期化する
        /// </summary>
        /// <param name="minParameter">最小パラメータ</param>
        /// <param name="_numOfUnitInInputLayer">入力層のユニット数<para>特徴ベクトルの次元数と同じ数を指定して下さい。</para></param>
        /// <param name="_numOfUnitInOutputLayer">出力層のユニット数<para>この数は出力状態量と同じ数を指定して下さい。</para></param>
        public void Setup(Object minParameter, int _numOfUnitInInputLayer, int _numOfUnitInOutputLayer)
        {
            if (minParameter is MinParameter)
            {
                MinParameter _minPara = (MinParameter)minParameter;
                if (_minPara.NumOfHiddenLayer == 0) Console.WriteLine("中間層数が0でしたので中間層のユニット数は無視しました。");  // 実質無視されます。
                Parameter para = new Parameter(_minPara, _numOfUnitInInputLayer, _numOfUnitInOutputLayer);
                this.parameter = para;
                if (this.parameter.Error) throw new Exception("NeuralNetworkクラスのSetup()メソッドにおいてエラーがスローされました。設定パラメータの値が不正です。");
                this.Init();
                return;
            }
            else
                throw new Exception("不正なパラメータが渡されました。minParameterの型をチェックして下さい。");
        }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NeuralNetwork()
        {
        }
    }
}
