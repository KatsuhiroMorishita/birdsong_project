/*************************************************
 * SignalTest
 * Signal名前空間内のクラス群をテストする静的クラス
 * 
 * K.Morishtia
 * 
 * [履歴]
 *          2012/6/15   整備開始
 *                      FftSin(), FftSquare()を整備
 * ***********************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Signal.FrequencyAnalysis;

namespace Signal
{
    /// <summary>
    /// Signal名前空間内のクラス群をテストする静的クラス
    /// </summary>
    public static class SignalTest
    {
        /// <summary>
        /// サイン波形を返す
        /// </summary>
        /// <param name="freq">サイン波の周波数[Hz]</param>
        /// <param name="phase">位相[rad]</param>
        /// <param name="samplingFreq">サンプリング周波数[Hz]</param>
        /// <param name="amplitude">振幅</param>
        /// <param name="dataNum">生成するデータ数</param>
        /// <returns>生成したサイン波</returns>
        private static double[] GetSin(double freq, double phase, double samplingFreq, double amplitude, int dataNum)
        {
            double step = 1.0 / samplingFreq;
            var sin = new double[dataNum];

            for (int i = 0; i < sin.Length; i++) sin[i] = amplitude * Math.Sin(2.0 * Math.PI * freq * (double)i * step + phase);
            return sin;
        }
        /// <summary>
        /// 矩形波形を返す
        /// </summary>
        /// <param name="freq">サイン波の周波数[Hz]</param>
        /// <param name="duty">デューティ比<para>0-1.0</para></param>
        /// <param name="samplingFreq">サンプリング周波数[Hz]</param>
        /// <param name="amplitude">振幅</param>
        /// <param name="dataNum">生成するデータ数</param>
        /// <returns>生成した矩形波</returns>
        private static double[] GetSquare(double freq, double duty, double samplingFreq, double amplitude, int dataNum)
        {
            double step = 1.0 / samplingFreq;
            var square = new double[dataNum];

            for (int i = 0; i < square.Length; i++)
            {
                double t = (double)i * step;
                double t2 = t % (1.0 / freq);
                if (t2 < (1.0 / freq) * duty)
                    square[i] = amplitude;
                else
                    square[i] = - amplitude;
            }
            return square;
        }
        /// <summary>
        /// サイン関数をFFTにより周波数分析を行う
        /// <para>実行後、2つのファイルが作成されますのでソースコードと合わせて確認してみてください。</para>
        /// </summary>
        public static void FftSin()
        {
            double freq, samplingFreq;
            var dft = new DFT(FFTpoint.size4096, Window.WindowKind.NoWindow);

            // part 1
            // デフォルトではFFT窓の幅よりも小さなデータになるようにした場合
            freq = 60.5;
            samplingFreq = freq * 4.0;
            var result = dft.FFT(SignalTest.GetSin(freq, 0.0, samplingFreq, 1.0, dft.FFTsize / 2), samplingFreq);
            result.Save("FftSin part 1.csv");

            // part 2
            // 窓いっぱいに波形を埋めた場合
            freq = 60.5;
            samplingFreq = freq * 4.0;
            result = dft.FFT(SignalTest.GetSin(freq, 0.0, samplingFreq, 1.0, dft.FFTsize), samplingFreq);
            result.Save("FftSin part 2.csv");
        }
        /// <summary>
        /// 矩形波をFFTにより周波数分析を行う
        /// <para>実行後、ファイルが作成されますのでソースコードと合わせて確認してみてください。</para>
        /// </summary>
        public static void FftSquare()
        {
            double freq = 50.0;
            double samplingFreq = freq * 100.0;
            var dft = new DFT(FFTpoint.size4096, Window.WindowKind.NoWindow);
            var result = dft.FFT(SignalTest.GetSquare(freq, 0.5, samplingFreq, 1.0, dft.FFTsize), samplingFreq);
            result.Save("FftSquare.csv");
            return;
        }
    }
}
