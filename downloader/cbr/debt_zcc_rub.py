import datetime
import urllib.request
import bs4 as bs
import io
import time

#today = (datetime.datetime.now() - datetime.timedelta(days = 1)).date().__str__().replace('-', '.')
today = (datetime.datetime.now()).date()
default_year = today.year
default_month = today.month
default_day = today.day
#default_year = datetime.datetime.now().date().year
#default_month = datetime.datetime.now().date().month
#default_day = datetime.datetime.now().date().day
url = 'http://www.cbr.ru/hd_base/zcyc_params/zcyc/?DateTo={}.{}.{}'.format(default_day, default_month, default_year)
opener = urllib.request.FancyURLopener({})
f = opener.open(url)
content = f.read()
page = bs.BeautifulSoup(content, "html.parser")
table = page.find('table', attrs = {'class': 'data'})
values = list(map(lambda x: x.text, table.find_all('tr')[1].find_all('td')[1:]))
if len(values) != 0 and values[0].strip() != '—':
    str = ''
    for x in values:
        str += x + ';'
    today_str = '{}.{}.{}'.format(default_day, default_month, default_year)
    str = today_str + ';' + str.strip(';') + '\n'
    file_path = 'C:/Users/bushuevroman/YandexDisk/MarketData/CBR/raw/debt_zcc_rub/debt_zcc_rub.csv'
    with open(file_path, "a") as myfile:
        myfile.write(str)
		
path = 'C:/Users/bushuevroman/YandexDisk/MarketData/queue.txt'
isWrite = False
while not isWrite:
    try:
        with io.open(path, "a") as myfile:
            time.sleep(2)
            myfile.write('debt_zcc_rub' + '\n')
        isWrite = True
    except:
        isWrite = False
    finally:
        time.sleep(2)