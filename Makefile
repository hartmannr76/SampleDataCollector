
prep :
	mkdir -p \
		data/whisper \
		data/elasticsearch \
		data/grafana \
		log/graphite \
		log/graphite/webapp \
		log/elasticsearch

run : prep
	docker-compose up -d

stop :
	docker-compose down

rebuild : stop
	docker-compose build