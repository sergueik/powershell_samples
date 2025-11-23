FROM docker.elastic.co/elasticsearch/elasticsearch:7.9.1
RUN yum install -y jq || apk add --no-cache jq || true
COPY elasticsearch.yml /usr/share/elasticsearch/config/elasticsearch.yml

RUN chown -R elasticsearch:elasticsearch /usr/share/elasticsearch

