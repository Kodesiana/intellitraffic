FROM nginx:1.27.5-bookworm

COPY ./nginx.conf /etc/nginx/nginx.conf
COPY ./nginxconfig.io /etc/nginx/nginxconfig.io
COPY ./sites-available/* /etc/nginx/sites-available
COPY ./tls /etc/nginx/tls
