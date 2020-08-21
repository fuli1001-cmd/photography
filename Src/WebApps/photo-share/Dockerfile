### STAGE 1: Build ###
FROM node:12.18.2-alpine3.12 AS build
RUN npm install -g @angular/cli
WORKDIR /usr/src/app
COPY Src/WebApps/photo-share/ ./
RUN npm install
RUN ng build --prod

### STAGE 2: Run ###
FROM nginx:1.19.1-alpine
mkdir -p /etc/nginx/cert
COPY Src/WebApps/photo-share/nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /usr/src/app/dist/photo-share /usr/share/nginx/html