import os,sys

nb = 5
if len(sys.argv) > 1:
	nb = int(sys.argv[1])

#os.chdir("D:\\Development\\C#\\Brownien\\bin\\Debug\\netcoreapp3.1")
os.chdir("bin\\Debug\\netcoreapp3.1")
for i in range(nb):
	print("Simulation n=",i+1)
	os.system("Brownien.exe")