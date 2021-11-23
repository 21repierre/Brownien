from matplotlib import pyplot as plt

orFile = open('datas/origins.txt', 'r')
taFile = open('datas/targets.txt', 'r')
speedsFile = open('datas/targets.txt', 'r')

origins = orFile.readlines()
targets = taFile.readlines()
speeds = taFile.readlines()

origin = []
target = []
speed = []

for line in origins:
	if line == '':
		continue
	spl = line[:-1]
	if spl != "NaN":
		origin.append(float(spl))
for line in targets:
	if line == '':
		continue
	spl = line[:-1]
	if spl != "NaN" and spl != "0":
		target.append(float(spl))
for line in speeds:
	if line == '':
		continue
	spl = line[:-1]
	if spl != "NaN":
		speed.append(float(spl))

plt.hist(origin, bins=len(set(origin))//5)#, range=(0,max(origin)))
plt.show()
plt.hist(target, bins=len(set(origin))//2)
plt.show()
plt.hist(speed)#, bins=len(set(origin))//1)
plt.show()