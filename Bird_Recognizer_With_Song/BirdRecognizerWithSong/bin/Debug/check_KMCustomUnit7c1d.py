# ユニット7c1dの出力を確認するスクリプト

import numpy as np
import matplotlib.pyplot as plt
import wave
import pyaudio

fname = "featured result.csv"
width = 25

def draw_line(data):
	"""　折れ線グラフを表示する
	"""
	X = np.arange(len(data))
	plt.figure(figsize=(4,4))  #図の縦横比を指定する
	plt.plot(X, data)
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
		Z = [float(mem) for mem in field]
		Z = np.array(Z)
		print("count: ", i)
		play(fpath, start_time, time_width)
		draw_line(Z)

