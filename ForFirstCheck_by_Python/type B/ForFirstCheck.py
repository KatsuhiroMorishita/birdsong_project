#!/usr/bin/env python3
# -*- coding:utf-8 -*-
#-------------------------------------------------------------------------------
# Name:        ForFirstCheck
# Purpose:      鳥の初認日検出に使える、日々の特定の鳥の検出数をファイルにまとめる
#
# Author:      morishita
#
# Created:     17/03/2013
# Copyright:   (c) morishita 2013
# Licence:     new BSD
# Ref:         文字コードについて：ttp://noritan-micon.blog.so-net.ne.jp/2010-01-14
# Histry:      2013/6/23 コメントを追加
#                       将来的に、単位時間当たりで正規化した方が良いかもしれない。
#-------------------------------------------------------------------------------
import re
import os
import sys
import glob
import datetime

# ファイル内の文字列にヒットする正規表現
rePattern         = re.compile(r'^\w+(,|\t)(?P<discriminatedResult>\w+)(,|\t)(?P<startTime>\d+\.?\d*)(,|\t)(?P<timeWidth>\d+\.?\d*)((,|\t).)*(?P<filePath>[A-Z]:(\w|\\|\.|\s|-)+)?')
#　ファイル名にヒットする正規表現
reFileNamePattern = re.compile(r'^discriminated_(?P<yyyy>\d{4})(?P<MM>\d{2})(?P<dd>\d{2})_(?P<hh_s>\d{2})(?P<mm_s>\d{2})_(?P<hh_e>\d{2})(?P<mm_e>\d{2})')

def getNames(filelist):
    """ リストで渡されたファイル群に含まれる識別結果の辞書を作って返す
    全ての識別結果を格納した辞書を返すものと考えてください。
    """
    d = {}
    for men in filelist:
        file = os.path.basename(men)
        matchTest = reFileNamePattern.search(file)
        if matchTest == None:
            continue
        #print(os.path.basename(file))
        fr = open(file, "r", encoding='UTF8')
        lines = fr.readlines()
        fr.close()
        for line in lines:
            matchTest = rePattern.search(line)
            if matchTest != None:
                #print(matchTest.groups())
                discriminatedResult = matchTest.group('discriminatedResult')
                #hoge = discriminatedResult in d
                if (discriminatedResult in d) == False:
                    d[discriminatedResult] = 0
    return d

def main():
    filelist = glob.glob(os.getcwd() + '/discriminated*.csv')   # 処理対象のファイル一覧を取得
    names = getNames(filelist)                              # 分類名を取得（辞書）
    fw = open("summary.csv", "w", encoding='UTF-8-sig')     # Excelに簡単に読み込ませるにはBOMが必要
    namesArray = names.keys()                               # 分類名をリストへ変換
    fw.write("Date")
    for men in namesArray:
        fw.write("," + men)
    fw.write("\n")
    for path in filelist:
        sum = {}
        print(os.path.basename(path))
        file = os.path.basename(path)
        fr = open(file, "r", encoding='UTF8')
        lines = fr.readlines()
        fr.close()
        # ファイル名から、時刻を取得
        matchTest = reFileNamePattern.search(file)
        year      = 0
        month     = 0
        day       = 0
        hour_s    = 0
        minuite_s = 0
        hour_e    = 0
        minuite_e = 0
        if matchTest != None:
            #print(matchTest.groups())
            year      = int(matchTest.group('yyyy'))
            month     = int(matchTest.group('MM'))
            day       = int(matchTest.group('dd'))
            hour_s    = int(matchTest.group('hh_s'))
            minuite_s = int(matchTest.group('mm_s'))
            hour_e    = int(matchTest.group('hh_e'))
            minuite_e = int(matchTest.group('mm_e'))
        else:
            continue                                                            # ファイル名の正規表現に合致しなかったものは飛ばす
        tStart = datetime.datetime(year, month, day, hour_s, minuite_s, 0, 0)   # 時刻オブジェクトに直す
        tEnd   = datetime.datetime(year, month, day, hour_e, minuite_e, 0, 0)
        tSpan  = tEnd - tStart                                                  # 念のために時刻差を取得
        # ファイル内を走査して、種数をカウントアップ
        for line in lines:
            matchTest = rePattern.search(line)
            if matchTest != None:
                #print(matchTest.groups())
                discriminatedResult = matchTest.group('discriminatedResult')
                startTime           = float(matchTest.group('startTime'))       # これも以降で使っていないが、今後の拡張に備えて取得
                timeWidth           = float(matchTest.group('timeWidth'))
                if discriminatedResult in sum:
                    sum[discriminatedResult] += 1
                else:
                    sum[discriminatedResult] = 1
        # 結果を出力
        fw.write(tStart.strftime("%Y-%m-%d"))                   # 日付を保存
        for men in namesArray:                                  # 全ての検出された鳥種（鳥に限らないが）について、出力
            if men in sum:
                fw.write("," + str(sum[men]))
            else:
                fw.write("," + "0")
        fw.write("\n")
    fw.close()
if __name__ == '__main__':
    main()
