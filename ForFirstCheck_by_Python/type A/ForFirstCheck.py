#!/usr/bin/env python3
# -*- coding:utf-8 -*-
#-------------------------------------------------------------------------------
# Name:        module1
# Purpose:
#
# Author:      morishita
#
# Created:     17/03/2013
# Copyright:   (c) morishita 2013
# Licence:     <your licence>
#-------------------------------------------------------------------------------
import re
import sys

key = "キビタキ"
rePattern         = re.compile(r'\w+(,|\t)(?P<startTime>\d+\.?\d*)(,|\t)(?P<timeWidth>\d+\.?\d*)((,|\t).)*(?P<filePath>[A-Z]:(\w|\\|\.|\s|-)+)')
reFileNamePattern = re.compile(r'(?P<yyyy>\d{4})(?P<MM>\d{2})(?P<dd>\d{2})_(?P<hh>\d{2})(?P<mm>\d{2})')

def main():
    argvs = sys.argv  # コマンドライン引数を格納したリストの取得
    argc = len(argvs) # 引数の個数
    # デバッグプリント
    #print(argvs)
    #print(argc)
    if (argc != 2):                                         # 引数が足りない場合は、その旨を表示
        print('Usage: # python %s filename' % argvs[0])     # 0はスクリプトファイル自体を指す
        quit()                                              # プログラムの終了
    fname = argvs[1]
    #fname = "20120202_movingStation.ubx"
    fr = open(fname, "r", encoding='UTF8')                  # ファイルを開く
    txt = fr.readlines()                                    # バイナリデータをbytes型変数に格納
    fr.close()
    for men in plist:
        matchTest = rePattern.search(men)
        if matchTest != None:
            #print(matchTest.groups())
            startTime   = int(matchTest.group('startTime'))
            tWidth  = int(matchTest.group('timeWidth'))
            path    = int(matchTest.group('filePath'))
            if path != None:
                matchTest = reFileNamePattern.search(path)
                if matchTest != None:
                    year    = int(matchTest.group('yyyy'))
                    month   = int(matchTest.group('MM'))
                    day     = int(matchTest.group('dd'))
                    hour    = int(matchTest.group('hh'))
                    minuite = int(matchTest.group('mm'))
if __name__ == '__main__':
    main()
