=begin
----------------------------------------------------------------------------------
本プログラムは、ファイルの作成日時をもってファイル名を変更するプログラムです。

開発者:   森下功啓
開発履歴: 2012/01/29 思い立ったが吉日で、1時間で作る
                     フォルダを作って、そこに保存します。
----------------------------------------------------------------------------------
=end

# 指定されたファイルを名前を変えて保存する
# 保存されるファイル名には作成日時が入る
def Rename(filename, folder)
  open(filename) {|source|
    newName = File::mtime(filename).strftime('%Y_%m_%d %H_%M_%S ') + filename # 保存用のファイル名を作る
    newName = folder + "\\" + newName if folder != ""
    p newName
    open(newName, "w") {|dest|
      dest.write(source.read)
    }
  }
end

# メイン処理部
puts "Program start...."
Dir::mkdir("save") unless FileTest::exist?("save") # 保存ファイルのためのフォルダを作る
Dir::foreach('.') {|f|              # カレントディレクトリ内のファイルを一つずつ処理する
  if File.ftype(f) == "file"        # ファイルであることを検査
    #p f
    extension = File::extname(f)    # 拡張子を取得
    #p extension
    if extension == ".wav" || extension == ".WAV"
      p f                           # デバッグのために画面にファイル名を表示する
      Rename(f, "save")
    end
  end
}
puts "Program fin."

