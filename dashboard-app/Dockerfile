﻿#
# Build stage
#

FROM node:14.15.0-alpine as build

WORKDIR /app

COPY . .
RUN npm install
RUN npm run ng build -- --prod

#
# Run stage
#

FROM nginx:alpine

RUN rm /etc/nginx/conf.d/default.conf
RUN rm -rf /usr/share/nginx/html/*

COPY --from=build /app/nginx.conf /etc/nginx/conf.d/nginx.conf
COPY --from=build /app/dist/dashboard-app /usr/share/nginx/html

ENTRYPOINT ["nginx", "-g", "daemon off;"]
