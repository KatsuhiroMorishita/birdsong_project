using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BirdSongFeature.SongDetection.KMCustom1stDetector
{
    /// <summary>
    /// 検出されたエッジの種類
    /// </summary>
    public enum Edge
    {
        /// <summary>立ち上がりエッジ</summary>
        rising,
        /// <summary>立下りエッジ</summary>
        falling,
        /// <summary>不明</summary>
        NA
    }
}
