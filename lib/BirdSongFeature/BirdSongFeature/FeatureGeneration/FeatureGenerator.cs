/***************************************************************************
 * [ソフトウェア名]
 *      FeatureGenerator.cs
 * [開発者]
 *      K.Morishita (Kumamoto-Univ. @ 2011.12)
 * [概要]
 *      鳥の鳴き声を用いて種の識別を実施するための特徴ベクトル生成器です。
 *      コアユニットと対にして使います。
 *      特徴ベクトルの生成方法を変えたい場合はコアユニットを変更してください。
 * [参考資料]
 *      
 * [検討課題]
 *      
 * [履歴]
 *      2011/12/30  開発開始
 *      2012/1/17   日付が変わった。。。三田研が伝統的に使用しているアルゴリズムに近いものを再現してみる。
 *                  肝心なところでフィルタリングしたりしているのでこちらのほうが精度高いっしょ？
 *      2012/1/17   サンプリング周波数をdouble表現に変更した（他の関連コードも）。
 *      2012/1/18   また日付が変わった。
 *                  一通りの機能を実装した。特徴ベクトルの出力まで確認したので満足。。
 *      2012/3/10   FeatureGeneratorやGeneratorにMinimumConditionのメンバをアクセスさせないように変更しました。
 *                  フィルタ情報がセットされなかった時の有効最小検出時間を最小分解能の3倍とするように更新した。
 *                  FeatureGeneratorクラスのコンストラクタにおいて、帯域情報の検査に正規表現を用いるようにした。
 *                  余計なエラーを除外することができるだろう。
 *      2012/5/4    FFT関連クラスの名前空間の変更に伴いコードを一部変更した。
 *      2012/5/18   読みやすいように、ファイルを分割する。
 *                  任意の特徴生成コアユニットを利用できるように、ジェネリック対応を進める。
 *      2012/6/16   汎用性を確保した。
 *      2012/6/19   プロパティLogのためのメンバ変数_logを削除した。
 * *************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PatternRecognition;
using Signal.FrequencyAnalysis;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using BirdSongFeature.Feature;
using BirdSongFeature.FeatureGeneration.Unit;
using BirdSongFeature.StatisticalMechanics;

namespace BirdSongFeature.FeatureGeneration
{
    /// <summary>
    /// 特徴ベクトル生成器
    /// </summary>
    /// <typeparam name="Unit">特徴ベクトル生成ユニット</typeparam>
    public class FeatureGenerator<Unit> : IFeatureGenerator
        where Unit : IUnit, new()
    {
        // ---- メンバ変数 -----------------------------------------------------------
        /// <summary>
        /// 鳴き声検出器・特徴ベクトル生成器等を組みにしたクラスのリスト
        /// <para>設定ファイルの数だけ生成します。</para>
        /// <para>構造体のままだと静的なのですが、クラスだと動的に扱えます。</para>
        /// </summary>
        List<Unit> _coreUnit = new List<Unit>(0);
        /// <summary>
        /// 特徴ベクトルのリスト
        /// <para>処理されないと次々と溜まるので注意してほしい</para>
        /// </summary>
        private List<Result> _generatedFeatures = new List<Result>(0);
        // ---- プロパティ -----------------------------------------------------------
        /// <summary>
        /// 特徴データを提供する用意状況
        /// <para>提供可能ならtureを返します。</para>
        /// </summary>
        public Boolean Ready
        {
            get {
                if (this._generatedFeatures.Count > 0)                  // リストに残っている在庫状況で確認する
                    return true;
                else
                    return false;                
            }
        }
        /// <summary>
        /// ログ保存の有無
        /// <para>trueならば保存する</para>
        /// </summary>
        public Boolean Log
        {
            set;
            get;
        }
        /// <summary>
        /// 主成分分析を通したベクトル演算を施すならtrue
        /// <para>2012/6/16 時点で未対応です。</para>
        /// </summary>
        public Boolean PcaAnalysis
        {
            set;
            get;
        }
        // ---- メソッド -------------------------------------------------------------
        /// <summary>
        /// 特徴データを返す
        /// <para>引き出した特徴データはストックから削除されます。</para>
        /// <para>特徴データの在庫が切れているとnullを返します。</para>
        /// </summary>
        /// <returns>特徴ベクトルを含む特徴データ</returns>
        public Result GetFeature()
        {
            if (this._generatedFeatures.Count > 0)                                  // リストに残っている在庫状況で確認する
            {
                Result feature = this._generatedFeatures[0];                        // リストから特徴データを引き出す
                this._generatedFeatures.RemoveAt(0);                                // 引き出したら無用なので削除
                return feature;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// FFTの結果を格納させ、検査を実施させる
        /// </summary>
        /// <param name="result">WAVEファイルをFFT処理したデータ</param>
        public void Add(FFTresult result)
        {
            //for (int i = 0; i < this._coreUnit.Count; i++)
            Parallel.For(0, this._coreUnit.Count, i =>                              // 可能なら並列で処理
            {
                this._coreUnit[i].Set(result);                                      // 発声検出器にデータを格納して発声部分の認識を行う
                if (this._coreUnit[i].Ready)
                {
                    Monitor.Enter(this._generatedFeatures);
                    Result feature = this._coreUnit[i].GetFeature();                // 生成された特徴ベクトルを取得
                    if (this.PcaAnalysis)                                           // 主成分分析を使ったベクトルのスリム化が必要なら処理する
                    {
                        var copyVector = feature.FeatureVector;
                        double[] neoVector = PCA.Convert(copyVector.ToArray());
                        var neoFeature = new Result(feature.FilterName, feature.Time, feature.TimeWidth, feature.SN, new PatternRecognition.Feature(neoVector));
                        this._generatedFeatures.Add(neoFeature);                    // 特徴データを取得可能ならもらって登録する
                    }
                    else 
                    {
                        this._generatedFeatures.Add(feature);                       // 特徴データを取得可能ならもらって登録する
                    }
                    Monitor.Exit(this._generatedFeatures);
                }
                if (this.Log) this._coreUnit[i].SaveConditionOfDetector("sound detection log " + this._coreUnit[i].FileterName + ".csv");// 必要ならファイルへ検出器のコンディションを出力
            });
            return;
        }
        /// <summary>
        /// コンストラクタ
        /// <para>ファイルを指定すると帯域制限を利用した検出器を構成します。</para>
        /// </summary>
        /// <param name="condition">コアユニットへ渡す計算条件</param>
        /// <param name="waveSamplingFrequency">Waveファイルのサンプリング周波数[Hz]</param>
        /// <param name="resamplingFrequency">リサンプリング周波数[Hz]<para>Waveファイルから0.1毎にデータを抜いているなら、1/0.1=10Hzです。</para></param>
        /// <param name="fname">読み込ませたいフィルタ情報を格納したファイルがあれば指定してください。</param>
        public FeatureGenerator(Object condition, double waveSamplingFrequency, double resamplingFrequency, string fname = "")
        {
            Boolean bandFound = false;

            if (System.IO.File.Exists(fname))
            {
                using (System.IO.StreamReader sr = new System.IO.StreamReader(fname, System.Text.Encoding.GetEncoding("UTF-8")))
                {
                    while (sr.EndOfStream == false)                         // テキストファイルを最後まで読み込む
                    {
                        string line = sr.ReadLine();
                        Unit unit = new Unit();
                        if (unit.CheckSettingStringMatch(line))             // マッチすれば以下を実行
                        {
                            bandFound = true;                               // 設定データの発見フラグを立てる
                            unit.Setup(condition, waveSamplingFrequency, resamplingFrequency, line);
                            this._coreUnit.Add(unit);
                        }
                    }
                }
            }
            if (bandFound == false)                                         // ファイルによる指定がなければ、音声の検出器を一つだけ構成する
            {
                Unit unit = new Unit();
                unit.Setup(condition, waveSamplingFrequency, resamplingFrequency, "default");
                this._coreUnit.Add(unit);
            }
            this.PcaAnalysis = false;
            this.Log = false;
            return;
        }
    }
}
