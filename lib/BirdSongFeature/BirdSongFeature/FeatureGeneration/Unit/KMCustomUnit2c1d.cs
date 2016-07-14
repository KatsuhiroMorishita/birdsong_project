/***************************************************************
 * KMCustomUnit1c1d
 * 音声の検出器と特徴ベクトルの生成器を組み合わせて特徴ベクトルを生成するクラス
 * KMCustomUnit1c1dクラスのバージョン違いとして整備しました。
 * 
 * [開発者]
 *          K.Morishita
 * 
 * [履歴]
 *          2012/6/16   KMCustomUnit1c1dクラスのバージョン違いとして整備した。
 * *************************************************************/
using BirdSongFeature.FeatureGeneration.Core.KMCustom1stCore;
using BirdSongFeature.FeatureGeneration.Core.KMCustom2ndCore;

namespace BirdSongFeature.FeatureGeneration.Unit
{
    /// <summary>
    /// 音声の検出器と特徴ベクトルの生成器を組み合わせて特徴ベクトルを生成するクラス
    /// <para>KMCustomUnit1c1dクラスを継承しています。</para>
    /// <para>コアにCore20009mPlusPlusクラスを使用します。</para>
    /// </summary>
    public class KMCustomUnit2c1d : KMCustomUnit1c1d
    {
        // ---- プロパティ -----------------------------------------------------------
        // ---- メソッド -------------------------------------------------------------
        /// <summary>
        /// コアの特徴ベクトル生成器を返す
        /// </summary>
        /// <returns>Core20009mPlusPlusオブジェクトのインスタンス</returns>
        protected override Core2009mPlus GetCoreGenerator()
        {
            return new Core2009mPlusPlus();
        }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public KMCustomUnit2c1d()
            : base()
        {
        }
    }
}
