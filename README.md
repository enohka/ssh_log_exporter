# ssh_log_exporter

Honeypot ssh log exporter

Designed to work together [with my OpenSSH Honeypot](https://github.com/enohka/openssh-portable)

This program can run as a linux daemon, scrape all logs from my SSH Honeypot and send them to an [InfluxDB](https://www.influxdata.com/products/influxdb/) instance.


### Building

simply run
```
dotnet publish -p:PublishProfile=FolderProfile
```

### Configuration

The configuration should be supplied via a json file given after a --config arg

```
hp_ssh_exporter --config /etc/hp_ssh_exporter/config.json
```

The config file can contain the following values

```
{
  "sshlogscraper": {
    "scrapeintervall": 20,   // Intervall in seconds, in which the daemon should scan the LogLocation folder for new logs
    "LogLocation": "<path to folder containing logs>" // LogLocation for my SSH Honeypot is currently always /var/log/hp
  },
  "influxdb": {
    "host": "", // hostname including protocol and port e.g. https://influxdb.example.com:8086 note that we are using the InfluxDB 2 API
    "token": "", // A token which should have write access to the given bucket
    "bucket": "", // Your bucket in which you want to store the ssh time series data
    "org": "" // Your Org in which you have the bucket
  }
}
```

### Installation 

Copy the published artifacts to your target machine and run:

```
mkdir -p /var/log/hp
mkdir /etc/hp_log_exporter

cp config.json /etc/hp_log_exporter

cp hp_ssh_exporter hp_ssh_exporter.pdb /usr/sbin/
cp sshlogscraper.service /etc/systemd/system

systemctl daemon-reload
systemctl enable sshlogscraper
systemctl start sshlogscraper
```

If your InfluxDb instance is using a self singed ssh certificate don''t forget to import your local root CA on the machine:

```
apt-get update
apt install ca-certificates

cp your-root-ca.crt /usr/local/share/ca-certificates
update-ca-certificates
```
