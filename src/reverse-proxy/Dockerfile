FROM nginx:1.27.5-bookworm

COPY ./nginx.conf /etc/nginx/nginx.conf
COPY ./default.conf /etc/nginx/conf.d/default.conf

COPY ./tls /etc/nginx/tls
COPY ./sites-enabled /etc/nginx/sites-enabled
COPY ./nginxconfig.io /etc/nginx/nginxconfig.io
