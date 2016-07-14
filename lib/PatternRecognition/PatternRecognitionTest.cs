/***************************************
 * PatternRecognitionTest
 * テストとサンプルコードを兼ねた静的クラス
 * 
 * [履歴]
 *          2012/6/20   作っただけ。まだ空です。
 *          2012/6/23   昔造ったテストコードを移植してみたけど、フォルダ名とデータセットはどうしよう？
 * *************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PatternRecognition;
using PatternRecognition.ArtificialNeuralNetwork;

namespace PatternRecognition
{
    /// <summary>
    /// パターン認識に関するテストとサンプルコードを兼ねた静的クラス
    /// </summary>
    static class PatternRecognitionTest
    {
        /// <summary>
        /// NeuralNetTeacherクラスを使って、既に生成されている特徴ベクトルをまとめたファイルを読み込ませて学習を行う part1
        /// </summary>
        static public void Iris_learning()
        {
            var minPara = new MinParameter(15, 1, 0.1);
            var NNteacher = new Teacher(minPara);                   // ニューラルネットの教師クラスのインスタンスを生成
            NNteacher.Setup("教師データの例　Iris");                // 学習データの入ったフォルダ名を渡して初期化
            int timeOfLearning = 10000;                             // 学習回数（厳密には一致しない）
            for (int i = 1; i <= 2; i++)                            // 2回ループさせる
            {
                NNteacher.Learn(timeOfLearning);
                Console.Write(NNteacher.SaveDecisionChart("decision chart " + (i * timeOfLearning).ToString() + ".csv", "\t")); // ファイルへ保存しつつコンソールへ出力する
            }
            NNteacher.SaveLearnigOutcome();                         // 学習した結合係数などをファイルに保存

            var discriminator = new Discriminator();                // 保存したファイルを使用して識別器を生成
            discriminator.Setup();                                  // これ以降、学習成果を利用して識別を行うことができる。このサンプルでは省略。
            return;
        }
        /// <summary>
        /// NeuralNetTeacherクラスを使って、既に生成されている特徴ベクトルをまとめたファイルを読み込ませて学習を行う part2
        /// </summary>
        static public void Iris_learning2()
        {
            var minPara = new MinParameter(13, 1, 0.1);
            var NNteacher = new Teacher(minPara);                               // ニューラルネットの教師クラスのインスタンスを生成
            NNteacher.Setup("教師データの例　Iris");
            
            int timeOfLearning = 10000;
            for (int i = 1; i <= 2; i++)                                        // 2回ループさせる
            {
                NNteacher.Learn(timeOfLearning);
                NNteacher.SaveDecisionChart("decision chart " + (i * timeOfLearning).ToString() + ".csv");
            }
            NNteacher.SaveLearnigOutcome();                                     // 学習した結合係数などをファイルに保存

            // これ以降は識別テスト
            var feature = new Feature("5.4	3.4	1.7	0.2");                      // セトナのデータ

            // 学習済みのニューラルネットワーククラスオブジェクトを渡して学習器を構成
            var discriminator = new Discriminator(NNteacher.GetNeuralNetwork(), NNteacher.ModelIDs);// これ以降、学習成果を利用して識別を行うことができる。
            var test = discriminator.Discriminate(feature, 2);                  // 結果を降順で2つもらう
            string outputstr = "";
            for (int i = 0; i < test.Length; i++)
                outputstr += test[i].ID + ", " + test[i].Likelihood.ToString("0.00") + "\n";
            Console.WriteLine(outputstr, "識別テストその1の結果");

            // ファイルから結合係数を読み込ませても同じ結果が出ることを確認する
            var discriminator2 = new Discriminator();
            discriminator2.Setup();
            var test2 = discriminator.Discriminate(feature);                    // テストを実施し、表示する
            Console.WriteLine("識別結果: " + test2.ID + ", " + test2.Likelihood.ToString("0.00"), "識別テストその2の結果");
            return;
        }
        /// <summary>
        /// XORの学習の例
        /// <para>NeuralNetworkクラスを直接操作して論理演算を実行する例です。</para>
        /// </summary>
        static public void XOR_learning()
        {
            var para = new Parameter(2, 5, 1, 1, 0.1);                          // ニューラルネットのパラメータ設定（中間層が多いと、学習係数を小さくとる必要がある）7層が限界・・
            var NNforXOR = new NeuralNetwork();                                 // ニューラルネットクラスのインスタンス生成
            NNforXOR.Setup(para);
            for (int i = 0; i < 100; i++)                                       // 学習開始
            {
                var ans = new double[4];
                Console.Write("now i: " + i.ToString() + "@ ");
                for (int k = 0; k < 100; k++)
                {
                    ans[0] = NNforXOR.Learn(new double[2] { 0.0, 0.0 }, 0.0);   // 特徴ベクトルと教師ベクトルを渡す
                    ans[1] = NNforXOR.Learn(new double[2] { 0.0, 1.0 }, 1.0);
                    ans[2] = NNforXOR.Learn(new double[2] { 1.0, 0.0 }, 1.0);
                    ans[3] = NNforXOR.Learn(new double[2] { 1.0, 1.0 }, 0.0);
                }
                Console.WriteLine("ans = " + ans[0].ToString("0.000") + ", " + ans[1].ToString("0.000") + ", " + ans[2].ToString("0.000") + ", " + ans[3].ToString("0.000") +
                    ", 最後の学習結果→Error variation," + NNforXOR.VariationOfError.ToString("0.000") + ", Total Error," + NNforXOR.TotalOutputError.ToString("0.000"));
            }
            NNforXOR.Save("NN.ini");
            return;
        }
        /// <summary>
        /// sin()関数の学習の例
        /// <para>NeuralNetworkクラスを直接利用して関数近似を行わせるサンプルです。</para>
        /// <para>
        /// 2011/12/25現在、0～1までの出力しか得られない。
        /// 出力関数と学習則を変更するか、関数近似時には値のマッピングが必要である。
        /// 他の人はどうやってんだろう？</para>
        /// </summary>
        static public void sin_learnig()
        {
            var para = new Parameter(1, 13, 1, 3, 0.01);            // ニューラルネットのパラメータ設定
            var NNforSin = new NeuralNetwork();                     // ニューラルネットクラスのインスタンス生成
            NNforSin.Setup(para);
            for (int i = 0; i < 500; i++)                           // 学習開始
            {
                Console.Write("\nnow i: " + i.ToString() + "@ ");
                Console.Write("ans = ");
                for (int k = 0; k < 15; k++)
                {
                    double theta = (double)k / 15.0 * 2.0 * Math.PI;
                    double ans = 0.0;
                    for (int l = 0; l < 100; l++) ans = NNforSin.Learn(theta, Math.Sin(theta));       // 特徴ベクトルと教師ベクトルを渡す
                    Console.Write(", " + ans.ToString("0.000"));
                }
            }
            NNforSin.Save("NN.ini");
            return;
        }
    }
}
