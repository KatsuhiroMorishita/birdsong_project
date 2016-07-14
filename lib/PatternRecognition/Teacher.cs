/***************************************************************************
 * [ソフトウェア名]
 *      Teacher.cs
 * [開発者]
 *      K.Morishita (Kumamoto-Univ. @ 2011.12)
 *      
 * [履歴]
 *      2012/5/9    ソースコードの分割によりファイルとして独立した。
 *      2012/5/10   コメントを一部見直し。
 *      2012/6/18   SetupCompleteプロパティをReadyへ改名
 *                  TotalAccurecyRateプロパティの型をdouble?からdoubleへ変更
 *                  NeuralNetworkクラスに依存するコードをなるだけ減らし中…。
 *                  最後に、インターフェイスを作って、本クラスからまとめて排除する予定。
 *                  学習時にモデルをランダムに学習することを可能とした。
 *                  特徴評価用に交差確認法（VC）を実装した。
 *                  が、コードには本当はMatrixクラスを使うべきところがたくさんあって、汚い。
 *                  そのうち書き換える予定です。
 *      2012/6/19   もう20日の朝だが、昨日から取り組んだ交差確認法のデバッグを終えた。というか、昨日組んだのは間違ったアルゴリズムだった。
 *                  あとちょっとの努力で並列演算までできるようになった。
 *                  識別率表も計算間違いも訂正できた。きつす。
 *                  たぶんだけど、教師データとしてファイル名だけの空のファイルがあったらエラーが出る。
 *      2012/6/20   交差確認用のCV()メソッドの実装を完了した。コードが汚いのがつらい。
 *      2012/6/22   学習器用のパラメータをObjectで受け取ってモデル情報とともに学習器へ送る形を目指して変更した。
 *                  Setup()において、モデルが2つ以上見つからなかった場合にエラーをスローするように変更した。
 *                  汎用性確保のためには、あとはNNからこちらで使用しているメソッドをインターフェイスとして抜いて、このクラスをジェネリック型へ変更するだけだ。
 *                  クラス名をNeuralNetTeacherからTeacherへ変更した。
 *                  [メモ]学習の終了判定方法がSVMとニューラルネットでは異なるのは将来問題になりそうだ。
 *      2012/6/23   Learn()メソッド内において、乱数ジェネレータが壊れた場合に新しいインスタンスを確保するように変更した。
 *                  Layerクラスでも同じような症状が出得るのだけど、同じような処置で対処できるだろう。
 *      2012/6/25   SaveDecisionChart()メソッドが判別表を文字列として返すようにもした。
 *                  コンソールアプリでのテストがやりやすくなったかもしれない。
 *      2012/6/26   CV()の返り値に、分割数を返すように変更した。
 *      2013/2/2    feaファイルの読み込み時にUTF8を使用するように変更した。
 *                  Setup()内で、特徴ベクトルの長さにチェックを入れるように改良しました。
 *                  Setup(Model[] models)を新設した。
 *                  外部のプログラムから利用しやすいように、GetModelIDs(string[] fnames)をpublicへ変更した。
 *                  Readyの定義を変更した。（不具合対策）
 *                  _classをListオブジェクトへ変更
 * *************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PatternRecognition.ArtificialNeuralNetwork;

namespace PatternRecognition
{
    /// <summary>
    /// ニューラルネットワークの教師クラス
    /// <para>
    /// ニューラルネットワークを教育するクラスです。
    /// 使用するには、インスタンス生成時にニューラルネットのパラメータを設定して下さい。
    /// [今後の予定]
    /// 2011/12/24 ジャックナイフ判定法の導入は必要だろうなぁ。
    /// </para>
    /// </summary>
    public class Teacher
    {
        /* メンバ変数 *********************************************************/
        /// <summary>
        /// クラスモデル
        /// <para>演算途中にソートすると識別結果がめちゃくちゃになる可能性があるので余りいじらないでください。</para>
        /// </summary>
        private List<Model> _class;
        /// <summary>
        /// ニューラルネットワーク
        /// </summary>
        private NeuralNetwork _NNet;
        /// <summary>
        /// 識別器のパラメータ
        /// </summary>
        private Object parameter;
        /// <summary>
        /// 乱数ジェネレータ
        /// <para>モデル内をランダムに選択するのに利用します。</para>
        /// </summary>
        private Random myRandom;
        /* プロパティ *********************************************************/
        /// <summary>
        /// クラスモデル数
        /// </summary>
        public int Length 
        {
            get { return this._class.Count; } 
        }
        /// <summary>
        /// 登録クラス内に同じIDを持つものがないかチェックし、同じIDを持つものが在ればtrueを返す
        /// </summary>
        private Boolean ExistOfSameID
        {
            get
            {
                Hashtable hoge = new Hashtable();
                for (int i = 0; i < this._class.Count; i++)
                {
                    if (hoge.Contains(this._class[i].ID) == false)
                        hoge.Add(this._class[i].ID, null);
                    else
                        return true;
                }
                return false;
            }
        }
        /// <summary>
        /// 登録されれている全特徴ベクトルについて2重登録を確認し、2重登録が在ればtrueを返す
        /// </summary>
        private Boolean ExistOfSameFeature
        {
            get
            {
                Hashtable hoge = new Hashtable();
                for (int i = 0; i < this._class.Count; i++)
                {
                    for (int k = 0; k < this._class[i].Length; k++)
                    {
                        if (hoge.Contains(this._class[i][k].GetHashCode()) == false)
                            hoge.Add(this._class[i][k].GetHashCode(), null);
                        else
                            return true;
                    }
                }
                return false;
            }
        }
        /// <summary>
        /// 保持しているクラスモデルのID
        /// </summary>
        public string[] ModelIDs
        {
            get
            {
                string[] id = new string[this._class.Count];
                for (int i = 0; i < id.Length; i++)
                    id[i] = this._class[i].ID;
                return id;
            }
        }
        /// <summary>
        /// 学習準備
        /// <para>学習準備が整っていればtrue</para>
        /// </summary>
        public Boolean Ready
        {
            get
            {
                if (this._class == null)
                    return false;
                else if (this._class.Count < 2)    // 最低2クラスなければ学習を実行することはできない。
                    return false;
                else
                    return true;
            }
        }
        /// <summary>
        /// モデル個々の識別率
        /// <para>識別率はモデルIDとセットにしてハッシュテーブルで表します。</para>
        /// <para>なお、処理の実態はGetAccurecyRate()の出力値です。</para>
        /// <para>格納される値の定義：モデルの正答数/モデルの特徴ベクトル数</para>
        /// <para>演算が不可能な場合はnullが返ります。</para>
        /// <para>演算には時間が少々かかりますので、まとまった学習を実施後にチェックするようにして下さい。</para>
        /// </summary>
        public Hashtable EachAccurecyRate
        {
            get
            {
                if (this.Ready == false)
                    return null;
                else
                    return this.GetEachAccurecyRate();
            }
        }
        /// <summary>
        /// 全モデルの総合識別率
        /// <para>処理の実態はGetTotalAccurecyRate()の出力値です。</para>
        /// <para>格納される値の定義：正答の総数/特徴ベクトルの総数</para>
        /// <para>演算が不可能な場合はNaNが返ります。</para>
        /// <para>演算には時間が少々かかりますので、まとまった学習を実施後にチェックするようにして下さい。</para>
        /// </summary>
        public double TotalAccurecyRate
        {
            get
            {
                if (this.Ready == false)
                    return double.NaN;
                else
                    return this.GetTotalAccurecyRate();
            }
        }
        /// <summary>
        /// 学習に使用する学習器の構成情報
        /// </summary>
        public Object Parameter 
        {
            get 
            {
                return this._NNet.GetParameter(); 
            }
        }
        /// <summary>
        /// 学習時にモデルの学習順をランダムにするかどうかを決める
        /// <para>true: ランダムに学習する</para>
        /// </summary>
        public Boolean RandomModelChoice
        {
            get;
            set;
        }
        /// <summary>
        /// クラスモデルから特徴ベクトルを選ぶ方法を選びます
        /// <para>順繰り抜き出す方法とランダムに抜き出す方法があります。</para>
        /// </summary>
        public Model.ReadSequenceMode PickOutMode
        {
            get;
            set;
        }
        /* メソッド **************************************************************/
        /// <summary>
        /// 学習に使用しているニューラルネットワーククラスオブジェクトを返す
        /// <para>学習結果を利用して識別を行う際に使用することを想定しています。</para>
        /// </summary>
        /// <returns>本クラスが保持するニューラルネットワーククラスオブジェクト</returns>
        public NeuralNetwork GetNeuralNetwork()
        {
            return this._NNet;
        }
        /// <summary>
        /// 登録されているクラスの特徴ベクトルをファイルへ出力する
        /// <para>モデル毎にファイルを分割して出力します。</para>
        /// <para>ファイル名にはモデルのIDが使用されます。</para>
        /// <para>ファイルは上書きされます。</para>
        /// <para>文字コードはUTF8です。</para>
        /// </summary>
        public void SaveModelFeatures()
        {
            for (int i = 0; i < this._class.Count; i++)
            {
                string fname = this._class[i].ID + ".fea";
                System.IO.File.WriteAllText(fname, this._class[i].ToString(), Encoding.UTF8);     //ファイルが存在しているときは、上書きする
            }
            return;
        }
        /// <summary>
        /// モデル名一覧、識別器のパラメータを保存する
        /// <para>ファイルは上書きされます。</para>
        /// </summary>
        /// <param name="fname">ファイル名<para>デフォルトはDiscriminatorクラスのSetup()に合わせて"discriminator.ini"です。</para></param>
        public void SaveLearnigOutcome(string fname = "discriminator.ini")
        {
            try
            {
                // モデルIDを保存
                string str = "<ID>\n";
                for (int i = 0; i < this.Length; i++)
                {
                    str = str + this.ModelIDs[i];
                    if (i < this.Length - 1) str = str + ",";
                }
                str = str + "\n</ID>\n";
                System.IO.File.WriteAllText(fname, str, Encoding.UTF8);     //ファイルが存在しているときは、上書きする
                // 以下、追記でニューラルネットワークのパラメータ，結合係数の順で追記保存する。
                this._NNet.Save(fname, true);
            }
            catch (Exception e)
            {
                throw new SystemException(e.GetType().FullName + "\n結合係数の保存中にエラーが発生しました。");
            }
            return;
        }
        /// <summary>
        /// 指定されたクラス名に応じた教師ベクトルを返す
        /// </summary>
        /// <param name="id">クラス名</param>
        /// <returns>教師ベクトル</returns>
        private Vector CreateTeachingVector(string id)
        {
            Vector teachingVector = new Vector(this._class.Count);

            for (int i = 0; i < this._class.Count; i++)
            {
                if (id == this._class[i].ID)
                    teachingVector[i] = 1.0;
                else
                    teachingVector[i] = 0.0;
            }
            return teachingVector;
        }
        /// <summary>
        /// 指定されたクラス番号に応じた教師ベクトルを返す
        /// </summary>
        /// <param name="index">クラス要素番号</param>
        /// <returns>教師ベクトル</returns>
        private Vector CreateTeachingVector(int index)
        {
            Vector teachingVector = new Vector(this._class.Count);
            for (int i = 0; i < this._class.Count; i++)
            {
                if (i == index)
                    teachingVector[i] = 1.0;
                else
                    teachingVector[i] = 0.0;
            }
            return teachingVector;
        }
        /// <summary>
        /// クラスのIDを文字列配列で返す
        /// </summary>
        /// <returns>ID名を配列にしたもの</returns>
        public string[] GetIDnames()
        {
            string[] ans = new string[this._class.Count];

            for (int i = 0; i < this._class.Count; i++)
            {
                ans[i] = this._class[i].ID;
            }
            return ans;
        }
        /// <summary>
        /// クラスモデルを追加する
        /// <para>クラスモデルはディープコピーされます。</para>
        /// </summary>
        /// <param name="newClass">追加するクラスモデル</param>
        /// <exception cref="SystemException">モデルの2重登録があるとスロー</exception>
        public void AddModel(Model newClass)
        {
            this._class.Add(new Model(newClass));
            if (this.ExistOfSameID) 
                throw new SystemException("モデルの2重登録が行われました。");
            return;
        }
        #region // 判別表など、成績評価関係----------------------------------------------------------
        /// <summary>
        /// 一通りの判別を行い、結果をヒストグラム（表：実態はVector[]）にまとめる
        /// <para>閾値による棄却は行っておりません。最大値の出力を持ったクラスを識別器の出した答えだとして集計を行っています。</para>
        /// <para>並列演算による交差確認法の演算のために、引数としてニューラルネットのインスタンスを受け取るようにしています。</para>
        /// </summary>
        /// <param name="discriminator">識別器</param>
        /// <param name="classForTest">識別対象の教師データモデル</param>
        /// <returns>判別結果を表にまとめたもの（実態はVector[]）</returns>
        /// <exception cref="SystemException">クラスモデルがセットされた状態でなければ例外をスロー</exception>
        private Vector[] CreateDecisionChart(NeuralNetwork discriminator, List<Model> classForTest)
        {
            if (this.Ready == false) 
                throw new SystemException("実験の準備はまだ完了しておりません。モデルを追加して下さい。");
            if (discriminator == null)
                throw new Exception("判別器のインスタンスが確保されておらず、判別表を作ることができません。");

            Vector[] decisionChart = new Vector[classForTest.Count];
            for (int i = 0; i < decisionChart.Length; i++) 
                decisionChart[i] = new Vector(this.Length);
            int modelIndex = 0;
            foreach (Model model in classForTest)                               // 保持しているモデルの数だけループ
            {

                Model.ReadSequenceMode backup = model.SequenceMode;
                model.SequenceMode = Model.ReadSequenceMode.InRotation;         // 重複を避けるため、特徴ベクトルの選出方法はローテーションとする
                // 特徴モデルを順番に呼び出す
                for (int count = 0; count < model.LengthForGetFeature; count++)
                {
                    Feature feature = model.GetFeature();                       // 特徴ベクトルをモデルから取得
                    Vector outputForDebug = discriminator.Recognize(feature);   // 識別
                    // 最大値となるインデックスを検索
                    double max = double.MinValue;
                    int maxIndex = 0;
                    for (int j = 0; j < outputForDebug.Length; j++)
                    {
                        if (max < outputForDebug[j])
                        {
                            max = outputForDebug[j];
                            maxIndex = j;
                        }
                    }
                    // 最大値となったインデックスについて、加算
                    decisionChart[modelIndex][maxIndex] += 1;
                }
                model.SequenceMode = backup;
                modelIndex++;
            }
            return decisionChart;
        }
        /// <summary>
        /// 個別のモデルの識別率を返す
        /// <para>格納される値の定義：モデルの正答数/モデルの特徴ベクトル数</para>
        /// <para>クラスモデルがセットされた状態でなければ例外をスローします。</para>
        /// </summary>
        /// <returns>クラスモデルIDと識別率をハッシュテーブルにまとめたもの</returns>
        /// <exception cref="SystemException">クラスモデルがセットされた状態でなければ例外をスロー</exception>
        public Hashtable GetEachAccurecyRate()
        {
            if (this.Ready == false) 
                throw new SystemException("準備はまだ完了しておりません。モデルを追加して下さい。");
            Hashtable ans = new Hashtable();
            // まずは判定
            Vector[] decisionChart = this.CreateDecisionChart(this._NNet, this._class); // 判定結果を格納する
            // 成績を取りまとめ
            for (int modelIndex = 0; modelIndex < this.Length; modelIndex++)
            {
                double accuracyRate = decisionChart[modelIndex][modelIndex] / decisionChart[modelIndex].Total;
                ans.Add(this._class[modelIndex].ID, accuracyRate);
            }
            return ans;
        }
        /// <summary>
        /// 全モデルの総合での識別率を返す
        /// <para>本メソッドが返すTotal Accuracy Rateの定義：Total Accuracy Rate = 正答の総数/特徴ベクトルの総数</para>
        /// <para>Total Accuracy Rateの定義からして、正答率の高いモデルの特徴ベクトルが多ければ全体の正答率も高くなることに留意して下さい。</para>
        /// <para>クラスモデルがセットされた状態でなければ例外をスローします。</para>
        /// </summary>
        /// <returns>総合識別率</returns>
        /// <exception cref="SystemException">クラスモデルがセットされた状態でなければ例外をスロー</exception>
        public double GetTotalAccurecyRate()
        {
            if (this.Ready == false) 
                throw new SystemException("準備はまだ完了しておりません。モデルを追加して下さい。");
            // まずは判定
            Vector[] decisionChart = this.CreateDecisionChart(this._NNet, this._class); // 判定結果を格納する
            // 成績を取りまとめ
            double featureSize = 0.0;                                                   // 特徴ベクトルの総数
            double rightAnswerSize = 0.0;                                               // 正答の総数
            for (int modelIndex = 0; modelIndex < this.Length; modelIndex++)
            {
                featureSize += this._class[modelIndex].Length;
                rightAnswerSize += decisionChart[modelIndex][modelIndex];
            }
            return rightAnswerSize / featureSize;
        }
        /// <summary>
        /// 判別表をCSV形式で保存する処理の本体です
        /// <para>正解率などの数値は引数で渡された結果を基に計算しています。</para>
        /// </summary>
        /// <param name="decisionChart">判定表</param>
        /// <param name="classForTest">識別対象の教師データモデル</param>
        /// <param name="format">文字出力フォーマット</param>
        /// <param name="fname">保存ファイル名</param>
        /// <param name="delimiter">区切り文字</param>
        /// <returns>文字列で表した判別表</returns>
        private string SaveDecisionChartBody(Vector[] decisionChart, List<Model> classForTest, string format = "0", string fname = "decision chart.csv", string delimiter = ",")
        {
            StringBuilder sb = new System.Text.StringBuilder(1000);

            // ラベルを出力
            sb.Append(delimiter);
            for (int j = 0; j < classForTest.Count; j++) sb.Append(delimiter + "output");
            sb.Append(System.Environment.NewLine);
            sb.Append(delimiter);
            for (int j = 0; j < classForTest.Count; j++) sb.Append(delimiter + classForTest[j].ID);
            sb.Append(System.Environment.NewLine);
            // 識別結果を出力
            for (int i = 0; i < decisionChart.Length; i++)
            {
                sb.Append("input" + delimiter + classForTest[i].ID);
                sb.Append(delimiter + decisionChart[i].ToString(format)).Append(System.Environment.NewLine);
            }
            // 個々の正答率を出力（あるモデルがそのモデルであると判別された割合）
            sb.Append(delimiter + "Accuracy Rate");
            for (int modelIndex = 0; modelIndex < classForTest.Count; modelIndex++)
            {
                double accuracyRate = decisionChart[modelIndex][modelIndex] / decisionChart[modelIndex].Total;
                sb.Append(delimiter + accuracyRate.ToString("f2"));
            }
            sb.Append(System.Environment.NewLine);
            double featureSize = 0.0;                                               // 特徴ベクトルの総数
            double rightAnswerSize = 0.0;                                           // 正答の総数
            for (int modelIndex = 0; modelIndex < classForTest.Count; modelIndex++)
            {
                featureSize += decisionChart[modelIndex].Total;
                rightAnswerSize += decisionChart[modelIndex][modelIndex];
            }
            // トータルでの正答率を出力
            double totalAccuracyRate = rightAnswerSize / featureSize;
            sb.Append(delimiter + "Total Accuracy Rate" + delimiter + totalAccuracyRate.ToString("f2")).Append(System.Environment.NewLine);

            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fname, false, Encoding.UTF8))
            {
                sw.Write(sb.ToString());
            }
            return sb.ToString();
        }
        /// <summary>
        /// 判別表をCSV形式で保存する
        /// </summary>
        /// <param name="fname">保存ファイル名</param>
        /// <param name="delimiter">区切り文字</param>
        /// <returns>文字列で表した判別表</returns>
        /// <exception cref="SystemException">クラスモデルがセットされた状態でなければ例外をスロー</exception>
        public string SaveDecisionChart(string fname = "decision chart.csv", string delimiter = ",")
        {
            if (this.Ready == false)
                throw new SystemException("実験の準備はまだ完了しておりません。モデルを追加して下さい。");
            // まずは判定
            Vector[] decisionChart = this.CreateDecisionChart(this._NNet, this._class); // 判定結果を格納する
            // 次はファイルへの出力
            return this.SaveDecisionChartBody(decisionChart, this._class, "0", fname, delimiter);
        }
        #endregion
        #region // 学習関係--------------------------------------------------------------------------
        /// <summary>
        /// 学習強度を格納した配列のチェックを行う
        /// <para>全て0以下であれば学習する必要がないのでfalseを返す。</para>
        /// </summary>
        /// <param name="learningStrength">学習強度</param>
        /// <returns>学習させる必要があるか<para>true：必要あり</para></returns>
        private Boolean CheckLearningStrength(int[] learningStrength)
        {
            Boolean ans = false;
            for (int i = 0; i < learningStrength.Length; i++)
                if (learningStrength[i] > 0) ans = true;
            return ans;
        }
        /// <summary>
        /// 学習を実行させる
        /// <para>学習強度は1:1に設定されますので、各モデルの保持する特徴ベクトルは等しい回数呼び出されます。</para>
        /// </summary>
        /// <param name="learningTimes">学習回数</param>
        public void Learn(int learningTimes)
        {
            this.Learn(learningTimes, this._NNet, this._class);
            return;
        }
        /// <summary>
        /// 学習を実行させる
        /// <para>学習強度は1:1に設定されますので、各モデルの保持する特徴ベクトルは等しい回数呼び出されます。</para>
        /// </summary>
        /// <param name="learningTimes">学習回数</param>
        /// <param name="discriminator">学習させる識別器</param>
        /// <param name="classForLearning">学習用教師データモデル</param>
        private void Learn(int learningTimes, NeuralNetwork discriminator, List<Model> classForLearning)
        {
            if (discriminator == null) 
                throw new Exception("判別器のインスタンスが確保されておらず、学習ができません。");
            int[] learningStrength = new int[this.Length];
            for (int i = 0; i < learningStrength.Length; i++) 
                learningStrength[i] = 1;
            this.Learn(learningTimes, learningStrength, discriminator, classForLearning);
        }
        /// <summary>
        /// 学習を実行させる
        /// <para>学習強度を引数で設定可能です。</para>
        /// </summary>
        /// <param name="learningTimes">学習回数</param>
        /// <param name="learningStrength">
        /// 学習強度（要素数はモデル数と一致すること）
        /// <para>
        /// 学習強度が各モデルの特徴数に比例しないように導入しました。
        /// 0に設定すると学習は行われません。
        /// 経験的には、1以外を設定してもろくなことはありませんでした。
        /// 浮動小数点に切り替えて、100回に1回多く学習するとかにしたら良いのかもしれませんが。。。
        /// </para>
        /// </param>
        /// <returns>識別器の出力誤差積算値</returns>
        public double Learn(int learningTimes, int[] learningStrength)
        {
            if (this.Ready == false) 
                throw new SystemException("学習の準備はまだ完了しておりません。モデルを追加して下さい。");
            if (this.Length != learningStrength.Length)
                throw new SystemException("学習強度の要素数がモデル数と一致しません。");
            return this.Learn(learningTimes, learningStrength, this._NNet, this._class); // 学習をスタート
        }
        /// <summary>
        /// 学習を実行させる
        /// <para>学習強度を引数で設定可能です。</para>
        /// </summary>
        /// <param name="learningTimes">学習回数</param>
        /// <param name="learningStrength">学習強度（要素数はモデル数と一致すること）</param>
        /// <param name="discriminator">学習させる識別器</param>
        /// <param name="classForLearning">学習用教師データモデル</param>
        /// <returns>識別器の出力誤差積算値</returns>
        private double Learn(int learningTimes, int[] learningStrength, NeuralNetwork discriminator, List<Model> classForLearning)
        {
            if (this.Ready == false) 
                throw new SystemException("学習の準備はまだ完了しておりません。モデルを追加して下さい。");
            if (classForLearning.Count != learningStrength.Length)
                throw new SystemException("学習強度の要素数がモデル数と一致しません。");
            if (discriminator == null) 
                throw new Exception("判別器のインスタンスが確保されておらず、学習ができません。");
            // 学習をスタート
            foreach (Model model in classForLearning)
                model.SequenceMode = this.PickOutMode;
            int learningCount = 0;
            double[] errorEachModel = new double[classForLearning.Count];
            Boolean need = CheckLearningStrength(learningStrength);                             // 学習させる必要性を学習強度からチェック
            int backupOfModelIndex = int.MaxValue;
            int sameCount = 0;

            while (learningCount < learningTimes && need)                                       // 学習回数をチェックしながらループ
            {
                int modelIndex;
                if (this.RandomModelChoice)
                    modelIndex = this.myRandom.Next(classForLearning.Count);
                else
                    modelIndex = learningCount % classForLearning.Count;
                if (backupOfModelIndex != modelIndex)                                           // 2回連続で学習させると学習が偏ってしまうので回避。エニグマみたい。
                {
                    Vector teature = this.CreateTeachingVector(modelIndex);
                    double error = 0.0;
                    int count = 0;
                    // 指定された強度分配に基づき、計算した回数分モデルを呼び出す
                    for (int i = 0; i < learningStrength[modelIndex]; i++)
                    {
                        Feature feature = classForLearning[modelIndex].GetFeature();
                        Vector outputForDebug = discriminator.Learn(feature, teature);          // 学習
                        learningCount++;
                        error += discriminator.TotalOutputError;
                        count++;
                    }
                    error /= (double)count;                                                     // 特徴量1件当たりの誤差量を計算
                    errorEachModel[modelIndex] = error;                                         // シーケンス上では何の役にも立っていないが、デバッグ用と思ってください。将来的には誤差の収束条件を入れたいと思っています。
                    sameCount = 0;
                }
                else
                {
                    if (sameCount < 100)                                                        // 同じ数が出るようになったらインスタンスをもう一度作ってみる
                        sameCount++;                                                            // 乱数ジェネレータが壊れるとかあほかと（並列演算が悪いの？）
                    else                                                                        // これで一応うまくいくことを確認した
                        this.myRandom = new Random();
                }
                backupOfModelIndex = modelIndex;
            }
            return discriminator.TotalOutputError;
        }
        /// <summary>
        /// 交差確認法（Cross Validation）を用いた特徴ベクトルの評価を行います
        /// <para>確認結果をファイルとして出力します。ファイル名は引数で指定可能です。</para>
        /// <para>モデル選択はランダム、モデルからの特徴データの取得もランダムとします。</para>
        /// <para>本メソッドによる学習成果はModelクラスの保持する識別器へは反映されません。</para>
        /// <para>交差検定で使用する分割数は教師データ数に合わせて自動的に決定されます。</para>
        /// <para>コード可読性が低いのでいつか変えると思う。。。</para>
        /// </summary>
        /// <param name="learningTimes">一回の試行当たりの学習回数</param>
        /// <param name="checkNum">試行回数</param>
        /// <param name="fname">保存ファイル名</param>
        /// <returns>交差検定で使用した分割数<para>最大でも10となります。</para></returns>
        public int CV(int learningTimes, int checkNum, string fname = "decision chart.csv")
        {
            if (learningTimes == 0 || checkNum == 0)
                throw new Exception("Teacherクラスにおいてエラーがスローされました。CV()の引数の値が不正です。");
            var backupOfPom = this.PickOutMode;                                 // 各設定のバックアップを取る
            this.PickOutMode = Model.ReadSequenceMode.AtRandom;                 // Lean()での学習をランダムにする
            var backupOfMode = this.RandomModelChoice;
            this.RandomModelChoice = true;

            int divideLimit = int.MaxValue;
            foreach (Model model in this._class)
            {
                model.Shuffle();                                                // 学習に使用する教師データをシャッフル
                if (divideLimit > model.Length) 
                    divideLimit = model.Length;                                 // モデル数に合わせて分割数を調整するために、最小の教師データ数を計る
                model.FeatureSupplyMode = Model.SupplyMode.NeglectParticularGroup;
            }
            if (divideLimit > 10)
                divideLimit = 10;                                               // 最大の分割数を10とする
            foreach (Model model in this._class) 
                model.DivisionNum = divideLimit;
            var sequence = new Vector[this.Length, checkNum * divideLimit];     // 時系列の識別率を格納する
            Parallel.For(0, checkNum * divideLimit, i =>
            //for(int i = 0; i < checkNum * divideLimit; i++)
            {
                NeuralNetwork nn = new NeuralNetwork();                         // ニューラルネットのインスタンスを確保
                nn.Setup(this._NNet.GetParameter());
                var localModel = new List<Model>(0);
                int maskIndex = i % divideLimit;
                foreach (Model model in this._class)
                {
                    var cpyModel = new Model(model);
                    cpyModel.IndexForDivision = maskIndex;
                    cpyModel.FeatureSupplyMode = Model.SupplyMode.NeglectParticularGroup;       // 特定のグループを無視するモードに切り替える
                    localModel.Add(cpyModel);
                }
                this.Learn(learningTimes, nn, localModel);                                      // 学習させる
                foreach (Model model in localModel)
                {
                    model.FeatureSupplyMode = Model.SupplyMode.TakeParticularGroup;             // 特定のグループのみをテストするモードに切り替える
                }
                var tempResult = this.CreateDecisionChart(nn, localModel);                      // 学習に使わなかった教師データを用いた識別を行う
                for (int modelIndex = 0; modelIndex < this.Length; modelIndex++) sequence[modelIndex, i] = tempResult[modelIndex]; // 識別の結果を保存しておく
            });
            // 平均の識別率を計算し、ファイルへ保存する
            var averageDecisionChart = new Vector[this.Length];
            for (int modelIndex = 0; modelIndex < this.Length; modelIndex++) 
                averageDecisionChart[modelIndex] = new Vector(this.Length);
            for (int i = 0; i < sequence.GetLength(1); i++)                                     // 平均の識別率を求めるために積算
                for (int modelIndex = 0; modelIndex < this.Length; modelIndex++)
                    averageDecisionChart[modelIndex] += sequence[modelIndex, i];
            for (int modelIndex = 0; modelIndex < this.Length; modelIndex++)                    // 平均の識別率を計算
                averageDecisionChart[modelIndex] /= ((double)checkNum * divideLimit);
            this.SaveDecisionChartBody(averageDecisionChart, this._class, "f1", fname);         // ファイルに保存
            // 識別率の標準偏差を計算し、ファイルへ保存する
            string fnameForDev = System.IO.Path.GetFileNameWithoutExtension(fname) + ".dev";    // 標準偏差を計算する準備
            var deviation = new Vector[this.Length];
            for (int modelIndex = 0; modelIndex < this.Length; modelIndex++) 
                deviation[modelIndex] = new Vector(this.Length);
            for (int i = 0; i < sequence.GetLength(1); i++)                                     // 分散*(n-1)を計算
                for (int modelIndex = 0; modelIndex < this.Length; modelIndex++)
                    deviation[modelIndex] += (sequence[modelIndex, i] - averageDecisionChart[modelIndex]).Pow(2.0);
            for (int modelIndex = 0; modelIndex < this.Length; modelIndex++)                    // 標準偏差を求める
                deviation[modelIndex] = (deviation[modelIndex] / (double)(checkNum * divideLimit - 1)).Pow(0.5);
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fnameForDev, false, Encoding.UTF8))
            {
                for (int j = 0; j < this.Length; j++) sw.Write("," + this.ModelIDs[j].ToString() + "を出力"); // ラベル
                sw.Write("\n");
                for (int i = 0; i < deviation.Length; i++)
                {
                    sw.Write(this.ModelIDs[i].ToString() + "を入力");
                    sw.WriteLine("," + deviation[i].ToString("f2"));
                }
                sw.Write("\n");
            }
            // 後始末
            this.PickOutMode = backupOfPom;                                                
            this.RandomModelChoice = backupOfMode;
            for (int modelIndex = 0; modelIndex < this.Length; modelIndex++)
            {
                this._class[modelIndex].FeatureSupplyMode = Model.SupplyMode.NonDivide;     // 分割しないように戻しておく
            }
            return divideLimit;
        }
        #endregion
        #region // コンストラクタ・初期化関係 -------------------------------------------------------
        /// <summary>
        /// ファイル名からクラスモデル名を抽出して返す
        /// </summary>
        /// <param name="fname">ファイル名</param>
        /// <returns>クラスモデル名</returns>
        public static string GetModelIDname(string fname)
        {
            string name = System.IO.Path.GetFileNameWithoutExtension(fname);
            string[] field = name.Split('_');
            return field[0];
        }
        /// <summary>
        /// ファイル名の一覧から、クラスモデル名を抽出して一覧として返す
        /// </summary>
        /// <param name="fnames">*.feaで該当したファイル名の一覧</param>
        /// <returns>クラスモデル名の一覧</returns>
        public static Hashtable GetModelIDs(string[] fnames)
        {
            Hashtable ans = new Hashtable();
            int modelIndex = 0;

            for (int i = 0; i < fnames.Length; i++)
            {
                string id = Teacher.GetModelIDname(fnames[i]);
                if (ans.Contains(id) == false)
                {
                    ans.Add(id, modelIndex);
                    modelIndex++;
                }
            }
            return ans;
        }
        /// <summary>
        /// モデルオブジェクトを用いてセットアップします
        /// <para>既にセットされていたモデルは初期化されます。</para>
        /// <para>なお、モデルオブジェクトはディープコピーされます。</para>
        /// </summary>
        /// <param name="models">セットするモデル</param>
        public void Setup(Model[] models)
        {
            this._class = new List<Model>(0);
            if (models == null)
                throw new Exception("TeacherクラスのSetup()メソッドにてエラーがスローされました。モデルのインスタンスが確保されていません。");

            int featureSize = 0;
            foreach (var model in models)
            {
                if (models == null)
                    throw new Exception("TeacherクラスのSetup()メソッドにてエラーがスローされました。モデルのインスタンスが確保されていません。");
                if (featureSize == 0)
                    featureSize = model.MemberSize;
                else if (featureSize != model.MemberSize)
                    throw new Exception("TeacherクラスのSetup()メソッドにてエラーがスローされました。特徴ベクトルの長さに統一性がありません。モデルに格納されている特徴ベクトルのサイズをチェックして下さい。");
            }
            // モデルをセット
            for(int i = 0; i < models.Length; i++)
            {
                this._class.Add(new Model(models[i]));
            }
            // ニューラルネットを構成
            this._NNet = new NeuralNetwork();                           // ニューラルネットクラスのインスタンスを生成
            this._NNet.Setup(this.parameter, featureSize, models.Length);
            return;
        }
        /// <summary>
        /// フォルダを指定してモデルデータを読み込む
        /// <para>既にセットされていたモデルは初期化されます。</para>
        /// <para>読み込んだファイル名の一覧よりモデルリストを作成し、ファイルの中身から入力層に必要なユニット数を確保します。</para>
        /// <para>読み込まれた特徴量がモデルによって異なる場合は、学習の段階で入力ユニット数と特徴ベクトルのサイズの不一致に関するエラーがスローされます。</para>
        /// </summary>
        /// <param name="dirName">特徴データの格納されたフォルダ名</param>
        /// <exception cref="SystemException">ファイルから読み込んだ特徴ベクトルの長さとニューラルネットの入力層ユニット数が一致しない場合にスロー</exception>
        public void Setup(string dirName)
        {
            this._class = new List<Model>(0);
            if (System.IO.Directory.Exists(dirName) == false)
                throw new ArgumentException("TeacherクラスのSetup()メソッドにてエラーがスローされました。指定されたフォルダは存在しません。");

            string[] fnames = System.IO.Directory.GetFiles(dirName, "*.fea");   // フォルダ内の指定拡張子を持つファイル一覧を取得
            if (fnames.Length > 1)
            {
                Hashtable modelID = Teacher.GetModelIDs(fnames);                 // ファイル名の一覧からユニークなモデルIDを取得
                if (modelID.Count >= 2)
                {
                    var models = new Model[modelID.Count];                      // 存在するファイルの数だけモデルを生成する
                    // クラスモデルを構成
                    foreach(var fname in fnames)
                    {
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
                                    string line = sr.ReadLine();                // 一行読み込み
                                    Feature feature = new Feature(line);        // 読み込んだテキストデータから特徴ベクトルを生成
                                    models[index].Add(feature);                 // 特徴を追加
                                }
                            }
                            catch (Exception e)
                            {
                                string message = "エラーがスローされました。\nエラーメッセージ：" + e.Message + "エラーの発生したファイル名：" + fname;
                                Console.WriteLine(message);
                                throw new Exception(message);                   // さらにスロー
                            }
                        }
                    }
                    this.Setup(models);
                }
                else
                    throw new Exception("TeacherクラスのSetup()メソッドにてエラーがスローされました。モデルが2つ以上見つかりませんでした。");
            }
            else
                throw new Exception("TeacherクラスのSetup()メソッドにてエラーがスローされました。特徴ベクトルを格納したファイル（*.fea）が2つ以上見つかりませんでした。フォルダ若しくは拡張子をチェックして下さい。");
            return;
        }
        /* コンストラクタ ----------------------------------------------------------*/
        /// <summary>
        /// 識別器を学習させるTeacherクラスのコンストラクタ
        /// </summary>
        /// <param name="parameter">学習器用のパラメータ<para>学習器のコンストラクタへモデル情報とともに渡されます。</para></param>
        public Teacher(Object parameter)
            :base()
        {
            this._class = new List<Model>(0);
            this.PickOutMode = Model.ReadSequenceMode.AtRandom;
            this.parameter = parameter;
            this.myRandom = new Random(parameter.GetHashCode());
            return;
        }
        #endregion
    }
}
