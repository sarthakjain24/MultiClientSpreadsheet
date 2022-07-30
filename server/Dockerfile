# docker build --tag app .
FROM ubuntu

# Set a default shell.
SHELL ["/bin/bash", "-c"]
ARG DEBIAN_FRONTEND=noninteractive
ENV TZ="America/Salt Lake City"

#Install all dependency
RUN apt -y update && apt-get -y install \
  python3.9 \
  make \
  git \
  vim \
  g++ \
  gcc \
  libboost-all-dev

RUN apt -y upgrade

RUN mkdir -p /server/ && mkdir -p /spreadsheet_data/

COPY ./ /server

RUN cd /server && \
  make all

RUN cp /server/docker/spreadsheet_server / && chmod +x /spreadsheet_server 

#build and run application 
#RUN cd /server && make && ./main portnum

#run following to bind mount a directory
#docker run -it \
#--mount type=bind,source="/root/final_proj/<name>",target="/<name>" \
#app


