# プロジェクトの目的  
本プロジェクトでは、鳴き声を基に野鳥の種類を当てるソフトウェア開発を行っています。
自然環境の評価や野鳥のモニタリングの効率化と大規模化に寄与することを目指しています。

# リポジトリの説明  
このリポジトリにはソースコードとビルド済みのツール群を含みます。

# 識別能力  
識別能力ですが、2013年の時点で特定の鳥種について絞った識別を行う上では85%程度の正答率を出すことができるのは確認しています。 
残りの15%は、ほとんどが不明に分類されます。 
他の鳥の声をターゲットに間違える割合も若干あるのですが、学習データの質と閾値に依ります。 
ノイズに関しても、学習データを用意すればほぼ間違えません。 ただし、学習データの作成などの熟練に1年程度かかります。

# 更新の方針  
現時点でのソースコードはC#が多いのですが、将来的にはPythonでほぼ全てを書き直すつもりです。  

# lisence  
全てのソースコードのライセンスはMITです。自由に配付・改変・公開して下さい。

# 参考文献  

<!-- 2016 -->
+ 鈴木麗璽, 松林志保, 奥乃博, [マイクロホンアレイを利用した野鳥の歌行動理解の試み](https://ipsj.ixsq.nii.ac.jp/ej/?action=pages_view_main&active_action=repository_view_main_item_detail&item_id=162542&item_no=1&page_id=13&block_id=8), 第78回全国大会講演論文集, No.1, pp. 17-18, 2016-03-10.
+ 井上遠, 鷲谷いづみ, 井上奈津美, [亜熱帯照葉樹林における録音による鳥類モニタリングの可能性とその手法](http://www.esj.ne.jp/meeting/abst/63/P1-334.html), 日本生態学会第63回全国大会講演要旨, 2016-03.
+ 山川将径, 北村俊平, [石川県林業試験場の森における鳥類記録手法（音声録音とスポットセンサス）の比較](http://www.n-muse-ishikawa.or.jp/motto/entries/620163.html), 石川県立自然史資料館研究報告, No.6, pp.15-24, 2016-03.
+ Ilyas Potamitis, [Deep learning for detection of bird vocalisations](https://arxiv.org/ftp/arxiv/papers/1609/1609.08408.pdf), 2016.
+ Dan Stowell, Mike Wood, Yannis Stylianou, Herve Glotin, [BIRD DETECTION IN AUDIO: A SURVEY AND A CHALLENGE](https://arxiv.org/pdf/1608.03417.pdf), 2016 IEEE INTERNATIONAL WORKSHOP ON MACHINE LEARNING FOR SIGNAL PROCESSING, SEPT. 13–16, SALERNO, ITALY, 2016.


<!-- 2015 -->
+ 富澤浩樹, 市川尚, 阿部昭博, [スマートフォンを用いた市民参加型調査支援システムの検討](https://ipsj.ixsq.nii.ac.jp/ej/?action=pages_view_main&active_action=repository_view_main_item_detail&item_id=144857&item_no=1&page_id=13&block_id=8), 研究報告情報システムと社会環境, Vol.2015-IS-133, No.5, pp.1-8, 2015-08-07.

<!-- 2014 -->
+ Dan Stowell​, Mark D. Plumbley, [Automatic large-scale classification of bird sounds is strongly improved by unsupervised feature learning](https://peerj.com/articles/488/), PeerJ, DOI: 10.7717/peerj.488, 2014-07.
+ Deepika M, Nagalinga Rajan A, [Automatic Identification of Bird Species from the Recorded Bird Song Using ART Approach](http://www.rroij.com/open-access/automatic-identification-of-bird-species-fromthe-recorded-bird-song-using-art-approach.pdf), ICIET'14, ISSN:2319-8753, 2014.
+ Ilyas Potamitisa, Stavros Ntalampirasb, Olaf Jahnc, Klaus Riedec, [Automatic bird sound detection in long real-field recordings: Applications and tools](http://www.sciencedirect.com/science/article/pii/S0003682X14000024), Applied Acoustics, Volume 80, Pages 1–9. June 2014.
+ Sara Keena, Jesse C. Rossb, Emily T. Griffithsa, Michael Lanzonec, Andrew Farnsworthd, [A comparison of similarity-based approaches in the classification of flight calls of four species of North American wood-warblers](http://www.sciencedirect.com/science/article/pii/S1574954114000028?np=y&npKey=a172312b9f94b36c31ee50552867fbe599053afa80255852bd27c8370904df52), Ecological Informatics, Volume 21, Pages 25–33, May 2014.

<!-- 2013 -->
+ 西宏之, 清田温子, 木村義政, [野鳥の鳴き声の自動認識による未知の渡り鳥の飛来感知](http://ci.nii.ac.jp/els/110009784522.pdf?id=ART0010280767&type=pdf&lang=jp&host=cinii&order_no=&ppv_type=0&lang_sw=&no=1466329477&cp=), 信学技報, vol. 113, no. 210, pp. 53-56, 2013.

+ Holly Root-Gutteridge, 他, [Identifying individual wild Eastern grey wolves \(Canis lupus lycaon\) using fundamental frequency and amplitude of howls](http://www.tandfonline.com/doi/full/10.1080/09524622.2013.817317#.UirTdca-1cY), Bioacoustics: The International Journal of Animal Sound and its Recording, 2013.  オオカミだけど、技術は同じ。日本語による[紹介記事](http://ggsoku.com/tech/wolf-voice-recognition/)もある。

+ Holly Root-Gutteridge, 他, [Improving individual identification in captive Eastern grey wolves \(Canis lupus lycaon\) using the time course of howl amplitudes](http://www.tandfonline.com/doi/full/10.1080/09524622.2013.817318#.UirTV8a-1cY) , Bioacoustics: The International Journal of Animal Sound and its Recording, 2013.
  （オオカミ）
+ 永井靖弘, [長期モニタリング手法－野生動物音声自動録音技術](http://ideacon.jp/contents/inet/vol34/vol34_new01s.pdf), i-net (建設・環境技術レポート &amp; トピックス), Vol. 34, pp. 2-3, 2013.
+ 藤田素子, 丸山晃央, 神崎護, 奥乃博（指導教員）, [音声録音データを用いた鳥類多様性評価の可能性](http://www.esj.ne.jp/meeting/abst/60/T17-2.html), 日本生態学会第60回全国大会 (2013年3月，静岡) 講演要旨, 2013.  
（アブストのみ）
+ 鈴木麗璽, [野鳥の歌コミュニケーション理解への試み](http://www.osaka-kyoiku.ac.jp/~challeng/SIG-Challenge-B302/B302-05.pdf), 第38回人工知能学会, 2013-12.
+ 孫栄, 内田和麿, 趙華安, [高認識率の野鳥の種自動識別法に関する研究](https://www.jstage.jst.go.jp/article/jceeek/2013/0/2013_598/_article/-char/ja/), 平成25年度　電気関係学会九州支部連合大会, 2013-09.
+ 黒沢令子, 植田睦之, 斎藤馨, [志賀おたの申す平における森林性鳥類のさえずり活動の研究：長期モニタリングの基礎資料](https://soar-ir.repo.nii.ac.jp/?action=pages_view_main&active_action=repository_view_main_item_detail&item_id=2178&item_no=1&page_id=13&block_id=45), 志賀自然教育研究施設研究業績, No.50, pp.7-11, 2013-03.


<!-- 2012 -->
+ 宇根健一郎, 他, [環境音を含む音データからのヤンバルクイナの鳴き声検出の検討](https://www.jstage.jst.go.jp/article/jjo1986/41/1/41_1_1/_article/-char/ja/), 情報処理学会 第74回全国大会講演論文集, Vol.1, pp.589-591, 2012.

+ 宇根健一郎, 他, [ヤンバルクイナの鳴き声検出のための閾値決定方法の検討](https://ipsj.ixsq.nii.ac.jp/ej/?action=pages_view_main&active_action=repository_view_main_item_detail&item_id=151950&item_no=1&page_id=13&block_id=8), 情報科学技術フォーラム講演論文集, Vol.11, No.4, pp.441-442, 2012-09-04.

+ 植田睦之, 平野敏明, 黒沢令子, [長時間の録音データから鳥のさえずり状況を知るための聞き取り時間帯の検討](https://www.jstage.jst.go.jp/article/birdresearch/8/0/8_T1/_article/-char/ja/), Bird Research, Vol. 8, pp. T1-T6, 2012.

+ 関伸一, [自動撮影カメラとタイマー付録音機で記録されたトカラ列島の無人島群における鳥類相](https://www.jstage.jst.go.jp/article/birdresearch/8/0/8_A35/_article/-char/ja/), Bird Research, Vol. 8, pp. A35-A48, 2012.
+ 植田睦之, 黒沢令子, 斎藤馨, [森林音のライブ配信から聞き取った森林性鳥類のさえずり頻度のデータ](https://www.jstage.jst.go.jp/article/birdresearch/8/0/8_R1/_article/-char/ja/), Bird Research, Vol. 8, pp. R1-R4, 2012.

+ 山本康仁, [東三河地域の土地利用の異なる2 地点におけるカエル類の音声モニタリング](http://www.toyohaku.gr.jp/sizensi/06shuppan/kenkyuuho/kenpou22/22kenkyuu-houkoku13.pdf), 豊橋市自然史博物館研報, No. 22, pp. 13-18, 2012.
+ 藤岡正博, [井川演習林および井川地域の鳥類相](http://www.tulips.tsukuba.ac.jp/limedio/dlam/M11/M1106410/2.pdf), 筑波大学農林技術センター演習林報告, 28号, pp. 1-27, 2012.


<!-- 2011 -->
+ 矢田豊, 江崎功二郎, 小谷二郎, [人工林における下層植生量と鳥類生息状況の関係](http://www.pref.ishikawa.lg.jp/ringyo/science/public/kh/), 石川県林業試験場研究報告, No. 43, pp. 13-18, 2011.

+ Peter Jancovic, Münevver Köküer, [Automatic Detection and Recognition of Tonal Bird Sounds in Noisy Environments](https://www.researchgate.net/publication/220058086_Automatic_Detection_and_Recognition_of_Tonal_Bird_Sounds_in_Noisy_Environments), Hindawi Publishing Corporation, EURASIP Journal on Advances in Signal Processing, Volume 2011, Article ID 982936, 10 pages, doi:10.1155/2011/982936, 2011.  


<!-- 2010 -->
+ 百瀬造, [鳥の鳴き声を分析しよう　実習編](http://ornithology.jp/osj/japanese/katsudo/Letter/no32/files/gakko-2010-text.pdf), 鳥の学校‐第４回テーマ別講習会, 2010.
+ R. Bardelia, D. Wolffb, F. Kurthc, M. Koche, K.-H. Tauchertf, K.-H. Frommolt, [Detecting bird sounds in a complex acoustic environment and application to bioacoustic monitoring](http://www.sciencedirect.com/science/article/pii/S0167865509002487), Pattern Recognition Letters, Volume 31, Issue 12, Pages 1524–1534, 1 September 2010.


<!-- 2009 -->
+ 石田健, [ADAM　音声情報による動物のモニタリング](http://forester.uf.a.u-tokyo.ac.jp/~ishiken/japanese/ADAM/ADAM20090309.pdf), サイバーフォレスト研究会発表スライド, 2009.
+ 百瀬浩, [鳥類研究者のための音声分析ガイド](http://ornithology.jp/osj/japanese/katsudo/Letter/no24/OL24.html#04), 鳥学通信 No. 24, 2009.
+ 高橋幸司, 三田長久, 他, [野鳥の音声データによる自動種識別システム](http://ci.nii.ac.jp/naid/110007094575), 電子情報通信学会総合大会講演論文集 2009年_情報・システム(1), 178, 2009-03-04.
+ 澁谷尚志, 横田康成, [ハシボソガラスの鳴き声のスペクトログラム解析，および検知](https://www.jstage.jst.go.jp/article/ieejeiss/129/12/129_12_2144/_article/-char/ja/), 電気学会論文誌Ｃ, Vol.129, No.12, pp.2144-2151, 2009.
+ CW Clark, [Advanced Technologies for Acoustic Monitoring of Bird Populations](http://www.dtic.mil/dtic/tr/fulltext/u2/a534222.pdf), SERDP SI-1461 Final Report, 2009.


<!-- 2008 -->
+ 植田睦之, (https://www.jstage.jst.go.jp/article/birdresearch/4/0/4_0_T1/_article/-char/ja/" target="_blank" title="J-STAGEへのリンク">森林の夜行性鳥類の効率的な調査時刻と録音による調査の可能性], Bird Research, Vol. 4, pp. T1-T8, 2008.
+ 高橋幸司, 三田長久, 他, [野鳥の音声データによる自動種識別システム](http://ci.nii.ac.jp/naid/110006868186), 電子情報通信学会総合大会講演論文集 2008年_情報・システム(1), 193, 2008-03-05.<br>
+ 牧野洋平, 三田長久, 他, [雑音の平均値を用いた長時間音声からの野鳥の鳴き声の抜出](http://ci.nii.ac.jp/naid/110006868185), 電子情報通信学会総合大会講演論文集 2008年_情報・システム(1), 192, 2008-03-05.
+ 東京大学演習林鳥類研究会, [東京大学演習林鳥類目録](http://repository.dl.itc.u-tokyo.ac.jp/dspace/handle/2261/24560), 演習林, 第四十八号, pp.103-131, 2008-01.

<!-- 2007 -->
+ 石田健, [音声データによる鳥類のモニタリング=ADAM](http://ornithology.jp/osj/japanese/katsudo/Letter/no17/OL17.html#01), 鳥学通信 No. 17, 2007.
+ 藤田剛, 植田睦之, 天野一葉, [鳥類モニタリングの可能性を探る](http://ornithology.jp/osj/japanese/katsudo/Letter/no17/OL17.html#04), 鳥学通信 No. 17, 2007.
+ 百瀬浩, [鳥類研究者のための音声分析ガイド](http://ornithology.jp/osj/japanese/katsudo/Letter/no12/OL12.html#02), 鳥学通信 No. 12, 2007.
+ 高橋幸司, 三田長久, 他, [野鳥の音声データの圧縮による種識別への影響の検討](http://ci.nii.ac.jp/naid/110007688513), 情報科学技術フォーラム一般講演論文集 6(2), 333-334, 2007-08-22.
+ 高橋幸司, 三田長久, 他, [雑音の平均値を用いた長時間音声からの野鳥の鳴き声の抜出](http://ci.nii.ac.jp/naid/110007688512), 情報科学技術フォーラム一般講演論文集 6(2), 331-332, 2007-08-22.
+ 岩崎祐介, 三田長久, 他, [音声情報を用いた夜行性鳥類の種識別](http://ci.nii.ac.jp/naid/110007688511), 情報科学技術フォーラム一般講演論文集 6(2), 329-330, 2007-08-22.<br>
+ 牧野洋平, 三田長久, 他, [テンプレートマッチングを用いた夜行性鳥類の種識別](http://ci.nii.ac.jp/naid/110006461604), 電子情報通信学会総合大会講演論文集 2007年_情報・システム(1), 147, 2007-03-07.
+ 東谷幸治, 三田長久, 他, [音声情報を用いたニューラルネットワークによる野鳥の種識別](http://ci.nii.ac.jp/naid/110006461603), 電子情報通信学会総合大会講演論文集 2007年_情報・システム(1), 146, 2007-03-07.

<!-- 2006 -->
+ 東谷幸治, 三田長久, 他, [音声情報によるニューラルネットワークを用いた夜行性野鳥の識別](http://ci.nii.ac.jp/naid/110007684853), 情報科学技術フォーラム一般講演論文集 5(2), 355-356, 2006-08-21.
+ 牧野洋平, 三田長久, 他, [時刻ごとの音量最大周波数を基準とした野鳥の品種識別](http://ci.nii.ac.jp/naid/110007684852), 情報科学技術フォーラム一般講演論文集 5(2), 353-354, 2006-08-21.

<!-- 2005 -->
+ 石田健, 松岡茂, [「第2回　音声データによる鳥類のモニタリングADAM (Acoustic Data for Avian Monitoring) －夜の鳥をモニタリングする」](http://ornithology.jp/osj/japanese/katsudo/Letter/no1/OL1.html#04), 鳥学通信 No. 12, 2005.
+ 松川 徹, 佐々木 公男, [離散ウェーブレット変換を用いた野鳥鳴声信号の識別](https://www.jstage.jst.go.jp/article/jacc/48/0/48_0_12/_article/-char/ja/), 第48回自動制御連合講演会, 2005-11-25.  
（野鳥に特に興味があったわけではなさそう。）

<!-- 2004 -->
+ Seppo Fagerlund, [Automatic Recognition of Bird Species by Their Sounds](https://www.researchgate.net/publication/27516447_Automatic_Recognition_of_Bird_Species_by_Their_Sounds), Helsinki University of Technology, 2004-11.

<!-- 2002 -->
+ 藤岡正博, [農業土木技術者のための生き物調査 (その2)鳥類調査法](https://www.jstage.jst.go.jp/article/jjo1986/41/1/41_1_1/_article/-char/ja/), 農業土木学会誌, Vol. 70, No. 10, pp. 935-940, a2, 2002.
+ 斎藤馨, 他, [森林景観ロボットカメラの新機能開発と環境音記録に関する研究](http://ci.nii.ac.jp/naid/110004308326), ランドスケープ研究 : 日本造園学会誌 : journal of the Japanese Institute of Landscape Architecture 65(5), 689-692, 2002-03-30.

<!-- 1998 -->
+ 加藤由香, 他, [ザトウクジラの鳴音による個体識別と保全への貢献（特徴量抽出方法の検討）](http://www.nacsj.or.jp/pn/houkoku/h01-08/pdf/h07-no08.pdf), 第7期プロ・ナトゥーラ・ファンド女性成果報告書1998, 1998.
[WEB版](http://www.nacsj.or.jp/pn/houkoku/h01-08/h07-no08.html)</span>

<!-- 1993 -->
+ 濱尾章二, [さえずりによるウグイスの個体識別](https://www.jstage.jst.go.jp/article/jjo1986/41/1/41_1_1/_article/-char/ja/), 日本鳥学会誌, Vol. 4, No. 1, pp. 1-7, 1993.

<!-- 1991 -->
+ 高村聡, 他, [野鳥の鳴き声の識別](http://ci.nii.ac.jp/naid/110000217937), 山梨大學工學部研究報告 42, 44-50, 1991-12-00.

<!-- 1987 -->
+ 明石全弘, 山岸哲, [ホオジロ Emberiza cioides の囀りに関する研究](https://www.jstage.jst.go.jp/article/jjo1986/36/1/36_1_19/_article/-char/ja/), 日本鳥学会誌, Vol. 36, No. 1, pp. 19-45, 1987-1988.
