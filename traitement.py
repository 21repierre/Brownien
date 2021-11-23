from matplotlib import pyplot as plt

orFile = open('origins.txt', 'r')
taFile = open('targets.txt', 'r')

origins = orFile.readlines()
targets = taFile.readlines()

origin = []
target = []

for line in origins:
	spl = line[:-1]
	if spl != "NaN":
		origin.append(float(spl))
for line in targets:
	spl = line[:-1]
	if spl != "NaN" and spl != "0":
		target.append(float(spl))

plt.hist(origin, bins=len(set(origin))//10)#, range=(0,max(origin)))
plt.show()
plt.hist(target, bins=len(set(origin))//2)
plt.show()