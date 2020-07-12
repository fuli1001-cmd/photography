### STAGE 1: Build ###
FROM node:12.18.2-alpine3.12 AS build
RUN npm install -g @angular/cli
WORKDIR /usr/src/app
COPY Src/photo-share/ ./
RUN npm install
RUN ng build --prod

### STAGE 2: Run ###
FROM nginx:1.19.1-alpine
COPY Src/photo-share/nginx.conf /etc/nginx/conf.d/photo-share.conf
COPY --from=build /usr/src/app/dist/photo-share /usr/share/nginx/html