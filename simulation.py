import os

os.chdir("D:\\Development\\C#\\Brownien\\bin\\Debug\\netcoreapp3.1")
for i in range(12):
	print("Simulation n=",i+1)
	os.system("Brownien.exe")