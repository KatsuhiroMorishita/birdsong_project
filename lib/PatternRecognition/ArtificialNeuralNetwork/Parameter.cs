/*******************************************
 * Parameter
 * ニューラルネットワークのパラメータ構造体
 * 
 * [履歴]
 *          2012/6/18   コメント見直し
 *                      運用の融通性を上げるために、いくつか見直し中。
 *                      CheckARGV()でエラーをチェックしていたが、これをErrorプロパティへ機能を移した。
 *                      エラーを検出しても本構造体ではエラーをスローしないようにする。
 *                      参照するコードでは、プロパティErrorを参照して対処すること。
 *                      メンバ変数とプロパティの見直しでメンバ数を減らした。
 * *****************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;


namespace PatternRecognition.ArtificialNeuralNetwork
{
    /// <summary>
    /// ニューラルネットワークのパラメータ構造体
    /// <para>主にパラメータの受け渡しと設定値の不正チェックを目的とする。</para>
    /// </summary>
    public struct Parameter
    {
        /************** 定数 **********************/
        /// <summary>
        /// 入力層のユニット数がこの文字列に続きます
        /// </summary>
        private const string keywordOfUnitInputLayer = "Number of unit in input-layer=";    // 検索・保存に利用する
        /// <summary>
        /// 中間層のユニット数がこの文字列に続きます
        /// </summary>
        private const string keywordOfUnitHiddenLayer = "Number of unit in hidden-layer=";
        /// <summary>
        /// 出力層のユニット数がこの文字列に続きます
        /// </summary>
        private const string keywordOfUnitOutputLayer = "Number of unit in output-layer=";
        /// <summary>
        /// 中間層数がこの文字列に続きます
        /// </summary>
        private const string keywordOfNumHiddenLayer = "Number of hidden-layer=";
        /// <summary>
        /// 学習係数がこの文字列に続きます
        /// </summary>
        private const string keywordOfLearningCoefficient = "Learning coefficient=";
        /************** メンバ変数 **********************/
        /// <summary>
        /// 中間層数
        /// </summary>
        private readonly int numOfHiddenLayer;
        /************** プロパティ **********************/
        /// <summary>
        /// 入力層のユニット数
        /// </summary>
        public int NumOfUnitInInputLayer 
        {
            get;
            set;
        }
        /// <summary>
        /// 中間層のユニット数
        /// </summary>
        public int NumOfUnitInHiddenLayer 
        {
            get;
            set;
        }
        /// <summary>
        /// 出力層のユニット数
        /// </summary>
        public int NumOfUnitInOutputLayer 
        {
            get;
            set;
        }
        /// <summary>
        /// 合計層数
        /// </summary>
        public int TotalLayer 
        {
            get { return this.numOfHiddenLayer + 2; } 
        }
        /// <summary>
        /// 学習係数
        /// </summary>
        public double LearningCoefficient 
        {
            get;
            set; 
        }
        /// <summary>
        /// エラー状況
        /// <para>true: エラーあり</para>
        /// </summary>
        public Boolean Error
        {
            get 
            {
                if (NumOfUnitInInputLayer == 0 ||       // 各層のユニット数が0はあり得ない。
                    NumOfUnitInHiddenLayer == 0 ||
                    NumOfUnitInOutputLayer == 0 ||
                    numOfHiddenLayer < 0 ||             // 中間層数が0は在り得ても負値は在り得ない
                    LearningCoefficient == 0.0)         // 学習係数0もあり得ない
                    return true;
                else
                    return false;
            }
        }
        /************** メソッド **********************/
        /// <summary>
        /// パラメータを文字列として返す
        /// </summary>
        /// <returns>文字列化したパラメータ</returns>
        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(1500);
            sb.Append("<Neural Network Parameter>").Append("\n");
            sb.Append(keywordOfUnitInputLayer).Append(this.NumOfUnitInInputLayer.ToString()).Append("\n");
            sb.Append(keywordOfUnitHiddenLayer).Append(this.NumOfUnitInHiddenLayer.ToString()).Append("\n");
            sb.Append(keywordOfUnitOutputLayer).Append(this.NumOfUnitInOutputLayer.ToString()).Append("\n");
            sb.Append(keywordOfNumHiddenLayer).Append(this.numOfHiddenLayer.ToString()).Append("\n");
            sb.Append(keywordOfLearningCoefficient).Append(this.LearningCoefficient.ToString("0.00#")).Append("\n");
            sb.Append("</Neural Network Parameter>").Append("\n");
            return sb.ToString();
        }
        /// <summary>
        /// ファイル名を指定して、パラメータをファイルから読み込ませるコンストラクタ
        /// </summary>
        /// <param name="fname">ファイル名<para>デフォルト："Neural Network Parameter.txt"</para></param>
        public Parameter(string fname = "Neural Network Parameter.txt")
            :this()
        {
            int _numOfUnitInInputLayer = 0, _numOfUnitInHiddenLayer = 0, _numOfUnitInOutputLayer = 0, _numOfHiddenLayer = 0;
            double _learningCoefficient = 0.0;

            if (System.IO.File.Exists(fname) == false) throw new SystemException("指定されたファイルは存在しません。");
            using (System.IO.StreamReader sr = new System.IO.StreamReader(fname, System.Text.Encoding.UTF8))
            {
                Regex ri = new Regex(@"\d+");
                Regex rd = new Regex(@"\d+.\d+");
                while (sr.EndOfStream == false)
                {
                    string line = sr.ReadLine();                            // 一行読み込み
                    Match m = ri.Match(line);
                    int i = 0;
                    if (m.ToString() != "") i = int.Parse(m.ToString());
                    m = rd.Match(line);
                    double d = 0.0;
                    if (m.ToString() != "") d = double.Parse(m.ToString());
                    if (line.IndexOf(Parameter.keywordOfUnitInputLayer) >= 0) _numOfUnitInInputLayer = i;
                    if (line.IndexOf(Parameter.keywordOfUnitHiddenLayer) >= 0) _numOfUnitInHiddenLayer = i;
                    if (line.IndexOf(Parameter.keywordOfUnitOutputLayer) >= 0) _numOfUnitInOutputLayer = i;
                    if (line.IndexOf(Parameter.keywordOfNumHiddenLayer) >= 0) _numOfHiddenLayer = i;
                    if (line.IndexOf(Parameter.keywordOfLearningCoefficient) >= 0) _learningCoefficient = d;
                }
            }
            this.NumOfUnitInInputLayer = _numOfUnitInInputLayer;
            this.NumOfUnitInHiddenLayer = _numOfUnitInHiddenLayer;
            this.NumOfUnitInOutputLayer = _numOfUnitInOutputLayer;
            this.numOfHiddenLayer = _numOfHiddenLayer;
            this.LearningCoefficient = _learningCoefficient;
        }
        /// <summary>
        /// 全パラメータを指定するコンストラクタ
        /// </summary>
        /// <param name="_numOfUnitInInputLayer">入力層のユニット数<para>特徴ベクトルの次元数と同じ数を指定して下さい。</para></param>
        /// <param name="_numOfUnitInHiddenLayer">中間層のユニット数<para>この数が多いと、過学習に陥りやすくなります。n個で2^n状態を表すことができることを考慮して決定して下さい。</para></param>
        /// <param name="_numOfUnitInOutputLayer">出力層のユニット数<para>この数は出力状態量と同じ数を指定して下さい。</para></param>
        /// <param name="_numOfHiddenLayer">中間層数<para>この数+1だけ識別面（超平面）が形成されます。特徴量の分布をよく検討しなければまずいことになりそうです。</para></param>
        /// <param name="_learningCoefficient">学習係数<para>この数が大きいと結合係数の修正量が大きくなりすぎていつまでも収束しなくなります。かといって小さいといつまでも終わりません。カットアンドトライで確認してみてください。</para></param>
        public Parameter(int _numOfUnitInInputLayer, int _numOfUnitInHiddenLayer, int _numOfUnitInOutputLayer, int _numOfHiddenLayer, double _learningCoefficient)
            :this()
        {
            if (_numOfUnitInHiddenLayer == 0) Console.WriteLine("中間層数が0でしたので中間層のユニット数は無視しました。");// NNに実質無視されるという話
            this.NumOfUnitInInputLayer = _numOfUnitInInputLayer;
            this.NumOfUnitInHiddenLayer = _numOfUnitInHiddenLayer;
            this.NumOfUnitInOutputLayer = _numOfUnitInOutputLayer;
            this.numOfHiddenLayer = _numOfHiddenLayer;
            this.LearningCoefficient = _learningCoefficient;
            return;
        }
        /// <summary>
        /// パラメータをMinParameterとパラで指定するコンストラクタ
        /// </summary>
        /// <param name="minParameter">最小パラメータ</param>
        /// <param name="_numOfUnitInInputLayer">入力層のユニット数<para>特徴ベクトルの次元数と同じ数を指定して下さい。</para></param>
        /// <param name="_numOfUnitInOutputLayer">出力層のユニット数<para>この数は出力状態量と同じ数を指定して下さい。</para></param>
        public Parameter(Object minParameter, int _numOfUnitInInputLayer, int _numOfUnitInOutputLayer)
            : this()
        {
            if (minParameter is MinParameter)
            {
                MinParameter _minPara = (MinParameter)minParameter;
                if (_minPara.NumOfHiddenLayer == 0) Console.WriteLine("中間層数が0でしたので中間層のユニット数は無視しました。");
                this.NumOfUnitInInputLayer = _numOfUnitInInputLayer;
                this.NumOfUnitInHiddenLayer = _minPara.NumOfUnitInHiddenLayer;
                this.NumOfUnitInOutputLayer = _numOfUnitInOutputLayer;
                this.numOfHiddenLayer = _minPara.NumOfHiddenLayer;
                this.LearningCoefficient = _minPara.LearningCoefficient;
            }
            else
                throw new Exception("不正なパラメータが渡されました。minParameterの型をチェックして下さい。");
            return;
        }
    }
}
