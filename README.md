This program is designed to watch a landing zone / folder for incoming csv files. 

It will then process the files - performing a brief data quality assessment, and then parsing and importing the csv contents into a sQL table. 

The SQL instance in this case is a SQLEXPRESS local instance. 

The data quality expectations include missing data, and duplicate dates. 

The CSV files used for this process are csv files downloaded from Yahoo finance, and are prices for a stock by Date, Open, High, Low and Close. 
