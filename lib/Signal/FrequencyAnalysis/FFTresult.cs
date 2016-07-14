/*************************************************************************
 * FFTresult.cs
 * FFT処理後の観測データを扱うクラス
 *  開発者：Katsuhiro Morishita(2010.5) Kumamoto-University
 * 
 * [名前空間]   Signal.FrequencyAnlysis
 * 
 * [更新情報]   
 *              2012/5/4    DFT.csより分割した。
 *              2012/6/16   コメントを見直す
 *              2012/6/17   GetFilterdResultの名前が間違っていたので、訂正ついでに名前をFilterへ変更した。
 *              2012/11/1   プロパティに、MaxAbs, MedianAbsを追加
 * ***********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Signal.FrequencyAnalysis
{
    public partial class FFTresult
    {
        /// <summary>
        /// FFTの処理データと周波数をセットにした構造体
        /// </summary>
        public struct FFTresultPlusFreq
        {
            /// <summary>
            /// FFT処理後のデータ
            /// </summary>
            public readonly FFTdata data;
            /// <summary>
            /// 周波数[Hz]
            /// </summary>
            public readonly double frequency;
            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="x">FFT処理後のデータ</param>
            /// <param name="freq">周波数[Hz]</param>
            public FFTresultPlusFreq(FFTdata x, double freq)
            {
                this.data = x;
                this.frequency = freq;
            }
        }
    }

    /// <summary>
    /// FFT処理後の観測データを扱うクラス
    /// </summary>
    public partial class FFTresult
    {
        /**************************** グローバル変数 *******************************/
        /// <summary>
        /// FFT処理データ
        /// </summary>
        private FFTdata[] _data;
        /// <summary>
        /// サンプリング周波数[Hz]
        /// </summary>
        private double _samplingFrequency = 0;
        /// <summary>
        /// FFTのサイズ（例：4096 point）
        /// </summary>
        private int _fftPoint = 0;
        /**************************** プロパティ *******************************/
        /// <summary>
        /// 観測データ数
        /// </summary>
        public int Length { get { return this._data.Length / 2; } }
        /// <summary>
        /// 表現されている最大の周波数[Hz]
        /// </summary>
        public double MaxFrequency { get { return this._samplingFrequency / 2L; } }
        /// <summary>
        /// 表現されている最小の周波数分解能[Hz]
        /// </summary>
        public double FrequencyResolution { get { return this.GetFreq(1); } }
        /// <summary>
        /// 最大のパワー（絶対値）
        /// </summary>
        public double MaxAbs
        {
            get;
            private set;
        }
        /// <summary>
        /// パワーの中央値（絶対値）
        /// </summary>
        public double MedianAbs
        {
            get;
            private set;
        }
        /**************************** メソッド *******************************/
        /// <summary>
        /// 結果を文字列で出力する
        /// </summary>
        /// <returns>文字列で表した結果</returns>
        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(5000);
            sb.Append("Frequency[Hz]").Append(",").Append("Altitude").Append("\n");
            for (int i = 0; i < this.Length; i++)
                sb.Append(this.GetFreq(i)).Append(",").Append(this._data[i].Log).Append("\n");
            return sb.ToString();
        }
        /// <summary>
        /// 結果をファイルに保存する
        /// </summary>
        /// <param name="fname">ファイル名</param>
        public void Save(string fname = "hoge.csv")
        {
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fname, false, System.Text.Encoding.GetEncoding("shift_jis")))
            {
                sw.Write(this.ToString());
            }
            return;
        }
        /// <summary>
        /// 要素番号を入れると、その要素の周波数を計算して返す
        /// </summary>
        /// <param name="index">要素番号</param>
        /// <returns>周波数[Hz]</returns>
        private double GetFreq(int index)
        {
            return (double)index * this._samplingFrequency / ((double)this._fftPoint - 2);
        }
        /// <summary>
        /// 指定した要素番号のFFTの結果を取得する
        /// </summary>
        /// <param name="index">要素番号（0～this.Lenght - 1）</param>
        /// <returns>周波数と結びつけたFFT結果</returns>
        public FFTresultPlusFreq GetData(int index)
        {
            if (index < this._data.Length && index >= 0)                         // 要素番号のチェック
                return new FFTresultPlusFreq(this._data[index], this.GetFreq(index));
            else
                return new FFTresultPlusFreq();
        }
        /// <summary>
        /// パワーをdouble型配列として返す
        /// </summary>
        /// <returns>double型一次元配列に格納されたパワー</returns>
        public double[] GetPowerArray()
        {
            double[] ans = new double[this.Length];
            for (int i = 0; i < ans.Length; i++) ans[i] = this._data[i].Power;
            return ans;
        }
        /// <summary>
        /// パワーに指定帯域の矩形窓をかけて、double型配列として返す
        /// <para>指定した帯域のみ値を格納するし、指定帯域外は0.0を格納します。</para>
        /// <para>配列には折り返し以降の周波数は含まれません。</para>
        /// </summary>
        /// <param name="band">帯域</param>
        /// <returns></returns>
        public double[] GetPowerArray(Band band)
        {
            double[] ans = new double[this.Length];
            for (int i = 0; i < ans.Length; i++)
            {
                double freq = this.GetFreq(i);
                if (freq >= band.Min && freq <= band.Max)
                    ans[i] = this._data[i].Power;
                else
                    ans[i] = 0.0;
            }
            return ans;
        }
        /// <summary>
        /// 指定帯域でフィルタをかけたFFTresultオブジェクトを返す
        /// </summary>
        /// <param name="band">帯域</param>
        /// <returns>フィルタリングされたFFTresult<para>サンプリング周波数より先の折り返し分はコピーされません。</para></returns>
        public FFTresult Filter(Band band)
        {
            FFTdata[] copy = new FFTdata[this._data.Length];
            for (int i = 0; i < copy.Length; i++)
            {
                double freq = this.GetFreq(i);
                if (freq >= band.Min && freq <= band.Max)
                    copy[i] = this._data[i];
                else
                    copy[i] = new FFTdata();
            }
            return new FFTresult(copy, this._samplingFrequency, this._fftPoint);
        }
        /// <summary>
        /// 指定周波数がどの要素番号に該当するかを返す
        /// </summary>
        /// <param name="frequency">周波数[Hz]</param>
        /// <returns>要素番号</returns>
        private int GetIndex(double frequency)
        {
            int index = 0;
            if (this._samplingFrequency != 0 && this._fftPoint != 0)
                index = (int)(frequency * ((long)this._fftPoint - 2) / this._samplingFrequency);
            return index;
        }
        /// <summary>
        /// 指定帯域におけるパワースペクトル密度 （Power Spectrum Density,PSD）を返す
        /// <para>例えば、20kHzまでしか帯域パワー場存在しない（@サンプリング周波数が40kHz）場合、30kHzまでの帯域パワー密度を要求すると20kHzまでを指定した時と比較して小さな値となります。</para>
        /// </summary>
        /// <param name="min_frequency">帯域の下限周波数[Hz]</param>
        /// <param name="max_frequency">帯域の上限周波数[Hz]</param>
        /// <returns>パワースペクトル密度[V^2/Hz]</returns>
        /// <exception cref="SystemException">帯域パワーを定義できない場合にスロー</exception>
        public double GetPSD(double min_frequency, double max_frequency)
        {
            double psd = 0.0;

            if (this._samplingFrequency == 0 || this._fftPoint == 0) // エラーを防ぐための処置 
            {
                Console.WriteLine("FFTresultクラスのGetPSDメソッドにてエラーがスローされました。\n帯域パワーを定義できませんでした。");
                throw new SystemException("帯域パワーを定義できませんでした。");
            }
            // 指定された帯域幅におけるインデックスの幅をあらかじめ計算しておく
            int indexWidth = this.GetIndex(max_frequency) - this.GetIndex(min_frequency) + 1;
            // 指定された帯域があり得なければ修正
            if (min_frequency < 0) min_frequency = 0;
            if (max_frequency > (this._samplingFrequency / 2)) max_frequency = (this._samplingFrequency / 2);
            // 可能なら帯域パワーを計算する
            if (min_frequency < max_frequency)
            {
                int max_index = this.GetIndex(max_frequency);           // 最大周波数に対応した要素番号を取得
                int min_index = this.GetIndex(min_frequency);           // 最小周波数に対応した要素番号を取得
                for (int i = min_index; i <= max_index; i++)
                    psd += this.GetData(i).data.Power;                  // 積分
                psd /= ((double)indexWidth * (double)this.FrequencyResolution); // 単位周波数あたりに変換
            }
            else
            {
                psd = 0.0;
            }
            return psd;
        }
        /// <summary>
        /// 指定帯域におけるパワースペクトル密度 （Power Spectrum Density,PSD）を返す その2
        /// <para>帯域を指定できる構造体を使用しているので、配列での処理がやりやすくなったと思う。</para>
        /// </summary>
        /// <param name="band">帯域</param>
        /// <returns>パワースペクトル密度[V^2/Hz]</returns>
        public double GetPSD(Band band)
        {
            return this.GetPSD(band.Min, band.Max);
        }
        /// <summary>
        /// 指定帯域におけるパワースペクトル密度 （Power Spectrum Density,PSD）を返す その3
        /// <para>窓関数を掛けた帯域パワーを返します。</para>
        /// </summary>
        /// <param name="band">帯域</param>
        /// <param name="windowKind">窓関数の種類</param>
        /// <returns>パワースペクトル密度[V^2/Hz]</returns>
        /// <exception cref="SystemException">帯域パワーを定義できない場合にスロー</exception>
        public double GetPSD(Band band, Window.WindowKind windowKind)
        {
            double psd = 0.0;

            if (this._samplingFrequency != 0 && this._fftPoint != 0 && band.Max < (this._samplingFrequency / 2)) // エラーを防ぐための処置 
            {
                int max_index = this.GetIndex(band.Max);                // 最大周波数に対応した要素番号を取得
                int min_index = this.GetIndex(band.Min);                // 最小周波数に対応した要素番号を取得
                double[] psd_arr = new double[max_index - min_index + 1];
                for (int i = min_index, k = 0; i <= max_index; i++, k++)
                    psd_arr[k] = this.GetData(i).data.Power;            // 指定帯域のパワーを取得
                psd_arr = Window.Windowing(psd_arr, windowKind);        // 窓関数を掛ける
                foreach (double v in psd_arr) psd += v;                 // 積分
                psd /= ((double)(max_index - min_index + 1) * (double)this.FrequencyResolution); // 単位周波数あたりに変換
            }
            else throw new SystemException("帯域パワーを定義できませんでした。");
            return psd;
        }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="setData">観測データ配列</param>
        /// <param name="samplingFrequency">元データのサンプリング周波数</param>
        /// <param name="FFTpoint">FFTポイント数</param>
        public FFTresult(FFTdata[] setData, double samplingFrequency, int FFTpoint)
        {
            this._samplingFrequency = samplingFrequency;
            this._fftPoint = FFTpoint;

            // 要素をコピー
            List<FFTdata> _list = new List<FFTdata>();
            for (int i = 0; i < setData.Length; i++)
                _list.Add(setData[i]);
            this._data = _list.ToArray();
            // 走査のためにソート
            _list.Sort();
            // 最大値を求める
            this.MaxAbs = _list[_list.Count - 1].Abs;
            // 中央値を求める
            var indexMedian1 = _list.Count / 2;
            var median1 = _list[indexMedian1];
            var median = median1.Abs;
            if (_list.Count % 2 == 0)
            {
                var indexMedian2 = indexMedian1 - 1;
                var median2 = _list[indexMedian2];
                median = (median1.Abs + median2.Abs) / 2.0;
            }
            this.MedianAbs = median;
            
        }
    }
}
