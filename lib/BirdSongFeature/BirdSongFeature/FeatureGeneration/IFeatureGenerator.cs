/*************************************************
 * IFeatureGenerator
 * FeatureGeneratorクラスのインターフェイス
 * 
 * [開発者]
 *          K.Morishita
 * 
 * [履歴]
 *          2012/6/16   バージョン違いのFeatureGeneratorを取り扱うために新設
 * ***********************************************/
using System;
using BirdSongFeature.Feature;

namespace BirdSongFeature.FeatureGeneration
{
    /// <summary>
    /// FeatureGeneratorクラスのインターフェイス
    /// </summary>
    interface IFeatureGenerator
    {
        /// <summary>
        /// FFT結果を入力する
        /// </summary>
        /// <param name="result">FFT演算結果</param>
        void Add(Signal.FrequencyAnalysis.FFTresult result);
        /// <summary>
        /// 特徴ベクトルを取得する
        /// </summary>
        /// <returns>特徴ベクトル</returns>
        Result GetFeature();
        /// <summary>
        /// 動作ログを取るかどうかの設定
        /// </summary>
        bool Log { get; set; }
        /// <summary>
        /// 主成分分析を通したベクトル演算を施すならtrue
        /// </summary>
        bool PcaAnalysis { get; set; }
        /// <summary>
        /// 特徴ベクトルを出力する準備が完了するとtrue
        /// </summary>
        bool Ready { get; }
    }
}
