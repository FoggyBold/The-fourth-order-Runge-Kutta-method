import json
import matplotlib.pyplot as plt

def lineplot(x_data, y_data, x_data2, y_data2, x_data3, y_data3):
    _, ax = plt.subplots()
    
    plt.plot(x_data, y_data, 'k')
    plt.plot(x_data2, y_data2, 'g')
    plt.plot(x_data3, y_data3, 'y')
    plt.show()


with open("D:\\лабы\\6 семестр\\ЧМ\\2.1laba\\Save\\temp.json", "r") as read_file:
    data = json.load(read_file)
lineplot(data['X1'], data['Y1'], data['X2'], data['Y2'], data['X3'], data['Y3'])