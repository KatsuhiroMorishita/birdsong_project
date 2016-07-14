/***************************************************************************
 * [ソフトウェア名]
 *      Model.cs
 * [開発者]
 *      K.Morishita (Kumamoto-Univ. @ 2011.12)
 * [概要]
 *      特徴を表すモデルです。
 *      
 * [履歴]
 *      2012/5/9    ソースコードの分割によりファイルとして独立した。
 *      2012/5/10   特徴ベクトルの読み出しをランダムにすることも可能なように改良した。
 *      2012/5/12   コンストラクタ　public Model(string _id, Feature[] _members)にエラー対策を追加した。
 *      2012/5/13   機能がかぶっているGetMember()を廃止
 *      2012/5/14   ジャックナイフ判定に備えて、保持している特徴ベクトルの一部をマスクすることで外部へ供用するデータを制限する機能を搭載した。
 *      2013/2/2    コメントを修正した。
 *                  特徴ベクトルのサイズを返すように、プロパティMemberSizeを追加した。
 * *************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PatternRecognition
{
    /// <summary>
    /// モデル
    /// <para>学習機に対して学習させたいデータを提供します。</para>
    /// </summary>
    public class Model
    {
        /* 列挙体 ***********************************/
        /// <summary>
        /// 特徴ベクトルの読み出しモード
        /// </summary>
        public enum ReadSequenceMode 
        { 
            /// <summary>
            /// ランダムに読み出す
            /// </summary>
            AtRandom,
            /// <summary>
            /// 格納されている特徴ベクトルを順番に読み出すモード
            /// </summary>
            InRotation
        }
        /// <summary>
        /// 特徴ベクトルの供給モード
        /// </summary>
        public enum SupplyMode
        {
            /// <summary>
            /// 保持する特徴データを分割したと仮定して、特定の組を無視して特徴ベクトルを供給する
            /// </summary>
            NeglectParticularGroup,
            /// <summary>
            /// 保持する特徴データを分割したと仮定して、特定の組から特徴ベクトルを供給する
            /// </summary>
            TakeParticularGroup,
            /// <summary>
            /// 分割を行わず、インデックス無視して特徴ベクトルを供給する
            /// </summary>
            NonDivide
        }
        /* メンバ変数 ***********************************/
        /// <summary>
        /// クラス名
        /// </summary>
        private readonly string className;
        /// <summary>
        /// クラスを構成する特徴ベクトルバッファ
        /// <para>特徴ベクトルを格納します。</para>
        /// </summary>
        private List<Feature> buff = new List<Feature>();
        /// <summary>
        /// 提供用の特徴ベクトルバッファ
        /// <para>特徴ベクトルを格納します。</para>
        /// </summary>
        private List<Feature> offerBuff = new List<Feature>();
        /// <summary>
        /// 特徴ベクトルの読み出しポイント
        /// <para>GetFeatureメソッドの読み出し時に使用する。</para>
        /// </summary>
        private int readPoint = 0;
        /// <summary>
        /// 乱数ジェネレータ
        /// <para>モデル内に格納している特徴ベクトルを外部から読み取る際にランダムにするのに利用します。</para>
        /// </summary>
        private Random myRandom;
        /// <summary>
        /// 分割数
        /// <para>供給モードがNonGroup以外の場合に有効となる特徴ベクトルの分割数です。</para>
        /// </summary>
        private int divisionNum;
        /// <summary>
        /// 分割時の注目インデックス
        /// <para>供給モードがNonGroup以外の場合に有効となるインデックスです。</para>
        /// <para>このインデックスに該当するグループはFeatureSupplyModeに従い無視・注目されます。</para>
        /// <para>インデックスはセット時に評価され、不正な場合は適当な数字に置き換えられます。</para>
        /// </summary>
        private int indexForDivision;
        /* プロパティ ***********************************/
        /// <summary>
        /// 構成メンバの大きさ
        /// <para>特徴ベクトルのサイズです。</para>
        /// </summary>
        public int MemberSize
        {
            get;
            private set;
        }
        /// <summary>
        /// 保持している全特徴ベクトル数
        /// </summary>
        public int Length 
        {
            get { return this.buff.Count; }
        }
        /// <summary>
        /// 現時点でGetFeature()メソッドが返すことのできる特徴ベクトルの数
        /// <para>バグを検出した場合に-1を返すことがあります。</para>
        /// </summary>
        public int LengthForGetFeature
        {
            get
            {
                int size = this.buff.Count / this.DivisionNum;
                if (this.FeatureSupplyMode == SupplyMode.NonDivide)
                    return this.Length;
                else if (this.FeatureSupplyMode == SupplyMode.NeglectParticularGroup)
                {
                    if (this.DivisionNum - 1 == this.IndexForDivision)
                        return size * (this.DivisionNum - 1);
                    else
                        return this.Length - size;
                }
                else if (this.FeatureSupplyMode == SupplyMode.TakeParticularGroup)
                {
                    if (this.DivisionNum - 1 == this.IndexForDivision)
                        return this.Length - size * (this.DivisionNum - 1);
                    else
                        return size;
                }
                return -1;// errorコード
            }
        }
        /// <summary>
        /// クラス名（ID）
        /// </summary>
        public string ID 
        { 
            get { return this.className; } 
        }
        /// <summary>
        /// 読み込みモード
        /// <para>ランダムに読み出すか、順番で読みだすかを決めます。</para>
        /// </summary>
        public ReadSequenceMode SequenceMode
        {
            get;
            set;
        }
        /// <summary>
        /// 特徴ベクトルの供給モード
        /// <para>特徴ベクトルの提供する際に分割し、特定のインデックスに注目するかどうかを決定します。</para>
        /// <para>NonGroupの場合、分割数設定</para>
        /// </summary>
        public SupplyMode FeatureSupplyMode
        {
            get;
            set;
        }
        /// <summary>
        /// 分割数
        /// <para>供給モードがNonGroup以外の場合に有効となる特徴ベクトルの分割数です。</para>
        /// <para>分割設定数はセット時に評価され、不正な場合は適当な数字に置き換えられます。</para>
        /// <para>なお、本値の変更によりIndexForDivisionに矛盾が生じる場合、IndexForDivisionは自動的に0にリセットされます。</para>
        /// </summary>
        public int DivisionNum
        {
            get
            {
                return this.divisionNum;
            }
            set
            {
                int setnum = value;
                if (setnum <= 0) setnum = 1;
                if(setnum > this.Length) setnum = this.Length + 1;
                this.divisionNum = setnum;
                if (this.divisionNum >= this.IndexForDivision) this.IndexForDivision = 0;
            }
        }
        /// <summary>
        /// 分割時の注目インデックス
        /// <para>供給モードがNonGroup以外の場合に有効となるインデックスです。</para>
        /// <para>
        /// インデックスはセット時に評価され、不正な場合は適当な数字に置き換えられます。
        /// GetFeature()は、DivisionNumにより分割したと仮定した教師ベクトル群の内、指定したインデックス番号のみFeatureSupplyModeに従い無視・注目してベクトルを返ます。
        /// ただし、DivisionNumが1の場合はこの引数自体が無視され、全ベクトルが返り値の対象となります。
        /// </para>
        /// </summary>
        public int IndexForDivision
        {
            get
            {
                return this.indexForDivision;
            }
            set
            {
                int setnum = value;
                if (setnum < 0) setnum = 0;
                if (setnum > this.DivisionNum) setnum = this.DivisionNum - 1;
                this.indexForDivision = setnum;
            }
        }
        /* インデクサ ***********************************/
        /// <summary>
        /// インデクサ
        /// <para>本モデルが保持している全ての特徴ベクトルにアクセス可能です。</para>
        /// </summary>
        /// <param name="i">インデックス番号</param>
        /// <returns>指定インデックスの特徴ベクトル</returns>
        public Feature this[int i]
        {
            get
            {
                if (i < 0 || i >= this.Length) throw new SystemException("インデックスの指定範囲が不正です。");
                return this.buff[i];
            }
        }
        /* メソッド *************************************/
        /// <summary>
        /// クラスの構成メンバ（特徴ベクトル）を文字列化する
        /// <para>統計処理に利用可能なように最後の要素にクラス名をラベルとして追加しています。</para>
        /// </summary>
        /// <returns>文字列化したクラスメンバ</returns>
        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(1500);
            for (int i = 0; i < this.Length; i++) sb.Append(this.buff[i].ToString()).Append(this.ID).Append("\n");
            return sb.ToString();
        }
        /// <summary>
        /// バッファメンバをシャッフルする
        /// </summary>
        public void Shuffle()
        {
            List<Feature> temp = new List<Feature>();               // コピーを作る
            foreach (Feature member in this.buff) temp.Add(member);
            this.offerBuff.Clear();
            while(temp.Count != 0)
            {
                int readpoint = this.myRandom.Next(temp.Count);
                this.offerBuff.Add(temp[readpoint]);
                temp.RemoveAt(readpoint);                           // 格納し終えた特徴は削除
            }
        }
        /// <summary>
        /// 保持する特徴ベクトルを分割し、指定インデックスを除いた部分から一つの特徴ベクトルを返す
        /// <para>学習用に特徴ベクトルを提供します。提供方法はランダムとシーケンシャルの二通りが可能です。設定はプロパティを通して設定してください。</para>
        /// </summary>
        /// <returns>特徴ベクトル</returns>
        public Feature GetFeature()
        {
            if (this.IndexForDivision < 0 || this.DivisionNum <= this.IndexForDivision) throw new Exception("ModelクラスのGetFeature()においてエラーがスローされました。IndexForDivisionが範囲外です。");
            if (this.offerBuff.Count < this.buff.Count) this.Shuffle(); // 供用するデータ数を確認して、少ない場合は供用バッファへ再格納する
            if (this.offerBuff.Count == 0) return null;                 // 供給は不可能

            Feature ans;
            int readpoint;
            int size = this.offerBuff.Count / this.DivisionNum;
            Boolean loopFlag = false;
            do
            {
                if (this.SequenceMode == ReadSequenceMode.InRotation)   // 順番に読み出す
                {
                    readpoint = (this.readPoint++) % this.offerBuff.Count;
                    this.readPoint %= this.offerBuff.Count;
                }
                else
                {
                    if (this.DivisionNum == 1 || this.FeatureSupplyMode != SupplyMode.TakeParticularGroup)
                        readpoint = this.myRandom.Next(this.offerBuff.Count);
                    else
                    {
                        
                        int min = this.IndexForDivision * size;
                        int max = (this.IndexForDivision + 1) * size;
                        readpoint = this.myRandom.Next(min, max);
                    }
                }
                if (this.FeatureSupplyMode == SupplyMode.NeglectParticularGroup)
                    loopFlag = ((readpoint / size) == this.IndexForDivision) && this.DivisionNum != 1;
                else if (this.FeatureSupplyMode == SupplyMode.TakeParticularGroup)
                    loopFlag = ((readpoint / size) != this.IndexForDivision) && this.DivisionNum != 1;
            } while (loopFlag);
            ans = this.offerBuff[readpoint];
            return ans;
        }
        /// <summary>
        /// 特徴ベクトルの追加
        /// </summary>
        /// <param name="newMember">追加メンバ</param>
        public void Add(Feature newMember)
        {
            this.buff.Add(newMember);
            if (this.Length == 1)
                this.MemberSize = newMember.Length;
            else
                if (this.MemberSize != newMember.Length) throw new SystemException("既に登録された特徴ベクトルのサイズと追加しようとした特徴ベクトルのサイズが異なります。");
            return;
        }
        // コンストラクタ関係------------------------------------------------------------------
        /// <summary>
        /// コンストラクタ
        /// <para>クラスIDのみを設定するコンストラクタです。</para>
        /// </summary>
        /// <param name="_id">クラス名</param>
        public Model(string _id)
            :base()
        {
            this.className = _id;
            this.SequenceMode = ReadSequenceMode.AtRandom;
            this.FeatureSupplyMode = SupplyMode.NonDivide;
            this.DivisionNum = 1;
            this.IndexForDivision = 0;
            this.myRandom = new Random(this.className.GetHashCode());
            this.MemberSize = 0;
        }
        /// <summary>
        /// メンバの初期化付コンストラクタ
        /// <para>クラスメンバはディープコピーされます。</para>
        /// </summary>
        /// <param name="_id">クラス名</param>
        /// <param name="_members">メンバ</param>
        /// <exception cref="SystemException">メンバがnullであったりメンバ数が0もしくは渡された特徴ベクトルの時限数が不一致だとスロー</exception>
        public Model(string _id, Feature[] _members)
            : base()
        {
            this.className = _id;
            this.SequenceMode = ReadSequenceMode.AtRandom;
            this.FeatureSupplyMode = SupplyMode.NonDivide;
            this.DivisionNum = 1;
            this.IndexForDivision = 0;
            if (_members == null) throw new SystemException("メンバがnullです。");
            if (_members.Length == 0) throw new SystemException("配列サイズが0です。");

            this.MemberSize = _members[0].Length;
            foreach (Feature newMenber in _members)
            {
                this.buff.Add(new Feature(newMenber));
                if (this.MemberSize != newMenber.Length) throw new SystemException("渡された特徴ベクトルのサイズが不統一です。");
            }
            this.myRandom = new Random(this.className.GetHashCode());
        }
        /// <summary>
        /// コピーコンストラクタ
        /// <para>メンバーはディープコピーされます。</para>
        /// </summary>
        /// <param name="model">コピー元のモデル</param>
        public Model(Model model)
            : base()
        {
            this.className = model.className;
            this.SequenceMode = model.SequenceMode;
            this.FeatureSupplyMode = model.FeatureSupplyMode;
            this.MemberSize = model.MemberSize;
            foreach (Feature newMenber in model.buff)
            {
                this.buff.Add(new Feature(newMenber));
                if (this.MemberSize != newMenber.Length) throw new SystemException("渡された特徴ベクトルのサイズが不統一です。");
            }
            this.DivisionNum = model.DivisionNum;           // これらはメンバーのコピーの後でないと、代入ができない。詳しくはプロパティを見てほしい。
            this.IndexForDivision = model.IndexForDivision;
            this.myRandom = new Random(this.className.GetHashCode());
        }
    }
}
