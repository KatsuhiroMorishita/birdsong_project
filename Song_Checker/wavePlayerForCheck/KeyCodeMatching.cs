/*****************************************
 * 文字列を解析して、入力されるキーとマッチングをとる
 * マッチしたと判断されると文字列を返す
 * 
 * 
 * 2012/1/26    森下作成
 * 
 * ***************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace wavePlayerForCheck
{
    /// <summary>
    /// 半角一文字で特定のキーワードを返すクラス
    /// <para>辞書の様に使います。</para>
    /// </summary>
    public class KeyCodeMatching
    {
        // ---- メンバ変数 ----------------------------------------------
        /// <summary>
        /// ファイルが開けていればtrue
        /// </summary>
        private Boolean _isOpen = false;
        /// <summary>
        /// コードリスト
        /// </summary>
        private Hashtable _codeTable = new Hashtable();
        // ---- プロパティ ----------------------------------------------
        /// <summary>
        /// ファイルを開いて処理できていればtrue
        /// </summary>
        public Boolean IsOen
        {
            get { return this._isOpen; }
        }
        // ---- メソッド ----------------------------------------------
        /// <summary>
        /// IDを取得する
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetID(string key)
        {
            if (key == "n") return "checked OK";
            else if (key == "r") return "rejected";     // この辺はデフォルトの動作
            if (this.IsOen && this._codeTable.ContainsKey(key)) return (string)this._codeTable[key];    // 該当したデータがあれば返す
            return "";                                  // 該当なしの場合
        }
        /// <summary>
        /// 登録がある文字列かどうかを確認します
        /// </summary>
        /// <param name="key">登録をチェックするキー</param>
        /// <returns>登録されていればtrue</returns>
        public Boolean IsContain(string key)
        {
            if (key == "n" || key == "r") return true;
            if (this.IsOen && this._codeTable.ContainsKey(key)) return true;
            return false;
        }
        /// <summary>
        /// テキストファイルを解析して、初期化します
        /// </summary>
        /// <param name="fname">解析させたいファイルの名前</param>
        public KeyCodeMatching(string fname)
        { 
            if(System.IO.File.Exists(fname))
            {
                try
                {
                    using (System.IO.StreamReader sr = new System.IO.StreamReader(fname, true))
                    {
                        while (sr.EndOfStream == false)             // 全行処理
                        {
                            string line = sr.ReadLine();
                            string[] field = line.Split(',');       // 文字列を分割して、保存させる
                            if (field.Length == 2)
                            {
                                string key = field[0];
                                string code = field[1];
                                this._codeTable.Add(key, code);
                            }
                        }
                    }
                    this._isOpen = true;
                }
                catch(Exception e)                              // エラー対策
                {
                    MessageBox.Show(e.Message);
                }
            }
        }
    }
}
