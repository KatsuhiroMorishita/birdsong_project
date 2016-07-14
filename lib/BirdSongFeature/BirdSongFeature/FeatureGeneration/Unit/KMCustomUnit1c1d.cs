/***************************************************************
 * KMCustomUnit1c1d
 * 音声の検出器と特徴ベクトルの生成器を組み合わせて特徴ベクトルを生成するクラス
 * 
 * [開発者]
 *          K.Morishita
 * 
 * [履歴]
 *          2012/6/16   IUnitインターフェイスの形を固める
 *                      発生検出帯域に対して、特徴生成用帯域フィルタは上下両方に1 kHzの幅を持たせることとした。
 *          2012/6/18   特徴ベクトル生成器に対して、最後の閉めのデータを保存していなかったので保存するように変更した。
 * *************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PatternRecognition;
using Signal.FrequencyAnalysis;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using BirdSongFeature.Feature;
using BirdSongFeature.FeatureGeneration.Core.KMCustom1stCore;
using BirdSongFeature.SongDetection.KMCustom1stDetector;

namespace BirdSongFeature.FeatureGeneration.Unit
{
    /// <summary>
    /// 音声の検出器と特徴ベクトルの生成器を組み合わせて特徴ベクトルを生成するクラス
    /// <para>コアにCore20009mPlusクラスを使用します。</para>
    /// </summary>
    public class KMCustomUnit1c1d : IUnit
    {
        // ---- メンバ変数 -----------------------------------------------------------
        /// <summary>
        /// 読み込んだ設定ファイルの文字列解析に使用する正規表現
        /// </summary>
        protected Regex regex;
        /// <summary>
        /// 時刻
        /// <para>データをセットすればするほどカウントアップします。</para>
        /// </summary>
        protected double _time;
        /// <summary>
        /// 渡されるFFTの頻度[Hz]
        /// <para>例えば、WAVEファイルから0.1秒ずつ読み込んでFFTをしているのであれば、1/0.1=10Hzです。</para>
        /// </summary>
        protected double _frequencyOfFFT;
        /// <summary>
        /// 最低の時間幅
        /// <para>鳴き声がこの時間幅以上に追加された場合に特徴ベクトルの出力準備が整ったと判断します。</para>
        /// </summary>
        protected double _minTimeWidth;
        /// <summary>
        /// 鳴き声検出器
        /// </summary>
        protected SongDetector _sensor;
        /// <summary>
        /// 特徴ベクトル生成器
        /// </summary>
        protected Core2009mPlus _genetator;
        /// <summary>
        /// フィルタリングするための帯域構造体
        /// </summary>
        protected Band _band;
        /// <summary>
        /// 特徴ベクトルのリスト
        /// <para>処理されないと次々と溜まるので注意してほしい</para>
        /// </summary>
        protected List<Result> _generatedFeatures;
        // ---- プロパティ -----------------------------------------------------------
        /// <summary>
        /// 本クラスが利用している帯域ID名
        /// <para>通常はファイルに書き込まれた名前が使用されています。</para>
        /// </summary>
        public string FileterName
        {
            get;
            protected set;
        }
        /// <summary>
        /// 特徴ベクトルを出力可能であればture
        /// </summary>
        public Boolean Ready
        {
            get
            {
                if (this._generatedFeatures.Count > 0)
                    return true;
                else
                    return false;
            }
        }
        /// <summary>
        /// エラー状況を示す
        /// <para>将来的には、明らかにおかしい場合にtrueとする</para>
        /// <para>例えば、帯域情報ファイルのフォーマットに発声時間の上限を設けるなどして（例えば、キビタキは10秒以上鳴き続けることはないとか。）はじくのに使えるかな？</para>
        /// </summary>
        public Boolean Error
        {
            get { return false; }
        }
        /// <summary>
        /// セットアップの完了の有無
        /// <para>true: セットアップ完了</para>
        /// </summary>
        public Boolean IsSetuped
        {
            get;
            protected set;
        }
        // ---- メソッド -------------------------------------------------------------
        /// <summary>
        /// 文字列を演算条件設定文字列フォーマットに合致するかどうか確認する
        /// </summary>
        /// <param name="setting">設定文字列</param>
        /// <returns>true: フォーマットに合致</returns>
        public Boolean CheckSettingStringMatch(string setting)
        {
            string line = setting.Replace("\r", "").Replace("\n", ""); // 改行コードを削除して1行格納
            if (line.IndexOf('#') < 0 && line != "")            // #はコメント, 空の行も読み飛ばす
            {
                if (this.regex.IsMatch(line)) return true;
            }
            return false;
        }
        /// <summary>
        /// WAVEファイルをFFTにかけた結果をセットする
        /// </summary>
        /// <param name="result">WaveファイルをFFT処理したデータ</param>
        public virtual void Set(FFTresult result)
        {
            if (this.IsSetuped)
            {
                this._time += 1.0 / this._frequencyOfFFT;                           // 既セット分から経過時刻を計算する
                this._sensor.Set(result);                                           // 検出器は内部でフィルタリングするため、下のフィルタリング後のデータを渡さなくともよい
                FFTresult filteredResult = result.Filter(this._band);               // フィルタをかけたオブジェクトを作る
                if (this._sensor.Detection)                                         // 鳴き声があればtrue（になって欲しい）
                {
                    this._genetator.Add(filteredResult);                            // フィルタリングしたFFT結果を使った方が明らかにノイズを拾わない
                }
                else
                {
                    if (this._genetator.Ready)
                    {
                        this._genetator.Add(filteredResult);                        // 最後の閉め
                        double timeWidth = this._genetator.TimeOfStock;
                        double startTime = this._time - timeWidth;                  // 検出開始時刻を計算
                        var feature = this._genetator.GetFeature();                 // 準備が完了しているようなら特徴ベクトルを作らせる
                        if (feature != null) this._generatedFeatures.Add(new Result(this.FileterName, startTime, timeWidth, this._genetator.SN, feature));  // nullでなければ、特徴ベクトルとして追加する
                    }
                    this._genetator.Init(filteredResult);                           // 特徴ベクトル生成器をリセット
                }
            }
            return;
        }
        /// <summary>
        /// 特徴ベクトルを含む検出情報を返す
        /// </summary>
        /// <returns>特徴ベクトル</returns>
        public Result GetFeature()
        {
            if (this.Ready)                                                         // 特徴ベクトルを出力可能なら出力する
            {
                Result ans = this._generatedFeatures[0];
                this._generatedFeatures.RemoveAt(0);
                return ans;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 音声検出器の状況をファイルへ保存する
        /// </summary>
        /// <param name="fname">ファイル名</param>
        public void SaveConditionOfDetector(string fname)
        {
            this._sensor.Save(fname);
            return;
        }
        /// <summary>
        /// コアの特徴ベクトル生成器を返す
        /// </summary>
        /// <returns>Core20009mPlusオブジェクトのインスタンス</returns>
        protected virtual Core2009mPlus GetCoreGenerator()
        {
            return new Core2009mPlus();
        }
        /// <summary>
        /// 演算条件の設定を行う
        /// <para>引数のbandInfoを用いて、フィルタの帯域情報を読み込みます。</para>
        /// <para>テキストフォーマットは、カンマ区切りで下記のように定めます。</para>
        /// <para>"ID,maxFrequency,minFrequency,minTimeWidth" 例："キビタキ,4500,2500,0.3"</para>
        /// </summary>
        /// <param name="condition">特徴ベクトル生成コアユニットに渡すパラメータ</param>
        /// <param name="waveSamplingFrequency">WAVEファイルのサンプリング周波数[Hz]</param>
        /// <param name="frequencyOfFft">FFTの実施周波数[Hz]<para>0.1秒毎のWAVEファイルを処理するのであれば、1/0.1=10 Hz</para></param>
        /// <param name="bandInfo">帯域情報</param>
        public virtual void Setup(Object condition, double waveSamplingFrequency, double frequencyOfFft, string bandInfo)
        {
            MinimumCondition mCondition;
            if (condition is MinimumCondition)
                mCondition = (MinimumCondition)condition;
            else
                throw new Exception("型の一致しない演算条件のセットを検出しました。conditionの型を再確認してください。");
            if (bandInfo == "default")
            {
                double minTimeWidth = 1.0 / frequencyOfFft * 3.0;      // 最小の有効検出時間を設定する。最小分解能の3倍とする。
                bandInfo = "NA," + waveSamplingFrequency.ToString("00") + ",0," + minTimeWidth.ToString();
            }
            if (this.CheckSettingStringMatch(bandInfo) == false)
                throw new Exception("帯域情報文字列のフォーマット不一致を検出しました。bandInfoのフォーマットを再確認してください。");
            this._time = 0.0;
            this._frequencyOfFFT = frequencyOfFft;
            this._generatedFeatures = new List<Result>(0);
            // 文字列を解析してパラメータをセットする
            try
            {
                string[] field = bandInfo.Split(',');
                this.FileterName = field[0];                                        // 帯域名例えば、"キビタキ"
                long maxFrequency = long.Parse(field[1]);                           // 文字を数値に直す
                long minFrequency = long.Parse(field[2]);                           // 
                this._minTimeWidth = double.Parse(field[3]);                        // 鳥の鳴き声が続く最小の時間幅を取得
                this._band = new Band(maxFrequency + 1000.0, minFrequency - 1000.0);// リストに帯域情報を書き込む

                this._sensor = new SongDetector();
                this._sensor.Setup(1, new Band(maxFrequency, minFrequency), new TimeSpan(0, 0, 0, (int)this._minTimeWidth, (int)((this._minTimeWidth % 1.0) * 1000.0)), this._frequencyOfFFT);

                Condition _condition = new Condition(mCondition, this._frequencyOfFFT, this._minTimeWidth);
                this._genetator = this.GetCoreGenerator();
                this._genetator.Setup(_condition);
                this.IsSetuped = true;
            }
            catch (SystemException e)
            {
                string message = "GeneratorUnit構造体のSetupメソッドにおいて、帯域情報ファイルの文字列解析に失敗しました。";
                Console.WriteLine(message);
                Console.WriteLine(e.Message);
                throw new SystemException(message);
            }
            return;
        }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public KMCustomUnit1c1d()
            : base()
        {
            this.regex = new System.Text.RegularExpressions.Regex(@"\w+,\d+,\d+,\d+[.]*\d*");  // 読み込んだ設定ファイルの文字列解析に使用する正規表現
        }
    }
}
