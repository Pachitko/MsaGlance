FROM nginx:1.23.1

COPY ./nginx/html /usr/share/nginx/html

COPY ./nginx/certs /etc/nginx/ssl

COPY ./nginx/nginx.conf /etc/nginx/nginx.conf
COPY ./nginx/api_json_errors.conf /etc/nginx/api_json_errors.conf

COPY ./nginx/access.log /etc/nginx/logs/access.log
COPY ./nginx/error.log /etc/nginx/logs/error.log