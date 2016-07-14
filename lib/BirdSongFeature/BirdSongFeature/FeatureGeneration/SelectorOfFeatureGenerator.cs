/***************************************************************************
 * [ソフトウェア名]
 *      SelectorOfFeatureGenerator.cs
 *      特徴ベクトル生成ユニットを任意に選択して特徴ベクトルを生成・場合によって識別を行うクラス
 *      
 * [開発者]
 *      K.Morishita (Kumamoto-Univ. @ 2012.6)
 *      
 * [履歴]
 *      2012/6/16   汎用性を確保した。
 *      2012/6/22   コメントを一部見直した。
 *      2012/6/23   プロパティMatchOnlyAtDiscriminationを設置。
 *                  名前がいい加減なので将来変える可能性大。
 *      2013/1/14   filter.txtという、帯域フィルタ情報ファイルのパスを実行ファイルのあるディレクトリに限定するように変更した。
 *                  Win 7では今までのでも問題はなかったが、XPだと音源ファイルが作業フォルダになっていたため問題となっていた。
 * *************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PatternRecognition;
using Sound.WAVE;
using Signal.FrequencyAnalysis;
using BirdSongFeature.Feature;
using BirdSongFeature.FeatureGeneration.Core.KMCustom1stCore;
using BirdSongFeature.FeatureGeneration.Unit;

namespace BirdSongFeature.FeatureGeneration
{
    /// <summary>
    /// 特徴ベクトル生成ユニットを任意に選択して特徴ベクトルを生成・場合によって識別を行うクラス
    /// <para>特徴ベクトル生成ユニットを追加した場合はSetGenerator()メソッドを必ず変更してください。</para>
    /// </summary>
    public class SelectorOfFeatureGenerator
    {
        /* メンバ変数 ****************************************************************/
        /// <summary>
        /// 特徴ベクトル生成器のインターフェイスを持つオブジェクトを格納する
        /// <para>特徴ベクトル生成ユニットの異なるオブジェクトを一括して使役するためのインターフェイス</para>
        /// </summary>
        private IFeatureGenerator _generator;
        /// <summary>
        /// WAVEファイルから読み出すデータ数
        /// </summary>
        private int readPoint;
        /// <summary>
        /// 特徴ベクトル生成に利用するユニット名
        /// </summary>
        private UnitMember id;
        /// <summary>
        /// 作動ログを残すかどうかの選択
        /// <para>大抵、作出されるファイルのサイズはかなり大きめになります。</para>
        /// </summary>
        private Boolean log;
        /* プロパティ ****************************************************************/
        /// <summary>
        /// プロパティの変更が可能かどうかを示す
        /// <para>true: 演算に使用されるプロパティの変更は不可能となります。</para>
        /// </summary>
        public Boolean Lock
        {
            get;
            private set;
        }
        /// <summary>
        /// WAVEファイルから読み出すデータ数
        /// </summary>
        public int ReadAmount
        {
            get { return this.readPoint; }
            set
            {
                if (this.Lock == false) this.readPoint = value;
            }
        }
        /// <summary>
        /// ログを残すかどうかの選択
        /// </summary>
        public Boolean Log
        {
            get { return this.log; }
            set
            {
                if (this.Lock == false) this.log = value;
            }
        }
        /// <summary>
        /// 識別時に、フィルタ名と識別結果が一致したときのみログを残すかどうかを選択
        /// </summary>
        public Boolean MatchOnlyAtDiscrimination
        {
            get;
            set;
        }
        /* メソッド ****************************************************************/
        /// <summary>
        /// コンストラクタ生成時に渡された情報に従って特徴ベクトル生成器を構成します
        /// </summary>
        /// <param name="reader">音源ファイルを開いた状態のWaveReaderオブジェクト<para>情報取得に利用します。</para></param>
        private void SetGenerator(WaveReader reader)
        {
            IFeatureGenerator generator = null;
            var startupDir = System.Windows.Forms.Application.StartupPath;
            var fname = startupDir + @"\" + "filter.txt";

            switch (this.id)
            { 
                case UnitMember.KMCustomUnit1c1d:
                    MinimumCondition condition1 = new MinimumCondition(50);
                    generator = new FeatureGenerator<KMCustomUnit1c1d>(condition1, reader.SamplingRate, reader.SamplingRate / (double)this.ReadAmount, fname); // 特徴ベクトル生成器のインスタンスを作る                   
                    break;
                case UnitMember.KMCustomUnit2c1d:
                    MinimumCondition condition2 = new MinimumCondition(50);
                    generator = new FeatureGenerator<KMCustomUnit2c1d>(condition2, reader.SamplingRate, reader.SamplingRate / (double)this.ReadAmount, fname); // 特徴ベクトル生成器のインスタンスを作る
                    break;
                case UnitMember.KMCustomUnit2c1dver2:
                    MinimumCondition condition3 = new MinimumCondition(50);
                    generator = new FeatureGenerator<KMCustomUnit2c1dver2>(condition3, reader.SamplingRate, reader.SamplingRate / (double)this.ReadAmount, fname); // 特徴ベクトル生成器のインスタンスを作る
                    break;
                case UnitMember.KMCustomUnit3c1d:
                    MinimumCondition condition4 = new MinimumCondition(5);
                    generator = new FeatureGenerator<KMCustomUnit3c1d>(condition4, reader.SamplingRate, reader.SamplingRate / (double)this.ReadAmount, fname); // 特徴ベクトル生成器のインスタンスを作る
                    break;
            }
            this._generator = generator;
            
            return;
        }
        /// <summary>
        /// 音声ファイルを開き、生成された特徴ベクトルのリストを返す
        /// </summary>
        /// <param name="waveFilePath">WAVEファイルのフルパス</param>
        /// <param name="discriminator">識別器<para>識別させない場合は省略して下さい。</para></param>
        /// <param name="threshold">棄却レベル<para>識別のさせる場合、この値を下回ると識別結果をUnknownと判定します。</para></param>
        /// <returns>
        /// 生成された特徴ベクトルのリスト
        /// <para>識別器を指定している場合は、識別結果を添付します。</para>
        /// </returns>
        public List<Result> GenerateFeatureList(string waveFilePath, Discriminator discriminator = null, double threshold = 0.0)
        {
            var ans = new List<Result>(0);                                                          // 結果を格納する

            if (System.IO.File.Exists(waveFilePath))
            {
                WaveReader wave_file = new WaveReader();
                wave_file.Open(waveFilePath);                                                       // waveファイルを開く
                if (wave_file.IsOpen)
                {
                    try
                    {
                        this.Lock = true;
                        this.SetGenerator(wave_file);
                        this._generator.Log = this.Log;
                        DFT fft = new DFT(this.ReadAmount, Window.WindowKind.Hanning);              // FFTを実施する準備
                        while (wave_file.IsOpen && wave_file.ReadLimit == false)                    // 読めるなら、全て読むまでループ
                        {
                            MusicData data = wave_file.Read(this.ReadAmount);                       // ファイルからread_size_sec秒間読み込み。
                            double[] sound;
                            if (data.UsableCH == MusicData.UsableChannel.Left)                      // データの入っている方を格納する
                                sound = data.GetData(MusicData.Channel.Left);                       // 
                            else
                                sound = data.GetData(MusicData.Channel.Right);
                            FFTresult result = fft.FFT(sound, (double)wave_file.SamplingRate);      // FFTを実行. FFTサイズが音声データよりも小さい場合、始めの方しか処理されないので注意.
                            this._generator.Add(result);
                            if (this._generator.Ready)
                            {
                                //Console.WriteLine("検出したようです。");                          // デバッグモードで動かしながら確認するためのコンソール出力
                                Result feature = this._generator.GetFeature();                      // 特徴ベクトル生成
                                if (discriminator == null)
                                    ans.Add(feature);                                               // 答えを格納
                                else
                                {
                                    Discriminator.IDandLikelihood id = discriminator.Discriminate(feature.FeatureVector);
                                    string name = id.ID;
                                    if (id.Likelihood < threshold) name = "Unknown";
                                    feature.DiscriminationResult = name;
                                    if (!(this.MatchOnlyAtDiscrimination == true && name != feature.FilterName)) ans.Add(feature);
                                }
                            }
                        }
                    }
                    catch (SystemException e)
                    {
                        throw new Exception(e.Message);
                    }
                }
                else
                    throw new Exception("Waveファイルを開くことができませんでした。\nWaveReaderクラスエラーメッセージ：" + wave_file.ErrorMessage);
                wave_file.Close();                                                              // waveファイルを閉じる
            }
            this.Lock = false;
            return ans;
        }
        /// <summary>
        /// コンストラクタ
        /// <para>ユニット名の解析に失敗すると、ユニットにKMCustomUnit1c1dを使用します。</para>
        /// </summary>
        public SelectorOfFeatureGenerator(string unitName)
            : base()
        {
            UnitMember unit = UnitMember.KMCustomUnit1c1d;
            if (unitName == UnitMember.KMCustomUnit1c1d.ToString()) unit = UnitMember.KMCustomUnit1c1d;
            if (unitName == UnitMember.KMCustomUnit2c1d.ToString()) unit = UnitMember.KMCustomUnit2c1d;
            if (unitName == UnitMember.KMCustomUnit2c1dver2.ToString()) unit = UnitMember.KMCustomUnit2c1dver2;
            if (unitName == UnitMember.KMCustomUnit3c1d.ToString()) unit = UnitMember.KMCustomUnit3c1d;

            this.id = unit;
            this.Log = false;
            this.ReadAmount = (int)Signal.FrequencyAnalysis.FFTpoint.size1024;
            this.MatchOnlyAtDiscrimination = false;
            return;
        }
    }
}
