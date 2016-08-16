using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BirdSongFeature.FeatureGeneration.Core.KMCustom1stCore;
using BirdSongFeature.FeatureGeneration.Core.KMCustom7thCore;

namespace BirdSongFeature.FeatureGeneration.Unit
{
    /// <summary>
    /// 音声の検出器と特徴ベクトルの生成器を組み合わせて特徴ベクトルを生成するクラス
    /// <para>KMCustomUnit2c1dver2クラスを継承しています。</para>
    /// <para>コアにCore2016m1クラスを使用します。</para>
    /// </summary>
    class KMCustomUnit7c1d : KMCustomUnit2c1dver2
    {
        // ---- プロパティ -----------------------------------------------------------
        // ---- メソッド -------------------------------------------------------------
        /// <summary>
        /// コアの特徴ベクトル生成器を返す
        /// </summary>
        /// <returns>Core20009mPlusPlusオブジェクトのインスタンス</returns>
        protected override Core2009mPlus GetCoreGenerator()
        {
            return new Core2016m3();
        }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public KMCustomUnit7c1d()
            : base()
        {
        }
    }
}
