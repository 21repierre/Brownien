from matplotlib import pyplot as plt

orFile = open('datas/origins.txt', 'r')
taFile = open('datas/targets.txt', 'r')
speedsFile = open('datas/speeds.txt', 'r')
positionsFile = open('datas/positions.txt', 'r')
durationsFile = open('datas/durations.txt', 'r')

origins = orFile.readlines()
targets = taFile.readlines()
speeds = speedsFile.readlines()
positions = positionsFile.readlines()
durations = durationsFile.readlines()

origin = []
target = []
speed = []
position = []
duration = []

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
for line in positions:
	if line == '':
		continue
	spl = line[:-1]
	if spl != "NaN" and spl != "0":
		position.append(float(spl))
for line in speeds:
	if line == '':
		continue
	spl = line[:-1]
	if spl != "NaN":
		speed.append(float(spl))
for line in durations:
	if line == '':
		continue
	spl = line[:-1]
	if spl != "NaN":
		duration.append(float(spl))


posMoy = sum(position)/len(position)
durMoy = sum(duration)/len(duration)
print("Moyenne des positions: ", posMoy)
print("Moyenne des durées: ", durMoy)

print("Coeeficient de diffusion: ", posMoy / (2 * 2 * durMoy))

fig, axes = plt.subplots(3,2)

axes[0,0].hist(origin, bins=len(set(origin))//5)
axes[0,0].set_title("Origine")

axes[0,1].hist(target, bins=len(set(target))//2)
axes[0,1].set_title("Cible")

axes[1,0].hist(position, bins=len(set(position)))
axes[1,0].set_title("Position")

axes[1,1].hist(speed, bins=len(set(speed)))
axes[1,1].set_title("Vitesse")

axes[2,0].hist(duration, bins=len(set(duration)))
axes[2,0].set_title("Durées")
plt.show()