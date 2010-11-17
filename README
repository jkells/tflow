# TFlow

## Overview
TFlow is a simple program for analysing your download speed from various hosts over time. It produces a report in CSV format that you can load into your favourite spreadsheet program to generate a nice graph.

It is handy for determining if you are suffering from network issues during certain periods of the day.

TFlow will only download the start of the file. By default it will download the file for 15 seconds or 8Mb which ever comes first. These values can be configured on the command line.

## Usage
`TFlow.exe --help` Provides the command line options.

You must provide a link file. A link file is just a text file with a single URL on each line. TFlow will measure the download speed from each url.


## Examples

For every url in links.txt download the file once and write the output to output.csv
`TFlow.exe -l links.txt -o output.csv`

Download all the url's in links.txt 72 times sleeping 1200 seconds between each run. In other words download the files every 20 minutes for 24 hours.
`TFlow.exe --link-file=links.txt --output-file=output.csv --sample-count 72 --sample-sleep 1200'
