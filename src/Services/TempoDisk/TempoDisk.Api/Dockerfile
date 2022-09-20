FROM golang:1.19 as build
WORKDIR /app

RUN apt-get -y update

COPY Services/TempoDisk/TempoDisk.Api .

RUN go mod download && go mod verify

# go get -d -v golang.org/x/net/html && 
# RUN CGO_ENABLED=0 GOOS=linux go build -a -installsuffix cgo -o ./bin/main .
RUN CGO_ENABLED=0 GOOS=linux GOARCH=amd64 go build -o /app/bin/main .

FROM alpine:3.16.2
RUN apk --no-cache add ca-certificates
WORKDIR /app/
COPY --from=build /app/bin/main ./

CMD [ "/app/main" ]
