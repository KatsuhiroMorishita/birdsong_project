using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BirdSongFeature.FeatureGeneration.Core.KMCustom1stCore
{
    /// <summary>
    /// Core20009mPlusクラスの最小限の計算条件構造体
    /// <para>ファイルから読み込むようなものではない、コードレベルの設定条件での使用を想定しています。</para>
    /// </summary>
    public struct MinimumCondition
    {
        // ---- メンバ変数 -----------------------------------------------------
        /// <summary>
        /// 変調スペクトルの最大周波数[Hz]
        /// </summary>
        private readonly int _maxModulationSpectrumFrequency;
        // ---- プロパティ -----------------------------------------------------
        /// <summary>
        /// 変調スペクトルの最大周波数[Hz]
        /// <para>生成したい変調スペクトルの最大周波数</para>
        /// </summary>
        public int MaxModulationSpectrumFrequency
        {
            get { return this._maxModulationSpectrumFrequency; }
        }
        // ---- メソッド -------------------------------------------------------
        /// <summary>
        /// コンストラクタを用いた変数の初期化
        /// <para>変調スペクトルの最大周波数[Hz]を引数として渡してください。</para>
        /// </summary>
        /// <param name="maxModulationSpectrumFrequency">特徴としたい、変調スペクトルの最大周波数[Hz]</param>
        /// <exception cref="Exception">引数が0以下の場合にスロー</exception>
        public MinimumCondition(int maxModulationSpectrumFrequency)
        {
            if (maxModulationSpectrumFrequency <= 0) throw new Exception("MinimumConditionのコンストラクタにてエラーがスローされました。引数の値が不正です。");
            this._maxModulationSpectrumFrequency = maxModulationSpectrumFrequency;
        }
    }
}
