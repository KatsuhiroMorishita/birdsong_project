using System;
using BirdSongFeature.Feature;

namespace BirdSongFeature.FeatureGeneration.Unit
{
    /// <summary>
    /// 特徴ベクトル生成ユニットのインターフェイス
    /// </summary>
    public interface IUnit
    {
        /// <summary>
        /// エラー状況を示す
        /// <para>true: エラーが発生しています</para>
        /// </summary>
        bool Error { get; }
        /// <summary>
        /// 本クラスが利用している帯域ID名
        /// </summary>
        string FileterName { get; }
        /// <summary>
        /// 文字列を演算条件設定文字列フォーマットに合致するかどうか確認する
        /// </summary>
        /// <param name="setting">設定文字列</param>
        /// <returns>true: フォーマットに合致</returns>
        Boolean CheckSettingStringMatch(string setting);
        /// <summary>
        /// 特徴ベクトルを含む検出情報を返す
        /// </summary>
        /// <returns>特徴ベクトル</returns>
        Result GetFeature();
        /// <summary>
        /// セットアップの完了の有無
        /// <para>true: セットアップ完了</para>
        /// </summary>
        bool IsSetuped { get; }
        /// <summary>
        /// 特徴ベクトルを出力可能であればture
        /// </summary>
        bool Ready { get; }
        /// <summary>
        /// 音声検出器の状況をファイルへ保存する
        /// </summary>
        /// <param name="fname">ファイル名</param>
        void SaveConditionOfDetector(string fname);
        /// <summary>
        /// WAVEファイルをFFTにかけた結果をセットする
        /// </summary>
        /// <param name="result">WaveファイルをFFT処理したデータ</param>
        void Set(Signal.FrequencyAnalysis.FFTresult result);
        /// <summary>
        /// 演算条件の設定を行う
        /// </summary>
        /// <param name="condition">特徴ベクトル生成コアユニットに渡すパラメータ</param>
        /// <param name="waveSamplingFrequency">WAVEファイルのサンプリング周波数[Hz]</param>
        /// <param name="frequencyOfFft">FFTの実施周波数[Hz]<para>0.1秒毎のWAVEファイルを処理するのであれば、1/0.1=10 Hz</para></param>
        /// <param name="bandInfo">帯域情報</param>
        void Setup(object condition, double waveSamplingFrequency, double frequencyOfFft, string bandInfo);
    }
}
