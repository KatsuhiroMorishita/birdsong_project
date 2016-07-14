/*******************************************
 * MinParameter
 * ニューラルネットワークの最小のパラメータ構造体
 * 
 * [履歴]
 *          2012/6/22   新設
 * *****************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PatternRecognition.ArtificialNeuralNetwork
{
    /// <summary>
    /// NN用最小パラメータ
    /// </summary>
    public struct MinParameter
    {
        /// <summary>
        /// 中間層のユニット数
        /// </summary>
        public int NumOfUnitInHiddenLayer
        {
            get;
            set;
        }
        /// <summary>
        /// 中間層数
        /// </summary>
        public int NumOfHiddenLayer
        {
            get;
            set;
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
        /// ニューラルネットワーク用の最小演算条件を表すMinParameter構造体のコンストラクタ
        /// </summary>
        /// <param name="_numOfUnitInHiddenLayer">中間層のユニット数<para>この数が多いと、過学習に陥りやすくなります。n個で2^n状態を表すことができることを考慮して決定して下さい。</para></param>
        /// <param name="_numOfHiddenLayer">中間層数<para>この数+1だけ識別面（超平面）が形成されます。特徴量の分布をよく検討しなければまずいことになりそうです。</para></param>
        /// <param name="_learningCoefficient">学習係数<para>この数が大きいと結合係数の修正量が大きくなりすぎていつまでも収束しなくなります。かといって小さいといつまでも終わりません。カットアンドトライで確認してみてください。</para></param>
        public MinParameter(int _numOfUnitInHiddenLayer, int _numOfHiddenLayer, double _learningCoefficient)
            :this()
        {
            this.NumOfUnitInHiddenLayer = _numOfUnitInHiddenLayer;
            this.NumOfHiddenLayer = _numOfHiddenLayer;
            this.LearningCoefficient = _learningCoefficient;
        }
    }
}
