using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BirdSongFeature.FeatureGeneration.Core.KMCustom1stCore
{
    /// <summary>
    /// Core20009mPlusクラスの計算条件構造体
    /// <para>設計する特徴ベクトル生成器に合わせて中身のメンバーを設計し直して下さい。</para>
    /// </summary>
    public struct Condition
    {
        // ---- メンバ変数 -----------------------------------------------------
        /// <summary>
        /// 変調スペクトルの最大周波数[Hz]
        /// </summary>
        private readonly int _maxModulationSpectrumFrequency;
        /// <summary>
        /// 渡されるFFTの頻度[Hz]<para>例えば、WAVEファイルから0.1秒ずつ読み込んでFFTをしているのであれば、1/0.1=10Hzです。</para>
        /// </summary>
        private readonly double _frequencyOfFFT;
        /// <summary>
        /// 最低の時間幅<para>鳴き声がこの時間幅以上に追加された場合に特徴ベクトルの出力準備が整ったと判断します。</para>
        /// </summary>
        private readonly double _minTimeWidth;
        // ---- プロパティ -----------------------------------------------------
        /// <summary>
        /// 変調スペクトルの最大周波数[Hz]
        /// <para>生成したい変調スペクトルの最大周波数</para>
        /// </summary>
        public int MaxModulationSpectrumFrequency
        {
            get { return this._maxModulationSpectrumFrequency; }
        }
        /// <summary>
        /// 渡されるFFTの頻度[Hz]<para>例えば、WAVEファイルから0.1秒ずつ読み込んでFFTをしているのであれば、1/0.1=10Hzです。</para>
        /// </summary>
        public double FrequencyOfFFT
        {
            get { return this._frequencyOfFFT; }
        }
        /// <summary>
        /// 最低の時間幅<para>鳴き声がこの時間幅以上に追加された場合に特徴ベクトルの出力準備が整ったと判断します。</para>
        /// </summary>
        public double MinTimeWidth
        {
            get { return this._minTimeWidth; }
        }
        // ---- メソッド -------------------------------------------------------
        /// <summary>
        /// 計算条件を生成する
        /// <para>FeatureGeneratorCoreUnitクラスの変更に伴い、この呼び出し部も変更する必要があります。</para>
        /// </summary>
        /// <param name="minimumCondition">最小の計算条件</param>
        /// <param name="frequencyOfFFT">渡されるFFTの頻度[Hz]<para>例えば、WAVEファイルから0.1秒ずつ読み込んでFFTをしているのであれば、1/0.1=10Hzです。</para></param>
        /// <param name="minTimeWidth">最低の時間幅<para>鳴き声がこの時間幅以上に追加された場合に特徴ベクトルの出力準備が整ったと判断します。</para></param>
        public Condition(MinimumCondition minimumCondition, double frequencyOfFFT, double minTimeWidth)
        {
            this._maxModulationSpectrumFrequency = minimumCondition.MaxModulationSpectrumFrequency; // 特徴としたい、変調スペクトルの最大周波数[Hz]
            this._frequencyOfFFT = frequencyOfFFT;
            this._minTimeWidth = minTimeWidth;
        }
    }
}
