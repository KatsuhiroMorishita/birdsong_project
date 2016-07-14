/****************************************************
 * Result
 * 生成された特徴ベクトル情報クラス
 * 
 * [履歴]
 *          2012/6/16   SN比の扱いをdouble?からdoubleへ変更した。
 *          2012/6/23   ToStringのコードの無駄を排除した。
 *                      プロパティDiscriminationResultを追加した。
 *                      プロパティNameをFilterNameへ変更した。
 * **************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PatternRecognition;
using Signal.FrequencyAnalysis;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using BirdSongFeature.FeatureGeneration.Core;

namespace BirdSongFeature.Feature
{
    /// <summary>
    /// 生成された特徴ベクトル情報クラス
    /// <para>nullを返せるようにクラスとしている。</para>
    /// <para>返すのは、帯域情報と時刻と特徴ベクトルの3つです。</para>
    /// </summary>
    public class Result
    {
        // ---- メンバ変数 ------------------------------------------------------
        /// <summary>
        /// 帯域情報名
        /// </summary>
        private readonly string _filterName;
        /// <summary>
        /// 検出時刻[s]
        /// <para>発声部分として検出された区間の検出立ち上がり時刻を表します。</para>
        /// </summary>
        private readonly double _time;
        /// <summary>
        /// 検出期間[s]
        /// <para>発声区間の時間幅です。</para>
        /// </summary>
        private readonly double _timeWidth;
        /// <summary>
        /// SN比
        /// </summary>
        private double _SNratio;
        /// <summary>
        /// 特徴ベクトル
        /// </summary>
        private readonly PatternRecognition.Feature _feature;
        // ---- プロパティ ------------------------------------------------------
        /// <summary>
        /// 帯域情報名
        /// </summary>
        public string FilterName { get { return this._filterName; } }
        /// <summary>
        /// 識別結果
        /// </summary>
        public string DiscriminationResult
        {
            get;
            set;
        }
        /// <summary>
        /// 検出時刻[s]
        /// <para>発声部分として検出された区間の検出立ち上がり時刻を表します。</para>
        /// </summary>
        public double Time { get { return this._time; } }
        /// <summary>
        /// 検出期間[s]
        /// <para>発声区間の時間幅です。</para>
        /// </summary>
        public double TimeWidth { get { return this._timeWidth; } }
        /// <summary>
        /// SN比
        /// </summary>
        public double SN { get { return this._SNratio; } }
        /// <summary>
        /// 特徴ベクトル
        /// </summary>
        public PatternRecognition.Feature FeatureVector { get { return this._feature; } }
        // ---- メソッド --------------------------------------------------------
        /// <summary>
        /// 文字列化
        /// </summary>
        /// <returns>パラメータを文字列として返します</returns>
        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(400);
            sb.Append(this._filterName).Append(",").Append(this.DiscriminationResult).Append(",").Append(this._time.ToString("0.00")).Append(",").Append(this._timeWidth.ToString("0.00")).Append(",");
            if (this._SNratio != double.NaN)
                sb.Append(this._SNratio.ToString("0.00"));
            else
                sb.Append("NA");
            sb.Append(",").Append(this._feature.ToString());
            return sb.ToString();
        }
        /// <summary>
        /// コンストラクタです。
        /// <para>パラメータの初期化を行います。</para>
        /// </summary>
        /// <param name="filterName">帯域フィルタ名</param>
        /// <param name="time">検出時刻[s]<para>発声部分として検出された区間の検出立ち上がり時刻を表します。</para></param>
        /// <param name="timeWidth">検出期間[s]<para>発声区間の時間幅です。</para></param>
        /// <param name="SNratio">SN比</param>
        /// <param name="feature">特徴ベクトル</param>
        public Result(string filterName, double time, double timeWidth, double SNratio, PatternRecognition.Feature feature)
        {
            this._filterName = filterName;
            this.DiscriminationResult = "";
            this._time = time;
            this._timeWidth = timeWidth;
            this._SNratio = SNratio;
            this._feature = feature;
            return;
        }
    }
}
