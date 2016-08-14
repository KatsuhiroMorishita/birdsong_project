# ユニット6c1dの出力を確認するスクリプト

import numpy as np
import matplotlib.pyplot as plt
import wave
import pyaudio

fname = "featured result.csv"
width = 25

def draw(data,cb_min,cb_max):  #cb_min,cb_max:カラーバーの下端と上端の値
	"""　コンター図を表示する
	http://qiita.com/AnchorBlues/items/0dd1499196670fdf1c46
	"""
	X, Y = np.meshgrid(np.arange(data.shape[1]), np.arange(data.shape[0]))
	plt.figure(figsize=(4,4))  #図の縦横比を指定する
	div = 20.0                    #図を描くのに何色用いるか
	delta = (cb_max - cb_min) / div
	interval = np.arange(cb_min, abs(cb_max) * 2 + delta, delta)[0:int(div)+1]
	plt.contourf(X, Y, data, interval)
	plt.show()


def play(input_filename, start_time, time_width):
	""" 音源を再生する
	"""
	buffer_size = 4096
	wav_file = wave.open(input_filename, 'rb' )
	p = pyaudio.PyAudio()
	stream = p.open(
				format = p.get_format_from_width(wav_file.getsampwidth()) ,
				channels = wav_file.getnchannels(),
				rate = wav_file.getframerate(),
				output = True
				)
	#print(wav_file.getframerate())
	dummy_size = int(start_time * wav_file.getframerate()) # 読み飛ばすサイズ
	size = int(time_width * wav_file.getframerate())       # 読み込むサイズ
	buf = wav_file.readframes(dummy_size)
	buf = wav_file.readframes(size)
	stream.write(buf)
	stream.close ()
	p.terminate ()
	wav_file.close ()


with open(fname, "r", encoding="utf-8-sig") as fr:
	lines = fr.readlines()
	for i in range(len(lines)):
		line = lines[i].rstrip()
		#with open("out_{0:02d}.csv".format(i), "w") as fw: # 使うならコメントアウトを外して、以降をインデントする
		field = line.split(",")
		filter_name = field.pop(0)
		result_name = field.pop(0)
		start_time = float(field.pop(0))
		time_width = float(field.pop(0))
		sn = field.pop(0)
		fpath = field.pop(-1)
		height = int(len(field) / width)
		s = 0
		e = width
		Z = []
		for _ in range(height):
			part = field[s:e]
			#fw.write(",".join(part))
			#fw.write("\n")
			s += width
			e += width
			part = [float(mem) for mem in part]
			Z.append(part)
		Z = np.array(Z)
		print("count: ", i)
		play(fpath, start_time, time_width)
		draw(Z, 0, 1.0)

