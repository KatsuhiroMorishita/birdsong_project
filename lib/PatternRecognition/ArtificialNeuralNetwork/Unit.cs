using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PatternRecognition.ArtificialNeuralNetwork
{
    /// <summary>
    /// ニューラルネットのユニット
    /// <para>internal属性を付けていますので、属する名前空間の外には公開されません。</para>
    /// </summary>
    internal class Unit
    {
        /* メンバ変数 ***********************************/
        /// <summary>
        /// 前の層との間の重みベクトル
        /// </summary>
        private Vector weightVector;
        /// <summary>
        /// 前の層との間の重みベクトルの修正量
        /// </summary>
        private Vector correctionWeightVector;
        /// <summary>
        /// 入力値
        /// </summary>
        private double inputValue;
        /// <summary>
        /// 出力値
        /// </summary>
        private double outputValue;
        /// <summary>
        /// 出力誤差
        /// </summary>
        private double error;
        /// <summary>
        /// 発火の閾値
        /// </summary>
        private double threshold;
        /// <summary>
        /// 乱数ジェネレータ
        /// <para>ユニットの初期化に利用します。</para>
        /// </summary>
        static private Random myRandom;
        /* プロパティ ***********************************/
        /// <summary>
        /// 重みベクトルサイズ
        /// </summary>
        public int Length { get { return this.weightVector.Length; } }
        /// <summary>
        /// 本ユニットの出力
        /// </summary>
        public double Output { get { return this.outputValue; } set { this.outputValue = value; } }
        /// <summary>
        /// 本ユニットの誤差
        /// </summary>
        public double Error
        {
            get { return this.error; }
            set { this.error = value; }
        }
        /// <summary>
        /// 発火の閾値
        /// </summary>
        public double Threshold
        {
            get { return this.threshold; }
            set { this.threshold = value; }
        }
        /* メソッド ***********************************/
        /// <summary>
        /// 重み行列を文字列化する
        /// </summary>
        /// <returns>文字列化した重み行列</returns>
        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(5000);
            sb.Append(this.weightVector.ToString());
            return sb.ToString();
        }
        /// <summary>
        /// 指定された修正量で指定された結合係数を修正する
        /// </summary>
        /// <param name="amountOfCorrection">修正量</param>
        /// <param name="index">インデックス番号</param>
        public void CalibrateWeightVector(double amountOfCorrection, int index)
        {
            this.weightVector[index] += amountOfCorrection;
            return;
        }
        /// <summary>
        /// 結合係数の修正量をセットする
        /// <para>本メソッドを呼び出しただけでは修正されません。DoCorrect()を呼び出すことで結合係数を修正します。</para>
        /// </summary>
        /// <param name="correction"></param>
        /// <param name="index"></param>
        public void SetCorrectionWeight(double correction, int index)
        {
            this.correctionWeightVector[index] = correction;
            return;
        }
        /// <summary>
        /// 結合係数を予めセットしている修正量で修正します
        /// <para>「新しい結合係数 = 古い結合係数 + 修正量」で定義しています。</para>
        /// </summary>
        public void DoCorrect()
        {
            for (int i = 0; i < this.weightVector.Length; i++) this.weightVector[i] += this.correctionWeightVector[i];
            return;
        }
        /// <summary>
        /// 出力値を計算してメンバ変数に保存する
        /// </summary>
        private void CalcOutput()
        {
            this.outputValue = 1.0 / (1 + Math.Exp(-this.inputValue - this.threshold));
            //this.outputValue = 2.0 / (1 + Math.Exp(-this.inputValue - this.threshold)) - 1.0;
        }
        /// <summary>
        /// 本ユニットに信号を入力し、出力を返す
        /// </summary>
        /// <param name="inputSignal">入力信号（ニューラルネットなら、前の層の出力ベクトル）</param>
        /// <returns>本ユニットの出力</returns>
        public double GetOutput(Vector inputSignal)
        {
            if (this.weightVector.Length != inputSignal.Length) throw new SystemException("重みベクトルの次元数と引数の次元数が異なります。");
            this.outputValue = 0.0;
            this.inputValue = 0.0;
            for (int i = 0; i < inputSignal.Length; i++) this.inputValue += this.weightVector[i] * inputSignal[i];
            this.CalcOutput();
            return this.outputValue;
        }
        /// <summary>
        /// 入力値を加算する
        /// <para>同時に、出力も計算する</para>
        /// </summary>
        /// <param name="value"></param>
        public void AddInputValue(double value)
        {
            this.inputValue += value;
            this.CalcOutput();
            return;
        }
        /// <summary>
        /// 発火閾値を初期化する
        /// <para>発火閾値は0.0～1.0の間の乱数で初期化されます。</para>
        /// </summary>
        public void ClearThreshold()
        {
            this.threshold = this.GetRandom();
            return;
        }
        /// <summary>
        /// 入力値を初期化する
        /// <para>入力値は0.0で初期化されます。</para>
        /// </summary>
        public void ClearInputValue()
        {
            this.inputValue = this.GetRandom();
            return;
        }
        /// <summary>
        /// 重みベクトルをセット（上書き）する
        /// </summary>
        /// <param name="_weightVector">セットする重みベクトル</param>
        public void SetWeightVector(double[] _weightVector)
        {
            if (this.weightVector.Length != _weightVector.Length) throw new SystemException("重みベクトルの大きさが一致しません。");
            for (int i = 0; i < this.weightVector.Length; i++) this.weightVector[i] = _weightVector[i];
            return;
        }
        /// <summary>
        /// 重みベクトルをセット（上書き）する
        /// </summary>
        /// <param name="_weightVector">セットする重みベクトル</param>
        public void SetWeightVector(Vector _weightVector)
        {
            if (this.weightVector.Length != _weightVector.Length) throw new SystemException("重みベクトルの大きさが一致しません。");
            for (int i = 0; i < this.weightVector.Length; i++) this.weightVector[i] = _weightVector[i];
            return;
        }
        /// <summary>
        /// 乱数を返す
        /// </summary>
        /// <returns>発生させた乱数</returns>
        private double GetRandom()
        {
            double v = 4.0 * myRandom.NextDouble() - 2.0;
            if (Math.Abs(v) < 0.1) v = this.GetRandom();    // 0近くだと学習が進まないため、バイアスを設ける
            return v;
        }
        /// <summary>
        /// 重みベクトルを乱数で初期化する
        /// </summary>
        public void InitWeightVectorWithRandom()
        {
            for (int i = 0; i < this.weightVector.Length; i++) this.weightVector[i] = this.GetRandom();
            return;
        }
        /// <summary>
        /// 指定要素の重みを返す
        /// </summary>
        /// <param name="index">要素番号</param>
        /// <returns>指定された要素番号の重み</returns>
        /// <exception cref="SystemException">指定要素番号が範囲内を超えるとスロー</exception>
        public double GetWeight(int index)
        {
            if (index >= 0 && index < this.weightVector.Length)
                return this.weightVector[index];
            else
                throw new SystemException("要素が範囲内を超えました。");
        }
        /// <summary>
        /// 重みベクトルを返す
        /// </summary>
        /// <returns>重みベクトル</returns>
        public Vector GetWeightVector()
        {
            return this.weightVector;
        }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="preLayerSize">前段のユニット数</param>
        public Unit(int preLayerSize)
        {
            if (myRandom == null) myRandom = new Random();
            this.weightVector = new Vector(preLayerSize);
            this.correctionWeightVector = new Vector(this.weightVector.Length);
            this.InitWeightVectorWithRandom();
            this.ClearInputValue();
            this.ClearThreshold();
            this.Error = 0.0;
        }
    }
}
