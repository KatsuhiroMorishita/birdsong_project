/***************************************************
 * UnitMember
 * ユニット名一覧
 * 
 * [開発者]
 *          K.Morishita
 * 
 * [履歴]
 *          2012/6/16   
 * *************************************************/

namespace BirdSongFeature.FeatureGeneration.Unit
{
    /// <summary>
    /// ユニット名一覧
    /// <para>使用ユニットの選択に利用できるようにしたい。</para>
    /// </summary>
    public enum UnitMember
    {
        /// <summary>
        /// 特徴ベクトル生成コアにKMCustom1stCoreを使用し、発声検出にKMCustom1stDetectorを利用するユニット
        /// </summary>
        KMCustomUnit1c1d,
        /// <summary>
        /// 特徴ベクトル生成コアにKMCustom2ndCoreを使用し、発声検出にKMCustom1stDetectorを利用するユニット
        /// </summary>
        KMCustomUnit2c1d,
        /// <summary>
        /// 特徴ベクトル生成コアにKMCustom2ndCoreを使用し、発声検出にKMCustom1stDetectorを利用するユニット
        /// <para>無音区間が閾値[s]以下で有れば結合します。</para>
        /// </summary>
        KMCustomUnit2c1dver2,
        /// <summary>
        /// 特徴ベクトル生成コアにKMCustom3rdCoreを使用し、発声検出にKMCustom1stDetectorを利用するユニット
        /// <para>無音区間が閾値[s]以下で有れば結合します。</para>
        /// <para>また、変調スペクトルの分解能を0.1 Hzとしています。</para>
        /// </summary>
        KMCustomUnit3c1d
    }
}
