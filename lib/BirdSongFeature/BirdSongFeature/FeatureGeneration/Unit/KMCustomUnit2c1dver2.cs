/***************************************************************
 * KMCustomUnit2c1dver2
 * 音声の検出器と特徴ベクトルの生成器を組み合わせて特徴ベクトルを生成するクラス
 * KMCustomUnit2c1dクラスのバージョン違いとして整備しました。
 * 
 * [開発者]
 *          K.Morishita
 * 
 * [履歴]
 *          2012/6/16   KMCustomUnit2c1dクラスのバージョン違いとして整備した。
 *          2012/6/26   連続して鳴くといっても長々と鳴く鳥もいるので、最大10秒と制限した。
 * *************************************************************/
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
using BirdSongFeature.FeatureGeneration.Core.KMCustom1stCore;
using BirdSongFeature.FeatureGeneration.Core.KMCustom2ndCore;
using BirdSongFeature.SongDetection.KMCustom1stDetector;

namespace BirdSongFeature.FeatureGeneration.Unit
{
    /// <summary>
    /// 音声の検出器と特徴ベクトルの生成器を組み合わせて特徴ベクトルを生成するクラス
    /// <para>KMCustomUnit2c1dクラスを継承しています。</para>
    /// <para>コアにCore20009mPlusPlusクラスを使用します。</para>
    /// </summary>
    public class KMCustomUnit2c1dver2 : KMCustomUnit2c1d
    {
        // ---- メンバ変数 -----------------------------------------------------------
        /// <summary>
        /// 発声非検出区間の時間
        /// </summary>
        private double nonDetectionTime;
        // ---- プロパティ -----------------------------------------------------------
        /// <summary>
        /// 結合の閾値
        /// <para>無音区間がこれ以下であれば結合する。</para>
        /// </summary>
        public double Threshold
        {
            get;
            set;
        }
        // ---- メソッド -------------------------------------------------------------
        /// <summary>
        /// WAVEファイルをFFTにかけた結果をセットする
        /// <para>KMCustomUnit2c1dクラスとの違いは、連続して鳴く鳥に対応しているという点です。</para>
        /// <para>背景ノイズが人いときはノイズと鳴き声がいっしょくたにされますけど、運が悪かったということで諦めるという方針です。</para>
        /// </summary>
        /// <param name="result">WaveファイルをFFT処理したデータ</param>
        public override void Set(FFTresult result)
        {
            if (this.IsSetuped)
            {
                this._time += 1.0 / this._frequencyOfFFT;                           // 既セット分から経過時刻を計算する
                this._sensor.Set(result);                                           // 検出器は内部でフィルタリングするため、下のフィルタリング後のデータを渡さなくともよい
                FFTresult filteredResult = result.Filter(this._band);               // フィルタをかけたオブジェクトを作る
                if (this._sensor.Detection)                                         // 鳴き声があればtrue（になって欲しい）
                {
                    this._genetator.Add(filteredResult);
                    this.nonDetectionTime = 0.0;
                }
                else
                {
                    if (this.nonDetectionTime < this.Threshold) this._genetator.Add(filteredResult);
                    if ((this.nonDetectionTime > this.Threshold || this._genetator.TimeOfStock > 10.0) && this._genetator.Ready)
                    {
                        double timeWidth = this._genetator.TimeOfStock;
                        double startTime = this._time - timeWidth;                  // 検出開始時刻を計算
                        var feature = this._genetator.GetFeature();                 // 準備が完了しているようなら特徴ベクトルを作らせる
                        if (feature != null) this._generatedFeatures.Add(new Result(this.FileterName, startTime, timeWidth, this._genetator.SN, feature));  // nullでなければ、特徴ベクトルとして追加する
                        this._genetator.Init(filteredResult);                       // 特徴ベクトル生成器をリセット
                    }
                    if (this._genetator.Ready == false) this._genetator.Init(filteredResult); // 特徴ベクトル生成器をリセット
                       
                    this.nonDetectionTime += 1.0 / this._frequencyOfFFT;
                }
            }
            return;
        }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public KMCustomUnit2c1dver2()
            : base()
        {
            this.nonDetectionTime = 0.0;
            this.Threshold = 0.25;
        }
    }
}
