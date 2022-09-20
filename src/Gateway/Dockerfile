FROM nginx:1.23.1

COPY Gateway/nginx/html /usr/share/nginx/html

COPY Gateway/nginx/certs /etc/nginx/ssl

COPY Gateway/nginx/nginx.conf /etc/nginx/nginx.conf
COPY Gateway/nginx/api_json_errors.conf /etc/nginx/api_json_errors.conf

COPY Gateway/nginx/access.log /etc/nginx/logs/access.log
COPY Gateway/nginx/error.log /etc/nginx/logs/error.log