from selenium import webdriver
from selenium.webdriver.common.keys import Keys
from selenium.common.exceptions import TimeoutException
from selenium.webdriver.common.by import By
import selenium.webdriver.support.ui as ui
import selenium.webdriver.support.expected_conditions as EC
import datetime
import os
import time
import io

options = webdriver.ChromeOptions()
options.add_argument('--ignore-certificate-errors')
options.add_argument('--ignore-ssl-errors')
options.add_argument('--browser.download.folderList=2')
options.add_argument('--browser.helperApps.neverAsk.saveToDisk=text/csv')
options.add_argument('--browser.helperApps.neverAsk.saveToDisk=application/vnd.openxmlformats-officedocument.spreadsheetml.sheet')
options.add_argument('--browser.helperApps.alwaysAsk.force=false')
options.add_argument('--browser.download.manager.showWhenStarting=false')
options.add_argument('--browser.download.dir=B:/OneDrive/MarketData/MOEX/tempfolder/')
today = str(datetime.datetime.now().date()).replace('-', '.')
default_path = 'C:/Users/bushuevroman/YandexDisk/MarketData/MOEX/raw/' + today + '/'
if not os.path.exists(default_path):
    os.mkdir(default_path)
prefs = {"download.default_directory": default_path}
options.add_experimental_option('prefs', prefs)

chromedriver = 'C:/Users/bushuevroman/YandexDisk/MarketData' + "/chromedriver"
os.environ["webdriver.chrome.driver"] = chromedriver
driver = webdriver.Chrome(chrome_options= options, executable_path=chromedriver)
url = 'http://www.moex.com/s1163#?sort_order=asc&sort_column=ISIN&faceunit=_,RUB,USD,EUR,GBP,CNY,CHF&internal=&qualinvestor=&currencyid=&collateral=&ncc_qualified=&listname=&rii=&start=0&instrumentgroups=stock_common_share,stock_preferred_share,stock_russian_depositary_receipt,stock_ofz_bond,stock_subfederal_bond,stock_municipal_bond,stock_corporate_bond,stock_exchange_bond,stock_euro_bond,stock_corporate_euro_bond,stock_etf_ppif,stock_foreign_share_dr,stock_foreign_share,stock_public_ppif,stock_interval_ppif,stock_private_ppif,stock_mortgage,stock_gcc&board_groups=stock_tplus,stock_ndm_tplus,stock_small_tplus,stock_d_tplus,stock_d_ndm_tplus,stock_t0,stock_ndm,stock_d_ndm,stock_bonds_d_main,stock_darkpool,stock_b_psau,stock_b_auct,stock_b_psbb,stock_b_aubb,stock_repo_na,stock_repo_gcc_na,stock_repo_gcc_a,stock_repo_adr,stock_repo,stock_cb_repo_auct,stock_cb_repo_fix&index='
driver.get(url)
time.sleep(10)
driver.find_element_by_id('export2csv').click()
time.sleep(10)
driver.close()
driver.quit()

path = 'C:/Users/bushuevroman/YandexDisk/MarketData/queue.txt'
isWrite = False
while not isWrite:
    try:
        with io.open(path, "a") as myfile:
            time.sleep(2)
            myfile.write('rates ' + today + '\n')
        isWrite = True
    except:
        isWrite = False
    finally:
        time.sleep(2)