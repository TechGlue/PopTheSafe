FROM --platform=$BUILDPLATFORM node:alpine as builder

RUN mkdir /project
WORKDIR /project

RUN npm install -g @angular/cli@18.0.0

COPY MySafeClient/package.json MySafeClient/package-lock.json ./

RUN npm ci

COPY MySafeClient/. .

CMD ["npm", "run", "container"]

