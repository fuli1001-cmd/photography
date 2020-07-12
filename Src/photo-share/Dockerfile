### STAGE 1: Build ###
FROM node:12.18.2-alpine3.12 AS build
WORKDIR /usr/src/app
COPY Src/photo-share/package.json Src/photo-share/package-lock.json ./
RUN npm install
COPY . .
WORKDIR "/usr/src/app/Src/photo-share"
RUN npm run build

### STAGE 2: Run ###
FROM nginx:1.19.1-alpine
COPY Src/photo-share/nginx.conf /etc/nginx/conf.d/photo-share.conf
COPY --from=build /usr/src/app/Src/photo-share/dist/photo-share /usr/share/nginx/html