package main

import (
	"log"
	"net/http"
	"os"
	"os/signal"
	"syscall"
	"tempodisk/internal/app/server"
	"tempodisk/internal/services/sha256FileNameGenerator"
	"time"
)

func main() {
	log.Println("Configuring server...")

	fileNameGenerator := sha256FileNameGenerator.New()

	serverConfig := server.Config{
		Addr:              ":8080",
		ShutdownTime:      2 * time.Second,
		FileSaverCount:    4,
		FileChanSize:      4,
		FileNameGenerator: fileNameGenerator,
		FileDir:           "./files",
	}
	serverWrapper := server.NewWrapper(serverConfig)

	log.Printf("Server has been configured!\n")

	go func() {
		err := serverWrapper.Run()
		if err != http.ErrServerClosed {
			log.Printf("Server error '%v'\n", err)
		} else {
			log.Printf("Server stopped successfully!\n")
		}
	}()

	sigChan := make(chan os.Signal, 1)
	signal.Notify(sigChan, syscall.SIGINT, syscall.SIGTERM)
	sig := <-sigChan

	log.Printf("Signal '%s' recieved\n", sig)

	err := serverWrapper.Shutdown()
	if err != nil {
		log.Printf("Shutdown error '%v'", err)
	}

	log.Printf("Shutdown completed\n")
}
