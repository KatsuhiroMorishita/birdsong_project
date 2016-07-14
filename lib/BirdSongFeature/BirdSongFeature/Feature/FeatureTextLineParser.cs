/***************************************************************************
 * [ソフトウェア名]
 *      FeatureTextLineParserクラス
 * [開発者]
 *      K.Morishita (Kumamoto-Univ. @ 2013.3)
 * [概要]
 *      鳥の音声解析用に開発したソフトウェアに必要なクラスです。
 *      特徴ベクトルを記述した文字列を解析します。
 *      
 *      解析文字列の例：
 *      キビタキ,雑音,226.23,0.67,2.32,0.0000E+000,0.0000E+000,0.0000E+000,0.0000E+000,2.9759E-001,1.0000E+000,9.7886E-001,9.9056E-001,7.9822E-001,7.3209E-001,6.7102E-001,6.0100E-001,5.1368E-001,8.1413E-002,0.0000E+000,0.0000E+000,1.0000E+000,7.0763E-001,2.1961E-001,2.3952E-002,1.4844E-002,1.3555E-002,2.1375E-002,1.9162E-002,9.5383E-003,2.1289E-003,7.2383E-003,1.1421E-002,6.1873E-003,1.8813E-003,4.3051E-003,9.5632E-003,1.2397E-002,1.1428E-002,1.0094E-002,9.7282E-003,7.7874E-003,2.9979E-003,0.0000E+000,0.0000E+000,0.0000E+000,0.0000E+000,0.0000E+000,0.0000E+000,0.0000E+000,0.0000E+000,0.0000E+000,0.0000E+000,0.0000E+000,0.0000E+000,0.0000E+000,0.0000E+000,0.0000E+000,0.0000E+000,0.0000E+000,0.0000E+000,0.0000E+000,0.0000E+000,0.0000E+000,0.0000E+000,0.0000E+000,0.0000E+000,0.0000E+000,0.0000E+000,0.0000E+000,0.0000E+000,C:\test\20120401_0500_0700_DS710174_0.wav,#コメントには大抵の文字を使うことができます。特徴ベクトルに対するメモ欄として活用して下さい。
 * 
 *      区切り文字はタブか半角カンマです。
 *      コメント用の正規表現にはとても全角の記号全てを書ききれていません。
 *      コメント内では半角カンマとタブの使用はできません。
 *      使用できない文字に注意して下さい。
 * [参考資料]
 *      
 * [検討課題]
 *      
 * [履歴]
 *      2013/3/16   開発開始
 * *************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Text.RegularExpressions;   // for Regex

namespace BirdSongFeature.Feature
{
    /// <summary>
    /// 特徴ベクトルを記述した文字列を解析するクラス
    /// </summary>
    public class FeatureTextLineParser
    {
        /// <summary>
        /// 文字列解析用正規表現モジュール
        /// </summary>
        private static Regex re = null;
        /// <summary>
        /// 特徴ベクトル
        /// <para>デリミタ付き文字列です。</para>
        /// </summary>
        private string _feature;
        // **** プロパティ *************************************************
        /// <summary>
        /// エラーメッセージ
        /// </summary>
        public static string ErrorMsg
        {
            get;
            private set;
        }
        /// <summary>
        /// 直前の解析結果
        /// <para>true: 解析に成功しています。</para>
        /// <para>なお、デフォルト値はfalseです。</para>
        /// </summary>
        public bool Success
        {
            get;
            private set;
        }
        /// <summary>
        /// 特徴ベクトル生成に用いた帯域（バンド）情報
        /// </summary>
        public string BandLabel
        {
            get;
            private set;
        }
        /// <summary>
        /// 識別結果
        /// </summary>
        public string Result
        {
            get;
            private set;
        }
        /// <summary>
        /// 検出時刻[s]
        /// </summary>
        public string StartTime
        {
            get;
            private set;
        }
        /// <summary>
        /// 検出時間幅[s]
        /// </summary>
        public string TimeWidth
        {
            get;
            private set;
        }
        /// <summary>
        /// SN比
        /// </summary>
        public string SN
        {
            get;
            private set;
        }
        /// <summary>
        /// 特徴ベクトル
        /// <para>特徴量をデリミタで結合した文字列です。</para>
        /// </summary>
        public string Feature
        {
            get 
            {
                return this._feature;
            }
            private set
            {
                if(value != "")
                    this._feature = value.Remove(0, 1);             // 文字列先頭に付くデリミタは削除しておく（Featureクラス仕様への対応）
            }
        }
        /// <summary>
        /// 音源ファイルのパス
        /// </summary>
        public string FilePath
        {
            get;
            private set;
        }
        /// <summary>
        /// コメント
        /// </summary>
        public string Comment
        {
            get;
            private set;
        }
        // **** メソッド *************************************************
        /// <summary>
        /// 初期化します
        /// </summary>
        private void Init()
        {
            this.Success = false;
            this.BandLabel = "";
            this.Result = "";
            this.StartTime = "";
            this.TimeWidth = "";
            this.SN = "";
            this.Feature = "";
            this.FilePath = "";
            this.Comment = "";
            return;
        }
        /// <summary>
        /// 引数で渡した文字列で初期化します
        /// <para>解析できなかった場合でも、メンバは初期化されます。</para>
        /// </summary>
        /// <param name="line">解析したい文字列</param>
        public void SetLine(string line)
        {
            this.Init();
            if (FeatureTextLineParser.re != null)
            {
                Match m = FeatureTextLineParser.re.Match(line);                // マッチチェック
                if (m.Success)
                {
                    this.Success = true;
                    this.BandLabel = m.Groups["bandLabel"].Value;
                    this.Result = m.Groups["result"].Value;
                    this.StartTime = m.Groups["starttime"].Value;
                    this.TimeWidth = m.Groups["timewidth"].Value;
                    this.SN = m.Groups["sn"].Value;
                    this.Feature = m.Groups["feature"].Value;
                    this.FilePath = m.Groups["filepath"].Value;
                    this.Comment = m.Groups["comment"].Value;
                }
            }
        }
        /// <summary>
        /// スタティックコンストラクタ
        /// </summary>
        static FeatureTextLineParser()
        {
            FeatureTextLineParser.ErrorMsg = "";
            try
            {
                FeatureTextLineParser.re = new Regex(
                    @"^"
                    + @"(?<bandLabel>(\w|\s)+)(\t|,)"                               // バンドフィルタ名（ユーザーにより、テキストファイル内に任意に記述されている）
                    + @"(?<result>(\w|\s)+)?(\t|,)"                                 // 結果の格納フィールド
                    + @"(?<starttime>\d+(\.)?\d*)(\t|,)"                            // 鳴き始め時刻[s]のフィールド
                    + @"(?<timewidth>\d+(\.)?\d*)(\t|,)"                            // 検出時間幅[s]のフィールド
                    + @"(?<sn>((\d+(\.)?\d*)|\+∞|NA))"                             // SN比のフィールド
                    + @"(?<feature>((\t|,)(?<value>\d\.?\d*((E|e)-?\+?\d+)?))+)"    // 特徴ベクトルのフィールド。×10^n表現にも対応。ここまでは必須フィールド
                    + @"((\t|,)(?<filepath>[A-Z]:(\w|\\|\.|\s|-)+))?"               // ファイルパスのフィールド
                    + @"((\t|,)#(?<comment>(\w|\\|\.|\s|-|[/]|:|,|\t|@|#|%|$|!|\^|(|)|\""|／|！|“|”|＃|＄|％|＆|’|（|）|＝|～|｜|‘|｛|＋|＊|｝|＜|＞|？|＿|－|＾|￥|＠|「|；|：|」|，|、|。|・)+))?"// コメントのフィールド。半角のシャープ以降はコメントとみなす　。
                    + @"(\t|,)?"
                    + @"$"
                    );
            }
            catch (SystemException e)
            {
                FeatureTextLineParser.ErrorMsg = "例外がスローされました。正規表現の宣言箇所チェックしてください。\nerror msg.: " + e.Message;
            }
        }
        /// <summary>
        /// デフォルトのコンストラクタ
        /// <para>メンバは""で初期化されます。</para>
        /// </summary>
        public FeatureTextLineParser()
        {
            this.Init();
        }
        /// <summary>
        /// 解析したい文字列を渡して初期化するコンストラクタ
        /// </summary>
        /// <param name="arg">解析したい文字列</param>
        public FeatureTextLineParser(string arg)
            : this()
        {
            this.SetLine(arg);
        }
    }
}
