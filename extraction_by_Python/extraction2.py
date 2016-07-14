#!/usr/bin/env python
# -*- coding: utf-8 -*-
#-------------------------------------------------------------------------------
# Name:        extraction2
# Purpose:     生成されたファイルに含まれる特徴ベクトルを整理します。　for 新フォーマット(種名（フィルタ名）,識別結果,時刻,抜き出し時間,SN比,特徴ベクトル,音源ファイルパス,コメント（有れば）)
#
# Memo:        ファイルはすべてUTF-8Nに統一しておいてください。
#              ファイル名に','を入れないでください。
#
#               extraction.py/extraction1.5.pyとは異なり、作成されるファイルはコメント等を含んだままです。
#               また、人間が耳で聞いて判別した種類別にファイルを作成する点で異なります。
#               学習用の教師データを作成するのに向いているはずです。
#
#              ファイルは追記されます。
#              実行にはコマンドプロンプトやPyScripterなどが利用可能です。
# Author:      morishita
#
# Created:     22/06/2013
# Copyright:   (c) morishita 2012
# Licence:     New BSD
#-------------------------------------------------------------------------------

import os
import os.path
import codecs
import glob

def GetDelimiter(str):
    """ 文字列を分割しているデリミタを返します
    Args:
        str: デリミタで区切られた文字列
    return:
        デリミタ文字列
        "": 解析できなかった場合
    """
    list = str.rstrip().split(',')                  # 改行コード（\tらも）を削除して、カンマで分割
    if(len(list) > 1):
        return ","
    list = str.rstrip().split('\t')                 # 改行コード（\tらも）を削除して、カンマで分割
    if(len(list) > 1):
        return "\t"
    return ""

def main():
    print("作出された特徴ベクトルを含むファイルより特徴ベクトルを取り出します。")
    # 処理対象になりそうなファイル名一覧を取得
    fnameList = glob.glob(os.getcwd() + "\*.csv")
    print("処理対象ファイルの一覧")
    for fname in fnameList:                         #
        print(fname)
    print("処理を開始します。。。")
    # 取得したファイル名すべてについて処理
    for fname in fnameList:
        file = os.path.basename(fname)              # ファイル名からルートパスを除く
        print(file, "を処理中です。")
        # ファイルを開く
        fr = codecs.open(fname, 'r', encoding="utf-8")  # ファイルを開く
        # 一行ずつ読み込んで、処理結果を保存していく
        try:
            for line in fr.readlines():                 # リストの中身を処理
                delimiter = GetDelimiter(line)          # デリミタを取得
                if(delimiter != ""):
                    _field = line.split(delimiter)
                    result = _field[1]
                    if(result != ""):
                        fname4save = result + ".fea"
                        fw = open(fname4save, 'a')      # 追記用にファイルを開く
                        fw.write(line)
                        fw.close()
        except:
            print("ファイル名：" + fname + "の処理中にエラーが発生しました。")
        # ファイルを閉じる
        fr.close()

if __name__ == '__main__':
    main()
