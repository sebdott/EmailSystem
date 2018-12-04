# base image
FROM node:8.9.4  as build-node
RUN mkdir /app
WORKDIR /app
ADD . /app
RUN yarn
RUN yarn build

FROM nginx
COPY --from=build-node /app/dist /usr/share/nginx/html
COPY ./nginx-config/nginx.conf /etc/nginx/nginx.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]