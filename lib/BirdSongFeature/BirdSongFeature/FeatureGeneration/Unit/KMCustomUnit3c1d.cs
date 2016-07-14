/***************************************************************
 * KMCustomUnit1c1d
 * 音声の検出器と特徴ベクトルの生成器を組み合わせて特徴ベクトルを生成するクラス
 * KMCustomUnit2c1dver2クラスのバージョン違いとして整備しました。
 * 
 * [開発者]
 *          K.Morishita
 * 
 * [履歴]
 *          2012/6/16   KMCustomUnit2c1dver2クラスのバージョン違いとして整備した。
 * *************************************************************/
using BirdSongFeature.FeatureGeneration.Core.KMCustom1stCore;
using BirdSongFeature.FeatureGeneration.Core.KMCustom3rdCore;

namespace BirdSongFeature.FeatureGeneration.Unit
{
    /// <summary>
    /// 音声の検出器と特徴ベクトルの生成器を組み合わせて特徴ベクトルを生成するクラス
    /// <para>KMCustomUnit2c1dver2クラスを継承しています。</para>
    /// <para>コアにCore20009mPlusPlus2クラスを使用します。</para>
    /// </summary>
    public class KMCustomUnit3c1d : KMCustomUnit2c1dver2
    {
        // ---- プロパティ -----------------------------------------------------------
        // ---- メソッド -------------------------------------------------------------
        /// <summary>
        /// コアの特徴ベクトル生成器を返す
        /// </summary>
        /// <returns>Core20009mPlusPlusオブジェクトのインスタンス</returns>
        protected override Core2009mPlus GetCoreGenerator()
        {
            return new Core2009mPlusPlus2();
        }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public KMCustomUnit3c1d()
            : base()
        {
        }
    }
}
