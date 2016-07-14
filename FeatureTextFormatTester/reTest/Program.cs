/***************************************************************************
 * [ソフトウェア名]
 *      FeatureTextFormatTester.exe
 * [開発者]
 *      K.Morishita (Kumamoto-Univ. @ 2013.3)
 * [概要]
 *      鳥の音声解析用ソフトウェアツール群のうちの一つです。
 *      本ソフトウェアでは、学習・識別に用いるテキストファイルのフォーマットを解析します。
 *      学習処理の前に一度実行させておくと安心できます。
 * [使用方法]
 *      コンソールを立ち上げて、アプリ名に続いて処理したいファイル名を指定してください。
 * 
 * [参考資料]
 *      
 * [検討課題]
 *      
 * [履歴]
 *      2013/3/16   開発開始
 *      2013/3/17   一応できた
 *      2013/3/22    FeatureTextLineParseクラスをBirdSongFeatureプロジェクトに移して、これを使うように変更した。
 * *************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Text.RegularExpressions;   // for Regex
using BirdSongFeature.Feature;

namespace reTest
{
    class Program
    {
        /// <summary>
        /// 引数で指定されたファイルに含まれる文字列について特徴の記述フォーマットに合致していることを確認し、エラー件数を返す
        /// <para>普通は返り値はシステムエラーを返すのですが、このプログラムに限っては処理結果としてエラー件数を返します。</para>
        /// </summary>
        /// <param name="args">第1引数：解析したいファイル名</param>
        /// <returns>エラー件数</returns>
        static int Main(string[] args)
        {
            int errorCount = 0;

            Console.WriteLine("Process start...");
            if (!System.IO.File.Exists(args[0]))
                Console.WriteLine("Please input a file name or path.");
            try
            {
                if (System.IO.File.Exists(args[0]))
                {
                    var fname = args[0];// "hoge.txt";
                    using (System.IO.StreamReader sr = new System.IO.StreamReader(fname, Encoding.UTF8))
                    {
                        //内容を一行ずつ読み込む
                        int i = 1;
                        while (sr.EndOfStream == false)
                        {
                            string line = sr.ReadLine();
                            FeatureTextLineParser parser = new FeatureTextLineParser(line);
                            if (parser.Success == false)
                            {
                                Console.WriteLine(i.ToString() + "行目にて解析エラーが発生しました。");
                                errorCount++;
                            }
                            i++;
                        }
                    }
                }
            }
            catch (SystemException e)
            {
                Console.WriteLine("例外がスローされました。選択したファイルをチェックしてください。\nerror msg.: " + e.Message);
            }
            Console.WriteLine("Error count: " + errorCount.ToString());
            Console.WriteLine("Process finished.");
            return errorCount;
        }
    }
}
