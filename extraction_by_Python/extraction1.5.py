#-------------------------------------------------------------------------------
# Name:        extraction1.5
# Purpose:     生成されたファイルに含まれる特徴ベクトルを整理します。 for 新フォーマット(種名（フィルタ名）,識別結果,時刻,抜き出し時間,SN比,特徴ベクトル,音源ファイルパス,コメント（有れば）)
#
# Memo:        ファイルはすべてUTF-8Nに統一しておいてください。
#
#               種別のフィルタを複数用いた結果を一気に処理するためのスクリプトです。
#               基本的には、識別結果をacceptとrejectに分けます。
#               rejectの理由をさらに細かく分析する用途には向きませんが、特徴ベクトルの性能確認の初期確認には使えます。
#               特徴ベクトル部分だけファイルに保存します。
#
#              なお、ファイルは追記されます。
#              本プログラムは、仕分け済みの特徴ベクトルデータに関して、“rejected”というキーワードを基にacceptと分離したファイルを自動的に生成します。
#              実行にはコマンドプロンプトやPyScripterなどが利用可能です。
#              また、"bird name.txt"というファイルの中にキーワードとなるクラス名（例えばキビタキ）を記入しておくと、生成されるファイルはキビタキというキーワードにヒットしたデータは一つのファイルに統一されます。
#              
# Author:      morishita
#
# Created:     21/01/2012
# Copyright:   (c) morishita 2012
# Licence:     New BSD
#-------------------------------------------------------------------------------
#!/usr/bin/env python
# -*- coding: utf-8 -*-

import os
import codecs
import glob


def GetBirdList(fname):
    """ 鳥の名前のリストを取得します.
    """
    if(os.path.exists(fname) == True):              # フォルダがなければ作る
        fr = codecs.open(fname, 'r', encoding="utf-8")
        birdList = []
        for line in fr.readlines():
            birdList.extend([line.strip()])
        #print(birdList)
        return birdList
    else:
        return []

def CreateFeature(line):
    """ 文字列（リスト）を解析して特徴ベクトルの部分だけ返す.
    """
    list = line.rstrip().split(',')                 # 改行コード（\tらも）を削除して、カンマで分割
    if(len(list) == 1):
        list = line.rstrip().split('\t')            # 改行コード（\tらも）を削除して、カンマで分割
    if(len(list) == 1):
        list = line.rstrip().split(' ')             # 改行コード（\tらも）を削除して、カンマで分割
    if(len(list) > 5):
        ans = ""
        for i in range(5, len(list)):               # 要素数が増えているので、4 -> 5
            ans += list[i] + "\t"
        return ans
    else:
        return ""

def GetDelimiter(str):
    """ 文字列を分割しているデリミタを返します
    """
    list = str.rstrip().split(',')                  # 改行コード（\tらも）を削除して、カンマで分割
    if(len(list) > 1):
        return ","
    list = str.rstrip().split('\t')                 # 改行コード（\tらも）を削除して、カンマで分割
    if(len(list) > 1):
        return "\t"
    list = str.rstrip().split(' ')                  # 改行コード（\tらも）を削除して、カンマで分割
    if(len(list) > 1):
        return " "
    return ""

def main():
    print("作出された特徴ベクトルを含むファイルより特徴ベクトルを取り出します。")
    # 鳥の名前の一覧を可能なら取得する
    birdList = GetBirdList("bird name.txt")
    # 保存フォルダを準備
    dir_accept = "accept"                           # フォルダ名をセット
    dir_reject = "reject"
    if(os.path.exists(dir_accept) == False):        # フォルダがなければ作る
        os.mkdir(dir_accept)
    if(os.path.exists(dir_reject) == False):
        os.mkdir(dir_reject)
    # 処理対象になりそうなファイル名一覧を取得
    fnameList = glob.glob(os.getcwd() + "\*.csv")
    print("処理対象ファイルの一覧")
    for fname in fnameList:                         #
        print(fname)
    print("処理を開始します。。。")
    # 取得したファイル名すべてについて処理
    for fname in fnameList:
        field = fname.split('.')
        file = os.path.basename(fname)
        print(file, "を処理中です。")
        name = file.split('.')[0]                   # ファイル名から拡張子を除いた部分を作る
        # ファイル名とバードリストとのマッチングをチェック
        check = -1
        hitName = ""
        for birdName in birdList:
            check = name.find(birdName)             # ファイル名に鳥の種名が含まれているかチェック
            if(check != -1):
                hitName = birdName
                break
        # ファイル名を決定する
        fname4accept = ""
        fname4reject = ""
        if(check == -1):
            print("Bird list not hit.")
            fname4accept = dir_accept + "\\" + name + "_accept.fea" # 保存ファイル名を作る
            fname4reject = dir_reject + "\\" + name + "_reject.fea"
        else:
            fname4accept = dir_accept + "\\" + hitName + "_accept.fea" # 保存ファイル名を作る
            fname4reject = dir_reject + "\\" + hitName + "_reject.fea"
        # ファイルを開く
        fr = codecs.open(fname, 'r', encoding="utf-8")  # ファイルを開く
        fw_accept = open(fname4accept, 'a')             # 書き込み用にファイルを開く
        fw_reject = open(fname4reject, 'a')
        # 一行ずつ読み込んで、処理結果を保存していく
        try:
            for line in fr.readlines():                 # リストの中身を処理
                check = line.find("#")
                if(check != -1):
                    line = line[0:check]                # コメントを除外
                check = line.find(":")
                if(check != -1):
                    line = line[0:check - 1]            # ファイルパスを除外
                hitName = birdName
                feature = CreateFeature(line)
                delimiter = GetDelimiter(line)          # デリミタを取得
                if(delimiter != ""):
                    _field = line.split(delimiter)
                    if(feature != ""):
                        if(_field[1] == "rejected"):
                            fw_reject.write(feature + "\n")
                        else:
                            fw_accept.write(feature + "\n")
        except:
            print("ファイル名：" + fname + "の処理中にエラーが発生しました。")
        # ファイルを閉じる
        fr.close()
        fw_accept.close()
        fw_reject.close()


if __name__ == '__main__':
    main()
