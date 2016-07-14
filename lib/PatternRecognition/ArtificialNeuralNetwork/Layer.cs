/***************************************************
 * Layer
 * ニューラルネットにおける入力・中間・出力層クラス
 * 
 * [履歴]
 *          2012/6/25   コメントを見やすく整形した。
 * *************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;


namespace PatternRecognition.ArtificialNeuralNetwork
{
    /// <summary>
    /// ニューラルネットにおける入力・中間・出力層クラス
    /// <para>入力層から出力層までを扱うクラスです。</para>
    /// <para>利用クラスにおいて層を配列として宣言して扱ってもよかったのだろうけど、なんだか嫌だったのでこのクラスを一つ宣言すれば完結するようにした。</para>
    /// <para>メモ：処理には再帰構造を使っています。</para>
    /// <para>2011/12/11現在、ユニット間の結合係数は層の出力を得るたびに逐次処理を行っているので厳密にはBPのアルゴリズムとはずれるんだけど上手く動いている。</para>
    /// </summary>
    internal class Layer
    {
        /* 列挙体 ***********************************/
        /// <summary>
        /// 層の種類
        /// </summary>
        private enum Kind
        {
            /// <summary>入力層</summary>
            InputLayer,
            /// <summary>中間層（隠れ層）</summary>
            HiddenLayer,
            /// <summary>出力層</summary>
            OutputLayer,
            /// <summary>不定</summary>
            NA
        }
        /* メンバ変数 ***********************************/
        /// <summary>
        /// 総層数
        /// </summary>
        private readonly int grossLayerNum;
        /// <summary>
        /// 通し層番号
        /// </summary>
        private readonly int myLayerNum;
        /// <summary>
        /// この層が保持するユニット
        /// </summary>
        private Unit[] units;
        /// <summary>
        /// ニューラルネットのパラメータ
        /// </summary>
        private readonly Parameter parameter;
        /// <summary>
        /// 下層レイヤ
        /// </summary>
        private Layer subsidiaryLayer;
        /// <summary>
        /// 学習係数
        /// </summary>
        public static double learningCoefficient = 0.01;
        /// <summary>
        /// 前層のユニット数
        /// </summary>
        private int preLayerUnitNum;
        /// <summary>
        /// 出力層における平均出力誤差の変化量
        /// </summary>
        static private double variationError = 0.0;
        /// <summary>
        /// 出力層における平均出力誤差のバックアップ
        /// </summary>
        static private double backupError = 0.0;
        /// <summary>
        /// 出力層における出力誤差の総計
        /// <para>以下で定義しています。</para>
        /// <para>sumError += Math.Abs(this.units[i].Error);</para>
        /// </summary>
        static private double totalOutputError = 0.0;
        /// <summary>
        /// 乱数生成器
        /// <para>教師データに対する誤差量に加える乱数に利用します。</para>
        /// </summary>
        private static Random myRandom;
        /* プロパティ ***********************************/
        /// <summary>
        /// この層の種類
        /// </summary>
        private Kind MyKind
        {
            get
            {
                if (this.myLayerNum == this.grossLayerNum - 1)
                    return Kind.InputLayer;
                else if (this.myLayerNum == 0)
                    return Kind.OutputLayer;
                else
                    return Kind.HiddenLayer;
            }
        }
        /// <summary>
        /// 学習係数
        /// </summary>
        public double LearningCoefficient
        {
            get { return learningCoefficient; }
            set { learningCoefficient = value; }
        }
        /// <summary>
        /// ユニット数
        /// </summary>
        public int Lenght { get { return this.units.Length; } }
        /// <summary>
        /// 出力誤差の変化量
        /// </summary>
        public double VariationOfError { get { return variationError; } }
        /// <summary>
        /// 出力層の全ユニット出力の誤差を合計したもの
        /// <para>絶対値をとった各ユニットの誤差を合計したものです。</para>
        /// </summary>
        public double TotalOutputError { get { return totalOutputError; } }
        /* メソッド ***********************************/
        /// <summary>
        /// この層が保持する全ユニットの結合係数を文字列化する
        /// </summary>
        /// <returns>文字列化したレイヤ情報</returns>
        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(5000);
            if (this.MyKind != Kind.InputLayer)                                         // 入力層は結合係数を持たないため飛ばす
            {
                sb.Append("<Layer = ").Append(this.myLayerNum.ToString()).Append(">\n");
                if (this.MyKind == Kind.HiddenLayer)
                    sb.Append("<Layer kind>Hidden</Layer kind>").Append("\n");
                else if (this.MyKind == Kind.OutputLayer)
                    sb.Append("<Layer kind>Output</Layer kind>").Append("\n");
                for (int i = 0; i < this.units.Length; i++)
                {
                    sb.Append("<unit num=").Append(i.ToString()).Append(">").Append("\n");
                    sb.Append("<threshold>").Append("\n");
                    sb.Append(this.units[i].Threshold.ToString()).Append("\n");
                    sb.Append("</threshold>").Append("\n");
                    sb.Append("<weight vector>").Append("\n");
                    sb.Append(this.units[i].ToString()).Append("\n");
                    sb.Append("</weight vector>").Append("\n");
                    sb.Append("</unit num=").Append(i.ToString()).Append(">").Append("\n");
                }
                sb.Append("</Layer = ").Append(this.myLayerNum.ToString()).Append(">\n");
            }
            if (this.MyKind != Kind.OutputLayer) sb.Append(this.subsidiaryLayer.ToString());// 出力層はさらに続く層がない。そのため、入力層と中間層のみ次の層を呼び出している。
            return sb.ToString();
        }
        /// <summary>
        /// 各ユニットが持つ発火の閾値を配列で取得する
        /// </summary>
        /// <returns></returns>
        private double[] GetThreshold()
        {
            double[] threshold = new double[this.units.Length];
            for (int i = 0; i < this.units.Length; i++)
                threshold[i] = this.units[i].Threshold;
            return threshold;
        }
        /// <summary>
        /// 指定された前段のユニット番号に対する重みを配列で返す
        /// <remarks>
        /// 例えば本オブジェクトが出力層である時に8という数字が指定された場合、直前の中間層の持つ第9ユニットに対する結合係数を返す。
        /// </remarks>
        /// </summary>
        /// <param name="index">前段のユニット番号</param>
        /// <returns>指定ユニットと結びついた重み</returns>
        public double[] GetWeight(int index)
        {
            double[] weight = new double[this.units.Length];
            for (int i = 0; i < this.units.Length; i++) weight[i] = this.units[i].GetWeight(index);
            return weight;
        }
        /// <summary>
        /// 各ユニットの出力誤差を配列で返す
        /// </summary>
        /// <returns>出力誤差</returns>
        public double[] GetError()
        {
            double[] error = new double[this.units.Length];
            for (int i = 0; i < error.Length; i++) error[i] = this.units[i].Error;
            return error;
        }
        /// <summary>
        /// 各ユニットの出力値を配列で返す
        /// </summary>
        /// <returns>ユニットの出力</returns>
        public double[] GetOutput()
        {
            double[] output = new double[this.units.Length];
            for (int i = 0; i < output.Length; i++) output[i] = this.units[i].Output;
            return output;
        }
        /// <summary>
        /// ユニットの出力誤差を計算し、ユニットに記憶させる
        /// </summary>
        /// <param name="NeuralNetTeacher">教師ベクトル</param>
        /// <param name="myOutput">出力ベクトル</param>
        public void CalcOwnUnitsError(Vector NeuralNetTeacher, Vector myOutput)
        {
            if (this.MyKind == Kind.OutputLayer)// 出力層の誤差を計算する-----------------
            {
                double sumError = 0.0;
                for (int i = 0; i < this.units.Length; i++)
                {
                    this.units[i].Error = myOutput[i] - NeuralNetTeacher[i];    // ユニット毎に誤差量を計算する。
                    sumError += Math.Abs(this.units[i].Error);
                    this.units[i].Error += 0.1 * myRandom.NextDouble();         // 乱数をすこーし加えると収束が速い。
                }
                totalOutputError = sumError;                                    // 出力層の出力値は学習の収束条件に利用できるので保存しておく
                double averageError = sumError / (double)this.units.Length;
                variationError = (averageError - backupError) / backupError;
                backupError = averageError;
            }
            else if (this.MyKind == Kind.HiddenLayer)// 中間層の誤差を計算する------------
            {
                double[] postError = this.subsidiaryLayer.GetError();           // 下の層の誤差を取得
                double[] postOutput = this.subsidiaryLayer.GetOutput();
                int postUnitSize = this.subsidiaryLayer.Lenght;                 // 下の層のユニット数を変数に確保（配列の判定文に入れると最適化されない限り何度も参照されるので、そのオーバーヘッドを避けるため）
                for (int i = 0; i < this.units.Length; i++)                     // この層の誤差を計算
                {
                    double[] postWeight = this.subsidiaryLayer.GetWeight(i);
                    double error = 0.0;
                    for (int j = 0; j < postUnitSize; j++)
                        error += postError[j] * postWeight[j];// * postOutput[j] * (1 - postOutput[j])
                    this.units[i].Error = error + 0.1 * myRandom.NextDouble();
                }
            }
            return;
        }
        /// <summary>
        /// 結合荷重の修正量を返す
        /// </summary>
        /// <param name="unitOutputError">ユニットの出力誤差</param>
        /// <param name="unitOutput">ユニットの出力値</param>
        /// <param name="preLayerOutput">調整したい結合荷重が接続された前段ユニットの出力値</param>
        /// <returns>重み修正量</returns>
        public double GetAmountOfCorrection(double unitOutputError, double unitOutput, double preLayerOutput)
        {
            return -learningCoefficient * unitOutputError * 2.0 * unitOutput * (1 - unitOutput) * preLayerOutput;
        }
        /// <summary>
        /// 結合係数修正量の計算、および発火の閾値修正を実施する
        /// <para>ユニットの誤差量を計算した後に呼び出してください。</para>
        /// </summary>
        /// <param name="preLayerOutput">前段の層の出力</param>
        /// <param name="myOutput">この層の出力ベクトル</param>
        /// <exception cref="SystemException">入力層で本メソッドが呼び出されるとスロー</exception>
        private void CalibrateWeightAndThreshold(Vector preLayerOutput, Vector myOutput)
        {
            if (this.MyKind != Kind.InputLayer)                                 // 出力層や中間層なら処理する
            {
                // この層が保持するユニット全てに対して処理を実施する
                for (int i = 0; i < this.units.Length; i++)
                {
                    // 前段のユニット数だけ結合係数がある。その分ループ
                    for (int j = 0; j < this.preLayerUnitNum; j++)
                    {
                        double amountOfCorrection = GetAmountOfCorrection(this.units[i].Error, myOutput[i], preLayerOutput[j]);
                        this.units[i].SetCorrectionWeight(amountOfCorrection, j);
                    }
                    // バイアスの修正
                    this.units[i].Threshold += GetAmountOfCorrection(this.units[i].Error, myOutput[i], 1.0);
                }
            }
            else
                throw new SystemException("この層は入力層なので結合係数の修正は必要ありません。");
            return;
        }
        /// <summary>
        /// 全てのユニットに対して予めセットした修正量を適用して、結合係数を調整する
        /// <para>より下層に位置するLayerの修正も指示する。</para>
        /// </summary>
        protected void CalibrateWeightVectors()
        {
            if (this.MyKind != Kind.InputLayer)
            {
                // この層の全ユニットに対して修正量を適用
                for (int i = 0; i < this.units.Length; i++)
                    this.units[i].DoCorrect();
            }
            // ネストになっている次の層の修正量の適用を指示
            if (this.MyKind != Kind.OutputLayer) this.subsidiaryLayer.CalibrateWeightVectors();
            return;
        }
        /// <summary>
        /// 特徴ベクトルを渡して、演算結果を返す
        /// </summary>
        /// <param name="preLayerOutput">特徴ベクトル／前段の出力ベクトル</param>
        /// <param name="teacher">教師ベクトル</param>
        /// <returns>演算結果</returns>
        public Vector GetResult(Vector preLayerOutput, Vector teacher)
        {
            Vector myOutput = new Vector(this.units.Length);                    // ユニット出力格納用のベクトルを用意
            // --------入力層なら、出力値はそのまま入力値とする-----------------------------
            if (this.MyKind == Kind.InputLayer)
            {
                myOutput = preLayerOutput;
                for (int i = 0; i < this.units.Length; i++) this.units[i].Output = preLayerOutput[i];
            }
            else // ---他の層なら、この層の出力ベクトルを渡して計算させて答えを格納する---
            {
                for (int i = 0; i < myOutput.Length; i++)
                    myOutput[i] = this.units[i].GetOutput(preLayerOutput);
            }
            // --------出力値を得る---------------------------------------------------------
            Vector result = new Vector(this.parameter.NumOfUnitInOutputLayer);  // 出力層の出力結果を格納するためにVectorクラス生成
            if (this.MyKind == Kind.OutputLayer)                                // 出力層での処理
                result = myOutput;                                              // 出力層の出力はそのまま結果である
            else                                                                // 出力層以外での処理
                result = this.subsidiaryLayer.GetResult(myOutput, teacher);     // 答えは、次の層から取得する（出力層のresultがリレーされる）
            // --------教師データがあれば結合係数の修正量を計算する-------------------------
            if (teacher != null && this.MyKind != Kind.InputLayer)              // 教師があれば、重みを修正する（入力層も実行してもいいけど、結合係数がないので意味ない。測度優先でここはスルーさせる。）
            {
                this.CalcOwnUnitsError(teacher, myOutput);                      // 出力エラーを計算
                this.CalibrateWeightAndThreshold(preLayerOutput, myOutput);     // 発火の閾値の調整および、結合係数の修正を実施する
            }
            // --------教師データがあり、この層が入力層であれば結合係数の修正を指示する-----
            if (teacher != null && this.MyKind == Kind.InputLayer)              // 最後にまとめて
                this.CalibrateWeightVectors();                                  // 結合係数を調律する
            return result;
        }
        /// <summary>
        /// 重みベクトルを乱数で初期化する
        /// </summary>
        public void InitWeightVectorWithRandom()
        {
            if (this.MyKind != Kind.InputLayer)
            {
                for (int i = 0; i < this.units.Length; i++) this.units[i].InitWeightVectorWithRandom();
            }
            if (this.MyKind != Kind.OutputLayer) this.subsidiaryLayer.InitWeightVectorWithRandom();
            return;
        }
        /// <summary>
        /// ファイル名を指定して結合係数を読み込ませる
        /// </summary>
        /// <param name="fname">ファイル名</param>
        public void Setup(string fname = "layer.ini")
        {
            if (System.IO.File.Exists(fname) == false) throw new SystemException("指定されたファイルは存在しません。");
            using (System.IO.StreamReader sr = new System.IO.StreamReader(fname, System.Text.Encoding.UTF8))
            {
                int layerNum = -1;
                int unitNum = -1;
                Kind kind = Kind.NA;
                string backupLine = "";
                while (sr.EndOfStream == false)
                {
                    string line = sr.ReadLine();                            // 一行読み込み
                    if (line.IndexOf("<Layer = ") >= 0)                     // 層番号を取得
                    {
                        Regex ri = new Regex(@"\d+");
                        Match m = ri.Match(line);
                        layerNum = int.Parse(m.ToString());
                    }
                    else if (line.IndexOf("<unit num=") >= 0)               // ユニット番号を取得
                    {
                        Regex ri = new Regex(@"\d+");
                        Match m = ri.Match(line);
                        unitNum = int.Parse(m.ToString());
                    }
                    else if (line.IndexOf("Hidden") >= 0) kind = Kind.HiddenLayer;  // 層の種類を取得
                    else if (line.IndexOf("Output") >= 0) kind = Kind.OutputLayer;
                    else if (layerNum == this.myLayerNum && this.MyKind == kind && backupLine == "<threshold>")
                    {
                        this.units[unitNum].Threshold = double.Parse(line);         // ユニットの閾値を取得
                    }
                    else if (layerNum == this.myLayerNum && this.MyKind == kind && backupLine == "<weight vector>")
                    {
                        Vector vect = new Vector(line);                             // 結合強度を取得
                        this.units[unitNum].SetWeightVector(vect);                  // 結合強度をセット
                    }
                    backupLine = line;
                }
            }
            if (this.MyKind != Kind.OutputLayer) this.subsidiaryLayer.Setup(fname); // 下層レイヤもセット（出力層は下層がないので呼び出しはしない）
            return;
        }
        /// <summary>
        /// レイヤークラスのコンストラクタ
        /// <para>このクラスインスタンスを一つ生成すれば、入力層から出力層までを表現可能です。</para>
        /// <para>
        /// 本クラスを生成する際、numOfPreLayerUnitは0と設定することをお勧めします。
        /// 0でなくともエラーは出ませんが、入力層には結合係数が定義されないためです。
        /// </para>
        /// </summary>
        /// <param name="_parameter">ニューラルネットのパラメータ</param>
        /// <param name="_myLayerNum">層番号</param>
        /// <param name="numOfPreLayerUnit">前の層のユニット数（省略可能です。デフォルトでは0となります。）</param>
        public Layer(Parameter _parameter, int _myLayerNum, int numOfPreLayerUnit = 0)      // 3層以上と限定すれば、numOfPreLayerUnitは要らないんだけど2層も試したかったので導入した
        {
            this.parameter = _parameter;
            this.grossLayerNum = this.parameter.TotalLayer;
            this.myLayerNum = _myLayerNum;
            // ユニットの準備と、前層のユニット数を記憶する
            switch (this.MyKind)
            {
                case Kind.InputLayer:                                                       // 入力層
                    this.units = new Unit[this.parameter.NumOfUnitInInputLayer];            // 層の種類に合わせて、ユニット数を宣言する
                    break;
                case Kind.HiddenLayer:                                                      // 中間層
                    this.units = new Unit[this.parameter.NumOfUnitInHiddenLayer];
                    break;
                case Kind.OutputLayer:                                                      // 出力層
                    this.units = new Unit[this.parameter.NumOfUnitInOutputLayer];
                    break;
            }
            for (int i = 0; i < this.units.Length; i++) this.units[i] = new Unit(numOfPreLayerUnit);        // 前の層のユニット数だけ結合係数を生成させる
            this.preLayerUnitNum = numOfPreLayerUnit;                                                       // 前の層のユニット数を記憶しておく
            if (this.MyKind != Kind.OutputLayer)
                this.subsidiaryLayer = new Layer(this.parameter, this.myLayerNum - 1, this.units.Length);   // 出力層以外なら子レイヤを生成
        }
        /// <summary>
        /// スタティックメンバ用のコンストラクタ
        /// </summary>
        static Layer()
        {
            if (Layer.myRandom == null) Layer.myRandom = new Random();
        }
    }
}
