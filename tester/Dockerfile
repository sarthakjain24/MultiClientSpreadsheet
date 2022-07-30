# docker build --tag app .
FROM ubuntu

# Set a default shell.
SHELL ["/bin/bash", "-c"]
ARG DEBIAN_FRONTEND=noninteractive
ENV TZ="America/Salt Lake City"

#Install all dependency
RUN apt -y update && apt-get -y install \
    python3.9

RUN apt -y upgrade

COPY ./ /

RUN chmod +x spreadsheet_tester
