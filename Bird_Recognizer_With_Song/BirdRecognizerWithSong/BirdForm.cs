/***************************************************************************
 * [ソフトウェア名]
 *      BirdRecognizerWithSong
 * [開発者]
 *      K.Morishita (Kumamoto-Univ. @ 2012)
 * [概要]
 *      鳥の鳴き声を用いて種の識別を実施するためのGUIソフトウェアです。
 *      プログラム全体としてはリアルタイム処理には対応していませんが、それをかなり意識した構成になっています。
 *      （最近はリアルタイム対応は無駄だったなぁとつくづく思います）
 * [使用している特徴]
 *      熊本大学三田研究室が開発した特徴抽出アルゴリズム+α（独自のアイデア）を実装しています。
 *      本ソースコードには特徴抽出に関して情報はありませんので、BirdSongFeatureプロジェクトのソースコードと説明書をご覧ください。
 * [参考資料]
 *      CINII（http://ci.nii.ac.jp/）で「三田長久」を検索してください。
 * [おまけ情報]
 *      三田教授が2013年3月で退職されたので、以後、このソフトの開発があまり進むとは思えません。
 * [履歴]
 *      2011/12/30  開発開始
 *      2012/1/6    Webでの公開のために整理。。
 *                  しかし、もう2012年ですか？
 *                  早すぎるなぁ。
 *      2012/1/22   ここ1週間ほどを使ってテスト関数を整備した。
 *                  ここまで来ると、もはやシステムとしてはほぼ完成かな…？
 *      2012/3/10   コメントの見直しと関数名の見直し
 *                  テスト関数と見本の関数を削除。
 *                  識別器の設定ファイルが自動的に読み込み・設定・保存されるように変更した。
 *      2012/3/12   大量のWAVEファイルを一度に処理できるようにした
 *      2012/3/13   大量のWAVEファイルから特徴ベクトルを生成する機能も実装した。識別しないだけなんだけどね。
 *      2012/5/4    WaveReaderクラスの所属名前空間を変更したのに対応させた。
 *      2012/5/4    FFT関連クラスの名前空間の変更に伴いコードを一部変更した。
 *                  プロジェクト全体も少し見直した。
 *                  ニューラルネット関係もdllとしてライブラリとする予定。
 *      2012/5/18   ユーザーが独自の特徴量計算ユニットを追加・使用できるように、FeatureGeneratorクラスをジェネリックとした。
 *                  IFeatureGeneratorCoreUnitインターフェイスを継承すれば、特徴量計算ユニットを新規で作成・運用できます。
 *      2012/6/16   特徴ベクトル生成関係のクラスを再編し、ほぼ確定させる。これに伴い上記のIFeatureGeneratorCoreUnitインターフェイスは廃止した。
 *                  特徴ベクトル生成メソッドを多数のユニットに対応できるようにクラス化して、SelectorOfFeatureGeneratorクラスとして機能を外に出した。
 *      2012/6/22   ニューラルネット用のパラメータを一元管理できるように構造体にまとめてメンバ変数に入れた。
 *                  SVM対応への布石でもある。
 *                  また、本クラスにおけるFormLoad時のエラー対策も追加した。
 *      2012/6/23   少しだけUIを追加
 *      2012/6/26   特徴ベクトル生成関係のクラスをプロジェクトファイルとして外部へ出した。
 *                  この方が応用が利くはず。
 *      2012/11/16  ファイルの保存方法を若干変更した。
 *                  将来的に、出力した特徴ベクトル検討用ファイルのフォーマットのまま学習に使うこともあると考えられる。
 *                  その場合は、WaveCheckerのコード内に宣言した正規表現をうまく利用すると、開発が早いだろう。
 *                  今日はここまでとする。タイムアップだ。
 *      2013/1/14    SelectorOfFeatureGenerator.csについて、filter.txtという、帯域フィルタ情報ファイルのパスを実行ファイルのあるディレクトリに限定するように変更した。
 *                  Win 7では今までのでも問題はなかったが、XPだと音源ファイルが作業フォルダになっていたため問題となっていた。
 *                   本ソースの範囲でも同じ問題のある個所が複数あったため、問題に対処した。
 *      2013/2/3     RecognizeFeaturesInTextFile()を改造して、音源情報を含んだタイプのファイルでも識別できるようになった。
 *                   GetModelsFromDir(string dir)を追加し、音源情報を含んだ状態でも学習が可能なようにした。
 *                  関連コード部分も変更した。
 *      2013/3/16    特徴ベクトルを記述した文字列にコメントを付加できるように改良した。
 *                  そのために、FeatureGenerationプロジェクトにFeatureTextLineParseクラスを取り込んで、記述が若干変更になっています。
 *      2013/3/22    鳴き声から特徴を作るプロジェクトをBirdSongFeatureプロジェクトへ変更した。
 *                  これはFeatureGenerationプロジェクトを再構成したもので、実質は同じものです。
 *      2013/3/24    たぶん、デバッグ完了。
 *      2013/5/22   コメントを修正
 *                  開発環境をVisual Studio 2012 proに移したが、問題はなさそう。
 * *************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;   // for Regex

using PatternRecognition;
using PatternRecognition.ArtificialNeuralNetwork;
using Sound.WAVE;
using Signal.FrequencyAnalysis;
using BirdSongFeature.Feature;
using BirdSongFeature.FeatureGeneration;
using BirdSongFeature.FeatureGeneration.Core.KMCustom1stCore;
using BirdSongFeature.FeatureGeneration.Unit;

namespace BirdRecognizerWithSong
{
    /// <summary>
    /// 鳥の鳴き声を用いて種の識別を実施するためのソフトウェアのフォームクラス
    /// </summary>
    public partial class BirdForm : Form
    {
        /*---- メンバ変数 ----------------------------------------------------*/
        /// <summary>
        /// 識別器
        /// </summary>
        private Discriminator myDiscriminator = new Discriminator();
        /// <summary>
        /// ニューラルネット用のパラメータ
        /// </summary>
        private MinParameter parameter = new MinParameter(15, 1, 0.07);
        /*---- プロパティ ----------------------------------------------------*/
        /// <summary>
        /// 識別器が利用可能かどうかを返す
        /// </summary>
        private Boolean AvailableOfDiscriminator
        {
            get {
                return this.myDiscriminator.Ready; 
            }
        }
        /*---- メソッド　初期化関係 ----------------------------------------------------*/
        /// <summary>
        /// フォームがロードされる前に実行されるコンストラクタ
        /// </summary>
        public BirdForm()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 起動時（フォームがロードされたとき）に実行されるメソッド
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BirdForm_Load(object sender, EventArgs e)
        {
            MessageBox.Show("本プログラムのご利用、ありがとうございます。\nメニューのOperationよりお好みの動作を選んでください。");
            // 識別器を準備
            try
            {
                if (System.IO.File.Exists(Properties.Settings.Default.usedDiscriminator)) this.myDiscriminator.Setup(Properties.Settings.Default.usedDiscriminator); // ファイルの存在を確認して、識別器の設定ファイルを読み込み
            }
            catch (Exception ex)
            {
                MessageBox.Show("学習済みの設定ファイルの読み込みに失敗しました。\n" + ex.Message);
            }
            // 使用する特徴ベクトル生成器を指定できるようにUIを初期化
            var unitNames =  Enum.GetNames(typeof(UnitMember));
            foreach (string name in unitNames)
            {
                this.FeatureGeneratorSelector.Items.Add(name);
            }
            this.FeatureGeneratorSelector.Text = unitNames[0];
            // 識別処理時のデフォルトの閾値を設定
            this.TBforThreshold.Text = (0.8).ToString("f1");
            // その他
            this.CmboxSoundDetectorLog.Text = "False";
            this.CmboxDetectionMode.Text = "Default";
        }
        /// <summary>
        /// フォームが閉じる前に実施される
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BirdForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //アプリケーションの設定を保存する
            Properties.Settings.Default.Save();
        }
        /*---- メソッド　汎用 ----------------------------------------------------*/
        /// <summary>
        /// 識別器の設定ファイルを指定する
        /// <para>ダイアログを利用します。</para>
        /// </summary>
        private void SetDiscriminator()
        {
            OpenFileDialog dialog4InitFile = new OpenFileDialog();
            dialog4InitFile.Filter = "識別器設定ファイル|*.ini";
            dialog4InitFile.Title = "識別器の設定ファイルを選択してください";
            if (dialog4InitFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.myDiscriminator.Setup(dialog4InitFile.FileName);                                                       // 設定ファイルを読み込み
                if (this.myDiscriminator.Ready) Properties.Settings.Default.usedDiscriminator = dialog4InitFile.FileName;   // 正常に読み込めていれば、設定としてファイルのパスをバックアップ
            }
            return;
        }
        /// <summary>
        /// 指定されたファイルを順次処理して特徴抽出または識別した結果を保存する
        /// </summary>
        /// <param name="headerName">保存するファイルのファイル名の先頭に付ける名前</param>
        /// <param name="fileNames">処理したいWAVEファイルのパスを格納した配列</param>
        /// <param name="discriminator">識別器<para>識別させたいならnull以外を渡してください。</para></param>
        private void SaveFeaturedOrDiscriminatedResultForWaveFiles(string headerName, string[] fileNames, Discriminator discriminator = null)
        {
            double threshold;
            var startupDir = System.Windows.Forms.Application.StartupPath;                          // 実行ファイルのあるフォルダパスを取得

            if (double.TryParse(this.TBforThreshold.Text, out threshold))
            {
                try
                {
                    System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();        // 処理時間を計測する
                    watch.Start();
                    using (System.IO.StreamWriter swCombined = new System.IO.StreamWriter(headerName + " result" + ".csv", false, Encoding.UTF8))   // 答えを書き込む
                    {
                        for (int k = 0; k < fileNames.Length; k++)
                        {
                            var generator = new SelectorOfFeatureGenerator(this.FeatureGeneratorSelector.Text);
                            if (this.CmboxSoundDetectorLog.Text == "True")
                                generator.Log = true;
                            else
                                generator.Log = false;
                            if (this.CmboxDetectionMode.Text == "Match case only")
                                generator.MatchOnlyAtDiscrimination = true;
                            else
                                generator.MatchOnlyAtDiscrimination = false;
                            var result = generator.GenerateFeatureList(fileNames[k], discriminator, threshold);                                     // 特徴抽出or識別を実施
                            string fname = System.IO.Path.GetFileNameWithoutExtension(fileNames[k]);
                            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(startupDir + @"\" + headerName + "_" + fname + ".csv", false, Encoding.UTF8)) // 答えを書き込む
                            {
                                sw.WriteLine("File Path," + fileNames[k]);
                                for (int i = 0; i < result.Count; i++)
                                {
                                    sw.WriteLine(result[i].ToString());                             // 音源毎版
                                    swCombined.WriteLine(result[i].ToString() + fileNames[k]);      // 統合版（フォーマットが少しだけ異なります）, デリミタは既に付いているので新たには不要
                                }
                            }
                        }
                    }
                    watch.Stop();
                    MessageBox.Show("処理が終了しました。\n処理時間は" + watch.Elapsed.TotalSeconds.ToString("0.0") + "秒でした。");
                }
                catch (SystemException error)
                {
                    MessageBox.Show(error.Message);
                }
            }
            else
                MessageBox.Show("閾値に関する解析エラーが発生しました。入力文字を確認してください。");
            return;
        }
        /// <summary>
        /// 特定のファイル内に記載された特徴ベクトルを識別のテストにかけます
        /// <example>this.RecognizeFeaturesInTextFile("test.txt");</example>
        /// </summary>
        /// <param name="testFileName">特徴ベクトルの格納されたファイル名</param>
        private void RecognizeFeaturesInTextFile(string testFileName)
        {
            try
            {
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter("test result.txt", false, Encoding.UTF8))
                {
                    using (System.IO.StreamReader sr = new System.IO.StreamReader(testFileName))
                    {
                        //内容を一行ずつ読み込む
                        while (sr.EndOfStream == false)
                        {
                            var line = sr.ReadLine();
                            var parser = new FeatureTextLineParser();
                            parser.SetLine(line);
                            Feature feature = null;
                            if (parser.Success)
                                feature = new Feature(parser.Feature);  // 読み込んだテキストデータから特徴ベクトルを生成
                            else
                                feature = new Feature(line);            // 読み込んだテキストデータから特徴ベクトルを生成
                            Discriminator.IDandLikelihood test = this.myDiscriminator.Discriminate(feature);    // テストを実施し、表示する
                            MessageBox.Show("識別結果: " + test.ID + ", " + test.Likelihood.ToString("0.00"), "識別テストの結果");
                            sw.WriteLine(test.ID + ", " + test.Likelihood.ToString("0.00"));
                        }
                    }
                }
                MessageBox.Show("\"test result.txt\"へ識別結果を保存しました。ご確認ください。");
            }
            catch (SystemException e)
            {
                MessageBox.Show("例外がスローされました。選択したファイルをチェックしてください。\nerror msg.: " + e.Message);
            }
            return;
        }
        /// <summary>
        /// フォルダ内の特徴ベクトルを読み込み、構成したモデルのインスタンスを返す
        /// </summary>
        /// <param name="dir">走査対象のフォルダパス</param>
        /// <returns>モデルの配列</returns>
        private Model[] GetModelsFromDir(string dir)
        {
            if (System.IO.Directory.Exists(dir) == false)
                throw new ArgumentException("指定されたフォルダは存在しません。");

            Model[] models = null;
            string[] fnames = System.IO.Directory.GetFiles(dir, "*.fea");       // フォルダ内の指定拡張子を持つファイル一覧を取得
            if (fnames.Length > 1)
            {
                Hashtable modelID = Teacher.GetModelIDs(fnames);                 // ファイル名の一覧からユニークなモデルIDを取得
                if (modelID.Count >= 2)
                {
                    models = new Model[modelID.Count];                          // 存在するファイルの数だけモデルを生成する
                    // クラスモデルを構成
                    foreach (var fname in fnames)
                    {
                        int linenum = 0;
                        string id = Teacher.GetModelIDname(fname);              // ファイル名からモデル名を取得
                        var index = (int)modelID[id];                           // modelID[id]で、番号を得ることができる
                        if (models[index] == null)
                            models[index] = new Model(id);                      // インスタンスが未生成なら作る
                        using (System.IO.StreamReader sr = new System.IO.StreamReader(fname, Encoding.UTF8))
                        {
                            try
                            {
                                while (sr.EndOfStream == false)
                                {
                                    var line = sr.ReadLine();
                                    linenum++;
                                    var parser = new FeatureTextLineParser();
                                    parser.SetLine(line);
                                    Feature feature = null;
                                    if (parser.Success)
                                        feature = new Feature(parser.Feature);  // 読み込んだテキストデータから特徴ベクトルを生成
                                    else
                                        feature = new Feature(line);            // 読み込んだテキストデータから特徴ベクトルを生成
                                    models[index].Add(feature);                 // 特徴を追加
                                }
                            }
                            catch (Exception e)
                            {
                                string message = "エラーがスローされました。\nエラーメッセージ：" + e.Message + 
                                    ", エラーの発生したファイル名：" + fname +
                                    ", 行番号：" + linenum.ToString();
                                Console.WriteLine(message);
                                throw new Exception(message);                           // さらにスロー
                            }
                        }
                    }
                }
                else
                    throw new Exception("TeacherクラスのSetup()メソッドにてエラーがスローされました。モデルが2つ以上見つかりませんでした。");
            }
            else
                throw new Exception("TeacherクラスのSetup()メソッドにてエラーがスローされました。特徴ベクトルを格納したファイル（*.fea）が2つ以上見つかりませんでした。フォルダ若しくは拡張子をチェックして下さい。");

            return models;
        }
        /// <summary>
        /// 特徴ベクトルが記載されたファイルを使用して、学習を実施させる
        /// <para>NeuralNetTeacherクラスを使って、既に生成されている特徴ベクトルをまとめたファイルを読み込ませて学習を行います。</para>
        /// </summary>
        private void LearnWithFeatureFilesInDirectory()
        {
            Teacher NNteacher = new Teacher(parameter);                         // ニューラルネットの教師クラスのインスタンスを生成
            var startupDir = System.Windows.Forms.Application.StartupPath;

            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)    // ダイアログを使って学習データの入っているフォルダを指定させる
            {
                try
                {
                    NNteacher.RandomModelChoice = true;
                    NNteacher.PickOutMode = Model.ReadSequenceMode.AtRandom;
                    var models = this.GetModelsFromDir(dialog.SelectedPath);    // フォルダ内のファイルから教師モデルを構成する
                    NNteacher.Setup(models);                                    // モデルを渡して初期化（エラーがスローされるかもしれないので、try-catchを使ったほうがいい）
                    int timeOfLearning = 10000;                                 // 学習回数
                    for (int i = 1; i <= 10; i++)                               // ループさせる
                    {
                        NNteacher.Learn(timeOfLearning);
                        NNteacher.SaveDecisionChart(startupDir + @"\" + "decision chart " + (i * timeOfLearning).ToString() + ".csv");  // 学習の進み具合が分かるデータをファイルに保存
                        NNteacher.SaveLearnigOutcome(startupDir + @"\" + "discriminator" + (i * timeOfLearning).ToString() + ".ini");   // 学習した結合係数などをファイルに保存
                    }
                    MessageBox.Show("学習が終了しました。学習結果を学習済みの識別器の設定ファイルと共に出力していますのでご確認ください。");
                }
                catch (Exception e)
                {
                    MessageBox.Show("例外がスローされました。選択したフォルダ内のファイルをチェックしてください。\n" + e.Message);
                }
            }
            return;
        }
        /// <summary>
        /// ユーザーがGUIによって選んだ一つのWAVEファイルから特徴ベクトルを生成します
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createFeatureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.FileName = "hoge.wave";                                                      // ファイルを開く前に初期設定
            dialog.Filter = "Wave Files (*.wav, *.wave)|*.wav;*.wave";                          // 拡張子のフィルタ設定
            dialog.Title = "解析したいWAVEファイルを選択してください";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string[] fname = { dialog.FileName };
                this.SaveFeaturedOrDiscriminatedResultForWaveFiles("featured", fname);
            }
            return;
        }
        /// <summary>
        /// ユーザーがGUIによって選んだフォルダ内にあるwaveファイルをまとめて特徴抽出にかけます
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createFeatureFileFromWAVEFilesInDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            //folder.RootFolder = Environment.SpecialFolder.Programs;
            MessageBox.Show("解析したいWAVEファイルが格納されたフォルダを選択してください");
            if (folder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                string[] files = System.IO.Directory.GetFiles(folder.SelectedPath, "*.wav", System.IO.SearchOption.TopDirectoryOnly);           // 拡張子がwavとなっているファイルの一覧を取得
                this.SaveFeaturedOrDiscriminatedResultForWaveFiles("featured", files);
            }
        }
        /// <summary>
        /// ツールバーから、学習を選択した際の動作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void learningToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.LearnWithFeatureFilesInDirectory();
            return;
        }
        /// <summary>
        /// ファイル内に記載された特徴ベクトルの識別を実施します
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void discriminatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.AvailableOfDiscriminator == false) this.SetDiscriminator();                        // 識別器が利用不可なら指定させる
            if (this.AvailableOfDiscriminator)
            {
                OpenFileDialog dialog4TestFile = new OpenFileDialog();
                dialog4TestFile.Filter = "識別させたいファイル|*.txt|特徴ベクトルファイル|*.fea|全てのファイル|*.*";
                dialog4TestFile.Title = "識別させたい特徴データの格納されたファイルを選択してください";

                if (dialog4TestFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    this.RecognizeFeaturesInTextFile(dialog4TestFile.FileName);
                }
            }
            return;
        }
        /// <summary>
        /// ユーザーがGUIによって選んだ一つのWAVEファイルの識別を実施します
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void discriminationWithWAVEFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.AvailableOfDiscriminator == false) this.SetDiscriminator();                        // 識別器が利用不可なら指定させる
            if (this.AvailableOfDiscriminator)
            {
                OpenFileDialog dialog4wave = new OpenFileDialog();
                dialog4wave.FileName = "hoge.wave";                                                     // ファイルを開く前に初期設定
                dialog4wave.Filter = "Wave Files (*.wav, *.wave)|*.wav;*.wave";                         // 拡張子のフィルタ設定
                dialog4wave.Title = "解析したいWAVEファイルを選択してください";
                if (dialog4wave.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string[] fname = {dialog4wave.FileName};
                    this.SaveFeaturedOrDiscriminatedResultForWaveFiles("discriminated", fname, this.myDiscriminator);
                }
            }
            return;
        }
        /// <summary>
        /// ユーザーがGUIによって選んだフォルダ内にあるwaveファイルをまとめて識別に掛けます
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void discriminationWithWAVEFilesInDirectryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.AvailableOfDiscriminator == false) this.SetDiscriminator();                        // 識別器が利用不可なら指定させる
            if (this.AvailableOfDiscriminator)
            {
                FolderBrowserDialog folder = new FolderBrowserDialog();
                //folder.RootFolder = Environment.SpecialFolder.Programs;
                MessageBox.Show("解析したいWAVEファイルが格納されたフォルダを選択してください");
                if (folder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    
                    string[] files = System.IO.Directory.GetFiles(folder.SelectedPath, "*.wav", System.IO.SearchOption.TopDirectoryOnly);           // 拡張子がwavとなっているファイルの一覧を取得
                    this.SaveFeaturedOrDiscriminatedResultForWaveFiles("discriminated", files, this.myDiscriminator);
                }
            }
            return;
        }
        /// <summary>
        /// ユーザーがGUIによって識別器の設定ファイルを読み込むように指示した際に実施する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openDiscriminatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.SetDiscriminator();
            if (this.AvailableOfDiscriminator == true)
                MessageBox.Show("識別器設定ファイルの正常な読み込みに成功しました。");
            else
                MessageBox.Show("識別器設定ファイルの正常な読み込みに失敗しました。");
        }
        /// <summary>
        /// ユーザーがGUIによって終了ボタンを押した際の動作
        /// <para>プログラムを終了します。</para>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// 交差確認による特徴ベクトルの評価を行う
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void crossValidationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            var startupDir = System.Windows.Forms.Application.StartupPath;              // 実行ファイルのあるフォルダパスを取得

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)            // ダイアログを使って学習データの入っているフォルダを指定させる
            {
                try
                {
                    Teacher NNteacher = new Teacher(parameter);                         // ニューラルネットの教師クラスのインスタンスを生成
                    for (int timeOfLearning = 1000; timeOfLearning <= 100000; timeOfLearning *= 10)
                    {
                        int numberOfTrials = 1;                                         // 演算巡回数。１巡で10回計算するので一般的な定義と等しくなる。
                        for (int i = 1; i <= 1; i++)                                    // 複数回回すと変動の大きさが分かる
                        {
                            NNteacher.RandomModelChoice = true;
                            NNteacher.PickOutMode = Model.ReadSequenceMode.AtRandom;
                            NNteacher.Setup(dialog.SelectedPath);                       // 学習データの入ったフォルダ名を渡して初期化（エラーがスローされるかもしれないので、try-catchを使ったほうがいい）
                            NNteacher.CV(timeOfLearning, numberOfTrials, startupDir + @"\" + "cross validation test " + timeOfLearning.ToString() + " no_" + i.ToString() + ".csv");
                        }
                    }
                    MessageBox.Show("CVが終了しました。結果を出力していますのでご確認ください。");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("例外がスローされました。選択したフォルダ内のファイルをチェックしてください。\n" + ex.Message);
                }
            }
            return;
        }
    }
}
