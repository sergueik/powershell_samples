FROM docker.elastic.co/kibana/kibana:7.9.1
USER root
# NOTE:
# Could not resolve host: mirrorlist.centos.org; Unknown error
# base image CentOS 7 mirrors are retired and unreachable
# RUN yum install -y epel-release && yum install -y jq
RUN curl -L -o /usr/bin/jq https://github.com/stedolan/jq/releases/download/jq-1.7/jq-linux64 \
    && chmod +x /usr/bin/jq
USER kibana
COPY kibana.yml /usr/share/kibana/config/kibana.yml

