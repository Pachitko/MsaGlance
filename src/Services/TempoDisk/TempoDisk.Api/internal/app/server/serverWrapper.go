package server

import (
	"context"
	"log"
	"net/http"
	"tempodisk/internal/abstractions"
	"tempodisk/internal/app/fileSavers"
	"tempodisk/internal/dtos/fileDto"
	"time"
)

type Config struct {
	Addr              string
	ShutdownTime      time.Duration
	FileSaverCount    int64
	FileChanSize      int64
	FileNameGenerator abstractions.FileNameGenerator
	FileDir           string
}

type serverWrapper struct {
	server       *http.Server
	shutdownTime time.Duration

	fileSaverPool abstractions.Pool
	poolCtx       context.Context
	poolCancel    context.CancelFunc
}

func NewWrapper(c Config) *serverWrapper {
	filesChan := make(chan fileDto.FileDto, c.FileChanSize)

	fileSaverPool := fileSavers.NewPool(c.FileSaverCount, filesChan, c.FileDir)
	httpHandler := NewHttpHandler(filesChan, c.FileNameGenerator)

	ctx, cancel := context.WithCancel(context.Background())

	server := &http.Server{Addr: c.Addr, Handler: httpHandler}

	return &serverWrapper{
		server:        server,
		shutdownTime:  c.ShutdownTime,
		fileSaverPool: fileSaverPool,
		poolCtx:       ctx,
		poolCancel:    cancel,
	}
}

func (sw *serverWrapper) Run() error {

	sw.fileSaverPool.Start(sw.poolCtx)

	log.Printf("Listening on '%s'", sw.server.Addr)
	return sw.server.ListenAndServe()
}

func (sw *serverWrapper) Shutdown() error {
	sw.poolCancel()

	sw.fileSaverPool.Wait()

	shutdownCtx, cancel := context.WithTimeout(context.Background(), sw.shutdownTime)
	defer cancel()

	return sw.server.Shutdown(shutdownCtx)
}
