package main

import (
	"context"
	"fmt"
	"log"
	"net/http"
	"os"
	"os/signal"
	"syscall"
	"tempodisk/internal/app/server"
	"time"
)

func handlerPanic() {
	if r := recover(); r != nil {
		fmt.Println(r)
		fmt.Println("Panic has been recovered")
	} else {
		fmt.Println("No panic")
	}
}

func main() {
	defer handlerPanic()

	log.Println("Configurating server...")

	httpHandlerConfiguration := server.HttpHandlerConfiguration{}

	httpHandler := server.NewHttpHandler(httpHandlerConfiguration)

	server := &http.Server{Addr: ":8080", Handler: httpHandler}

	log.Printf("Server has been configured!\n")

	go func() {
		err := server.ListenAndServe()
		if err != http.ErrServerClosed {
			log.Printf("Server error: %v\n", err)
		}
		log.Printf("Server stopped\n")
	}()

	sigCh := make(chan os.Signal)
	signal.Notify(sigCh, syscall.SIGINT, syscall.SIGTERM, syscall.SIGKILL)
	sig := <-sigCh

	log.Printf("Signal '%s' recieved\n", sig)

	ctx, cancel := context.WithTimeout(context.Background(), 2*time.Second)
	defer cancel()
	err := server.Shutdown(ctx)
	if err != nil {
		log.Printf("Shutdown error: %v", err)
	}

	log.Printf("Shutdown completed\n")
}
